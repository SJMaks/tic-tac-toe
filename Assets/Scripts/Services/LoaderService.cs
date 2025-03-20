using UnityEditor;
using UnityEngine.SceneManagement;

public class LoaderService
{
    private readonly GameConfiguration _gameConfiguration;

    public LoaderService(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
    }

    public void SetRecursionAndLoad(int recursionLevel, SceneAsset sceneAsset)
    {
        _gameConfiguration.RecursionLevel = recursionLevel;
        LoadScene(sceneAsset);
    }

    public void LoadScene(SceneAsset scene)
    {
        SceneManager.LoadScene(scene.name);
    }
}