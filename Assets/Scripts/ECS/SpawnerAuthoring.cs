using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS
{
    class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject[] bookPrefabs;
        public float spawnRate;
    }

    class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            var buffer = AddBuffer<SpawnerPrefab>(entity);
            foreach (var bookPrefab in authoring.bookPrefabs)
            {
                var prefabEntity = GetEntity(bookPrefab, TransformUsageFlags.Dynamic);
                buffer.Add(new SpawnerPrefab { Prefab = prefabEntity });
            }

            AddComponent(entity, new Spawner
            {
                SpawnPosition = authoring.transform.position,
                SpawnRate = authoring.spawnRate,
                NextSpawnTime = 0f
            });
        }
    }

    public struct Spawner : IComponentData
    {
        public float3 SpawnPosition;
        public float SpawnRate;
        public float NextSpawnTime;
    }
    
    public struct SpawnerPrefab : IBufferElementData
    {
        public Entity Prefab;
    }
}