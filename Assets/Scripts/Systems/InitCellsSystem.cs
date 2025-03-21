using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public class InitCellsSystem : IEcsInitSystem
{
    private readonly EcsCustomInject<GameConfiguration> _gameConfig = default;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        var cellStatePool = world.GetPool<CellStateComponent>();
        var positionPool = world.GetPool<TransformComponent>();
        var parentPool = world.GetPool<ParentLinkComponent>();
        var childrenPool = world.GetPool<ChildrenLinkComponent>();
        var mainFieldPool = world.GetPool<MainFieldComponent>();
        var levelPool = world.GetPool<CellLevelComponent>();
        var gameObjectPool = world.GetPool<GameObjectComponent>();

        // Создаем корневую сущность
        int rootEntity = world.NewEntity();
        cellStatePool.Add(rootEntity).State = CellStates.Empty;
        ref var rootChildren = ref childrenPool.Add(rootEntity);
        rootChildren.Children = new int[9]; // Инициализируем массив
        mainFieldPool.Add(rootEntity).Entity = rootEntity;
        positionPool.Add(rootEntity).Position = Vector2.zero;
        levelPool.Add(rootEntity).Level = _gameConfig.Value.RecursionLevel;
        gameObjectPool.Add(rootEntity);

        // Запускаем рекурсивное создание структуры
        CreateSubCells(world, rootEntity, _gameConfig.Value.RecursionLevel, Vector2.zero);
    }

    private void CreateSubCells(
        EcsWorld world,
        int parentEntity,
        int currentLevel,
        Vector2 parentPosition)
    {
        var cellStatePool = world.GetPool<CellStateComponent>();
        var clickablePool = world.GetPool<ClickableComponent>();
        var positionPool = world.GetPool<TransformComponent>();
        var parentLinkPool = world.GetPool<ParentLinkComponent>();
        var childrenPool = world.GetPool<ChildrenLinkComponent>();
        var activePool = world.GetPool<ActiveComponent>();
        var levelPool = world.GetPool<CellLevelComponent>();
        var gameObjectPool = world.GetPool<GameObjectComponent>();

        if (currentLevel == 0)
        {
            activePool.Add(parentEntity);
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
        float step = (Mathf.Pow(3, currentLevel) - currentLevel) * 0.75f;

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
            gameObjectPool.Add(childEntity);

            // Рекурсивный вызов
            CreateSubCells(world, childEntity, currentLevel - 1, childPosition);
        }
    }
}