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
        SceneManager.LoadScene("springIntroCavernA", LoadSceneMode.Additive);

        while(SceneManager.GetSceneByName("springIntroCavernA").isLoaded) yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(.1f);

        spring = RuntimeHelper.FindObjectsOfTypeAll<Spring>().FirstOrDefault().transform.parent.gameObject;
    }
}