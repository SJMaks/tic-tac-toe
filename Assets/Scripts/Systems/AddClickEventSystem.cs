using Leopotam.EcsLite;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class AddClickEventSystem : IEcsRunSystem
{
    private EcsFilter _filter;
    private EcsPool<ClickEventComponent> _clickEvents;
    private EcsPool<ClickableComponent> _clickables;

    public void Run(IEcsSystems systems)
    {
        if (!Input.GetMouseButtonDown(0)) return;

        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<ClickableComponent>().End();

        _clickEvents = world.GetPool<ClickEventComponent>();
        _clickables = world.GetPool<ClickableComponent>();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            foreach (int entity in _filter)
            {
                ref ClickableComponent clickable = ref _clickables.Get(entity);

                if (hit.collider.gameObject == clickable.GameObject)
                {
                    int clickEventEntity = world.NewEntity();
                    ref ClickEventComponent clickEventComponent = ref _clickEvents.Add(clickEventEntity);
                    clickEventComponent.Entity = entity;
                }
            }
        }
    }
}
