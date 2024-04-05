using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bros.Utils {

    /* Calls function on every Update until it returns true */
    public class FuncUpdater {

        /* Class to hook Actions into MonoBehaviour */
        private class MonoBehaviourHook : MonoBehaviour {
            public Action OnUpdate;

            private void Update() {
                if (OnUpdate != null) OnUpdate();
            }
        }

        private static List<FuncUpdater> updaterList; // Holds a reference to all active updaters
        private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded() {
            if (initGameObject == null) {
                initGameObject = new GameObject("FuncUpdaterGlobal");
                updaterList = new List<FuncUpdater>();
            }
        }

        public static FuncUpdater Create(Action updateFunc) {
            return Create(() => { updateFunc(); return false; }, "", true, false);
        }
        public static FuncUpdater Create(Func<bool> updateFunc) {
            return Create(updateFunc, "", true, false);
        }
        public static FuncUpdater Create(Func<bool> updateFunc, string functionName) {
            return Create(updateFunc, functionName, true, false);
        }
        public static FuncUpdater Create(Func<bool> updateFunc, string functionName, bool active) {
            return Create(updateFunc, functionName, active, false);
        }
        public static FuncUpdater Create(Func<bool> updateFunc, string functionName, bool active, bool stopAllWithSameName) {
            InitIfNeeded();

            if (stopAllWithSameName) {
                StopAllUpdatersWithName(functionName);
            }

            GameObject gameObject = new GameObject("FuncUpdaterObject " + functionName, typeof(MonoBehaviourHook));
            gameObject.transform.SetParent(initGameObject.transform);
            FuncUpdater funcUpdater = new FuncUpdater(gameObject, updateFunc, functionName, active);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = funcUpdater.Update;

            updaterList.Add(funcUpdater);
            return funcUpdater;
        }
        private static void RemoveUpdater(FuncUpdater funcUpdater) {
            InitIfNeeded();
            updaterList.Remove(funcUpdater);
        }
        public static void DestroyUpdater(FuncUpdater funcUpdater) {
            InitIfNeeded();
            if (funcUpdater != null) {
                funcUpdater.DestroySelf();
            }
        }
        public static void StopUpdaterWithName(string functionName) {
            InitIfNeeded();
            for (int i = 0; i < updaterList.Count; i++) {
                if (updaterList[i].functionName == functionName) {
                    updaterList[i].DestroySelf();
                    return;
                }
            }
        }
        public static void StopAllUpdatersWithName(string functionName) {
            InitIfNeeded();
            for (int i = 0; i < updaterList.Count; i++) {
                if (updaterList[i].functionName == functionName) {
                    updaterList[i].DestroySelf();
                    i--;
                }
            }
        }


        private GameObject gameObject;
        private string functionName;
        private bool active;
        private Func<bool> updateFunc; // Destroy Updater if return true;

        public FuncUpdater(GameObject gameObject, Func<bool> updateFunc, string functionName, bool active) {
            this.gameObject = gameObject;
            this.updateFunc = updateFunc;
            this.functionName = functionName;
            this.active = active;
        }
        public void Pause() {
            active = false;
        }
        public void Resume() {
            active = true;
        }

        private void Update() {
            if (!active) return;
            if (updateFunc()) {
                DestroySelf();
            }
        }
        public void DestroySelf() {
            RemoveUpdater(this);
            if (gameObject != null) {
                UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
}