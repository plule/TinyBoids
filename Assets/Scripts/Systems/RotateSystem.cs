using System.Runtime.CompilerServices;
using Assets.Scripts.ComponentData;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    public class RotateSystem : SystemBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion RotateTowards(
            quaternion from,
            quaternion to,
            float maxDegreesDelta)
        {
            float num = Angle(from, to);
            return num < float.Epsilon ? to : math.slerp(from, to, math.min(1f, maxDegreesDelta / num));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(quaternion q1, quaternion q2)
        {
            var dot = math.dot(q1, q2);
            return !(dot > 0.999998986721039) ? (float)(math.acos(math.min(math.abs(dot), 1f)) * 2.0) : 0.0f;
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Rotation rotation, in MovementComponentData movement) =>
            {
                //rotation.Value = math.slerp(rotation.Value, movement.TargetDirection, deltaTime * movement.AngularVelocity);
                rotation.Value = RotateTowards(rotation.Value, movement.TargetDirection, deltaTime * movement.AngularVelocity);
            }).ScheduleParallel();
        }
    }
}
