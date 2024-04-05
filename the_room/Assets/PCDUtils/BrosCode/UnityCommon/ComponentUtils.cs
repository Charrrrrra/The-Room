using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public static class ComponentUtils
{
    const BindingFlags NaiveCopyFieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    const BindingFlags NaiveCopyPropertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    static public void NaiveCopy<T>(T src, T dst, bool copyFileds = true, bool copyProperties = true) where T : Component {
        NaiveCopy(src, dst, typeof(T));
    }

    static public void NaiveCopy(Component src, Component dst, Type type, bool copyProperties = true, bool copyFileds = true) {
        if (src.GetType() != type || dst.GetType() != type)
            throw new Exception("Invalid arguments!");

        /* Loop until Component or MonoBehavior */
        while (type != typeof(Component) && type != typeof(MonoBehaviour) && type != null) {
            /* Copy properties */
            if (copyProperties) {
                foreach (var property in type.GetProperties(NaiveCopyPropertyFlags)) {
                    try {
                        if (!property.CanWrite)
                            continue;
                        property.SetValue(dst, property.GetValue(src));
                    } catch {}
                }
            }
            /* Copy fields */
            if (copyFileds) {
                foreach (var field in type.GetFields(NaiveCopyFieldFlags)) {
                    try {
                        if (field.IsInitOnly || field.IsStatic)
                            continue;
                        field.SetValue(dst, field.GetValue(src));
                    } catch {}
                }
            }
            type = type.BaseType;
        }
    }
}