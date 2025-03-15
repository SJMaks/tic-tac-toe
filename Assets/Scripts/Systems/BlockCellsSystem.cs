using Leopotam.EcsLite;
using System;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;

public class BlockCellsSystem : IEcsRunSystem
{
    private EcsFilter _filter;
    private EcsFilter _mainFieldFilter;
    private EcsFilter _activeFilter;

    private EcsPool<ClickEventComponent> _clickEvents;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ParentLinkComponent> _parents;
    private EcsPool<ChildrenLinkComponent> _children;
    private EcsPool<ActiveComponent> _actives;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickEventComponent>().End();
        _mainFieldFilter = world.Filter<MainFieldComponent>().End();
        _activeFilter = world.Filter<ActiveComponent>().End();

        _clickEvents = world.GetPool<ClickEventComponent>();
        _cellStates = world.GetPool<CellStateComponent>();
        _parents = world.GetPool<ParentLinkComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();
        _actives = world.GetPool<ActiveComponent>();

        foreach (int mainFieldEntity in _mainFieldFilter)
        {
            foreach (int clickEventEntity in _filter)
            {
                ref ClickEventComponent clickEventComponent = ref _clickEvents.Get(clickEventEntity);

                bool hasState = _cellStates.Has(clickEventComponent.Entity);
                bool hasParent = _parents.Has(clickEventComponent.Entity);

                if (hasState && hasParent)
                {
                    ref ParentLinkComponent parent = ref _parents.Get(clickEventComponent.Entity);
                    ref ChildrenLinkComponent localChildren = ref _children.Get(parent.Parent);

                    int index = Array.IndexOf(localChildren.Children, clickEventComponent.Entity);

                    //Удалить все флаги активности
                    foreach (int cell in _activeFilter)
                    {
                        _actives.Del(cell);
                    }

                    //Получить детей корня
                    var childrenPool = world.GetPool<ChildrenLinkComponent>();
                    ref ChildrenLinkComponent mainChildren = ref childrenPool.Get(mainFieldEntity);

                    //Получить состояние целевого поля
                    ref CellStateComponent targetCellState = ref _cellStates.Get(mainChildren.Children[index]);
                    if (targetCellState.State != CellStates.Empty)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (i != index)
                                AddActiveComponent(mainChildren.Children[i]);
                        }
                    }
                    else
                    {
                        AddActiveComponent(mainChildren.Children[index]);
                    }
                }
            }
        }
    }

    void AddActiveComponent(int parentEntity)
    {
        bool hasChildren = _children.Has(parentEntity);

        if (!hasChildren) 
        {
            _actives.Add(parentEntity);
        }
        else
        {
            ref ChildrenLinkComponent children = ref _children.Get(parentEntity);

            foreach (int child in children.Children)
            {
                AddActiveComponent(child);
            }
        }
    }
}