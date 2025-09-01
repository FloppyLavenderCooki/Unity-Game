using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ECS
{
    public class BookAttributesAuthoring : MonoBehaviour
    {
        public Color color = Color.orange;
    }

    class BookAttributesBaker : Baker<BookAttributesAuthoring>
    {
        private Random _random;
        
        public override void Bake(BookAttributesAuthoring authoring)
        {
            _random = new Random((uint)System.DateTime.Now.Ticks);
            
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            float3 randomColor = _random.NextFloat3();

            var bookAttributes = new BookAttributes
            {
                Color = new float4(randomColor, 1f),
            };

            AddComponent(entity, bookAttributes);
        }
    }
}