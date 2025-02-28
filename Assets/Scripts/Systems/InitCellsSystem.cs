using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public struct CellLevelComponent
{
    public int Level;
}

public class InitCellsSystem : IEcsInitSystem
{
    [Range(1, 5)]
    public int RecursionLevel = 2; // Уровень рекурсии можно менять в инспекторе

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        var cellStatePool = world.GetPool<CellStateComponent>();
        var positionPool = world.GetPool<PositionComponent>();
        var parentPool = world.GetPool<ParentLinkComponent>();
        var childrenPool = world.GetPool<ChildrenLinkComponent>();
        var levelPool = world.GetPool<CellLevelComponent>();
        var mainFieldPool = world.GetPool<MainFieldComponent>();

        // Создаем корневую сущность
        int rootEntity = world.NewEntity();
        cellStatePool.Add(rootEntity).State = CellStates.Empty;
        ref var rootChildren = ref childrenPool.Add(rootEntity);
        rootChildren.Children = new int[9]; // Инициализируем массив
        levelPool.Add(rootEntity).Level = RecursionLevel;
        mainFieldPool.Add(rootEntity).Entity = rootEntity;
        positionPool.Add(rootEntity).Position = Vector2.zero;

        // Запускаем рекурсивное создание структуры
        CreateSubCells(world, rootEntity, RecursionLevel, Vector2.zero);
    }

    private void CreateSubCells(
        EcsWorld world,
        int parentEntity,
        int currentLevel,
        Vector2 parentPosition)
    {
        var cellStatePool = world.GetPool<CellStateComponent>();
        var clickablePool = world.GetPool<ClickableComponent>();
        var positionPool = world.GetPool<PositionComponent>();
        var parentLinkPool = world.GetPool<ParentLinkComponent>();
        var childrenPool = world.GetPool<ChildrenLinkComponent>();
        var levelPool = world.GetPool<CellLevelComponent>();

        if (currentLevel == 0)
        {
            clickablePool.Add(parentEntity);
            return;
        }

        // Гарантируем наличие компонента ChildrenLink у родителя
        if (!childrenPool.Has(parentEntity))
        {
            ref var children = ref childrenPool.Add(parentEntity);
            children.Children = new int[9];
        }

        // Создаем дочерние клетки
        ref var parentChildren = ref childrenPool.Get(parentEntity);
        float step = Camera.main.orthographicSize * (1f / (RecursionLevel * (Camera.main.orthographicSize + 5))) * Mathf.Pow(3, currentLevel);

        for (int i = 0; i < 9; i++)
        {
            int childEntity = world.NewEntity();
            parentChildren.Children[i] = childEntity;

            // Рассчет позиции
            int x = i % 3;
            int y = i / 3;
            Vector2 childPosition = new Vector2(
                parentPosition.x + (x - 1) * step,
                parentPosition.y + (y - 1) * step
            );

            // Добавляем компоненты
            cellStatePool.Add(childEntity).State = CellStates.Empty;
            positionPool.Add(childEntity).Position = childPosition;
            parentLinkPool.Add(childEntity).Parent = parentEntity;
            levelPool.Add(childEntity).Level = currentLevel - 1;

            // Рекурсивный вызов
            CreateSubCells(world, childEntity, currentLevel - 1, childPosition);
        }
    }
}