using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private CellInitData _cellInitData;

    private EcsWorld _world;
    private EcsSystems _systems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems
            .Add(new GameInitSystem(_world))
            .Add(new CellClickSystem())
            .Inject(_cellInitData)
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
