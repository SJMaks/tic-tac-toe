using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveClickEventSystem : IEcsRunSystem
{
    private EcsFilter _filter;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickEventComponent>().End();

        foreach (int entity in _filter)
        {
            world.DelEntity(entity);
        }
    }
}
