using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EasyKeyLock : MonoBehaviour {
    public int lockNum = 5;
    public int unlockDelay = 500;
    public UnityEvent onUnlock;
    public void UnLockNum(int keyNum) {
        lockNum -= keyNum;
        if (lockNum <= 0) {
            OnUnlock();
        }
    }

    public void OnUnlock() {
        Action action = async () => {
            await Task.Delay(unlockDelay);
            onUnlock?.Invoke();
        };
        action.Invoke();
    }
}
