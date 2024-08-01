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

    public System.Collections.IEnumerator SetupPrefabs()
    {
        // Cache Spring Plant

        SceneManager.LoadScene("springIntroCavernA", LoadSceneMode.Additive); // First load the scene that contains the object in question

        while (RuntimeHelper.FindObjectsOfTypeAll<Spring>().Length < 1) yield return new WaitForFixedUpdate();

        spring = RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject;

        MelonLogger.Msg("Spring Loaded");

        // Cache Life Plant

        SceneManager.LoadScene("kwoloksCavernO", LoadSceneMode.Additive);

        while (RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").Count() < 1) yield return new WaitForFixedUpdate();

        lifePlant = RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").FirstOrDefault().gameObject;

        MelonLogger.Msg("Life Plant Loaded");

        // Cache Horn Bug

        SceneManager.LoadScene("kwoloksCavernLeashGate", LoadSceneMode.Additive);

        while (RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").Count() < 1) yield return new WaitForFixedUpdate(); // I don't know if this is the correct object to look for

        hornBug = RuntimeHelper.FindObjectsOfTypeAll<HornBugPlaceholder>().Where(g => g.name == "hornBugPlaceholder").FirstOrDefault().gameObject;

        MelonLogger.Msg("Horn Bug Loaded");

    }
}