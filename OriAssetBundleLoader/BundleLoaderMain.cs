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

namespace OriAssetBundleLoader
{
    public class BundleLoaderMain : MelonMod
    {
        Il2CppAssetBundle bundle;
        System.Collections.Generic.Dictionary<string, ElementConverter> Converters = new System.Collections.Generic.Dictionary<string, ElementConverter>();

        public override void OnApplicationStart()
        {
            SetupConverters();

            bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/assets/ori");
        }

        void SetupConverters()
        {
            Converters.Add("Bash", new BashConverter());
            Converters.Add("Leash", new LeashConverter());
            Converters.Add("Spring", new SpringConveter());
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
            if (UniverseLib.Input.InputManager.GetKeyDown(KeyCode.U))
            {
                LoadObject();
            }

            if (UniverseLib.Input.InputManager.GetKeyDown(KeyCode.Y))
            {
                GameObject.Find("systems/scenesManager").GetComponent<GoToSceneController>().GoToScene("stressTestMaster");
            }

            if (UniverseLib.Input.InputManager.GetKeyDown(KeyCode.J))
            {
                SceneManager.LoadScene("kwoloksCavernF", LoadSceneMode.Additive);
            }
        }

        public void LoadObject()
        {
            GameObject Root = bundle.LoadAsset<GameObject>("prefab root");

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
            if (!Converters.ContainsKey(Asset.name)) return;

            Converters[Asset.name].ConvertElement(Asset);
        }
    }
}