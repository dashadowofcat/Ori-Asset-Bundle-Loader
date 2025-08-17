using Il2Cpp;
using System.Linq;
using UnityEngine;


public class BashLanternConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject bashGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("BashLantern"), Asset.transform.position, Quaternion.identity, Asset.transform);
        bashGameObject.SetActive(true);

        bashGameObject.transform.localPosition = new Vector3(0f, -5.3f, 0f);

        //GameObject vineObject = bashGameObject.transform.FindChild("lanternVineA").gameObject;
        //vineObject.GetComponent<MeshRenderer>().material = new Material(PrefabCachingManager.GetPrefab("BashLantern").transform.FindChild("lanternVineA").gameObject.GetComponent<MeshRenderer>().material);

        //Texture2D ObjTexture = null;
        //if (ObjRenderer.material.mainTexture != null)
        //    ObjTexture = ObjRenderer.material.mainTexture.TryCast<Texture2D>();

        //ObjRenderer.material = new Material(Constants.EnvironmentMaterial);
        //ObjRenderer.material.color = ObjColor * Constants.EnvironmentColor;
        //ObjRenderer.material.mainTexture = null;

        //if (ObjTexture != null)
        //    ObjRenderer.material.mainTexture = ObjTexture;
    }
}