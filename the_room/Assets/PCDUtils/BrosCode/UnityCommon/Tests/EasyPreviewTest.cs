using System;
using UnityEngine;
using UnityEditor;

public class EasyPreviewTest : MonoBehaviour {
    public GameObject testObject;
    public Color testColor;
    public Shader testShader;
    public bool autoTestOnStart;

    private void Start() {
        if (autoTestOnStart)
            TestCreatePreview();
    }

    public void TestCreatePreview() {
        GameObject preview = EasyPreview.CreatePreview(testObject, testColor, testShader);
        preview.transform.position = testObject.transform.position;
        testObject.SetActive(false);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EasyPreviewTest))]
public class EasyPreviewTestEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        EasyPreviewTest tester = (EasyPreviewTest)target;
        if (EditorApplication.isPlaying) {
            if (GUILayout.Button("Test Create Preview")) {
                tester.TestCreatePreview();
            }
        }
    }
}
#endif