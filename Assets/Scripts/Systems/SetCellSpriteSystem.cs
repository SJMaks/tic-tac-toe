using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class SetCellSpriteSystem : IEcsRunSystem
{
    private readonly EcsCustomInject<CellInitData> _cellConfig = default;

    private EcsFilter _filter;
    private EcsPool<ClickEventComponent> _clickEvents;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ParentLinkComponent> _parents;
    private EcsPool<GameObjectComponent> _gameObjectComponents;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickEventComponent>().End();

        _clickEvents = world.GetPool<ClickEventComponent>();
        _cellStates = world.GetPool<CellStateComponent>();
        _parents = world.GetPool<ParentLinkComponent>();
        _gameObjectComponents = world.GetPool<GameObjectComponent>();

        foreach (int entity in _filter)
        {
            ref ClickEventComponent clickEventComponent = ref _clickEvents.Get(entity);

            bool hasState = _cellStates.Has(clickEventComponent.Entity);
            bool hasParent = _parents.Has(clickEventComponent.Entity);

            if (hasState && hasParent)
            {
                SetCellSprite(clickEventComponent.Entity, 1);
            }
        }
    }


    private void SetCellSprite(int entity, int order)
    {
        ref CellStateComponent currentStateComponent = ref _cellStates.Get(entity);
        ref GameObjectComponent gameObjectComponent = ref _gameObjectComponents.Get(entity);

        SpriteRenderer spriteRenderer = gameObjectComponent.GameObject.GetComponent<Transform>()
            .Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = order;


        if (currentStateComponent.State == CellStates.Cross)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = _cellConfig.Value.cross;
        }
        else if (currentStateComponent.State == CellStates.Zero)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.sprite = _cellConfig.Value.zero;
        }

        if (_parents.Has(entity))
        {
            ref ParentLinkComponent parentComponent = ref _parents.Get(entity);
            SetCellSprite(parentComponent.Parent, order + 1);
        }
    }
}
