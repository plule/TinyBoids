using Assets.Scripts.ComponentData;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    public class TranslateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation translation, in MovementComponentData movement) =>
            {
                translation.Value += movement.LinearVelocity * deltaTime;
            }).ScheduleParallel();
        }
    }
}
