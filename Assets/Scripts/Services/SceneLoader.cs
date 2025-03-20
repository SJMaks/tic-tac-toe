using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SceneLoadData
{
    public SceneAsset Scene;
    public int Recursion;
}

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private GameConfiguration _gameConfiguration;

    [SerializeField]
    private SceneLoadData _sceneLoadData;

    private LoaderService _loaderService;

    private void Awake()
    {
        _loaderService = new LoaderService(_gameConfiguration);
    }

    public void LoadScene()
    {
        _loaderService.SetRecursionAndLoad(_sceneLoadData.Recursion, _sceneLoadData.Scene);
    }
}