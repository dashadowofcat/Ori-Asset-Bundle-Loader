using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using UnityEngine;
using UniverseLib;
using Il2CppSystem.IO;
using UniverseLib.Config;
using MelonLoader.Utils;
using UniverseLib.Input;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

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

        public static GameObject ori = null;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("Loaded scene: " + sceneName);

            if(sceneName == "wotwTitleScreen") // Loaded title screen
            {
                if(!Initialized)
                {
                    // Sets stressTestMaster's boundaries
                    GameObject ScenesManager = GameObject.Find("systems/scenesManager");
                    RuntimeSceneMetaData stressTestMaster = ScenesManager.GetComponent<ScenesManager>().AllScenes.ToArray().Where(S => S.Scene == "stressTestMaster").FirstOrDefault();

                    MelonLogger.Msg("Setting stressTestMaster boundaries...");
                    Rect newRect = new Rect(-2859.5f, -4838.5f, 5000f, 5000f);

                    stressTestMaster.SceneBoundaries[0] = newRect;
                    stressTestMaster.m_totalRect = newRect;

                    MelonLogger.Msg("Loading prefabs...");
                    PrefabManager.InitializeHolder();
                    PrefabManager.RegisterBuiltInPrefabs();
                }

                // Sets up the Enter Level menu item
                MelonLogger.Msg("Setting up pause menu element...");
                MelonCoroutines.Start(LevelManager.SetupPauseMenuElement());

                Initialized = true;
            }
            else if(sceneName == "stressTestMaster") // Loaded Stress Test Master
            {
                MelonCoroutines.Start(LevelManager.OnLoadStressTestMasterSceneRoutine());
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasUnloaded(buildIndex, sceneName);

            MelonLogger.Msg("Unloaded scene: " + sceneName);
        }

        public override void OnUpdate()
        {
            if(ori == null)
            {
                ori = GameObject.Find("seinCharacter");
            }

            if (InputManager.GetKeyDown(KeyCode.U) && InputManager.GetKey(KeyCode.LeftControl))
            {
                if(LevelManager.LevelInstance == null)
                {
                    // Loads the custom level and teleports to it.
                    LevelManager.teleportToLevel = true;
                    LevelManager.LoadStressTestMaster();
                }
                else
                {
                    LevelManager.LoadLevel(); // Reloads level
                    LevelManager.TeleportToLevelSpawnPosition();
                }
            }

            LevelManager.Update();
        }
    }
}