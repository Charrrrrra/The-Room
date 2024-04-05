using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetCompHelper
{
    static public T[] GetComponentsInChildrenAndChildren<T>(this Transform parent) where T : Component {
        List<T> foundComponentsList = new List<T>();

        // 查找父物体上的组件
        T component = parent.GetComponent<T>();

        // 如果找到了组件，将它添加到列表中
        if (component != null)
        {
            foundComponentsList.Add(component);
        }

        // 查找子物体的子物体
        foreach (Transform child in parent.transform)
        {
            // 递归调用以查找子物体的子物体并将它们的组件添加到列表中
            T[] childComponents = child.GetComponentsInChildrenAndChildren<T>();
            foundComponentsList.AddRange(childComponents);
        }

        // 将List转换为数组并返回
        return foundComponentsList.ToArray();
    }
}
