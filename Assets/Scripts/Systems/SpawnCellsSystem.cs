using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;

public class SpawnCellsSystem : IEcsInitSystem
{
    private readonly EcsCustomInject<CellInitData> _cellConfig = default;

    private EcsFilter _filter;
    private EcsPool<ClickableComponent> _clickables;
    private EcsPool<PositionComponent> _positions;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _clickables = world.GetPool<ClickableComponent>();
        _positions = world.GetPool<PositionComponent>();

        _filter = world.Filter<CellStateComponent>().Exc<ClickableComponent>().End();

        Transform parentCellTransform = null;
        foreach (int entity in _filter)
        {
            Object parentCell = new GameObject("Parent Cell");
            parentCellTransform = parentCell.GetComponent<Transform>();
        }

        _filter = world.Filter<CellStateComponent>().Inc<ClickableComponent>().Inc<PositionComponent>().End();

        foreach (int entity in _filter)
        {
            if (parentCellTransform != null)
            {
                ref ClickableComponent clickable = ref _clickables.Get(entity);
                ref PositionComponent position = ref _positions.Get(entity);

                Object spawnedCellPrefab = GameObject.Instantiate(_cellConfig.Value.CellPrefab, position.Position, Quaternion.identity);
                Transform spawnedCellTransform = spawnedCellPrefab.GetComponent<Transform>();
                spawnedCellTransform.parent = parentCellTransform;

                clickable.GameObject = spawnedCellPrefab as GameObject;
            }
        }
    }
}
