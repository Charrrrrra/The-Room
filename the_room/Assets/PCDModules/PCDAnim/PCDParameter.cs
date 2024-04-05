using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PCDParameter : MonoBehaviour {
	public Vector3 velocity;
	public Vector3 moveDir;
	// 待添加功能：用目标运动方向代替当前运动方向来判断Roll动画
	public float turnAngle;
	public float speed;
	public IVelocitySyncer velocitySyncer;

	public void UpdateParameter() {
		if (velocitySyncer == null) velocitySyncer = GetComponent<IVelocitySyncer>();
		velocity = velocitySyncer.GetCharacterVelocity();

		Vector3 lastMoveDir = moveDir;
		speed = velocity.magnitude;

		if (speed > 0.01f) {
			moveDir = velocity.ClearY().normalized;
		}
		if (moveDir == Vector3.zero || lastMoveDir == Vector3.zero) {
			turnAngle = 0;
		} else {
			turnAngle = Vector3.Angle(lastMoveDir, moveDir);
		}
	}

}
