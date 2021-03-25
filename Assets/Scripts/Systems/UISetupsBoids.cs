using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.ComponentData;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Tiny.UI;
using UnityEngine;

public class UISetupsBoids : SystemBase
{
    delegate void Setter(ref BoidSettingsComponentData settings, float value);

    protected override void OnUpdate()
    {
        var uiSystem = World.GetExistingSystem<ProcessUIEvents>();

        Dictionary<Entity, Setter> sliderToSetter = new Dictionary<Entity, Setter>
        {
            {uiSystem.GetEntityByUIName("LinearVelocity"), (ref BoidSettingsComponentData s, float v) => s.LinearVelocity = v},
            {uiSystem.GetEntityByUIName("AngularVelocity"), (ref BoidSettingsComponentData s, float v) => s.AngularVelocity = v},
            {uiSystem.GetEntityByUIName("AttiranceDistance"), (ref BoidSettingsComponentData s, float v) => s.AttiranceDistance = v},
            {uiSystem.GetEntityByUIName("AttiranceStrength"), (ref BoidSettingsComponentData s, float v) => s.AttiranceStrength = v},
            {uiSystem.GetEntityByUIName("AvoidanceDistance"), (ref BoidSettingsComponentData s, float v) => s.AvoidanceDistance = v},
            {uiSystem.GetEntityByUIName("AvoidanceStrength"), (ref BoidSettingsComponentData s, float v) => s.AvoidanceStrength = v},
            {uiSystem.GetEntityByUIName("AllignanceDistance"), (ref BoidSettingsComponentData s, float v) => s.AllignanceDistance = v},
            {uiSystem.GetEntityByUIName("AllignanceStrength"), (ref BoidSettingsComponentData s, float v) => s.AllignanceStrength = v},
            {uiSystem.GetEntityByUIName("CentranceStrength"), (ref BoidSettingsComponentData s, float v) => s.CentranceStrength = v}
        };

        var settings = GetSingleton<BoidSettingsComponentData>();
        Entities
            .WithoutBurst()
            .ForEach((in Entity e, in Slider slider, in UIState sr, in UIName name) =>
            {
                float value = slider.Value;
                if (!sliderToSetter.TryGetValue(e, out Setter setter))
                {
                    Debug.LogWarning($"Unknown property {name.ToString()}");
                    return;
                }

                setter(ref settings, value);
            }).Run();

        SetSingleton(settings);
    }
}
