using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 bias = new Vector3(0f, 200f, 0f);

	public void LateUpdate() {
		Vector3 pos = Camera.main.WorldToScreenPoint(target.position) + bias;
		transform.position = pos;
	}
}
