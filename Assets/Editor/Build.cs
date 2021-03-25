using System;
using UnityEditor;
using Unity.Build;
using UnityEngine;

namespace Assets.Editor
{
    public static class Build
    {
        [MenuItem("Build/Build Release")]
        public static void BuildRelease()
        {
            var buildConfiguration = AssetDatabase.LoadAssetAtPath<BuildConfiguration>("Assets/Build/Wasm-Release.buildconfiguration");

            if (buildConfiguration == null)
                throw new Exception("The build configuration was not found.");

            Debug.Log($"Building {buildConfiguration.name}");
            var buildResult = buildConfiguration.Build();
            if (!buildResult.Succeeded)
            {
                Debug.LogError(buildResult.Message);
                throw buildResult.Exception;
            }
            Debug.Log("Success");
        }
    }
}
