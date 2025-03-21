using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;

public class SpawnCellsSystem : IEcsInitSystem
{
    private readonly EcsCustomInject<CellInitData> _cellConfig = default;
    private readonly EcsCustomInject<FieldInitData> _fieldConfig = default;

    private EcsFilter _filter;
    private EcsPool<ClickableComponent> _clickables;
    private EcsPool<TransformComponent> _positions;
    private EcsPool<ChildrenLinkComponent> _children;
    private EcsPool<GameObjectComponent> _gameObjects;
    private EcsPool<CellLevelComponent> _levels;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _clickables = world.GetPool<ClickableComponent>();
        _positions = world.GetPool<TransformComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();
        _gameObjects = world.GetPool<GameObjectComponent>();
        _levels = world.GetPool<CellLevelComponent>();

        _filter = world.Filter<MainFieldComponent>().Inc<CellStateComponent>().End();

        foreach (int entity in _filter)
        {
            spawnCells(entity);
        }
    }

    private Transform spawnCells(int entity)
    {
        bool isClickable = _clickables.Has(entity);

        ref TransformComponent position = ref _positions.Get(entity);
        ref CellLevelComponent currentLevelComponent = ref _levels.Get(entity);

        Object spawnedCellPrefab = GameObject.Instantiate(_cellConfig.Value.CellPrefab, position.Position, Quaternion.identity);
        Transform currentCellTransform = spawnedCellPrefab.GetComponent<Transform>();
        position.Scale = currentCellTransform.localScale;
        Transform spriteTransform = spawnedCellPrefab.GetComponent<Transform>().Find("Sprite");
        SpriteRenderer spriteRenderer = spriteTransform.gameObject.GetComponent<SpriteRenderer>();
        spriteTransform.localScale = new Vector3(
                _fieldConfig.Value.FieldScales[currentLevelComponent.Level] / 4,
                _fieldConfig.Value.FieldScales[currentLevelComponent.Level] / 4,
                1);
        //spriteTransform.localPosition = currentPositionComponent.Position;

        if (!isClickable)
        { 
            BoxCollider2D currentBoxCollider = spawnedCellPrefab.GetComponent<BoxCollider2D>();
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

        ref GameObjectComponent objectComponent = ref _gameObjects.Get(entity);
        objectComponent.GameObject = spawnedCellPrefab as GameObject;

        return currentCellTransform;
    }
}
