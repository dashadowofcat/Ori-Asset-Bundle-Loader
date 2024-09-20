using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using UnityEngine;
using UniverseLib;
using Il2CppSystem.IO;
using UniverseLib.Config;
using MelonLoader.Utils;
using UniverseLib.Input;

namespace OriAssetBundleLoader
{
    public class BundleLoaderMain : MelonMod
    {
        public static Il2CppAssetBundle Bundle;

        public static ConverterManager ConverterManager = new ConverterManager();

        public static PrefabCachingManager PrefabManager = new PrefabCachingManager();

        public static GameObject LevelInstance;

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

            ConverterManager.RegisterConverters();

            Bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/assets/ori");
        }

        void OnUniverseInit() { }
        void OnUniverseLog(string txt, LogType type) { }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "wotwTitleScreen") return;

            PrefabManager.RegisterBuiltInPrefabs();

            MelonCoroutines.Start(LevelManager.SetupPauseMenuElement());
        }

        public override void OnUpdate()
        {
            if(InputManager.GetKeyDown(KeyCode.U))
            {
                LevelManager.LoadLevel();
            }
        }
    }
}