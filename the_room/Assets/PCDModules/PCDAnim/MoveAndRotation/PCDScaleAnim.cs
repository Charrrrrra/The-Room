using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PCDScaleAnim : PCDTransformAnim {
    
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;
    public float animTime = 1.0f;
    public Ease animEase = Ease.Linear;

    public override void PlayAnim() {
        DOTween.Kill(transform);
        transform.localScale = startScale;
        transform.DOScale(targetScale, animTime).SetEase(animEase);
    }
}
