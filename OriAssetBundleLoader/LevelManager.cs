using Il2Cpp;
using Il2CppCatlikeCoding.TextBox;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UniverseLib;
using OriAssetBundleLoader;
using UnityEngine.SceneManagement;
using Il2CppMoon.Timeline;
using MelonLoader;
using System.Collections.Generic;
using System.IO;

public class LevelManager
{
    public static bool useLevelHub = false;

    public static GameObject LevelInstance = null;
    public static bool teleportToLevel = false;

    public static List<string> levelJsonFiles;
    public static string currentLevelJsonFile = null;

    private static Il2CppAssetBundle Bundle = null;

    private static bool firstFrameUpdate = false;

    public static GameObject ori = null;

    public static ScenesManager scenesManager = null;
    public static GoToSceneController goToSceneController = null;

    public static GameController gameController = null;
    //public static SavePedestalController savePedestalController = null;

    private static List<Transform> bashTransforms = new List<Transform>();

    private static List<EntityPlaceholder> enemyPlaceholders = new List<EntityPlaceholder>();
    private static List<Animator> animators = new List<Animator>();

    private static bool reachedGoal = false;

    public static void Initialize()
    {
        // Finds the system game objects
        GameObject scenesManagerObject = GameObject.Find("systems/scenesManager");
        scenesManager = scenesManagerObject.GetComponent<ScenesManager>();
        goToSceneController = scenesManagerObject.GetComponent<GoToSceneController>();

        GameObject gameControllerObject = GameObject.Find("systems/gameController");
        gameController = gameControllerObject.GetComponent<GameController>();

        //GameObject worldMapLogicObject = GameObject.Find("systems/worldMapLogic");
        //savePedestalController = worldMapLogicObject.GetComponent<SavePedestalController>();

        // Sets stressTestMaster's boundaries
        RuntimeSceneMetaData stressTestMaster = scenesManager.AllScenes.ToArray().Where(S => S.Scene == "stressTestMaster").FirstOrDefault();

        MelonLogger.Msg("Setting stressTestMaster boundaries...");
        Rect newRect = new Rect(-2859.5f, -4838.5f, 5000f, 5000f);

        stressTestMaster.SceneBoundaries[0] = newRect;
        stressTestMaster.m_totalRect = newRect;
    }

    public static void UpdateInFirstFrameAfterLevelLoad()
    {
        foreach(Transform bashTransform in bashTransforms)
        {
            if(bashTransform != null)
            {
                bashTransform.localPosition = Vector3.zero;
            }
        }
    }

    public static void Update()
    {
        if(LevelInstance != null)
        {
            if(firstFrameUpdate)
            {
                UpdateInFirstFrameAfterLevelLoad();
                firstFrameUpdate = false;
            }

            if(ori != null)
            {
                if(!reachedGoal)
                {
                    Vector2 oriPosition = ori.transform.position;
                    Rect goalRect = LevelInstanceSettings.GoalRect;

                    if (oriPosition.x >= goalRect.xMin && oriPosition.x <= goalRect.xMax && oriPosition.y >= goalRect.yMin && oriPosition.y <= goalRect.yMax)
                    {
                        // Ori reached the goal! Teleports back to Wellspring Glade
                        reachedGoal = true;
                        SavePedestalController.BeginTeleportation(new Vector2(-307f, -4151f));
                    }
                }
            }

            foreach (EntityPlaceholder enemyPlaceholder in enemyPlaceholders)
            {
                if (enemyPlaceholder != null)
                {
                    enemyPlaceholder.UpdateAutoSpawnState();
                }
            }

            foreach(Animator animator in animators)
            {
                if(animator != null)
                {
                    for(int i = 0; i < animator.parameters.Length; ++i)
                    {
                        if (animator.parameters[i].name == "GameTime")
                        {
                            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                            if (animationLength > 0f)
                            {
                                float t = gameController.GameTime / animationLength;
                                animator.SetFloat("GameTime", t - ((int)t));
                            }

                            break;
                        }
                    }
                }
            }

            // In Level Hub
            if (currentLevelJsonFile == Constants.levelHubJsonFileName)
            {
                if(ori != null)
                {
                    Transform levelHolder = LevelInstance.transform.Find("LevelHolder");
                    if(levelHolder)
                    {
                        for(int i = 0; i < levelHolder.childCount; ++i)
                        {
                            Transform level = levelHolder.GetChild(i);

                            Vector2 oriPosition = ori.transform.position;
                            if(oriPosition.x >= level.position.x - Constants.levelImageSize / 2f && oriPosition.x <= level.position.x + Constants.levelImageSize / 2f &&
                                oriPosition.y >= level.position.y - Constants.levelImageSize / 2f && oriPosition.y <= level.position.y + Constants.levelImageSize / 2f)
                            {
                                // Ori jump into a level image. Load the level and teleport Ori to it!
                                LevelManager.currentLevelJsonFile = level.gameObject.name; // The object name is the level json file to load
                                LevelManager.LoadLevel();
                                LevelManager.TeleportToLevelSpawnPosition();
                            }
                        }
                    }
                }
            }
        }
    }

    public static void AddBashTransform(Transform bashTransform)
    {
        if(!bashTransforms.Contains(bashTransform))
        {
            bashTransforms.Add(bashTransform);
        }
    }

    public static void AddEnemyPlaceholder(EntityPlaceholder enemyPlaceholder)
    {
        if (!enemyPlaceholders.Contains(enemyPlaceholder))
        {
            enemyPlaceholders.Add(enemyPlaceholder);
        }
    }

    public static void AddAnimator(Animator animator)
    {
        if(!animators.Contains(animator))
        {
            animators.Add(animator);
        }
    }

    public static void FindLevelFiles()
    {
        levelJsonFiles = Directory.GetFiles("Mods/OriCanvasLevels", "*.json").ToList<string>();
        MelonLogger.Msg("Level Json Files");
        foreach(string levelJsonFile in levelJsonFiles)
        {
            MelonLogger.Msg("  " + levelJsonFile);
        }
    }

    public static void RegisterComponents()
    {
        ClassInjector.RegisterTypeInIl2Cpp<TextSetter>();
        ClassInjector.RegisterTypeInIl2Cpp<TeleportToLevelOnActivate>();
    }

    public static IEnumerator SetupPauseMenuElement()
    {
        while (RuntimeHelper.FindObjectsOfTypeAll<PauseScreen>().FirstOrDefault() == null) yield return new WaitForFixedUpdate();

        // setup variables

        GameObject PauseScreen = RuntimeHelper.FindObjectsOfTypeAll<PauseScreen>().FirstOrDefault().gameObject;

        CleverMenuItemSelectionManager selectionManager = PauseScreen.GetComponent<CleverMenuItemSelectionManager>();

        CleverMenuItemLayout layout = PauseScreen.transform.Find("faderRoot/items").GetComponent<CleverMenuItemLayout>();

        GameObject OriginalButton = PauseScreen.transform.Find("faderRoot/items/options").gameObject;

        // instantiate button;

        CleverMenuItem Button = GameObject.Instantiate(OriginalButton, layout.transform).GetComponent<CleverMenuItem>();

        // setup button

        Button.name = "Visit Level";

        layout.MenuItems.Add(Button);
        selectionManager.AddMenuItem(Button);

        GameObject text = Button.transform.Find("exitGame").gameObject;

        TextSetter setter = text.AddComponent<TextSetter>();

        if(useLevelHub)
            setter.Text = "LEVEL HUB";
        else
            setter.Text = "ENTER LEVEL";

        // setup on pressed action sequence

        ActionSequence PressedSequence = Button.transform.Find("pressed").GetComponent<ActionSequence>();

        PressedSequence.Actions.Clear();

        GameObject.Destroy(PressedSequence.transform.Find("01. Show Options Action"));

        ActivateAction TeleportToLevelAction = new GameObject("01. Teleport To Level").AddComponent<ActivateAction>();

        GameObject TeleportToLevelOnActive = new GameObject("TeleportToLevelOnActive");


        TeleportToLevelAction.Activate = true;

        TeleportToLevelAction.Target = TeleportToLevelOnActive;

        TeleportToLevelOnActive.transform.parent = TeleportToLevelAction.transform;

        TeleportToLevelOnActive.AddComponent<TeleportToLevelOnActivate>();

        ShowMainMenuAction ShowMainMenuAction = new GameObject("02. Hide Menu").AddComponent<ShowMainMenuAction>();

        ShowMainMenuAction.Show = false;
        ShowMainMenuAction.Immediate = false;

        PressedSequence.Actions.Add(TeleportToLevelAction);
        PressedSequence.Actions.Add(ShowMainMenuAction);
    }

    public static void LoadStressTestMaster()
    {
        RuntimeSceneMetaData stressTestMaster = scenesManager.AllScenes.ToArray().Where(S => S.Scene == "stressTestMaster").FirstOrDefault();
        goToSceneController.GoToScene(stressTestMaster, new Action(GoToStressTestMasterAction), false);
    }

    public static void GoToStressTestMasterAction()
    {
    }

    public static void LoadLevel()
    {
        // Resets and destroys the current level
        bashTransforms.Clear();
        enemyPlaceholders.Clear();
        animators.Clear();

        if (LevelInstance != null)
        {
            GameObject.Destroy(LevelInstance);
            LevelInstance = null;
        }

        LevelInstanceSettings.ResetSettings();

        // If no current level json file specified, loads the default level hub.
        if (currentLevelJsonFile == null)
            currentLevelJsonFile = Constants.levelHubJsonFileName;

        // Creates the level
        if(useLevelHub)
            LevelInstance = LevelManager.LoadLevelFromJsonFile(currentLevelJsonFile);
        else
            LevelInstance = LevelManager.LoadLevelFromAssetBundle("Mods/assets/ori");

        if (LevelInstance == null)
        {
            MelonLogger.Error("Failed to create level!");
            return;
        }

        // If loaded the level hub, loads the level images for Ori to jump into
        if(currentLevelJsonFile == Constants.levelHubJsonFileName)
        {
            GameObject levelHolder = new GameObject("LevelHolder");
            levelHolder.transform.parent = LevelInstance.transform;

            float x = -740f;

            foreach(string levelJsonFile in levelJsonFiles)
            {
                if (levelJsonFile.Contains("LevelHub.json"))
                    continue;

                string levelFileName = levelJsonFile.Substring(0, levelJsonFile.Length - 5);
                string fullLevelImageFileName = levelFileName + "/level.png";

                MelonLogger.Msg("Custom Level Image: " + fullLevelImageFileName);

                GameObject levelObject = new GameObject(levelJsonFile);
                levelObject.transform.parent = levelHolder.transform;
                levelObject.transform.position = new Vector3(x, -995f, 0f);

                if (File.Exists(fullLevelImageFileName))
                {
                    GameObject levelImageObject = JSONLevelManager.CreateQuadFromSprite(fullLevelImageFileName, levelObject.transform, new Vector3(x, -995f, 0.5f), Vector2.one);
                    levelImageObject.transform.localScale = new Vector3(Constants.levelImageSize, Constants.levelImageSize, 1f);
                }

                JSONLevelManager.CreateQuadFromSprite("Mods/OriCanvasLevels/LevelHub/Sprites/woodSlats.png", levelObject.transform, new Vector3(x, -995f,   0.6f), new Vector2(1.3f, 1.3f));
                JSONLevelManager.CreateQuadFromSprite("Mods/OriCanvasLevels/LevelHub/Sprites/woodPlank.png", levelObject.transform, new Vector3(x, -998.5f, 0.7f), new Vector2(2f, 2f));
                x += 7f;
            }
        }

        // Sets the level to the stressTestMaster scene
        LevelInstance.transform.position = Constants.LevelSpawnPosition;

        GameObject stressTestMasterObject = GetStressTestMasterObject();
        LevelInstance.transform.parent = stressTestMasterObject.transform;

        // Converts all child objects into WotW elements
        BundleLoaderMain.ConverterManager.ConvertToWOTW(LevelInstance.transform);

        // Sets the camera's default position and fov
        SceneSettingsComponent sceneSettingsComponent = stressTestMasterObject.GetComponent<SceneSettingsComponent>();
        sceneSettingsComponent.m_sceneSettings.DefaultCameraZoom = LevelInstanceSettings.CameraPosition;
        sceneSettingsComponent.FieldOfView = LevelInstanceSettings.CameraFoV;

        firstFrameUpdate = true;
    }

    public static GameObject LoadLevelFromAssetBundle(string assetBundle)
    {
        if (Bundle != null)
            Bundle.Unload(true);

        Bundle = Il2CppAssetBundleManager.LoadFromFile(assetBundle);

        currentLevelJsonFile = assetBundle;

        return UnityEngine.Object.Instantiate(Bundle.LoadAsset<GameObject>("Level"));
    }

    public static GameObject LoadLevelFromJsonFile(string levelJsonFile)
    {
        MelonLogger.Msg("Loading json file " + levelJsonFile + "...");
        string json = File.ReadAllText(levelJsonFile);

        if (json == null)
            return null;

        currentLevelJsonFile = levelJsonFile;

        return JSONLevelManager.GenerateLevelFromJson(json);
    }

    public static void TeleportToLevelSpawnPosition()
    {
        TeleportToLocation(LevelInstanceSettings.PlayerSpawnPosition);
    }

    public static void TeleportToLocation(Vector2 location)
    {
        GameObject ori = GameObject.Find("seinCharacter");
        if(ori != null)
        {
            MelonLogger.Msg("Teleporting Ori to " + location + "...");

            GameplayCamera camera = RuntimeHelper.FindObjectsOfTypeAll<GameplayCamera>().FirstOrDefault();

            ori.transform.position = location;
            camera.MoveCameraToTargetInstantly(true);
        }
    }

    public static GameObject GetStressTestMasterObject()
    {
        return SceneManager.GetSceneByName("stressTestMaster").GetRootGameObjects().ToArray().Where(R => R.name == "stressTestMaster").FirstOrDefault();
    }

    public static IEnumerator OnLoadStressTestMasterSceneRoutine()
    {
        MelonLogger.Msg("OnLoadStressTestMasterScene");

        // Gets the stressTestMaster object
        GameObject stressTestMasterObj = GameObject.Find("stressTestMaster");
        while (stressTestMasterObj == null)
        {
            yield return new WaitForFixedUpdate();
            stressTestMasterObj = GameObject.Find("stressTestMaster");
        }

        stressTestMasterObj = GetStressTestMasterObject();

        for (int i = 0; i < stressTestMasterObj.transform.childCount; ++i)
        {
            Transform child = stressTestMasterObj.transform.GetChild(i);
            MelonLogger.Msg("  " + child.name);

            if (child.name == "ground")
            {
                child.gameObject.SetActive(false);
            }
        }

        // Finds the Oi
        MelonLogger.Msg("Finding the Oi...");
        ori = GameObject.Find("seinCharacter");

        // Loads the custom level.
        LoadLevel();

        // Resets reach goal flag
        reachedGoal = false;

        // If teleporting to level, shows level title and teleports to it.
        if (teleportToLevel)
        {
            if (LevelInstanceSettings.ShowLevelTitle)
                ShowLevelTitle();

            DelayedActionManager.Instance.ExecuteAfter(0.1f, new Action(TeleportToLevelSpawnPosition));

            teleportToLevel = false;
        }
    }

    static void ShowLevelTitle()
    {
        GameObject AreaTitle = GameObject.Instantiate(PrefabCachingManager.GetPrefab("AreaTextTimeline"));

        ShowAreaMessageAnimatorEntity AreaTitleAnimator = AreaTitle.GetComponentInChildren<ShowAreaMessageAnimatorEntity>();

        TranslatedMessageProvider TitleMessage = new TranslatedMessageProvider();

        TranslatedMessageProvider.MessageItem item = new TranslatedMessageProvider.MessageItem();

        item.English = LevelInstanceSettings.LevelTitle;

        TitleMessage.Messages.Add(item);

        AreaTitleAnimator.Message = TitleMessage;

        AreaTitle.GetComponent<MoonTimeline>().StartPlayback();
    }

    class TeleportToLevelOnActivate : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            if(LevelInstance == null)
            {
                LevelManager.teleportToLevel = true;
                LevelManager.currentLevelJsonFile = null;
                LevelManager.LoadStressTestMaster();
            }
            else
            {
                LevelManager.currentLevelJsonFile = null;
                LevelManager.LoadLevel(); // Reload level
                LevelManager.TeleportToLevelSpawnPosition();
            }

            gameObject.SetActive(false);
        }
    }

    class TextSetter : MonoBehaviour
    {
        public string Text;

        TextBox TextBox;

        void Update()
        {
            if (TextBox == null) TextBox = GetComponent<TextBox>();

            TextBox.SetText(Text);
            TextBox.RefreshText();
        }
    }
}

