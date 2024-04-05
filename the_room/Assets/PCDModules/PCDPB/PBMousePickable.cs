using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBMousePickable : PBPickable
{
    public Transform[] ignoreRaycastParts;

    public override void OnPick() {
        base.OnPick();
        RaycastCursor.GetInstance()?.AddIgnore(transform);
        if (ignoreRaycastParts != null)
            foreach (Transform part in ignoreRaycastParts)
                RaycastCursor.GetInstance()?.AddIgnore(part);
    }

    public override void OnDrop() {
        base.OnDrop();
        RaycastCursor.GetInstance()?.RemoveIgnore(transform);
        if (ignoreRaycastParts != null)
            foreach (Transform part in ignoreRaycastParts)
                RaycastCursor.GetInstance()?.RemoveIgnore(part);
    }
}
