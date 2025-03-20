using UnityEditor;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private GameConfiguration _gameConfiguration;

    [SerializeField]
    private SceneAsset _scene;

    private LoaderService _loaderService;

    private void Awake()
    {
        _loaderService = new LoaderService(_gameConfiguration);
    }

    public void LoadScene(int recursion)
    {
        _loaderService.SetRecursionAndLoad(recursion, _scene);
    }
}