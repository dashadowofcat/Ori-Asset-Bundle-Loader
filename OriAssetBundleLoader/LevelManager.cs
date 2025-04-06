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
using MelonLoader.TinyJSON;
using System.IO;
using Il2CppZenFulcrum.EmbeddedBrowser;

public class LevelManager
{

    public static GameObject LevelInstance;

    public static List<string> levelJsonFiles;

    public static void FindLevelFiles()
    {
        levelJsonFiles = new List<string>();
        levelJsonFiles.Add("Mods/OriCanvasLevels/TestLevel.json");
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

        //MelonLogger.Msg("Loading json file " + LevelManager.levelJsonFiles[0] + "...");
        //string json = File.ReadAllText(LevelManager.levelJsonFiles[0]);

        //LevelManager.LoadLevelFromJSON(json);

        GameObject ScenesManager = GameObject.Find("systems/scenesManager");

        RuntimeSceneMetaData stressTestMaster = ScenesManager.GetComponent<ScenesManager>().AllScenes.ToArray().Where(S => S.Scene == "stressTestMaster").FirstOrDefault();

        ScenesManager.GetComponent<GoToSceneController>().GoToScene(stressTestMaster, new Action(OnLoadStressTestMasterScene), false);
    }

    public static void LoadLevel()
    {
        if (LevelInstance != null) GameObject.Destroy(LevelInstance);

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

    public static void LoadLevelFromJSON(string json)
    {
        if(json == null) return;

        var levelObj = MelonLoader.TinyJSON.JSON.Load(json);
        string levelName = levelObj["name"];
        string levelTitle = levelObj["title"];
        Vector2 spawnPosition = levelObj["spawnPosition"].Make<Vector2>();

        MelonLogger.Msg("Level Name: " + levelName);
        MelonLogger.Msg("Level Title: " + levelTitle);
        MelonLogger.Msg("Spawn Position: " + spawnPosition);

        if (LevelInstance != null) GameObject.Destroy(LevelInstance);

        LevelInstanceSettings.ResetSettings();

        GameObject level = new GameObject("Level");

        GameObject spawnPositionObject = new GameObject("Spawn Position");
        spawnPositionObject.transform.parent = level.transform;
        spawnPositionObject.transform.position = spawnPosition;

        var spriteArray = levelObj["sprites"];
        if(spriteArray != null)
        {
            MelonLogger.Msg("Sprites");

            foreach(var sprite in spriteArray as ProxyArray)
            {
                string spriteFile = sprite["file"];
                Vector3 spritePosition = sprite["position"].Make<Vector3>();

                MelonLogger.Msg("Sprite File: " + spriteFile);
                MelonLogger.Msg("Sprite Position: " + spritePosition);

                string fullSpriteFileName = "Mods/OriCanvasLevels/" + spriteFile;

                if (!File.Exists(fullSpriteFileName))
                    continue;

                byte[] spriteFileData = File.ReadAllBytes(fullSpriteFileName);

                Texture2D texture = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture, spriteFileData);

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.parent = level.transform;

                Renderer renderer = plane.GetComponent<Renderer>();

                UnityEngine.Object.DestroyImmediate(plane.GetComponent<MeshCollider>());

                Material spriteMaterial = new Material(Shader.Find("Sprites/Default"));
                spriteMaterial.mainTexture = texture;
                renderer.sharedMaterial = spriteMaterial;

                plane.transform.position = spritePosition;

                plane.transform.localScale = new Vector3(texture.width / 1000f, 1, texture.height / 1000f);
                plane.transform.eulerAngles = new Vector3(90, 0, 180);

                plane.name = texture.name;
            }
        }

        var colliderArray = levelObj["colliders"];
        if(colliderArray != null)
        {
            MelonLogger.Msg("Colliders");

            foreach(var collider in colliderArray as ProxyArray)
            {
                string colliderType = collider["type"];
                Vector2[] colliderPoints = collider["points"].Make<Vector2[]>();
                Color colliderColor = collider["color"].Make<Color>();
                bool goThrough = collider["goThrough"];

                MelonLogger.Msg("Collider Type: " + colliderType);
                MelonLogger.Msg("Collider Points Size: " + colliderPoints.Length);
                MelonLogger.Msg("Collider Color: " + colliderColor);
                MelonLogger.Msg("Collider GoThrough: " + goThrough);

                GameObject colliderObj = new GameObject("collider");
                colliderObj.transform.parent = level.transform;

                if(colliderType == "Cubic" || colliderType == "Bezier" || colliderType == "Catmull Rom")
                {
                    int colliderSplineCount = 50;
                    if (collider["splineCount"] != null)
                        colliderSplineCount = collider["splineCount"];

                    MelonLogger.Msg("Collider Spline Count: " + colliderSplineCount);

                    List<Vector2> InterpolatedPoints;
                    switch(colliderType)
                    {
                        case "Bezier": InterpolatedPoints = MeshGenerator.Bezier.Interpolate(colliderPoints, colliderSplineCount).ToList(); break;
                        case "Catmull Rom": InterpolatedPoints = MeshGenerator.CatmullRom.Interpolate(colliderPoints, colliderSplineCount).ToList(); break;
                        default: InterpolatedPoints = MeshGenerator.Cubic.Interpolate(colliderPoints, colliderSplineCount).ToList(); break;
                    }

                    GameObject ColliderGameObject = new GameObject("Collision");

                    ColliderGameObject.layer = 10;
                    ColliderGameObject.transform.parent = colliderObj.transform;

                    List<MeshFilter> ColliderMeshes = new List<MeshFilter>();

                    for (int y = 0; y < InterpolatedPoints.ToArray().Length; y++)
                    {
                        if (y == InterpolatedPoints.ToArray().Length - 1) break;

                        var Quad = MeshGenerator.InstanciateCollisionQuad(InterpolatedPoints[y], InterpolatedPoints[y + 1], ColliderGameObject.transform, 1);

                        ColliderMeshes.Add(Quad.GetComponent<MeshFilter>());
                    }

                    List<Vector3> vertices = new List<Vector3>();
                    List<int> triangles = new List<int>();
                    int meshVertexIndex = 0;

                    foreach(MeshFilter colliderMesh in ColliderMeshes)
                    {
                        Mesh collMesh = colliderMesh.sharedMesh;
                        for(int i = 0; i < collMesh.vertices.Length; i++)
                        {
                            vertices.Add(colliderMesh.transform.localToWorldMatrix.MultiplyPoint3x4(collMesh.vertices[i]));
                        }
                        for(int i = 0; i < collMesh.triangles.Length; i++)
                        {
                            triangles.Add(collMesh.triangles[i] + meshVertexIndex);
                        }

                        meshVertexIndex = vertices.Count;

                        colliderMesh.gameObject.SetActive(false);
                    }

                    Mesh mesh = new Mesh();
                    mesh.vertices = vertices.ToArray();
                    mesh.triangles = triangles.ToArray();

                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    ColliderGameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
                    MeshCollider meshCollider = ColliderGameObject.AddComponent<MeshCollider>();

                    foreach (MeshFilter colliderMesh in ColliderMeshes)
                    {
                        UnityEngine.Object.DestroyImmediate(colliderMesh.gameObject);
                    }

                    if(colliderColor != null)
                    {
                        Mesh SplineMesh = MeshGenerator.CreateMeshFromSpline(InterpolatedPoints, 0.1f);

                        GameObject Renderer = new GameObject("Renderer");
                        Renderer.transform.parent = colliderObj.transform;

                        MeshFilter meshFilter = Renderer.AddComponent<MeshFilter>();

                        MeshRenderer meshRenderer = Renderer.AddComponent<MeshRenderer>();
                        meshRenderer.material = new Material(Constants.EnvironmentMaterial);
                        meshRenderer.material.color = Color.white;
                        meshRenderer.sharedMaterial.color = colliderColor;

                        meshFilter.mesh = SplineMesh;
                    }

                    if (goThrough)
                    {
                        CreateGameObjectProperty("GoThrough", "true", ColliderGameObject.transform);
                    }
                }
            }
        }

        var enemyArray = levelObj["enemies"];
        if(enemyArray != null)
        {
            MelonLogger.Msg("Enemies");

            foreach(var enemy in enemyArray as ProxyArray)
            {
                string enemyType = enemy["type"];
                Vector2 enemyPosition = enemy["position"].Make<Vector2>();

                MelonLogger.Msg("Enemy Type: " + enemyType);
                MelonLogger.Msg("Enemy Position: " + enemyPosition);

                GameObject enemyObj = new GameObject(enemyType);
                enemyObj.transform.parent = level.transform;
                enemyObj.transform.position = enemyPosition;

                if (enemyType == "Mantis")
                {
                    CreateGameObjectProperty("MantisType", "Base", enemyObj.transform);
                    CreateGameObjectProperty("MaxHealth", "10", enemyObj.transform);
                    CreateGameObjectProperty("MaxSensorRadius", "-1", enemyObj.transform);
                    CreateGameObjectProperty("LoseSightRadius", "-1", enemyObj.transform);
                    CreateGameObjectProperty("ShouldSpawnLoot", "true", enemyObj.transform);
                    CreateGameObjectProperty("EnergyOrbsNumber", "-1", enemyObj.transform);
                    CreateGameObjectProperty("HealthOrbsNumber", "-1", enemyObj.transform);
                    CreateGameObjectProperty("SpawnsExpOrbs", "false", enemyObj.transform);
                    CreateGameObjectProperty("ExpOrbNumber", "-1", enemyObj.transform);
                    CreateGameObjectProperty("RespawnOnScreen", "true", enemyObj.transform);
                    CreateGameObjectProperty("RespawnTime", "0", enemyObj.transform);
                }
            }
        }

        LevelInstance = level;

        level.transform.position = Constants.LevelSpawnPosition;

        UnityEngine.Object.DontDestroyOnLoad(level);

        BundleLoaderMain.ConverterManager.ConvertToWOTW(level.transform);

        if (LevelInstanceSettings.ShowLevelTitle) ShowLevelTitle();

        DelayedActionManager.Instance.ExecuteAfter(.03f, new Action(EnterLevel));
    }

    public static void CreateGameObjectProperty(string name, string value, Transform parent)
    {
        GameObject objectName = new GameObject(name);
        objectName.transform.parent = parent.transform;

        GameObject valueName = new GameObject(value);
        valueName.transform.parent = objectName.transform;
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

