using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaticStringList", menuName = "Custom/StaticStringList", order = 1)]
public class StaticStringList : ScriptableObject
{
    public List<string> managedItems = new List<string>();
}