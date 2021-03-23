using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ComponentData
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct BoidSettingsComponentData : IComponentData
    {
        public float3 Center;
        public float LinearVelocity;
        public float AngularVelocity;
        public float AttiranceDistance;
        public float AttiranceStrength;
        public float AvoidanceDistance;
        public float AvoidanceStrength;
        public float AllignanceDistance;
        public float AllignanceStrength;
        public float CentranceStrength;
    }
}
