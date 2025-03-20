using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private CellInitData _cellInitData;
    [SerializeField]
    private FieldInitData _fieldInitData;
    [SerializeField]
    private WinCombinations _winCombinations;
    [SerializeField]
    private GameConfiguration _gameConfiguration;

    private EcsWorld _world;
    private EcsSystems _systems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems
            .Add(new PlayerInputInitSystem())
            .Inject(_gameConfiguration)
            .Add(new InitCellsSystem())
            .Add(new SpawnCellsSystem())
            .Inject(_cellInitData)
            .Inject(_fieldInitData)
            .Add(new SpawnFieldsViewSystem())
            .Inject(_fieldInitData)
            .Add(new AddClickEventSystem())
            .Add(new SwitchPlayerInputTurnSystem())
            .Add(new SetCellStateSystem())
            .Add(new CheckCellStateSystem())
            .Inject(_winCombinations)
            .Add(new SetCellSpriteSystem())
            .Inject(_cellInitData)
            .Add(new BlockCellsSystem())
            .Add(new SetCellBlockSpriteSystem())
            .Inject(_cellInitData)
            .Add(new RemoveClickEventSystem())
            .Inject(_gameConfiguration)
            .Init();
    }

    private void Update()
    {
        _systems.Run();
    }

    private void OnDestroy()
    {
        _systems.Destroy();

        _world.Destroy();
    }
}
