using Il2Cpp;
using Il2CppMoon;
using Il2CppMoon.ArtOptimization;
using MelonLoader;
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

    public void RegisterBuiltInConverters()
    {
        RegisterConverter("Bash", new BashConverter());
        RegisterConverter("Leash", new LeashConverter());
        RegisterConverter("Spring", new SpringConverter());
        RegisterConverter("Life Plant", new LifePlantConverter());
        RegisterConverter("Energy Plant", new EnergyPlantConverter());
        RegisterConverter("HornBug", new HornBugConverter());
        RegisterConverter("Keystone", new KeystoneConverter());
        RegisterConverter("Mantis", new MantisConverter());
        RegisterConverter("Damage Dealer", new DamageDealerConverter());
        RegisterConverter("TerrainDamageDealer", new TerrainDamageDealerConverter());
        RegisterConverter("Auto Rotate", new AutoRotateConverter());
        RegisterConverter("Spawn Position", new SpawnPositionConverter());
        RegisterConverter("Leaper", new LeaperConverter());
        RegisterConverter("Mortar", new MortarConverter());
        RegisterConverter("Slime", new SlimeConverter());
        RegisterConverter("Level Title", new LevelTitleConverter());
        RegisterConverter("Checkpoint", new CheckpointConverter());

        RegisterConverter("Four Slot Door", new SlotDoorConverter(SlotDoorConverter.DoorType.FourSlot));
        RegisterConverter("Two Slot Door", new SlotDoorConverter(SlotDoorConverter.DoorType.TwoSlot));
    }


    /// <summary>
    /// registers a converter into the converter 
    /// dictionary to be used in level loading
    /// </summary>
    public static void RegisterConverter(string GameObjectName, ElementConverter Converter)
    {
        Converters.Add(GameObjectName, Converter);
    }

    public void ConvertToWOTW(Transform LevelParent)
    {
        MelonLogger.Msg("Converting assets to WotW...");
        foreach (Transform child in RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(m => m.root == LevelParent && m != LevelParent))
        {
            //MelonLogger.Msg("  " + child.gameObject.name);
            ConvertAssetToWOTW(child.gameObject);
        }

        MelonLogger.Msg("Converting colliders to WotW...");
        foreach (Transform child in RuntimeHelper.FindObjectsOfTypeAll<Transform>().Where(m => m.root == LevelParent && m != LevelParent))
        {
            GameObject Asset = child.gameObject;
            //MelonLogger.Msg("  " + Asset.name);

            if (Asset.GetComponent<Collider>())
            {
                ConvertColliderToWOTW(Asset);
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
        if (Asset == null) return;

        MeshRenderer ObjRenderer = Asset.GetComponent<MeshRenderer>();
        if (ObjRenderer == null) return;

        if(ObjRenderer.material != null)
        {
            Color ObjColor = ObjRenderer.material.color;
            Texture2D ObjTexture = null;
            if(ObjRenderer.material.mainTexture != null)
                ObjTexture = ObjRenderer.material.mainTexture.TryCast<Texture2D>();

            ObjRenderer.material = Constants.EnvironmentMaterial;
            ObjRenderer.material.color = ObjColor * Constants.EnvironmentColor;

            if(ObjTexture != null)
                ObjRenderer.material.mainTexture = ObjTexture;
        }
        else
        {
            ObjRenderer.material = Constants.EnvironmentMaterial;
            ObjRenderer.material.color = Constants.EnvironmentColor;
        }

        UberShaderAPI.SetTexture(ObjRenderer, UberShaderProperty_Texture.MultiplyLayerMaskTexture, null);
        UberShaderAPI.SetTexture(ObjRenderer, UberShaderProperty_Texture.MultiplyLayerTexture, null);
    }

    private void ConvertColliderToWOTW(GameObject Asset)
    {
        Asset.layer = 10;

        //MelonLogger.Msg("Checking if " + Asset.name + " has GoThrough object...");
        Transform goThroughObj = Asset.transform.FindChild("GoThrough");
        if (goThroughObj != null)
        {
            //MelonLogger.Msg("Adding GoThroughPlatform component...");
            GoThroughPlatform goThroughPlatform = Asset.AddComponent<GoThroughPlatform>();
            if (!GoThroughPlatformManager.GoThroughPlatforms.Contains(goThroughPlatform))
                GoThroughPlatformManager.Register(goThroughPlatform);
            goThroughPlatform.enabled = true;
        }
    }
    private void ConvertElementToWOTW(GameObject Asset)
    {
        if (!Converters.ContainsKey(Asset.name)) return;

        Converters[Asset.name].ConvertElement(Asset);
    }
}

