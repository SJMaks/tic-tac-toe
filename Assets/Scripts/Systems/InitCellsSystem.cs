using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.LightTransport;

public class InitCellsSystem : IEcsInitSystem
{
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        int mainField = world.NewEntity();

        EcsPool<CellStateComponent> cellStatePool = world.GetPool<CellStateComponent>();
        EcsPool<ClickableComponent> clickablePool = world.GetPool<ClickableComponent>();
        EcsPool<PositionComponent> positionPool = world.GetPool<PositionComponent>();

        ref CellStateComponent mainCellStateComponent = ref cellStatePool.Add(mainField);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int cell = world.NewEntity();
                ref CellStateComponent cellStateComponent = ref cellStatePool.Add(cell);
                ref ClickableComponent clickableComponent = ref clickablePool.Add(cell);
                ref PositionComponent positionComponent = ref positionPool.Add(cell);
                cellStateComponent.State = CellStates.Empty;
                positionComponent.Position = new Vector2(j * 1.5f - 1.5f, i * 1.5f - 1.5f);
            }
        }
    }
}
