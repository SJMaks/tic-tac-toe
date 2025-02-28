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
    private EcsPool<ChildrenLinkComponent> _children;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _clickables = world.GetPool<ClickableComponent>();
        _positions = world.GetPool<PositionComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();

        _filter = world.Filter<MainFieldComponent>().Inc<CellStateComponent>().End();

        foreach (int entity in _filter)
        {
            spawnCells(entity);
        }
    }

    private Transform spawnCells(int entity)
    {
        bool isClickable = _clickables.Has(entity);

        ref PositionComponent position = ref _positions.Get(entity);

        Object spawnedCellPrefab = GameObject.Instantiate(_cellConfig.Value.CellPrefab, position.Position, Quaternion.identity);
        Transform currentCellTransform = spawnedCellPrefab.GetComponent<Transform>();

        if (!isClickable)
        { 
            SpriteRenderer currentSpriteRenderer = spawnedCellPrefab.GetComponent<SpriteRenderer>();
            BoxCollider2D currentBoxCollider = spawnedCellPrefab.GetComponent<BoxCollider2D>();
            currentSpriteRenderer.enabled = false;
            Object.Destroy(currentBoxCollider);

            ref ChildrenLinkComponent children = ref _children.Get(entity);

            foreach (int child in children.Children)
            {
                Transform childCellTransform = spawnCells(child);
                childCellTransform.parent = currentCellTransform;
            }
        } 
        else
        {
            ref ClickableComponent clickable = ref _clickables.Get(entity);
            clickable.GameObject = spawnedCellPrefab as GameObject;
        }

        return currentCellTransform;
    }
}
