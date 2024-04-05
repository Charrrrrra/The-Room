using System;

namespace Bros.Utils
{
    public class RefFunc<T1, T2>
    {
        public T2 Invoke(T1 t1) {
            string funcName = null;
            var fieldInfo = GetType().GetField("selector");
            if (fieldInfo != null) {
                funcName = fieldInfo.GetValue(this).ToString();
            }
            if (funcName != null) {
                var methodInfo = GetType().GetMethod(funcName);
                if (methodInfo != null) {
                    Func<T1, T2> funcDelegate = (Func<T1, T2>)Delegate.CreateDelegate(typeof(Func<T1, T2>), methodInfo);
                    return funcDelegate.Invoke(t1);
                }
            }

            throw new System.Exception("Method not found!");
        }
    }
}
