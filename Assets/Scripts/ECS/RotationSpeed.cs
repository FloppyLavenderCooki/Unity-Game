using Unity.Entities;

namespace ECS
{
    public struct RotationSpeed : IComponentData
    {
        public float RadiansPerSecond;
    }
}