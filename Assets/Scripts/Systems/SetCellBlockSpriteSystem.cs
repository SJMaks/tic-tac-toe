using Leopotam.EcsLite;
using UnityEngine;
using static Unity.VisualScripting.Metadata;
using UnityEngine.UIElements;
using System;
using Leopotam.EcsLite.Di;

public class SetCellBlockSpriteSystem : IEcsRunSystem
{
    private readonly EcsCustomInject<CellInitData> _cellConfig = default;

    private EcsFilter _filter;
    private EcsFilter _mainFieldFilter;

    private EcsPool<ActiveComponent> _actives;
    private EcsPool<ChildrenLinkComponent> _children;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<GameObjectComponent> _gameObjectComponents;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickEventComponent>().End();
        _mainFieldFilter = world.Filter<MainFieldComponent>().End();

        _cellStates = world.GetPool<CellStateComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();
        _actives = world.GetPool<ActiveComponent>();
        _gameObjectComponents = world.GetPool<GameObjectComponent>();

        foreach (int entity in _mainFieldFilter)
        {
            foreach (int clickEventEntity in _filter)
            {
                ref ChildrenLinkComponent mainChildren = ref _children.Get(entity);

                foreach (int child in mainChildren.Children)
                {
                    ref CellStateComponent currentStateComponent = ref _cellStates.Get(child);
                    ref GameObjectComponent gameObjectComponent = ref _gameObjectComponents.Get(child);

                    SpriteRenderer spriteRenderer = gameObjectComponent.GameObject.GetComponent<Transform>()
                        .Find("Sprite").gameObject.GetComponent<SpriteRenderer>();

                    bool isActive = hasActive(child);

                    if (!isActive && currentStateComponent.State == CellStates.Empty)
                    {
                        spriteRenderer.sprite = _cellConfig.Value.block;
                        spriteRenderer.color = new Color(0, 0, 0, 0.75f);
                    }
                    else if (isActive)
                    {
                        spriteRenderer.sprite = null;
                        spriteRenderer.color = new Color(0, 0, 0, 1f);
                    }
                }
            }
        }
    }

    private bool hasActive(int entity)
    {
        bool hasChildren = _children.Has(entity);

        if (!hasChildren)
        {
            return _actives.Has(entity);
        }
        else
        {
            ref ChildrenLinkComponent children = ref _children.Get(entity);

            return hasActive(children.Children[0]);
        }
    }
}
