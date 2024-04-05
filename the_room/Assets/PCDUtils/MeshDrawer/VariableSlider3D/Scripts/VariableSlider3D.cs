using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(TransfromLimiter_MoveOnLine))]
public class VariableSlider3D : MonoBehaviour {

    [Header("Variable Binding")]
    public Component targetScript;
    public string targetVariable;

    [Space]
    [Header("Value Setting")]
    public Vector2 valueRange = new Vector2(0, 1);

    [Space]
    [Header("Info")]
    [Range(0, 1)]
    public float process;
    private float lastProcess;
    public float testBindValue;

    private TransfromLimiter_MoveOnLine lineLimiter;

    private void Awake() {
        lineLimiter = GetComponent<TransfromLimiter_MoveOnLine>();
    }

    private void Update() {

        process = lineLimiter.process;

        if (process != lastProcess) {
            OnProcessChanged();
        }

        lastProcess = process;

    }

    private void OnProcessChanged() {
        SetBindingVariableValue(Mathf.Lerp(valueRange.x, valueRange.y, process));
    }

    private void SetBindingVariableValue(float value) {

        if (targetScript && !string.IsNullOrEmpty(targetVariable)) {
            System.Reflection.FieldInfo field = targetScript.GetType().GetField(targetVariable);
            if (field != null && field.FieldType == typeof(float)) {
                field.SetValue(targetScript, value);
                return;
            }       
        }

    }


}