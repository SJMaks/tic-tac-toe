using UnityEngine;

[System.Serializable]
public class IntArray
{
    public int[] Array;
}

[CreateAssetMenu(fileName = "WinCombinations", menuName = "Scriptable Objects/WinCombinations")]
public class WinCombinations : ScriptableObject
{
    public IntArray[] Combinations;
}
