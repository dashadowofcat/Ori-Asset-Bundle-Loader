using Il2Cpp;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib;
using Il2CppMoon.Timeline;

public class PrefabCachingManager
{

    public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

    public static GameObject CacheHolder;


    public void InitializeHolder()
    {
        CacheHolder = new GameObject("Prefab Cache Holder");

        GameObject.DontDestroyOnLoad(CacheHolder);
    }

    /// <summary>
    /// registers a prefab to be cached into
    /// the Prefabs dictionary
    /// </summary>
    public static void RegisterPrefabToCache(string GameObjectName, string SceneName, Func<bool> FindCondition, Func<GameObject> GameObjectCondition, string FinishCacheMessage = "")
    {
        MelonCoroutines.Start(CachePrefab(GameObjectName, SceneName, FindCondition, GameObjectCondition, FinishCacheMessage));
    }

    private static IEnumerator CachePrefab(string GameObjectName, string SceneName, Func<bool> FindCondition, Func<GameObject> GameObjectCondition, string FinishCacheMessage = "")
    {
        if(!SceneManager.GetSceneByName(SceneName).isLoaded) SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);

        while (FindCondition.Invoke()) yield return new WaitForFixedUpdate();

        GameObject gameObject = GameObject.Instantiate(GameObjectCondition.Invoke());

        gameObject.transform.parent = CacheHolder.transform;

        GameObject.DontDestroyOnLoad(gameObject);

        if(!Prefabs.ContainsKey(GameObjectName)) Prefabs.Add(GameObjectName, gameObject);

        if(FinishCacheMessage != "") MelonLogger.Msg(FinishCacheMessage);

        SceneManager.UnloadSceneAsync(SceneName);
    }

    public static GameObject GetPrefab(string GameObjectName)
    {
        if(!Prefabs.ContainsKey(GameObjectName))
        {
            MelonLogger.Error($"{GameObjectName} does not exist in the Prefabs dictionary");
        }

        return Prefabs[GameObjectName];
    }

    public void RegisterBuiltInPrefabs()
    {
        RegisterPrefabToCache("Spring", "springIntroCavernA", () => RuntimeHelper.FindObjectsOfTypeAll<Spring>().Length < 1, () => RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject, "Spring Loaded.");

        RegisterPrefabToCache("FlingHook", "waterMillPool__clone0", () => RuntimeHelper.FindObjectsOfTypeAll<RigidbodySolverIterationsModifier>().Where(H => H.name == "swampLeashFlingLanternShort" && H.gameObject.scene.name == "waterMillPool__clone0").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<RigidbodySolverIterationsModifier>().Where(H => H.name == "swampLeashFlingLanternShort" && H.gameObject.scene.name == "waterMillPool__clone0").FirstOrDefault().gameObject, "Fling Hook Loaded.");

        RegisterPrefabToCache("BashLantern", "wellspringTransitionA", () => RuntimeHelper.FindObjectsOfTypeAll<RigidbodySolverIterationsModifier>().Where(H => H.name == "lianaBashLanternNewShort" && H.gameObject.scene.name == "wellspringTransitionA").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<RigidbodySolverIterationsModifier>().Where(H => H.name == "lianaBashLanternNewShort" && H.gameObject.scene.name == "wellspringTransitionA").FirstOrDefault().gameObject, "Bash Lantern Loaded.");

        RegisterPrefabToCache("Hook", "waterMillBGetLeash", () => RuntimeHelper.FindObjectsOfTypeAll<HookPlant>().Length < 1, () => RuntimeHelper.FindObjectsOfTypeAll<HookPlant>().FirstOrDefault().transform.parent.gameObject, "Hook Plant Loaded.");

        RegisterPrefabToCache("HornBug", "kwoloksCavernLeashGate", () => RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").FirstOrDefault().gameObject, "Horn Bug Loaded.");

        RegisterPrefabToCache("LifePlant", "swampNightcrawlerA", () => RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs" && g.gameObject.scene.name == "swampNightcrawlerA").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs" && g.gameObject.scene.name == "swampNightcrawlerA").FirstOrDefault().gameObject, "Life Plant Loaded.");

        RegisterPrefabToCache("EnergyPlant", "kwoloksCavernF", () => RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(g => g.name == "energyPlantMedium").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(g => g.name == "energyPlantMedium").FirstOrDefault().gameObject, "Energy Plant Medium Loaded.");

        RegisterPrefabToCache("ElectricMantis", "lumaSwampTransitionB", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "lumaSwampTransitionB" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "lumaSwampTransitionB" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Electric Mantis Loaded.");

        RegisterPrefabToCache("BaseMantis", "swampTorchIntroductionA", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Base Mantis Loaded.");

        RegisterPrefabToCache("YellowLeaper", "swampTorchIntroductionA", () => RuntimeHelper.FindObjectsOfTypeAll<TurtlePlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "turtleLizardPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<TurtlePlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "turtleLizardPlaceholder").FirstOrDefault().gameObject, "Yellow Leaper Loaded.");

        RegisterPrefabToCache("Slime", "kwoloksCavernH", () => RuntimeHelper.FindObjectsOfTypeAll<SlugPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernH" && g.name == "spikeSlugPlaceholder (1)").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<SlugPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernH" && g.name == "spikeSlugPlaceholder (1)").FirstOrDefault().gameObject, "Slime Loaded.");

        RegisterPrefabToCache("GreenMantis", "kwoloksCavernBossRoom", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernBossRoom" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernBossRoom" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Green Mantis Loaded.");

        RegisterPrefabToCache("Keystone", "swampWalljumpChallengeB", () => RuntimeHelper.FindObjectsOfTypeAll<KeystonePickup>().Where(g => g.name == "keystone").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<KeystonePickup>().Where(g => g.name == "keystone").FirstOrDefault().gameObject, "Keystone Loaded.");

        RegisterPrefabToCache("Mortar", "kwoloksCavernUpperMainRoom", () => RuntimeHelper.FindObjectsOfTypeAll<MortarPlaceholder>().Where(g => g.name == "mortarPlaceholder" && g.gameObject.scene.name == "kwoloksCavernUpperMainRoom").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MortarPlaceholder>().Where(g => g.name == "mortarPlaceholder" && g.gameObject.scene.name == "kwoloksCavernUpperMainRoom").FirstOrDefault().gameObject, "Mortar Loaded.");

        RegisterPrefabToCache("AreaTextTimeline", "baursReachA", () => RuntimeHelper.FindObjectsOfTypeAll<MoonTimeline>().Where(g => g.name == "timeline" && g.gameObject.scene.name == "baursReachA" && g.transform.parent.name == "areaTextZone").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MoonTimeline>().Where(g => g.name == "timeline" && g.gameObject.scene.name == "baursReachA" && g.transform.parent.name == "areaTextZone").FirstOrDefault().gameObject, "Area Text Loaded.");
    }

}
