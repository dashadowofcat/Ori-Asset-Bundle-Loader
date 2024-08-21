using Il2Cpp;
using Il2CppMoon;
using Il2CppMoon.ArtOptimization;
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
    public static Dictionary<string, ElementConverter> Converters = new Dictionary<string, ElementConverter>();

    public void SetupConverters()
    {
        Converters.Add("Bash", new BashConverter());
        Converters.Add("Leash", new LeashConverter());
        Converters.Add("Spring", new SpringConverter());
        Converters.Add("LifePlant", new LifePlantConverter());
        Converters.Add("HornBug", new HornBugConverter());
        Converters.Add("Mantis", new MantisConverter());
        Converters.Add("DamageDealer", new DamageDealerConverter());
        Converters.Add("TerrainDamageDealer", new TerrainDamageDealerConverter());
        Converters.Add("RotatingSpikeHazard", new RotatingSpikeHazardConverter());
        Converters.Add("AutoRotate", new AutoRotateConverter());
        Converters.Add("SpawnPosition", new SpawnPositionConverter());
    }


    /// <summary>
    /// registers a converter into the converter 
    /// dictionary to be used in level loading
    /// </summary>
    public static void RegisterConverter(string GameObjectName, ElementConverter Converter)
    {
        Converters.Add("SpawnPosition", new SpawnPositionConverter());
    }

    public void ConvertToWOTW(Transform LevelParent)
    {
        foreach (Transform child in RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(m => m.root == LevelParent && m != LevelParent))
        {
            ConvertAssetToWOTW(child.gameObject);
        }

        foreach (Transform child in RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(m => m.root == LevelParent && m != LevelParent))
        {
            GameObject Asset = child.gameObject;

            if (Asset.GetComponent<Collider>())
            {
                ConvertColliderToWOTW(Asset);

                return;
            }
        }
    }


    private void ConvertAssetToWOTW(GameObject Asset)
    {
        ConvertElementToWOTW(Asset);

        if (Asset.GetComponent<MeshRenderer>())
        {
            ConvertRendererToWOTW(Asset);
            return;
        }
    }

    private void ConvertRendererToWOTW(GameObject Asset)
    {
        MeshRenderer ObjRenderer = Asset.GetComponent<MeshRenderer>();

        Color ObjColor = ObjRenderer.material.color;

        Texture2D ObjTexture = ObjRenderer.material.mainTexture.TryCast<Texture2D>();

        ObjRenderer.material = Settings.EnvironmentMaterial;

        ObjRenderer.material.color = ObjColor * Settings.EnvironmentColor;

        ObjRenderer.material.mainTexture = ObjTexture;

        UberShaderAPI.SetTexture(ObjRenderer, UberShaderProperty_Texture.MultiplyLayerMaskTexture, null);
        UberShaderAPI.SetTexture(ObjRenderer, UberShaderProperty_Texture.MultiplyLayerTexture, null);
    }

    private void ConvertColliderToWOTW(GameObject Asset)
    {
        Asset.layer = 10;
    }
    private void ConvertElementToWOTW(GameObject Asset)
    {
        if (!Converters.ContainsKey(Asset.name)) return;

        Converters[Asset.name].ConvertElement(Asset);
    }
}

