using UnityEngine;

public class FuncHook<T> : MonoBehaviour
{
    [SerializeField]
    public System.Func<T> hookFunc;
    [SerializeField]
    public Object attachedObj;

    public void AttachObject(Object comp) {
        if (attachedObj != null && attachedObj != comp)
            throw new System.InvalidOperationException("Attempted to attach FuncHook to different components!");
        attachedObj = comp;
    }

    public T InvokeHook(Component caller) {
        if (attachedObj == null)
            throw new System.InvalidOperationException("Attempted to invoke hook without attach to it!");
        if (attachedObj != caller)
            throw new System.InvalidOperationException("Attempted to invoke hook attached with another component!");

        if (hookFunc == null)
            return default(T);
        return hookFunc.Invoke();
    }
}
