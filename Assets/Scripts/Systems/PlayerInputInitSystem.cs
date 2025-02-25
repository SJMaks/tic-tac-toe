using Leopotam.EcsLite;
using UnityEngine;

public class PlayerInputInitSystem : IEcsInitSystem
{
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        int playerInput = world.NewEntity();

        EcsPool<PlayerInputComponent> playerInputPool = world.GetPool<PlayerInputComponent>();

        ref PlayerInputComponent playerInputComponent = ref playerInputPool.Add(playerInput);

        playerInputComponent.Turn = Random.Range(0, 2) == 1;
        if (playerInputComponent.Turn)
            Debug.Log("Первыми ходят крестики");
        else
            Debug.Log("Первыми ходят нолики");
    }
}
