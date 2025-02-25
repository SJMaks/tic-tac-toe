using Leopotam.EcsLite;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class SwitchPlayerInputTurnSystem : IEcsRunSystem
{
    private EcsFilter _clickEventFilter;
    private EcsFilter _playerInputFilter;
    private EcsPool<ClickEventComponent> _clickEvents;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ParentLinkComponent> _parents;
    private EcsPool<PlayerInputComponent> _playerInputs;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _clickEventFilter = world.Filter<ClickEventComponent>().End();
        _playerInputFilter = world.Filter<PlayerInputComponent>().End();

        _clickEvents = world.GetPool<ClickEventComponent>();
        _cellStates = world.GetPool<CellStateComponent>();
        _parents = world.GetPool<ParentLinkComponent>();
        _playerInputs = world.GetPool<PlayerInputComponent>();

        foreach (int playerInputEntity in _playerInputFilter)
        {
            foreach (int clickEventEntity in _clickEventFilter)
            {
                ref ClickEventComponent clickEventComponent = ref _clickEvents.Get(clickEventEntity);

                bool hasState = _cellStates.Has(clickEventComponent.Entity);
                bool hasParent = _parents.Has(clickEventComponent.Entity);

                if (hasState && hasParent)
                {
                    SwitchTurn(playerInputEntity);
                }
            }
        }
    }

    void SwitchTurn(int entity)
    { 
        ref PlayerInputComponent playerInputComponent = ref _playerInputs.Get(entity);

        playerInputComponent.Turn = !playerInputComponent.Turn;
    }
}
