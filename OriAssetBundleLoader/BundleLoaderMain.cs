using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.IO;
using UniverseLib.Input;
using UniverseLib.Config;
using MelonLoader.Utils;
using Il2CppGame;
using System.Collections;
using Il2CppCatlikeCoding.TextBox;

namespace OriAssetBundleLoader
{
    public class BundleLoaderMain : MelonMod
    {
        public static Il2CppAssetBundle Bundle;

        public static ConverterManager ConverterManager = new ConverterManager();

        public static PrefabManager PrefabManager = new PrefabManager();

        public static GameObject LatestLevelInstance;

        public override void OnInitializeMelon()
        {

            if (!File.Exists($"{MelonEnvironment.ModsDirectory}/UnityExplorer.ML.IL2CPP.net6preview.interop.dll"))
            {
                string UnhollowedModulesFolder = Path.Combine(Path.GetDirectoryName(MelonEnvironment.ModsDirectory),Path.Combine("MelonLoader", "Il2CppAssemblies"));

                Universe.Init(0, OnUniverseInit, OnUniverseLog, new UniverseLibConfig()
                {
                    Disable_EventSystem_Override = false,
                    Force_Unlock_Mouse = false,
                    Unhollowed_Modules_Folder = UnhollowedModulesFolder
                });
            }

            LevelManager.RegisterComponents();

            ConverterManager.SetupConverters();

            Bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/assets/ori");
        }

        void OnUniverseInit() { }
        void OnUniverseLog(string txt, LogType type) { }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "wotwTitleScreen") return;

            MelonCoroutines.Start(PrefabManager.SetupPrefabs());

            MelonCoroutines.Start(LevelManager.SetupPauseMenuElement());
        }

        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(KeyCode.J))
            {
                LevelManager.ReLoadLevel();
            }
        }
    }
}