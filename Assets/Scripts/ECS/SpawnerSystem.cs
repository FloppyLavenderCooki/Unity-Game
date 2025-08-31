using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    [BurstCompile]
    public partial struct SpawnerSystemMultithreaded : ISystem
    {
        private Random _random;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Spawner>();

            _random = new Random((uint)System.DateTime.Now.Ticks);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

            new ProcessSpawnerJob
            {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                Ecb = ecb,
                RandomSeed = _random.NextUInt()
            }.ScheduleParallel();

            _random.InitState(_random.NextUInt());
        }

        private EntityCommandBuffer.ParallelWriter 
            GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton
                <BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }

    [BurstCompile]
    public partial struct ProcessSpawnerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public double ElapsedTime;
        public uint RandomSeed;
        
        private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
        {
            if (spawner.NextSpawnTime < ElapsedTime)
            {
                var randomGenerator = new Random(RandomSeed + (uint)chunkIndex);

                Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);

                float3 randomOffset = (randomGenerator.NextFloat3() - 0.5f) * 10f;
                randomOffset.y = 0;

                float3 newPosition = spawner.SpawnPosition + randomOffset;

                Ecb.SetComponent(chunkIndex, newEntity,
                    LocalTransform.FromPosition(newPosition));

                spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
            }
        }
    }
}