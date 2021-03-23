using Assets.Scripts.ComponentData;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnerCreatesBoidsSystem : SystemBase
{
    EntityQuery _boidsQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        _boidsQuery = GetEntityQuery(ComponentType.ReadOnly<BoidTag>());
    }

    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
        int boidCount = _boidsQuery.CalculateEntityCount();
        var prototype = GetSingletonEntity<SpawnerTag>();

        Entities.ForEach((in BoidSettingsComponentData settings) =>
        {
            int boidNumberToSpawn = 1000 - boidCount;
            var random = new Random(1);
            for (int i = 0; i < boidNumberToSpawn; ++i)
            {
                var e = cmdBuffer.Instantiate(prototype);
                cmdBuffer.RemoveComponent<SpawnerTag>(prototype);

                cmdBuffer.AddComponent(e, new Translation
                {
                    Value = random.NextFloat3()
                });
                cmdBuffer.AddComponent(e, new Rotation
                {
                    Value = quaternion.LookRotation(random.NextFloat3Direction(), math.up())
                });
                cmdBuffer.AddComponent(e, new MovementComponentData
                {
                    TargetDirection = quaternion.identity,
                    LinearVelocity = new float3(i, 0, 0)
                });
                cmdBuffer.AddComponent(e, new BoidTag());
            }
        }).WithoutBurst().Run();

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
