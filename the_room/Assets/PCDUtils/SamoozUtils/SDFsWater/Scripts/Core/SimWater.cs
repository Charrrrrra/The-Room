using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimWater : MonoBehaviour {
    public SimWaterSetting simSetting;
    public float radius = 0.5f;
    public float radius2 => Mathf.Pow(radius, 2.0f);
    public Rigidbody rb;
    public float scaleMult = 1.0f;
    public bool isCanMerge = true;

    void Awake() {
        rb = GetComponentInChildren<Rigidbody>();
        UpdateScale();
        // transform.localScale = Vector3.one * radius;
        UpdateMass();
    }

    void Update() {
        if (radius > simSetting.waterEvapRadius) {
            radius -= simSetting.waterEvapSpeed * Time.deltaTime * Mathf.Max(1.0f, radius2 * 0.25f);
            if (radius <= 0)
                DestorySelf();
            UpdateScale();
            UpdateMass();
        } else if (radius <= simSetting.waterDrainRadius) {
            radius -= simSetting.waterDrainSpeed * Time.deltaTime;
            if (radius <= 0)
                DestorySelf();
            UpdateScale();
            UpdateMass();
        }
    }

    void OnTriggerStay(Collider other) {
        SimWater otherWater = other.GetComponent<SimWater>();
        if (!otherWater) {
            return;
        }

        float waterDis = Vector3.Distance(transform.position, otherWater.transform.position);
        float insetDis = Mathf.Abs(otherWater.radius - Mathf.Abs(waterDis - this.radius));
        float mergeOffset = (radius > otherWater.radius ? radius : otherWater.radius) * simSetting.waterMergeDisOffset;

        // bool needMerge = waterDis <= radius / 2.0f || waterDis <= otherWater.radius / 2.0f;
        bool needMerge = waterDis <= radius - mergeOffset 
        || waterDis <= otherWater.radius - mergeOffset;
        if (needMerge) {
            MergeWith(otherWater);
            return;
        }

        MixWith(otherWater, insetDis);

        float dragStrength = Mathf.Max(GetWaterDragStrengthFrom(otherWater, waterDis, insetDis) - GetGroundDragStrength(), 0);
        rb.AddForce((otherWater.transform.position - transform.position).normalized 
        * (dragStrength > 0.05f ? dragStrength : 0), ForceMode.Force);
    }

    public float GetWaterDragStrengthFrom(SimWater otherWater, float waterDis, float insetDis) {
        float strength = insetDis * simSetting.waterDragStrengthPerInsetDis;
        
        return Mathf.Min(strength, waterDis);
    }
    public float GetGroundDragStrength() {
        return radius2 * simSetting.groundDragStrengthPerRadius;
    }

    public void MixWith(SimWater otherWater, float insetDis) {
        if (radius >= otherWater.radius || radius < otherWater.radius * 3.0f) {
            return;
        }

        float dRadius = insetDis * simSetting.waterMixSpeedPerInsetDis * otherWater.radius * Time.deltaTime / radius;
        dRadius = Mathf.Clamp(dRadius, 0, radius);
        radius -= dRadius;
        otherWater.radius += dRadius;

        UpdateScale();
        UpdateMass();
        otherWater.UpdateScale();
        otherWater.UpdateMass();
    }

    public void MergeWith(SimWater otherWater) {
        if (radius < otherWater.radius || otherWater.gameObject == null || otherWater.radius >= simSetting.waterMergeMaxRadius || !isCanMerge || !otherWater.isCanMerge) {
            return;
        }
        // transform.position = (transform.position * rb.mass + otherWater.transform.position * otherWater.rb.mass) / (rb.mass + otherWater.rb.mass);
        // Vector3 mergePos = (transform.position * Mathf.Pow(rb.mass, 2.0f) + otherWater.transform.position * Mathf.Pow(otherWater.rb.mass, 2.0f)) / (Mathf.Pow(rb.mass, 2.0f) + Mathf.Pow(otherWater.rb.mass, 2.0f));
        // transform.position = mergePos;
        radius = Mathf.Sqrt(radius2 + otherWater.radius2);

        UpdateScale();
        UpdateMass();

        otherWater.DestorySelf();
    }

    public void UpdateMass() {
        rb.mass = radius2;
    }

    public void UpdateScale() {
        transform.localScale = Vector3.one * radius;
    }

    public void DestorySelf() {
        DOTween.Complete(transform);
        GameObject.Destroy(gameObject);
    }

}
