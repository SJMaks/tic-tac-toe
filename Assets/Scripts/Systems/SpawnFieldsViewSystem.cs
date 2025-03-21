using Leopotam.EcsLite;
using UnityEngine;
using static Unity.VisualScripting.Metadata;
using UnityEngine.UIElements;
using Leopotam.EcsLite.Di;
using Unity.VisualScripting;

public class SpawnFieldsViewSystem : IEcsInitSystem
{
    private readonly EcsCustomInject<FieldInitData> _fieldConfig = default;

    private EcsFilter _filter;

    private EcsPool<TransformComponent> _positions;
    private EcsPool<CellLevelComponent> _levels;
    private EcsPool<ClickableComponent> _clickables;
    private EcsPool<ChildrenLinkComponent> _children;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _positions = world.GetPool<TransformComponent>();
        _levels = world.GetPool<CellLevelComponent>();
        _clickables = world.GetPool<ClickableComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();

        _filter = world.Filter<MainFieldComponent>().Inc<CellStateComponent>().End();

        foreach (int entity in _filter)
        {
            spawnField(entity);
        }
    }

    private Transform spawnField(int entity)
    {
        ref TransformComponent position = ref _positions.Get(entity);
        ref CellLevelComponent level = ref _levels.Get(entity);

        GameObject spawnedFieldPrefab = GameObject.Instantiate(
                _fieldConfig.Value.FieldPrefab,
                position.Position,
                Quaternion.identity);
        Transform currentFieldTransform = spawnedFieldPrefab.GetComponent<Transform>();

        currentFieldTransform.localScale = new Vector3(
                _fieldConfig.Value.FieldScales[level.Level - 1],
                _fieldConfig.Value.FieldScales[level.Level - 1],
                1f);

        ref ChildrenLinkComponent children = ref _children.Get(entity);

        foreach (int child in children.Children)
        {
            bool isClickable = _clickables.Has(child);

            if (!isClickable)
            {
                Transform childFieldTransform = spawnField(child);
                childFieldTransform.parent = currentFieldTransform;
            }
        }

        return currentFieldTransform;
    }
}
