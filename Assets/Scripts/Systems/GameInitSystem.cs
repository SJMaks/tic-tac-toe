using Leopotam.EcsLite;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameInitSystem : IEcsInitSystem
{
    EcsWorld _world = null;

    public GameInitSystem(EcsWorld world)
    {
        _world = world;
    }

    public void Init(IEcsSystems systems)
    {
        int cell = _world.NewEntity();

        EcsPool<CellStateComponent> cellStatePool = _world.GetPool<CellStateComponent>();
        EcsPool<ClickableComponent> clickablePool = _world.GetPool<ClickableComponent>();

        ref CellStateComponent cellStateComponent = ref cellStatePool.Add(cell);
        ref ClickableComponent clickableComponent = ref clickablePool.Add(cell);

        Object spawnedCellPrefab = GameObject.Instantiate((GameAssets.i.CellInitData as CellInitData).CellPrefab, Vector3.zero, Quaternion.identity);

        cellStateComponent.State = CellStates.Empty;
        clickableComponent.GameObject = spawnedCellPrefab as GameObject;
    }
}
