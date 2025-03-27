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

public class LevelManager
{

    public static GameObject LevelInstance;

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

    public static void TeleportToLevel()
    {
        LoadLevel();

        GameObject ScenesManager = GameObject.Find("systems/scenesManager");

        RuntimeSceneMetaData stressTestMaster = ScenesManager.GetComponent<ScenesManager>().AllScenes.ToArray().Where(S => S.Scene == "stressTestMaster").FirstOrDefault();

        ScenesManager.GetComponent<GoToSceneController>().GoToScene(stressTestMaster, new Action(OnLoadStressTestMasterScene), false);
    }

    public static void LoadLevel()
    {
        if(LevelInstance != null) GameObject.Destroy(LevelInstance);

        LevelInstanceSettings.ResetSettings();

        GameObject Root = BundleLoaderMain.Bundle.LoadAsset<GameObject>("Level");

        GameObject level = UnityEngine.Object.Instantiate(Root);

        LevelInstance = level;

        level.transform.position = Constants.LevelSpawnPosition;

        UnityEngine.Object.DontDestroyOnLoad(level);

        BundleLoaderMain.ConverterManager.ConvertToWOTW(level.transform);

        if(LevelInstanceSettings.ShowLevelTitle) ShowLevelTitle();

        DelayedActionManager.Instance.ExecuteAfter(.03f, new Action(EnterLevel));
    }

    static void EnterLevel()
    {
        GameplayCamera camera = RuntimeHelper.FindObjectsOfTypeAll<GameplayCamera>().FirstOrDefault();

        GameObject.Find("seinCharacter").transform.position = LevelInstanceSettings.PlayerSpawnPosition;
        camera.MoveCameraToTargetInstantly(true);
    }

    static bool SetSize = false;
    static void OnLoadStressTestMasterScene()
    {

        if (!GameObject.Find("stressTestMaster")) return;

        GameObject ScenesManager = GameObject.Find("systems/scenesManager");

        RuntimeSceneMetaData metaData = ScenesManager.GetComponent<ScenesManager>().ActiveScenes.ToArray().Where(S => S.SceneRoot.name == "stressTestMaster").FirstOrDefault().MetaData;

        if (SetSize == false)
        {
            Rect rect = metaData.SceneBoundaries[0];

            float enlargeAmount = Constants.LevelBoundsEndlargeAmount;

            Rect newRect = new Rect(rect.x - enlargeAmount / 2, rect.y - enlargeAmount / 2, enlargeAmount, enlargeAmount);

            metaData.SceneBoundaries[0] = newRect;

            metaData.m_totalRect = newRect;

            SetSize = true;
        }

        SceneManager.UnloadSceneAsync("stressTestMaster");
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
            LevelManager.TeleportToLevel();

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

