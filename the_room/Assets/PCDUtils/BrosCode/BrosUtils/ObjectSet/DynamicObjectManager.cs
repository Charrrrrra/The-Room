using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class DynamicObjectManager : SingletonMono<DynamicObjectManager>
{
    [SerializeField]
    private List<ManagedItem<GameObject>> preRegistedItems = new List<ManagedItem<GameObject>>();

    private Dictionary<string, List<Object>> objectDict;
	private void Awake() {
        objectDict = new Dictionary<string, List<Object>>();
        foreach (var item in preRegistedItems) {
            RegisterObject<GameObject>(item.name, item.item);
		}
    }

	void OnValidate() {
        // Check for duplicate names
        var duplicateNames = preRegistedItems.GroupBy(x => x.name).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
        if (duplicateNames.Count > 0) {
            Debug.LogError($"DynamicObjectManager: Duplicate names found - {string.Join(", ", duplicateNames)}");
        }

        // Retrieve the objectName value, whether it's a property or a field
        for (int i = 0; i < preRegistedItems.Count; i++) {
            if (string.IsNullOrEmpty(preRegistedItems[i].name) && preRegistedItems[i].item != null) {
                var type = preRegistedItems[i].item.GetType();

                // Get the value of objectName, either from a property or a field
                object objectNameValue = type.GetProperty("objectName", BindingFlags.Public | BindingFlags.Instance)?.GetValue(preRegistedItems[i].item)
                                    ?? type.GetField("objectName", BindingFlags.Public | BindingFlags.Instance)?.GetValue(preRegistedItems[i].item);

                if (objectNameValue != null) {
                    preRegistedItems[i] = new ManagedItem<GameObject>(objectNameValue.ToString(), preRegistedItems[i].item);
                }
            }
        }
    }

    public void RegisterObject<T>(string name, T item) where T : Object {
        if (objectDict.TryGetValue(name, out List<Object> list)) {
            list.Add(item);
		} else {
            objectDict[name] = new List<Object> { item };
        }
    }

    public T GetObject<T>(string name) where T : Object {
        if (objectDict.TryGetValue(name, out List<Object> list)) {
            foreach (var item in list) {
                if (item is T)
                    return item as T;
			}
		}
        return null;
    }
}

