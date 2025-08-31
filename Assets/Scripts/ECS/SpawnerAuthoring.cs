using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public float spawnRate;
    }

    class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            // This line converts the Spawner GameObject into an Entity.
            // TransformUsageFlags is None because the Spawner entity is not
            // rendered and does not need a LocalTransform component.
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Spawner
            {
                // This GetEntity call converts a GameObject prefab into an entity
                // prefab. The prefab is rendered, so it requires the standard Transform
                // components, that's why TransformUsageFlags is set to Dynamic.
                Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                SpawnPosition = authoring.transform.position,
                SpawnRate = authoring.spawnRate,
                NextSpawnTime = 0f
            });
        }
    }

    public struct Spawner : IComponentData
    {
        public Entity Prefab;
        public float3 SpawnPosition;
        public float SpawnRate;
        // This field is used only for the multi-threading example.
        public float NextSpawnTime;
    }
}