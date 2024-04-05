using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCursor : SingletonMono<RaycastCursor>
{
    public LayerMask focusLayer;
    public Transform focusingObj;
    public Transform cursor;
    public float maxRaycastDis = 100.0f;
    public List<Transform> ignoreList = new List<Transform>();
    protected RaycastHit hitInfo;

    protected virtual void Update() {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitInfos = Physics.RaycastAll(mouseRay, maxRaycastDis, focusLayer);
        if (hitInfos.Length > 0) {
            RaycastHit firstHitableInfo = GetFirstRaycastableInfo(hitInfos, mouseRay.origin);
            if (firstHitableInfo.transform == null) {
                return;
            }
            hitInfo = firstHitableInfo;   
            focusingObj = hitInfo.collider.transform;
            Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal, Color.red);
            cursor.position = hitInfo.point;
            cursor.up = Vector3.up;
        } else {
            focusingObj = null;
        }
    }

    private RaycastHit GetFirstRaycastableInfo(RaycastHit[] hitInfos, Vector3 rayStartPoint) {
        float closestDistance = float.MaxValue;
        RaycastHit closestHitInfo = new RaycastHit();

        foreach (RaycastHit hitInfo in hitInfos) {
            if (!ignoreList.Contains(hitInfo.transform)) {
                float distance = hitInfo.distance;
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestHitInfo = hitInfo;
                }
            }
        }

        return closestHitInfo; // 返回最近的对象，如果没有找到，则为null
    }

    public void AddIgnore(Transform ignoreObj) {
        if (ignoreList.Contains(ignoreObj)) {
            return;
        }
        ignoreList.Add(ignoreObj);
    }

    public void RemoveIgnore(Transform ignoreObj) {
        if (ignoreList.Contains(ignoreObj)) {
            ignoreList.Remove(ignoreObj);
        }
    }

}
