using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaticObjectList", menuName = "Custom/StaticObjectList", order = 1)]
public class StaticObjectList : ScriptableObject
{
    public List<Object> managedItems = new List<Object>();
}