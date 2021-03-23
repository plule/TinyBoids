using Assets.Scripts.ComponentData;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    public class BoidSystem : SystemBase
    {
        private EntityQuery _otherBoidsQuery;

        protected override void OnCreate()
        {
            _otherBoidsQuery = GetEntityQuery(
                ComponentType.ReadOnly<BoidTag>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Rotation>()
            );
        }
        protected override void OnUpdate()
        {
            NativeArray<Translation> otherBoidPositions = _otherBoidsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Rotation> otherBoidRotations = _otherBoidsQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);
            var boidSettings = GetSingleton<BoidSettingsComponentData>();
            /*NativeArray<BoidSettingsComponentData> settingsArray = _settingsQuery.ToComponentDataArray<BoidSettingsComponentData>(Allocator.Temp);
            BoidSettingsComponentData boidSettings = settingsArray[0];
            settingsArray.Dispose();*/

            var job = Entities
                .WithAll<BoidTag>()
                .WithReadOnly(otherBoidPositions)
                .WithReadOnly(otherBoidRotations)
                .ForEach((
                    ref MovementComponentData movement,
                    in Translation pos,
                    in Rotation rot,
                    in LocalToWorld localToWorld
                ) =>
                {
                    movement.AngularVelocity = boidSettings.AngularVelocity;
                    movement.TargetDirection = rot.Value;

                    float3 attiranceCenterOfMass = float3.zero;
                    uint attiranceCount = 0;

                    float3 avoidanceCenterOfMass = float3.zero;
                    uint avoidanceCount = 0;

                    float3 allignanceDirection = float3.zero;
                    uint allignanceCount = 0;

                    int otherBoidCount = otherBoidPositions.Length;

                    for (int i=0; i < otherBoidCount; i++)
                    {
                        var otherBoidPosition = otherBoidPositions[i];
                        float distance = math.distance(pos.Value, otherBoidPosition.Value);
                        if (distance <= boidSettings.AttiranceDistance)
                        {
                            attiranceCount++;
                            attiranceCenterOfMass += otherBoidPosition.Value;
                        }

                        if (distance <= boidSettings.AvoidanceDistance)
                        {
                            avoidanceCount++;
                            avoidanceCenterOfMass += otherBoidPosition.Value;
                        }

                        if (distance <= boidSettings.AllignanceDistance)
                        {
                            var otherBoidRotation = otherBoidRotations[i];
                            allignanceCount++;
                            allignanceDirection += math.forward(otherBoidRotation.Value);
                        }
                    }

                    float3 centranceDirection = boidSettings.Center - pos.Value;

                    float3 attiranceDirection = float3.zero;
                    if (attiranceCount > 1)
                    {
                        attiranceCenterOfMass /= (float) attiranceCount;
                        attiranceDirection = math.normalizesafe(attiranceCenterOfMass - pos.Value);
                    }

                    float3 avoidanceDirection = float3.zero;
                    if (avoidanceCount > 1)
                    {
                        avoidanceCenterOfMass /= (float) avoidanceCount;
                        avoidanceDirection = math.normalizesafe(pos.Value - avoidanceCenterOfMass);
                    }

                    if (allignanceCount > 1)
                    {
                        allignanceDirection = math.normalizesafe(allignanceDirection);
                    }

                    float3 direction =
                        attiranceDirection * boidSettings.AttiranceStrength
                        + avoidanceDirection * boidSettings.AvoidanceStrength
                        + allignanceDirection * boidSettings.AllignanceStrength
                        + centranceDirection * boidSettings.CentranceStrength;
                    movement.TargetDirection = quaternion.LookRotationSafe(direction, localToWorld.Up);
                    movement.LinearVelocity = math.mul(rot.Value, math.forward()) * boidSettings.LinearVelocity;
                }).ScheduleParallel(Dependency);

            Dependency = JobHandle.CombineDependencies(otherBoidRotations.Dispose(job), otherBoidPositions.Dispose(job));
        }
    }
}
