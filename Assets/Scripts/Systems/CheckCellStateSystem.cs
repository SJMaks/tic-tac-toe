using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckCellStateSystem : IEcsRunSystem
{
    private readonly EcsCustomInject<WinCombinations> _combinationsConfig = default;

    private EcsFilter _filter;
    private EcsPool<ClickEventComponent> _clickEvents;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ParentLinkComponent> _parents;
    private EcsPool<ChildrenLinkComponent> _children;
    private EcsPool<MainFieldComponent> _mainField;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickEventComponent>().End();

        _clickEvents = world.GetPool<ClickEventComponent>();
        _cellStates = world.GetPool<CellStateComponent>();
        _parents = world.GetPool<ParentLinkComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();
        _mainField = world.GetPool<MainFieldComponent>();

        foreach (int entity in _filter)
        {
            ref ClickEventComponent clickEventComponent = ref _clickEvents.Get(entity);

            bool hasState = _cellStates.Has(clickEventComponent.Entity);
            bool hasParent = _parents.Has(clickEventComponent.Entity);

            if (hasState && hasParent)
            {
                ref ParentLinkComponent parent = ref _parents.Get(clickEventComponent.Entity);
                CheckParentCellState(parent.Parent);
            }
        }
    }


    private void CheckParentCellState(int entity)
    {
        ref ChildrenLinkComponent childrenComponent = ref _children.Get(entity);
        ref CellStateComponent currentStateComponent = ref _cellStates.Get(entity);

        //TODO: проверка на количество детей

        CellStates[] cellStates = new CellStates[9];
        int i = 0;
        foreach (int child in childrenComponent.Children)
        {
            ref CellStateComponent childStateComponent = ref _cellStates.Get(child);
            cellStates[i] = childStateComponent.State;
            i++;
        }

        currentStateComponent.State = CellStates.Empty;
        foreach (var line in _combinationsConfig.Value.Combinations)
        {
            if (cellStates[line.Array[0]] != CellStates.Empty &&
                cellStates[line.Array[0]] == cellStates[line.Array[1]] &&
                cellStates[line.Array[1]] == cellStates[line.Array[2]])
            {
                currentStateComponent.State = cellStates[line.Array[0]];
            }
        }

        if (_mainField.Has(entity))
        {
            if (currentStateComponent.State == CellStates.Cross)
            {
                Debug.Log("Победили крестики!");
            }
            else if (currentStateComponent.State == CellStates.Zero)
            {
                Debug.Log("Победили нолики!");
            }
        }

        if (_parents.Has(entity))
        {
            ref ParentLinkComponent parentComponent = ref _parents.Get(entity);
            CheckParentCellState(parentComponent.Parent);
        }
    }
}
