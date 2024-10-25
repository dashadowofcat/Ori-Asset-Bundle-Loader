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

        Prefabs.Add(GameObjectName, GameObject.Instantiate(gameObject));

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

        RegisterPrefabToCache("HornBug", "kwoloksCavernLeashGate", () => RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").FirstOrDefault().gameObject, "Horn Bug Loaded.");

        RegisterPrefabToCache("LifePlant", "kwoloksCavernLeashGate", () => RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").FirstOrDefault().gameObject, "Life Plant Loaded.");

        RegisterPrefabToCache("EnergyPlantMedium", "kwoloksCavernF", () => RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(g => g.name == "energyPlantMedium").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(g => g.name == "energyPlantMedium").FirstOrDefault().gameObject, "Energy Plant Medium Loaded.");

        RegisterPrefabToCache("ElectricMantis", "lumaSwampTransitionB", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "lumaSwampTransitionB" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "lumaSwampTransitionB" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Electric Mantis Loaded.");

        RegisterPrefabToCache("BaseMantis", "swampTorchIntroductionA", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Base Mantis Loaded.");

        RegisterPrefabToCache("YellowLeaper", "swampTorchIntroductionA", () => RuntimeHelper.FindObjectsOfTypeAll<TurtlePlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "turtleLizardPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<TurtlePlaceholder>().Where(g => g.gameObject.scene.name == "swampTorchIntroductionA" && g.name == "turtleLizardPlaceholder").FirstOrDefault().gameObject, "Yellow Leaper Loaded.");

        RegisterPrefabToCache("Slime", "kwoloksCavernH", () => RuntimeHelper.FindObjectsOfTypeAll<SlugPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernH" && g.name == "spikeSlugPlaceholder (1)").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<SlugPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernH" && g.name == "spikeSlugPlaceholder (1)").FirstOrDefault().gameObject, "Slime Loaded.");

        RegisterPrefabToCache("GreenMantis", "kwoloksCavernBossRoom", () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernBossRoom" && g.name == "mantisPlaceholder").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.gameObject.scene.name == "kwoloksCavernBossRoom" && g.name == "mantisPlaceholder").FirstOrDefault().gameObject, "Green Mantis Loaded.");

        RegisterPrefabToCache("Keystone", "swampWalljumpChallengeB", () => RuntimeHelper.FindObjectsOfTypeAll<KeystonePickup>().Where(g => g.name == "keystone").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<KeystonePickup>().Where(g => g.name == "keystone").FirstOrDefault().gameObject, "Keystone Loaded.");

        RegisterPrefabToCache("Mortar", "kwoloksCavernUpperMainRoom", () => RuntimeHelper.FindObjectsOfTypeAll<MortarPlaceholder>().Where(g => g.name == "mortarPlaceholder" && g.gameObject.scene.name == "kwoloksCavernUpperMainRoom").Count() < 1, () => RuntimeHelper.FindObjectsOfTypeAll<MortarPlaceholder>().Where(g => g.name == "mortarPlaceholder" && g.gameObject.scene.name == "kwoloksCavernUpperMainRoom").FirstOrDefault().gameObject, "Mortar Loaded.");
    }
}