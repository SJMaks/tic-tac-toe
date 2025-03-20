using UnityEngine;

[CreateAssetMenu(fileName = "GameConfiguration", menuName = "Scriptable Objects/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{
    [Range(1, 3)]
    public int RecursionLevel = 1;
}
