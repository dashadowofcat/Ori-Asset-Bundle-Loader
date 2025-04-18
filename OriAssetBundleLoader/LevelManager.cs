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
using UnityEngine.Profiling.Memory.Experimental;

public class LevelManager
{
    public static GameObject LevelInstance = null;

    public static List<string> levelJsonFiles;

    private static Il2CppAssetBundle Bundle = null;

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
        if (LevelInstanceSettings.ShowLevelTitle)
            ShowLevelTitle();

        GoToStressTestMaster();

        DelayedActionManager.Instance.ExecuteAfter(0.1f, new Action(TeleportToLevelSpawnPosition));
    }

    public static void GoToStressTestMaster()
    {
        GameObject ScenesManager = GameObject.Find("systems/scenesManager");
        RuntimeSceneMetaData stressTestMaster = ScenesManager.GetComponent<ScenesManager>().AllScenes.ToArray().Where(S => S.Scene == "stressTestMaster").FirstOrDefault();
        ScenesManager.GetComponent<GoToSceneController>().GoToScene(stressTestMaster, new Action(GoToStressTestMasterAction), false);
    }

    public static void GoToStressTestMasterAction()
    {
    }

    public static void LoadLevel()
    {
        LevelManager.LoadLevelFromAssetBundle("Mods/assets/ori");
        //LevelManager.LoadLevelFromJsonFile(0);
    }

    public static void LoadLevelFromAssetBundle(string assetBundle)
    {
        if (LevelInstance != null)
            GameObject.Destroy(LevelInstance);

        LevelInstanceSettings.ResetSettings();

        if (Bundle != null)
            Bundle.Unload(true);

        Bundle = Il2CppAssetBundleManager.LoadFromFile(assetBundle);

        LevelInstance = UnityEngine.Object.Instantiate(Bundle.LoadAsset<GameObject>("Level"));
        LevelInstance.transform.position = Constants.LevelSpawnPosition;

        UnityEngine.Object.DontDestroyOnLoad(LevelInstance);

        BundleLoaderMain.ConverterManager.ConvertToWOTW(LevelInstance.transform);
    }

    public static void LoadLevelFromJsonFile(int levelJsonFileIndex)
    {
        MelonLogger.Msg("Loading json file " + LevelManager.levelJsonFiles[levelJsonFileIndex] + "...");
        string json = File.ReadAllText(LevelManager.levelJsonFiles[levelJsonFileIndex]);

        if (json == null)
            return;

        var levelObj = MelonLoader.TinyJSON.JSON.Load(json);
        string levelName = levelObj["name"];
        string assetBundle = levelObj["assetBundle"];

        MelonLogger.Msg("Level Name: " + levelName);
        MelonLogger.Msg("Asset Bundle: " + assetBundle);

        if (LevelInstance != null)
            GameObject.Destroy(LevelInstance);

        LevelInstanceSettings.ResetSettings();

        if (assetBundle != null && assetBundle != "")
        {
            if (Bundle != null)
                Bundle.Unload(true);

            Bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/OriCanvasLevels/" + assetBundle);
            LevelInstance = UnityEngine.Object.Instantiate(Bundle.LoadAsset<GameObject>("Level"));
        }
        else
        {
            string levelTitle = levelObj["title"];
            Vector2 spawnPosition = levelObj["spawnPosition"].Make<Vector2>();

            MelonLogger.Msg("Level Title: " + levelTitle);
            MelonLogger.Msg("Spawn Position: " + spawnPosition);

            GameObject level = new GameObject("Level");

            GameObject spawnPositionObject = new GameObject("Spawn Position");
            spawnPositionObject.transform.parent = level.transform;
            spawnPositionObject.transform.position = spawnPosition;

            var spriteArray = levelObj["sprites"];
            if (spriteArray != null)
            {
                MelonLogger.Msg("Sprites");

                foreach (var sprite in spriteArray as ProxyArray)
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
            if (colliderArray != null)
            {
                MelonLogger.Msg("Colliders");

                foreach (var collider in colliderArray as ProxyArray)
                {
                    string colliderType = collider["type"];
                    Vector2 colliderPosition = collider["position"].Make<Vector2>();
                    Vector2[] colliderPoints = collider["points"].Make<Vector2[]>();
                    Color colliderColor = collider["color"].Make<Color>();
                    bool goThrough = collider["goThrough"];

                    MelonLogger.Msg("Collider Type: " + colliderType);
                    MelonLogger.Msg("Collider Position: " + colliderPosition);
                    MelonLogger.Msg("Collider Points Size: " + colliderPoints.Length);
                    MelonLogger.Msg("Collider Color: " + colliderColor);
                    MelonLogger.Msg("Collider GoThrough: " + goThrough);

                    GameObject colliderObj = new GameObject("collider");
                    colliderObj.transform.parent = level.transform;
                    colliderObj.transform.position = colliderPosition;

                    GameObject ColliderGameObject = new GameObject("Collision");

                    ColliderGameObject.layer = 10;
                    ColliderGameObject.transform.parent = colliderObj.transform;

                    List<MeshFilter> ColliderMeshes = new List<MeshFilter>();
                    Mesh rendererMesh = null;
                    Vector3 rendererRotation = Vector3.zero;

                    if (colliderType == "Cubic" || colliderType == "Bezier" || colliderType == "Catmull Rom")
                    {
                        int colliderSplineCount = 50;
                        if (collider["splineCount"] != null)
                            colliderSplineCount = collider["splineCount"];

                        MelonLogger.Msg("Collider Spline Count: " + colliderSplineCount);

                        List<Vector2> InterpolatedPoints;
                        switch (colliderType)
                        {
                            case "Bezier": InterpolatedPoints = MeshGenerator.Bezier.Interpolate(colliderPoints, colliderSplineCount).ToList(); break;
                            case "Catmull Rom": InterpolatedPoints = MeshGenerator.CatmullRom.Interpolate(colliderPoints, colliderSplineCount).ToList(); break;
                            default: InterpolatedPoints = MeshGenerator.Cubic.Interpolate(colliderPoints, colliderSplineCount).ToList(); break;
                        }

                        for (int y = 0; y < InterpolatedPoints.Count; y++)
                        {
                            if (y == InterpolatedPoints.Count - 1) break;

                            var Quad = MeshGenerator.InstanciateCollisionQuad(InterpolatedPoints[y], InterpolatedPoints[y + 1], ColliderGameObject.transform, 1);

                            ColliderMeshes.Add(Quad.GetComponent<MeshFilter>());
                        }

                        if (colliderColor != null)
                        {
                            rendererMesh = MeshGenerator.CreateMeshFromSpline(InterpolatedPoints, 0.1f);
                        }
                    }
                    else
                    {
                        for (int y = 0; y < colliderPoints.Length; y++)
                        {
                            GameObject Quad;
                            if (y == colliderPoints.Length - 1)
                                Quad = MeshGenerator.InstanciateCollisionQuad(colliderPoints[y], colliderPoints[0], ColliderGameObject.transform, 1);
                            else
                                Quad = MeshGenerator.InstanciateCollisionQuad(colliderPoints[y], colliderPoints[y + 1], ColliderGameObject.transform, 1);

                            ColliderMeshes.Add(Quad.GetComponent<MeshFilter>());
                        }

                        if (colliderColor != null)
                        {
                            rendererMesh = MeshGenerator.CreateMeshFromPolygon(colliderPoints.ToList<Vector2>());
                            rendererRotation = new Vector3(-90, 0, 0);
                        }
                    }

                    List<Vector3> vertices = new List<Vector3>();
                    List<int> triangles = new List<int>();
                    int meshVertexIndex = 0;

                    foreach (MeshFilter colliderMesh in ColliderMeshes)
                    {
                        Mesh collMesh = colliderMesh.sharedMesh;
                        for (int i = 0; i < collMesh.vertices.Length; i++)
                        {
                            vertices.Add(colliderMesh.transform.localToWorldMatrix.MultiplyPoint3x4(collMesh.vertices[i]));
                        }
                        for (int i = 0; i < collMesh.triangles.Length; i++)
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

                    if (rendererMesh != null)
                    {
                        GameObject Renderer = new GameObject("Renderer");
                        Renderer.transform.parent = colliderObj.transform;
                        Renderer.transform.Rotate(rendererRotation);

                        MeshFilter meshFilter = Renderer.AddComponent<MeshFilter>();

                        MeshRenderer meshRenderer = Renderer.AddComponent<MeshRenderer>();
                        meshRenderer.material = new Material(Constants.EnvironmentMaterial);
                        meshRenderer.material.color = Color.white;
                        meshRenderer.sharedMaterial.color = colliderColor;

                        meshFilter.mesh = rendererMesh;
                    }

                    if (goThrough)
                    {
                        CreateGameObjectProperty("GoThrough", "true", ColliderGameObject.transform);
                    }
                }
            }

            var waterZoneArray = levelObj["waterZones"];
            if(waterZoneArray != null)
            {
                MelonLogger.Msg("Water Zones");

                foreach(var waterZone in waterZoneArray as ProxyArray)
                {
                    Vector2 leftBottom = waterZone["leftBottom"].Make<Vector2>();
                    Vector2 rightTop = waterZone["rightTop"].Make<Vector2>();

                    Rect waterZoneBounds = new Rect();
                    waterZoneBounds.Set(leftBottom.x + Constants.LevelSpawnPosition.x,
                        leftBottom.y + Constants.LevelSpawnPosition.y,
                        rightTop.x - leftBottom.x,
                        rightTop.y - leftBottom.y);

                    MelonLogger.Msg("Water Zone Bounds: " + waterZoneBounds);

                    GameObject waterZoneObj = new GameObject("waterZone");
                    waterZoneObj.transform.parent = level.transform;
                    waterZoneObj.transform.localPosition = leftBottom + waterZoneBounds.size / 2f;
                    waterZoneObj.transform.localScale = waterZoneBounds.size;

                    WaterZone waterZoneComp = waterZoneObj.AddComponent<WaterZone>();
                    waterZoneComp.Bounds = waterZoneBounds;
                    waterZoneComp.DamageCondition = WaterZone.DamageApplyType.NeverApplyDamage;
                }
            }

            var enemyArray = levelObj["enemies"];
            if (enemyArray != null)
            {
                MelonLogger.Msg("Enemies");

                foreach (var enemy in enemyArray as ProxyArray)
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

            var checkpoints = levelObj["checkpoints"];
            if (checkpoints != null)
            {
                MelonLogger.Msg("Checkpoints");

                foreach (var checkpoint in checkpoints as ProxyArray)
                {
                    Vector2 leftBottom = checkpoint["leftBottom"].Make<Vector2>();
                    Vector2 rightTop = checkpoint["rightTop"].Make<Vector2>();

                    Rect checkpointBounds = new Rect();
                    checkpointBounds.Set(leftBottom.x + Constants.LevelSpawnPosition.x,
                        leftBottom.y + Constants.LevelSpawnPosition.y,
                        rightTop.x - leftBottom.x,
                        rightTop.y - leftBottom.y);

                    MelonLogger.Msg("Checkpoint Bounds: " + checkpointBounds);

                    GameObject checkpointObj = new GameObject("checkpoint");
                    checkpointObj.transform.parent = level.transform;

                    InvisibleCheckpoint invisibleCheckpoint = checkpointObj.AddComponent<InvisibleCheckpoint>();
                    invisibleCheckpoint.m_bounds = checkpointBounds;
                }
            }

            LevelInstance = level;
        }

        LevelInstance.transform.position = Constants.LevelSpawnPosition;

        UnityEngine.Object.DontDestroyOnLoad(LevelInstance);

        BundleLoaderMain.ConverterManager.ConvertToWOTW(LevelInstance.transform);
    }

    public static void CreateGameObjectProperty(string name, string value, Transform parent)
    {
        GameObject objectName = new GameObject(name);
        objectName.transform.parent = parent.transform;

        GameObject valueName = new GameObject(value);
        valueName.transform.parent = objectName.transform;
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

    public static IEnumerator OnLoadStressTestMasterSceneRoutine()
    {
        MelonLogger.Msg("OnLoadStressTestMasterScene");

        GameObject stressTestMasterObj = GameObject.Find("stressTestMaster");
        while (stressTestMasterObj == null)
        {
            yield return new WaitForFixedUpdate();
            stressTestMasterObj = GameObject.Find("stressTestMaster");
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
            LevelManager.LoadLevel();
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

