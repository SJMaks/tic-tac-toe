using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

public class CellClickSystem : IEcsRunSystem
{
    private EcsFilter _filter;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ClickableComponent> _clickables;

    public void Run(IEcsSystems systems)
    {
        if (!Input.GetMouseButtonDown(0)) return;

        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<CellStateComponent>().Inc<ClickableComponent>().End();

        _cellStates = world.GetPool<CellStateComponent>();
        _clickables = world.GetPool<ClickableComponent>();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            foreach (int entity in _filter)
            {
                ref CellStateComponent cellState = ref _cellStates.Get(entity);
                ref ClickableComponent clickable = ref _clickables.Get(entity);

                if (hit.collider.gameObject == clickable.GameObject)
                {
                    OnClickButton(entity);
                }
            }
        }
    }

    private void OnClickButton(int entity)
    {
        ref CellStateComponent cellState = ref _cellStates.Get(entity);

        if (cellState.State == CellStates.Empty)
        {
            cellState.State = CellStates.Cross;
        }
        else if (cellState.State == CellStates.Cross)
        {
            cellState.State = CellStates.Zero;
        }
        else
        {
            cellState.State = CellStates.Empty;
        }
        Debug.Log("Cell clicked - " + cellState.State);
    }
}
