using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ComponentData
{
    [GenerateAuthoringComponent]
    public struct MovementComponentData : IComponentData
    {
        public float3 LinearVelocity;
        public quaternion TargetDirection;
        public float AngularVelocity;
    }
}
