using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDTransformAnim : MonoBehaviour {
    public bool playOnAwake;

    protected virtual void Awake() {
        if (playOnAwake) {
            PlayAnim();
        }
    }

    public virtual void PlayAnim() {
        
    }
    
    public virtual void StopAnim (){

    }
}
