using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventEmitter : MonoBehaviour
{
    public AnimationEvent[] animEvents;
    private Dictionary<string, UnityEvent> animEventDic;

    void Awake() {
        animEventDic = new Dictionary<string, UnityEvent>();
        foreach (AnimationEvent animEvent in animEvents) {
            if (!animEventDic.ContainsKey(animEvent.eventName)) {
                animEventDic.Add(animEvent.eventName, animEvent.uEvent);
            }
        }
    }

    public void TriggerEventByName(string eventName) {
        animEventDic[eventName]?.Invoke();
    }

    public void TriggerChildFuncByName(string message) {
        BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
    }

    public void TriggerParentFuncByName(string message) {
        SendMessageUpwards(message, SendMessageOptions.DontRequireReceiver);
    }

    public void TriggerFuncByName(string message) {
        SendMessage(message, SendMessageOptions.DontRequireReceiver);
    }

    [System.Serializable]
    public class AnimationEvent {
        public string eventName;
        public UnityEvent uEvent;
    }

}
