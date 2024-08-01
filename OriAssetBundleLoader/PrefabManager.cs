using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib;

public class PrefabManager
{
    public static GameObject spring;
    public static GameObject lifePlant;
    public static GameObject hornBug;
    public static GameObject electricMantis;
    public static GameObject baseMantis;
    public static GameObject greenMantis;

    public System.Collections.IEnumerator SetupPrefabs()
    {
        SceneManager.LoadScene("springIntroCavernA", LoadSceneMode.Additive); // First load the scene that contains the object in question

        // Cache Spring Plant From springIntroCavernA

        while (RuntimeHelper.FindObjectsOfTypeAll<Spring>().Length < 1) yield return new WaitForFixedUpdate();

        spring = RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject;

        MelonLogger.Msg("Spring Loaded");


        SceneManager.LoadScene("kwoloksCavernLeashGate", LoadSceneMode.Additive);

        // Cache Horn Bug From kwoloksCavernLeashGate

        while (RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").Count() < 1) yield return new WaitForFixedUpdate(); 

        hornBug = RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").FirstOrDefault().gameObject;

        // Cache Life Plant From kwoloksCavernLeashGate

        while (RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").Count() < 1) yield return new WaitForFixedUpdate();

        lifePlant = RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").FirstOrDefault().gameObject;

        MelonLogger.Msg("Life Plant Loaded");

        MelonLogger.Msg("Horn Bug Loaded");


        SceneManager.LoadScene("lumaSwampTransitionB", LoadSceneMode.Additive);

        // Cache Shockwave Mantis From lumaSwampTransitionB

        while (RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.name == "mantisPlaceholder").Count() < 1) yield return new WaitForFixedUpdate(); 

        electricMantis = RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.name == "mantisPlaceholder").FirstOrDefault().gameObject;

        MelonLogger.Msg("Electric Mantis Loaded");


        SceneManager.LoadScene("swampTorchIntroductionA", LoadSceneMode.Additive);

        // Cache Base Mantis From swampTorchIntroductionA

        while (RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.name == "mantisPlaceholder").Count() < 1) yield return new WaitForFixedUpdate();

        baseMantis = RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.name == "mantisPlaceholder").FirstOrDefault().gameObject;

        MelonLogger.Msg("Base Mantis Loaded");


        SceneManager.LoadScene("kwoloksCavernBossRoom", LoadSceneMode.Additive);

        // Cache Green Mantis From kwoloksCavernBossRoom

        while (RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.name == "mantisPlaceholder").Count() < 1) yield return new WaitForFixedUpdate();

        greenMantis = RuntimeHelper.FindObjectsOfTypeAll<MantisPlaceholder>().Where(g => g.name == "mantisPlaceholder").FirstOrDefault().gameObject;

        MelonLogger.Msg("Green Mantis Loaded");


    }
}