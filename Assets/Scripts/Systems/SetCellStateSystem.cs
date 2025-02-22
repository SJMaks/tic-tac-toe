using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UIElements;

public class SetCellStateSystem : IEcsRunSystem
{
    private EcsFilter _filter;
    private EcsPool<ClickEventComponent> _clickEvents;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ClickableComponent> _clickables;
    private EcsPool<ParentLinkComponent> _parents;
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickEventComponent>().End();

        _clickEvents = world.GetPool<ClickEventComponent>();
        _cellStates = world.GetPool<CellStateComponent>();
        _clickables = world.GetPool<ClickableComponent>();
        _parents = world.GetPool<ParentLinkComponent>();

        foreach (int entity in _filter)
        {
            ref ClickEventComponent clickEventComponent = ref _clickEvents.Get(entity);

            bool hasState = _cellStates.Has(clickEventComponent.Entity);
            bool hasParent = _parents.Has(clickEventComponent.Entity);

            if (hasState && hasParent)
            {
                SetCellState(clickEventComponent.Entity);
            }
        }
    }
    private void SetCellState(int entity)
    {
        ref CellStateComponent cellState = ref _cellStates.Get(entity);
        ref ClickableComponent clickable = ref _clickables.Get(entity);
        SpriteRenderer sprite = clickable.GameObject.GetComponent<SpriteRenderer>();

        if (cellState.State == CellStates.Empty)
        {
            if (Input.GetMouseButtonDown(0))
            {
                cellState.State = CellStates.Cross;
                sprite.color = Color.red;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                cellState.State = CellStates.Zero;
                sprite.color = Color.blue;
            }
        }
    }
}