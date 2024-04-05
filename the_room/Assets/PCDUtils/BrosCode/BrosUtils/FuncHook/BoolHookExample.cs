using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolHook : FuncHook<bool> {
    private enum Functionlector {
        AlwaysActive
    }

    static private Dictionary<Functionlector, Func<bool>> funcDict = new Dictionary<Functionlector, Func<bool>>() {
        { Functionlector.AlwaysActive, AlwaysActive }
    };

    static private bool AlwaysActive() {
        return true;
    }

    /* --- don't need to change --- */
    [SerializeField]
    private Functionlector funcOption;

    private void Awake() {
        hookFunc = CallFunction;
    }

    private bool CallFunction() {
        return funcDict[funcOption].Invoke();
    }
}
