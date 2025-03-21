using Leopotam.EcsLite;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class CameraScaleSystem : IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsPool<TransformComponent> _scalePool;
        private Camera _camera;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            _filter = world
                .Filter<TransformComponent>()
                .Inc<MainFieldComponent>()
                .Exc<ParentLinkComponent>()
                .End();

            _scalePool = world.GetPool<TransformComponent>();

            _camera = Camera.main;

            foreach (var entity in _filter)
            {
                ref var scaleComponent = ref _scalePool.Get(entity);
                _camera.orthographicSize = scaleComponent.Scale.magnitude * 2;
            }
        }
    }
}