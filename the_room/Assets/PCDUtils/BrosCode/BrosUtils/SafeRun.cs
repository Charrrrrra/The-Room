using System;
using UnityEngine;

public static class SafeRun
{
    static public void Run(Action action) {
		try {
			action?.Invoke();
		} catch (Exception ex) {
			Debug.Log("∑¢…˙“Ï≥££∫" + ex.Message);
		}
	}
}
