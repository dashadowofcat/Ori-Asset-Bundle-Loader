using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using OriAssetBundleLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib;

public class PrefabCachingManager
{

    public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

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

        GameObject gameObject = GameObjectCondition.Invoke();

        Prefabs.Add(GameObjectName, gameObject);

        if(FinishCacheMessage != "") MelonLogger.Msg(FinishCacheMessage);
    }

    public static GameObject GetPrefab(string GameObjectName)
    {
        if(!Prefabs.ContainsKey(GameObjectName))
        {
            MelonLogger.Error($"{GameObjectName} does not exist in the Prefabs dictionary");
        }

        return Prefabs[GameObjectName];
    }

    public void SetupPrefabs()
    {
        RegisterPrefabToCache("Spring", "springIntroCavernA", () => RuntimeHelper.FindObjectsOfTypeAll<Spring>().Length < 1, () => RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject, "Spring Loaded.");

        RegisterPrefabToCache("HornBug", "kwoloksCavernLeashGate", () => RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").FirstOrDefault().gameObject, "Horn Bug Loaded.");

        RegisterPrefabToCache("LifePlant", "kwoloksCavernLeashGate", () => RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").FirstOrDefault().gameObject, "Life Plant Loaded.");

        RegisterPrefabToCache("ElectricMantis", "lumaSwampTransitionB", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "lumaSwampTransitionB" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "lumaSwampTransitionB" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Electric Mantis Loaded.");

        RegisterPrefabToCache("BaseMantis", "swampTorchIntroductionA", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Base Mantis Loaded.");

        RegisterPrefabToCache("GreenMantis", "kwoloksCavernBossRoom", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernBossRoom" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernBossRoom" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Green Mantis Loaded.");

        RegisterPrefabToCache("Keystone", "swampWalljumpChallengeB", () => RuntimeHelper.FindObjectsOfTypeAll<KeystonePickup>().Where(g => g.name == "keystone").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<KeystonePickup>().Where(g => g.name == "keystone").FirstOrDefault().gameObject, "Keystone Loaded.");
        // Ben // Keystone seems to be loading properly but for some reason the game speed slows way down. Maybe it's performance related?
        // Ben // The keystone also isn't spawned in the test level when placed in the editor. I must be missing something.

        RegisterPrefabToCache("TwoSlotDoor", "swampNightcrawlerCavernA", () => RuntimeHelper.FindObjectsOfTypeAll<MoonDoorWithSlots>().Where(g => g.name == "doorWithTwoSlots").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MoonDoorWithSlots>().Where(g => g.name == "doorWithTwoSlots").FirstOrDefault().gameObject, "Two Slot Door Loaded.");
        // Ben // This is not loading correctly, I'm not sure which script too look for here. I thought there's a "doorWithTwoSlots" script but it doesn't autofill here.


        // not implemented yet
        // Cache Rotating Spike Hazard From kwoloksCavernLeashGate

        /*
        while (RuntimeHelper.FindObjectsOfTypeAll<AutoRotate>().Where(g => g.transform.parent.name == "midGroup" && g.name == "rotatingObstacleA (10)").Count() < 1) yield return new WaitForFixedUpdate();

        rotatingSpikeHazaard = RuntimeHelper.FindObjectsOfTypeAll<AutoRotate>().Where(g => g.transform.parent.name == "midGroup" && g.name == "rotatingObstacleA (5)").FirstOrDefault().gameObject;

        MelonLogger.Msg("Rotating Spike Hazard Loaded");
        */
    }
}