using Leopotam.EcsLite;
using UnityEngine;

public class Loader : MonoBehaviour
{
    EcsWorld world;
    EcsSystems systems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        world = new EcsWorld();
        systems = new EcsSystems(world);

        systems.Add(new GameInitSystem(world));
        systems.Add(new CellClickSystem());

        systems.Init();
    }

    private void Update()
    {
        systems.Run();
    }

    private void OnDestroy()
    {
        systems.Destroy();

        world.Destroy();
    }
}
