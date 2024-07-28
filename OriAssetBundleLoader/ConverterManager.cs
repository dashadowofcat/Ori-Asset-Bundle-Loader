using Il2Cpp;
using OriAssetBundleLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib;

public class ConverterManager
{
    public Dictionary<string, ElementConverter> Converters = new Dictionary<string, ElementConverter>();

    public void SetupConverters()
    {
        Converters.Add("Bash", new BashConverter());
        Converters.Add("Leash", new LeashConverter());
        Converters.Add("Spring", new SpringConverter());
    }

    public void ConvertToWOTW(Transform parent)
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

        ObjRenderer.material = Settings.EnvironmentMaterial;

        ObjRenderer.material.color = Settings.EnvironmentColor;

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

