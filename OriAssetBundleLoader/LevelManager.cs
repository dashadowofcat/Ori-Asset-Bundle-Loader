using Il2Cpp;
using Il2CppCatlikeCoding.TextBox;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib;
using Il2CppInterop.Runtime;
using OriAssetBundleLoader;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.SceneManagement;

public class LevelManager
{

    public static void RegisterComponents()
    {
        ClassInjector.RegisterTypeInIl2Cpp<TextSetter>();

        ClassInjector.RegisterTypeInIl2Cpp<TeleportToLevelOnActivate>();
    }

    public static IEnumerator SetupPauseMenuElement()
    {
        while (RuntimeHelper.FindObjectsOfTypeAll<PauseScreen>().FirstOrDefault() == null) yield return new WaitForFixedUpdate();

        GameObject PauseScreen = RuntimeHelper.FindObjectsOfTypeAll<PauseScreen>().FirstOrDefault().gameObject;

        CleverMenuItemSelectionManager selectionManager = PauseScreen.GetComponent<CleverMenuItemSelectionManager>();

        CleverMenuItemLayout layout = PauseScreen.transform.Find("faderRoot/items").GetComponent<CleverMenuItemLayout>();

        GameObject OriginalButton = PauseScreen.transform.Find("faderRoot/items/options").gameObject;

        CleverMenuItem Button = GameObject.Instantiate(OriginalButton, layout.transform).GetComponent<CleverMenuItem>();

        Button.name = "Visit Level";

        layout.MenuItems.Add(Button);
        selectionManager.AddMenuItem(Button);

        GameObject text = Button.transform.Find("exitGame").gameObject;

        TextSetter setter = text.AddComponent<TextSetter>();

        setter.Text = "ENTER LEVEL";

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
        if(BundleLoaderMain.LatestLevelInstance != null)
        {
            ReLoadLevel();
            return;
        }

        GameObject Root = BundleLoaderMain.Bundle.LoadAsset<GameObject>("Level");

        GameObject obj = UnityEngine.Object.Instantiate(Root);

        BundleLoaderMain.LatestLevelInstance = obj;

        obj.transform.position = Settings.LevelSpawnPosition;

        UnityEngine.Object.DontDestroyOnLoad(obj);

        BundleLoaderMain.ConverterManager.ConvertToWOTW(obj.transform);
    }

    public static void ReLoadLevel()
    {
        GameObject Root = BundleLoaderMain.Bundle.LoadAsset<GameObject>("Level");

        GameObject obj = UnityEngine.Object.Instantiate(Root);

        obj.transform.position = BundleLoaderMain.LatestLevelInstance.transform.position;

        GameObject.Destroy(BundleLoaderMain.LatestLevelInstance);

        BundleLoaderMain.LatestLevelInstance = obj;

        UnityEngine.Object.DontDestroyOnLoad(obj);

        BundleLoaderMain.ConverterManager.ConvertToWOTW(obj.transform);
    }

    static bool SetSize = false;
    public static void OnLoadStressTestMasterScene()
    {
        GameObject.Find("seinCharacter").transform.position = Settings.PlayerSpawnPosition;


        GameObject ScenesManager = GameObject.Find("systems/scenesManager");

        RuntimeSceneMetaData metaData = ScenesManager.GetComponent<ScenesManager>().ActiveScenes.ToArray().Where(S => S.SceneRoot.name == "stressTestMaster").FirstOrDefault().MetaData;

        if (SetSize == false)
        {
            Rect rect = metaData.SceneBoundaries[0];

            float enlargeAmount = 1000;

            Rect newRect = new Rect(rect.x - enlargeAmount / 2, rect.y - enlargeAmount / 2, enlargeAmount, enlargeAmount);

            metaData.SceneBoundaries[0] = newRect;

            metaData.m_totalRect = newRect;

            SetSize = true;
        }

        SceneManager.UnloadSceneAsync("stressTestMaster");
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

        void Start()
        {
            TextBox text = GetComponent<TextBox>();

            text.SetText(Text);
            text.RefreshText();
        }
    }
}

