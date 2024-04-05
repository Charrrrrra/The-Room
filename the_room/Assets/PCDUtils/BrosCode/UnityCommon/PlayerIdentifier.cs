using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentifier : SingletonMono<PlayerIdentifier>
{
	static public Transform Player { get => Instance.player; }

	public Transform player;
	public GameObject[] relatedGameObjects;

	public void SetPlayer(Transform player) {
		this.player = player;
	}

	public void SetPlayerActive(bool isActive) {
		foreach (var obj in relatedGameObjects) {
			obj.SetActive(isActive);
		}
	}
}