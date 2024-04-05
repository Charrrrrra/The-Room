using System;
using System.Reflection;
using UnityEngine;

public static class EasyPreview
{
    static public GameObject CreatePreview(GameObject target, Color? baseColor = null, Shader shader = null) {
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        Color color = baseColor.HasValue ? baseColor.Value : new Color(1f, 1f, 1f, 0.25f);
        GameObject preview = CreatePreviewRecursive(target, color, shader);
        CopyColliderAsTrigger(preview, target);
        return preview;
    }

    static public GameObject CreatePreviewWithFullGFX(GameObject target) {
        GameObject preview = CreatePreviewRecursiveWithFullGFX(target);
        CopyColliderAsTrigger(preview, target);
        return preview;
    }

    static public void ChangePreviewColor(GameObject preview, Color color) {
        foreach (var renderer in preview.GetComponentsInChildren<MeshRenderer>()) {
            foreach (var mat in renderer.sharedMaterials) {
                mat.color = color;
            }
        }
    }

    static private GameObject CreatePreviewRecursive(GameObject target, Color color, Shader shader) {
        GameObject preview = new GameObject(target.name + "_preview");
        CopyGraphcis(preview, target, color, shader);

        /* Recursive copy children */
        for (int i = 0 ; i < target.transform.childCount; i++) {
            Transform child = target.transform.GetChild(i);
            GameObject childPreview = CreatePreviewRecursive(child.gameObject, color, shader);
            childPreview.transform.SetParent(preview.transform);
        }
        return preview;
    }

    static public GameObject CreatePreviewRecursiveWithFullGFX(GameObject target) {
        GameObject preview = new GameObject(target.name + "_preview");
        CopyGraphcisWithFullGFX(preview, target);

        /* Recursive copy children */
        for (int i = 0 ; i < target.transform.childCount; i++) {
            Transform child = target.transform.GetChild(i);
            GameObject childPreview = CreatePreviewRecursiveWithFullGFX(child.gameObject);
            childPreview.transform.SetParent(preview.transform);
        }
        return preview;
    }

    static private void CopyGraphcis(GameObject preview, GameObject target, Color color, Shader shader) {
        /* Copy base transform settings */
        preview.transform.position = target.transform.position;
        preview.transform.rotation = target.transform.rotation;
        Vector3 targetGlobalScale = target.transform.lossyScale;
        Vector3 parentGlobalScale = preview.transform.parent ? preview.transform.parent.lossyScale : Vector3.one;
        preview.transform.localScale = new Vector3(
            parentGlobalScale.x != 0 ? targetGlobalScale.x / parentGlobalScale.x : 0,
            parentGlobalScale.y != 0 ? targetGlobalScale.y / parentGlobalScale.y : 0,
            parentGlobalScale.z != 0 ? targetGlobalScale.z / parentGlobalScale.z : 0
        );

        /* Copy MeshFilter and MeshRenderer */
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        if (meshFilter != null && meshRenderer != null) {
            var newMeshFilter = preview.AddComponent<MeshFilter>();
            newMeshFilter.mesh = Mesh.Instantiate(meshFilter.sharedMesh);

            var newMeshRenderer = preview.AddComponent<MeshRenderer>();
            Material[] materials = new Material[meshRenderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++) {
                materials[i] = new Material(shader) {
                    mainTexture = meshRenderer.sharedMaterials[i]?.mainTexture
                };
                materials[i].SetFloat("_Surface", 1); // Transparent
                materials[i].SetFloat("_Blend", 0); // Blend Mode
                materials[i].color = color;

                materials[i] = Material.Instantiate(materials[i]);
            }
            newMeshRenderer.materials = materials;
        }
    }

    static private void CopyGraphcisWithFullGFX(GameObject preview, GameObject target) {
        /* Copy base transform settings */
        preview.transform.position = target.transform.position;
        preview.transform.rotation = target.transform.rotation;
        Vector3 targetGlobalScale = target.transform.lossyScale;
        Vector3 parentGlobalScale = preview.transform.parent ? preview.transform.parent.lossyScale : Vector3.one;
        preview.transform.localScale = new Vector3(
            parentGlobalScale.x != 0 ? targetGlobalScale.x / parentGlobalScale.x : 0,
            parentGlobalScale.y != 0 ? targetGlobalScale.y / parentGlobalScale.y : 0,
            parentGlobalScale.z != 0 ? targetGlobalScale.z / parentGlobalScale.z : 0
        );

        /* Copy MeshFilter and MeshRenderer */
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        if (meshFilter != null && meshRenderer != null) {
            var newMeshFilter = preview.AddComponent<MeshFilter>();
            newMeshFilter.mesh = Mesh.Instantiate(meshFilter.sharedMesh);

            var newMeshRenderer = preview.AddComponent<MeshRenderer>();
            Material[] materials = new Material[meshRenderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++) {
                materials[i] = Material.Instantiate(meshRenderer.sharedMaterials[i]);
            }
            newMeshRenderer.materials = materials;
        }
    }

    static private void CopyColliderAsTrigger(GameObject preview, GameObject target) {
        foreach (var collider in target.GetComponents<Collider>()) {
            Type type = collider.GetType();
            var copy = preview.AddComponent(type);
            ComponentUtils.NaiveCopy(collider, copy, type, true, true);
            if (copy is MeshCollider meshCollider)
                meshCollider.convex = true;
            (copy as Collider).enabled = true;
            (copy as Collider).isTrigger = true;
        }
    }
}