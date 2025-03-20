using UnityEngine;

[CreateAssetMenu(fileName = "FieldInitData", menuName = "Scriptable Objects/FieldInitData")]
public class FieldInitData : ScriptableObject
{
    public GameObject FieldPrefab;
    public float[] FieldScales =
    {
        0.75f,
        2f,
        4f
    };
}
