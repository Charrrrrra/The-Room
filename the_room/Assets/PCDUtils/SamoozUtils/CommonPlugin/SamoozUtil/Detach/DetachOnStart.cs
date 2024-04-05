using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachOnStart : MonoBehaviour {
    void Start() {
        transform.SetParent(null);
    }

}
