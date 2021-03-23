using Assets.Scripts.ComponentData;
using Unity.Entities;
using Unity.Transforms;

public class SpawnerCenterItsBoids : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithChangeFilter<Translation>()
            .ForEach((ref BoidSettingsComponentData boid, in Translation translation) =>
            {
                boid.Center = translation.Value;
            }).ScheduleParallel();
    }
}
