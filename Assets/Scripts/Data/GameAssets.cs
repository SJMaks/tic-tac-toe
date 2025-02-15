using UnityEngine;

///TODO: Скорее всего не нужен, т.к. я установил DI для внедрения компонентов напрямую в Startup-е
public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("Data/GameAssets")) as GameObject).GetComponent<GameAssets>();
            }
            return _i;
        }
    }

    public Object CellInitData;
}
