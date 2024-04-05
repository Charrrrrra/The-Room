using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PCDForward : MonoBehaviour {
    public Transform center;
    public Transform forward;
    public Transform up;
    public float lerpSpeed = 5.0f;
    public bool clearY = false;
    
    private void Update() => UpdateForward();
    private void LateUpdate() => UpdateForward();

    private void UpdateForward() {
        if (lerpSpeed < 0) {
            if (!up) {
                if (clearY) {
                    transform.rotation = Quaternion.LookRotation((forward.position - center.position).ClearY().normalized, Vector3.up);
                } else {
                    transform.rotation = Quaternion.LookRotation((forward.position - center.position).normalized, Vector3.up);
                }
            } else {
                if (clearY) {
                    transform.rotation = Quaternion.LookRotation((forward.position - center.position).ClearY().normalized, (up.position - center.position).normalized);
                } else {
                    transform.rotation = Quaternion.LookRotation((forward.position - center.position).normalized, (up.position - center.position).normalized);
                }
            }
        } else {
            if (!up) {
                if (clearY) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((forward.position - center.position).ClearY().normalized, Vector3.up), lerpSpeed * Time.deltaTime);
                } else {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((forward.position - center.position).normalized, Vector3.up), lerpSpeed * Time.deltaTime);
                }
            } else {
                if (clearY) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((forward.position - center.position).ClearY().normalized, (up.position - center.position).normalized), lerpSpeed * Time.deltaTime);
                } else {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((forward.position - center.position).normalized, (up.position - center.position).normalized), lerpSpeed * Time.deltaTime);
                }
                
            }
        }
    }
}
