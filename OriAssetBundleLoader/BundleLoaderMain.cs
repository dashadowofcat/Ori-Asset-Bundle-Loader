using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using System.Collections;
using Il2CppSystem.IO;
using UniverseLib.Input;

namespace OriAssetBundleLoader
{
    public class BundleLoaderMain : MelonMod
    {
        Il2CppAssetBundle Bundle;

        ConverterManager Convertermanager = new ConverterManager();

        PrefabManager PrefabManager = new PrefabManager();

        public override void OnApplicationStart()
        {
            if (!File.Exists("Mods/UnityExplorer.ML.IL2CPP.net6preview.interop.dll"))
            {
                Universe.Init();
            }

            Convertermanager.SetupConverters();

            Bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/assets/ori");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "wotwTitleScreen") return;

            MelonCoroutines.Start(PrefabManager.SetupPrefabs());
        }

        public static Material OriMaterial
        {
            get { return GameObject.Find("seinCharacter/ori3D/mirrorHolder/rigHolder/oriRig/Model_GRP/body_MDL").GetComponent<SkinnedMeshRenderer>().material; }
        }

        public static GameObject OriObject
        {
            get { return GameObject.Find("seinCharacter"); }
        }

        public override void OnUpdate()
        {

            if (InputManager.GetMouseButton(2))
            {
                MelonLogger.Msg("click");
            }

            if (InputManager.GetKeyDown(KeyCode.U))
            {
                LoadObject();
            }

            if (InputManager.GetKeyDown(KeyCode.Y))
            {
                GameObject.Find("systems/scenesManager").GetComponent<GoToSceneController>().GoToScene("stressTestMaster");
            }
        }
        
        public void LoadObject()
        {
            GameObject Root = Bundle.LoadAsset<GameObject>("prefab root");

            GameObject obj = UnityEngine.Object.Instantiate(Root);

            obj.transform.position = OriObject.transform.position;

            UnityEngine.Object.DontDestroyOnLoad(obj);

            ConvertChildrenToWOTW(obj.transform);
        }

        void ConvertChildrenToWOTW(Transform parent)
        {
            foreach (Transform child in RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(m => m.root == parent && m != parent))
            {
                ConvertAssetToWOTW(child.gameObject);
            }
        }


        public void ConvertAssetToWOTW(GameObject Asset)
        {
            ConvertElementToWOTW(Asset);

            if (Asset.GetComponent<MeshRenderer>())
            {
                ConvertRendererToWOTW(Asset);
                return;
            }

            if (Asset.GetComponent<Collider>())
            {
                ConvertColliderToWOTW(Asset);

                return;
            }
        }

        void ConvertRendererToWOTW(GameObject Asset)
        {
            MeshRenderer ObjRenderer = Asset.GetComponent<MeshRenderer>();

            Texture2D ObjTexture = ObjRenderer.material.mainTexture.TryCast<Texture2D>();

            ObjRenderer.material = OriMaterial;

            ObjRenderer.material.color = Settings.EnviormentColor;

            ObjRenderer.material.mainTexture = ObjTexture;

            UberShaderAPI.SetTexture(ObjRenderer, UberShaderProperty_Texture.MultiplyLayerMaskTexture, null);
            UberShaderAPI.SetTexture(ObjRenderer, UberShaderProperty_Texture.MultiplyLayerTexture, null);
        }

        void ConvertColliderToWOTW(GameObject Asset)
        {
            Asset.layer = 10;
        }
        void ConvertElementToWOTW(GameObject Asset)
        {
            if (!Convertermanager.Converters.ContainsKey(Asset.name)) return;

            Convertermanager.Converters[Asset.name].ConvertElement(Asset);
        }
    }
}