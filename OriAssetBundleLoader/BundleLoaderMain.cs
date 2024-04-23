using Il2Cpp;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

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
        }

        public void LoadObject()
        {
            GameObject Root = bundle.LoadAsset<GameObject>("prefab root");

            GameObject obj = UnityEngine.Object.Instantiate(Root);

            obj.transform.position = Settings.OriObject.transform.position;

            Object.DontDestroyOnLoad(obj);

            ConvertChildrenToWOTW(obj.transform);
        }

        void ConvertChildrenToWOTW(Transform parent)
        {
            foreach (Transform child in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(m => m.root == parent && m != parent))
            {
                ConvertAssetToWOTW(child.gameObject);
            }
        }


        public void ConvertAssetToWOTW(GameObject Asset)
        {
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
    }
}
