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
        public static ConverterManager ConverterManager = new ConverterManager();

        public static PrefabCachingManager PrefabManager = new PrefabCachingManager();

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

            ConverterManager.RegisterBuiltInConverters();

            LevelManager.FindLevelFiles();
        }

        void OnUniverseInit() { }
        void OnUniverseLog(string txt, LogType type) { }

        private bool Initialized;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "wotwTitleScreen" || Initialized) return;

            PrefabManager.InitializeHolder();

            PrefabManager.RegisterBuiltInPrefabs();

            MelonCoroutines.Start(LevelManager.SetupPauseMenuElement());

            Initialized = true;
        }

        public override void OnUpdate()
        {
            if(InputManager.GetKeyDown(KeyCode.U))
            {
                MelonLogger.Msg("Loading json file " + LevelManager.levelJsonFiles[0] + "...");
                string json = File.ReadAllText(LevelManager.levelJsonFiles[0]);

                LevelManager.LoadLevelFromJSON(json);
                //LevelManager.LoadLevel();
            }
        }
    }
}