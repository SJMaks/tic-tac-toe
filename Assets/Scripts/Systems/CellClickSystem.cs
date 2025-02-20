using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Metadata;

public class CellClickSystem : IEcsRunSystem
{
    private EcsFilter _filter;
    private EcsPool<CellStateComponent> _cellStates;
    private EcsPool<ClickableComponent> _clickables;
    private EcsPool<ParentLinkComponent> _parents;
    private EcsPool<ChildrenLinkComponent> _children;

    public void Run(IEcsSystems systems)
    {
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;

        EcsWorld world = systems.GetWorld();

        _filter = world.Filter<CellStateComponent>().Inc<ClickableComponent>().Inc<ParentLinkComponent>().End();

        _cellStates = world.GetPool<CellStateComponent>();
        _clickables = world.GetPool<ClickableComponent>();
        _parents = world.GetPool<ParentLinkComponent>();
        _children = world.GetPool<ChildrenLinkComponent>();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            foreach (int entity in _filter)
            {
                ref CellStateComponent cellState = ref _cellStates.Get(entity);
                ref ClickableComponent clickable = ref _clickables.Get(entity);
                ref ParentLinkComponent parent = ref _parents.Get(entity);

                if (hit.collider.gameObject == clickable.GameObject)
                {
                    SetCellState(entity);
                    //TODO: проверка на наличие родител€
                    CheckParentCellState(parent.Parent);
                }
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
            } else if (Input.GetMouseButtonDown(1)) {
                cellState.State = CellStates.Zero;
                sprite.color = Color.blue;
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

        int[][] winConditions = {
            new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8}, // строки
            new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8}, // столбцы
            new[] {0,4,8}, new[] {2,4,6} // диагонали
        };

        currentStateComponent.State = CellStates.Empty;
        foreach (var line in winConditions)
        {
            if (cellStates[line[0]] != CellStates.Empty &&
                cellStates[line[0]] == cellStates[line[1]] &&
                cellStates[line[1]] == cellStates[line[2]])
            {
                currentStateComponent.State = cellStates[line[0]];
            }
        }

        //TODO: рекурсивна€ проверка всех родителей

        if (currentStateComponent.State == CellStates.Cross)
        {
            Debug.Log("ѕобедили крестики!");
        } else if (currentStateComponent.State == CellStates.Zero)
        {
            Debug.Log("ѕобедили нолики!");
        }
    }
}
