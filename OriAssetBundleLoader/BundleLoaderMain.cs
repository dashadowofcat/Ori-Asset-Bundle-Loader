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

namespace OriAssetBundleLoader
{
    public class BundleLoaderMain : MelonMod
    {
        Il2CppAssetBundle bundle;

        public override void OnApplicationStart()
        {
            bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/assets/ori");
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

            if (UniverseLib.Input.InputManager.GetKeyDown(KeyCode.G))
            {
                Il2CppSystem.Collections.Generic.List<RuntimeSceneMetaData> scenes = GameObject.Find("systems/scenesManager").GetComponent<ScenesManager>().AllScenes;

                foreach (RuntimeSceneMetaData scene in scenes)
                {
                    LoggerInstance.Msg(scene.Scene);
                }
            }
        }

        public void LoadObject()
        {
            GameObject Root = bundle.LoadAsset<GameObject>("prefab root");

            GameObject obj = UnityEngine.Object.Instantiate(Root);

            obj.transform.position = Settings.OriObject.transform.position;

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

            ObjRenderer.material = Settings.OriMaterial;

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

            if (Asset.name == "Bash")
            {
                SpiritLantern lantern = Asset.AddComponent<SpiritLantern>();

                lantern.OnBashSoundProvider = RuntimeHelper.FindObjectsOfTypeAll<Varying2DSoundProvider>().Where(S => S.name == "startDashForward").FirstOrDefault();

                return;
            }

            if (Asset.name == "Leash" || Asset.name == "LeashBulb")
            {
                HookFlingPlant Hook = Asset.AddComponent<HookFlingPlant>();

                Rigidbody rb = Asset.AddComponent<Rigidbody>();

                Hook.HookTarget = Asset.transform;

                Hook.m_rigidbody = rb;

                Hook.IsSticky = !(Asset.name == "LeashBulb");

                rb.constraints = RigidbodyConstraints.FreezeAll;

                return;
            }
        }
    }
}
