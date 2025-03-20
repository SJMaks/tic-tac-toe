using UnityEngine;

[CreateAssetMenu(fileName = "CellInitData", menuName = "Scriptable Objects/CellInitData")]
public class CellInitData : ScriptableObject
{
    public GameObject CellPrefab;
    public Sprite zero;
    public Sprite cross;
    public Sprite block;
}
