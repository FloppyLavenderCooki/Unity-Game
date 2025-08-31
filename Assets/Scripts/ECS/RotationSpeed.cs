using Unity.Entities;

// This component defines the rotation speed of an entity.
namespace ECS
{
    public struct RotationSpeed : IComponentData
    {
        public float RadiansPerSecond;
    }
}