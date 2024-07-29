using Il2Cpp;
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

    public System.Collections.IEnumerator SetupPrefabs()
    {
        // Cache Spring Plant

        SceneManager.LoadScene("springIntroCavernA", LoadSceneMode.Additive);

        while (RuntimeHelper.FindObjectsOfTypeAll<Spring>().Length < 1) yield return new WaitForFixedUpdate();

        spring = RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject;

        MelonLogger.Msg("Spring Loaded");

        // Cache Life Plant

        SceneManager.LoadScene("kwoloksCavernO", LoadSceneMode.Additive);

        while (RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").Count() < 1) yield return new WaitForFixedUpdate();

        lifePlant = RuntimeHelper.FindObjectsOfTypeAll<OrbSpawner>().Where(g => g.name == "landOnAndSpawnOrbs").FirstOrDefault().gameObject;

        MelonLogger.Msg("Life Plant Loaded");

    }
}