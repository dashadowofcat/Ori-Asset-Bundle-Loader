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

    public System.Collections.IEnumerator SetupPrefabs()
    {
        MelonLogger.Msg("start");

        SceneManager.LoadScene("springIntroCavernA", LoadSceneMode.Additive);

        MelonLogger.Msg("point 1");

        while (RuntimeHelper.FindObjectsOfTypeAll<Spring>().Length < 1) yield return new WaitForFixedUpdate();

        MelonLogger.Msg("point 2");

        spring = RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject;

        MelonLogger.Msg("end");
    }
}