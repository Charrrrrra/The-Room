using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

[CreateAssetMenu(fileName = "StaticObjectSet", menuName = "Custom/StaticObjectSet", order = 1)]
public class StaticObjectSet : ScriptableObject
{
    [SerializeField]
    private List<ManagedItem<Object>> managedItems = new List<ManagedItem<Object>>();

    void OnValidate() {
        // Check for duplicate names
        var duplicateNames = managedItems.GroupBy(x => x.name).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
        if (duplicateNames.Count > 0) {
            Debug.LogError($"StaticObjectSet: Duplicate names found - {string.Join(", ", duplicateNames)}");
        }

        // Retrieve the objectName value, whether it's a property or a field
        for (int i = 0; i < managedItems.Count; i++) {
            if (string.IsNullOrEmpty(managedItems[i].name) && managedItems[i].item != null) {
                var type = managedItems[i].item.GetType();

                // Get the value of objectName, either from a property or a field
                object objectNameValue = type.GetProperty("objectName", BindingFlags.Public | BindingFlags.Instance)?.GetValue(managedItems[i].item)
                                    ?? type.GetField("objectName", BindingFlags.Public | BindingFlags.Instance)?.GetValue(managedItems[i].item);

                if (objectNameValue != null) {
                    managedItems[i] = new ManagedItem<Object>(objectNameValue.ToString(), managedItems[i].item);
                }
            }
        }
    }

    public T GetObject<T>(string name) where T : Object {
        var foundItem = managedItems.FirstOrDefault(x => x.name == name && x.item is T);
        if (foundItem.item != null) {
            return foundItem.item as T;
        }
        return null;
    }
}

[System.Serializable]
public struct ManagedItem<T>
{
    public string name;
    public T item;

    public ManagedItem(string name, T item) {
        this.name = name;
        this.item = item;
    }
}