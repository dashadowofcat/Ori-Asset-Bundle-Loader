using Il2Cpp;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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

            if (UniverseLib.Input.InputManager.GetKeyDown(KeyCode.Y))
            {

                SceneManager.LoadScene(294, LoadSceneMode.Additive);

                GameObject ori = GameObject.Find("seinCharacter");

                ori.transform.position = new Vector3(-333, -2310, 0);
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

                lantern.OnBashSoundProvider = RuntimeHelper.FindObjectsOfTypeAll<Varying2DSoundProvider>().Where(S => S.name == "spiritLanternOnBashSoundProvider").FirstOrDefault();

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

            /*
            if(Asset.name == "CheckPoint")
            {
                if (!Asset.GetComponent<BoxCollider2D>())
                {
                    LoggerInstance.Warning("Check point requires a BoxCollider2D");
                    return;
                }

                BoxCollider2D coll = Asset.GetComponent<BoxCollider2D>();

                InvisibleCheckpoint checkPoint = Asset.AddComponent<InvisibleCheckpoint>();

                checkPoint.m_bounds = new Rect(Asset.transform.position, coll.bounds.size);

                return;
            }
            */
        }
    }
}
