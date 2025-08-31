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
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Spawner
            {
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
        public float NextSpawnTime;
    }
}