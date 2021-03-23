using Unity.Entities;

[GenerateAuthoringComponent]
public struct BoidSpawnerComponentData : IComponentData
{
    public int EntityCount;
    public float Radius;
}
