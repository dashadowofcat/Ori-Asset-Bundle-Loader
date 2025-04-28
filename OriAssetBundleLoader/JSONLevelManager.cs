using Il2Cpp;
using MelonLoader.TinyJSON;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using OriAssetBundleLoader;

public class JSONLevelManager
{
    public class MaterialTexture
    {
        public Texture2D texture;
        public Material material;
    }

    private static Dictionary<string, MaterialTexture> materialDict = new Dictionary<string, MaterialTexture>();

    public static void ClearMaterials()
    {
        materialDict.Clear();
    }

    public static GameObject GenerateLevelFromJson(string json)
    {
        ClearMaterials();

        // Reads the json file as an object
        var levelObj = MelonLoader.TinyJSON.JSON.Load(json);
        string levelName = levelObj["name"];
        string assetBundle = levelObj["assetBundle"];

        MelonLogger.Msg("Level Name: " + levelName);
        MelonLogger.Msg("Asset Bundle: " + assetBundle);

        // If an asset bundle is provided in the json file, loads from that instead
        if (assetBundle != null && assetBundle != "")
        {
            return LevelManager.LoadLevelFromAssetBundle("Mods/OriCanvasLevels/" + assetBundle);
        }

        string levelTitle = levelObj["title"];
        Vector3 cameraPosition = levelObj["cameraPosition"].Make<Vector3>();
        float cameraFoV = levelObj["cameraFoV"];
        Vector2 spawnPosition = levelObj["spawnPosition"].Make<Vector2>();

        MelonLogger.Msg("Level Title: " + levelTitle);
        MelonLogger.Msg("Camera Position: " + cameraPosition);
        MelonLogger.Msg("Camera FoV: " + cameraFoV);
        MelonLogger.Msg("Spawn Position: " + spawnPosition);

        GameObject level = new GameObject("Level");

        if(levelTitle != null && levelTitle != "")
        {
            GameObject titleObj = new GameObject("Level Title");
            titleObj.transform.parent = level.transform;

            CreateGameObjectProperty("Title", levelTitle, titleObj.transform);
        }

        GameObject cameraSettingsObj = new GameObject("Camera Settings");
        cameraSettingsObj.transform.parent = level.transform;

        CreateGameObjectProperty("X", "" + cameraPosition.x, cameraSettingsObj.transform);
        CreateGameObjectProperty("Y", "" + cameraPosition.y, cameraSettingsObj.transform);
        CreateGameObjectProperty("Z", "" + cameraPosition.z, cameraSettingsObj.transform);

        CreateGameObjectProperty("FoV", "" + cameraFoV, cameraSettingsObj.transform);

        GameObject spawnPositionObject = new GameObject("Spawn Position");
        spawnPositionObject.transform.parent = level.transform;
        spawnPositionObject.transform.position = spawnPosition;
        
        // List of sprites
        var spriteArray = levelObj["sprites"];
        if (spriteArray != null)
        {
            MelonLogger.Msg("Sprites");

            foreach (var sprite in spriteArray as ProxyArray)
            {
                string spriteFile = sprite["file"];
                Vector3 spritePosition = sprite["position"].Make<Vector3>();
                Vector2 spriteScale = sprite["scale"].Make<Vector2>();

                MelonLogger.Msg("Sprite File: " + spriteFile);
                MelonLogger.Msg("Sprite Position: " + spritePosition);
                MelonLogger.Msg("Sprite Scale: " + spriteScale);

                string fullSpriteFileName = "Mods/OriCanvasLevels/" + spriteFile;

                if (!File.Exists(fullSpriteFileName))
                    continue;

                CreateQuadFromSprite(fullSpriteFileName, level.transform, spritePosition, spriteScale);
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

                    if (colliderColor != null && colliderColor.a > 0f)
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

                    if (colliderColor != null && colliderColor.a > 0f)
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
        if (waterZoneArray != null)
        {
            MelonLogger.Msg("Water Zones");

            foreach (var waterZone in waterZoneArray as ProxyArray)
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

                bool shouldSpawnLoot = enemy["shouldSpawnLoot"];
                int numExpOrbs = enemy["expOrbs"];

                CreateGameObjectProperty("MaxHealth", enemy["maxHealth"], enemyObj.transform);
                CreateGameObjectProperty("MaxSensorRadius", enemy["maxSensorRadius"], enemyObj.transform);
                CreateGameObjectProperty("LoseSightRadius", enemy["loseSightRadius"], enemyObj.transform);
                CreateGameObjectProperty("ShouldSpawnLoot", "" + shouldSpawnLoot, enemyObj.transform);
                if (shouldSpawnLoot)
                {
                    CreateGameObjectProperty("EnergyOrbsNumber", enemy["energyOrbs"], enemyObj.transform);
                    CreateGameObjectProperty("HealthOrbsNumber", enemy["healthOrbs"], enemyObj.transform);
                }
                else
                {
                    CreateGameObjectProperty("EnergyOrbsNumber", "-1", enemyObj.transform);
                    CreateGameObjectProperty("HealthOrbsNumber", "-1", enemyObj.transform);
                }
                CreateGameObjectProperty("SpawnsExpOrbs", "" + (numExpOrbs != -1), enemyObj.transform);
                CreateGameObjectProperty("ExpOrbNumber", "" + numExpOrbs, enemyObj.transform);
                CreateGameObjectProperty("MinDistanceFromPlayer", enemy["minDistanceFromPlayer"], enemyObj.transform);
                CreateGameObjectProperty("RespawnOnScreen", enemy["respawnOnScreen"], enemyObj.transform);
                CreateGameObjectProperty("RespawnTime", enemy["respawnTime"], enemyObj.transform);

                switch (enemyType)
                {
                    case "Mantis": CreateGameObjectProperty("MantisType", "Base", enemyObj.transform); break;
                    case "GreenMantis": CreateGameObjectProperty("MantisType", "Green", enemyObj.transform); break;
                    case "ElectricMantis": CreateGameObjectProperty("MantisType", "Electric", enemyObj.transform); break;
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
                invisibleCheckpoint.RespawnPosition = new Vector2(checkpointBounds.x + checkpointBounds.width / 2f, checkpointBounds.y + 0.5f);
            }
        }

        return level;
    }

    public static void CreateGameObjectProperty(string name, string value, Transform parent)
    {
        GameObject objectName = new GameObject(name);
        objectName.transform.parent = parent.transform;

        GameObject valueName = new GameObject(value);
        valueName.transform.parent = objectName.transform;
    }

    public static MaterialTexture CreateTextureMaterialFromFile(string textureFile)
    {
        if (materialDict.ContainsKey(textureFile))
        {
            MelonLogger.Msg("Sprite " + textureFile + " created already. Reusing...");
            return materialDict[textureFile];
        }

        MelonLogger.Msg("Creating sprite " + textureFile + "...");

        byte[] spriteFileData = File.ReadAllBytes(textureFile);

        Texture2D texture = new Texture2D(2, 2);
        ImageConversion.LoadImage(texture, spriteFileData);

        Material material = new Material(Shader.Find("Sprites/Default"));
        material.mainTexture = texture;

        MaterialTexture materialTexture = new MaterialTexture();
        materialTexture.texture = texture;
        materialTexture.material = material;

        materialDict.Add(textureFile, materialTexture);

        return materialTexture;
    }

    public static GameObject CreateQuadFromSprite(string spriteFile, Transform parent, Vector3 position, Vector2 scale)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        plane.transform.parent = parent;

        Renderer renderer = plane.GetComponent<Renderer>();

        UnityEngine.Object.DestroyImmediate(plane.GetComponent<MeshCollider>());

        MaterialTexture materialTexture = CreateTextureMaterialFromFile(spriteFile);

        renderer.sharedMaterial = materialTexture.material;

        plane.transform.position = position;
        plane.transform.localScale = new Vector3(materialTexture.texture.width * scale.x / 100f, materialTexture.texture.height * scale.y / 100f, 1f);

        plane.name = "Sprite_" + materialTexture.texture.name;

        return plane;
    }
}
