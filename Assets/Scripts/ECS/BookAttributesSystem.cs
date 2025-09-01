using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace ECS
{
    public partial struct BookAttributesSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (attributes, meshRenderer) in
                     SystemAPI.Query<RefRO<BookAttributes>, RefRW<URPMaterialPropertyBaseColor>>())
            {
                Debug.Log($"{meshRenderer.ValueRW.Value}");
                meshRenderer.ValueRW.Value = new float4(attributes.ValueRO.Color);
            }
        }
    }
}