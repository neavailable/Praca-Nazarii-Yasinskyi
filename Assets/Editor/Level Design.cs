using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using MainScene.Chaser;
using MainScene.Coin;
using MainScene.StartPoints;
using MainScene.UI.Buttons;
using MainScene.UI.Timer;
using Object = UnityEngine.Object;

namespace BallFinisher.Editor
{
    // Difficulty and look of each level: how many enemies and objects it gets, and its colours,
    // textures, light, sky and fog. Level 1 is calm and green, level 2 a busier desert, level 3 a crowded night.
    public static partial class FeatureSetup
    {
        private const string LevelMaterialsFolder = "Assets/Materials/Levels/";
        private const string LevelTexturesFolder  = "Assets/Images/Levels/";
        private const float GroundTiling = 100.0f;

        private static readonly Color TimeCoinColor = new (0.25f, 0.6f, 1.0f);
        private static readonly Color TimeTrapColor = new (0.95f, 0.2f, 0.25f);
        private static readonly Color ChaserColor   = new (0.4f, 0.08f, 0.35f);

        private enum GroundPattern { Grass, Sand, Stone }

        private class LevelTheme
        {
            public string Name;
            public GroundPattern Pattern;
            public Color Ground, Wall, WallEmission, Light, SkyTint, Ambient, Fog;
            public float LightIntensity, SkyExposure, FogStart, FogEnd;
            public bool UseFog;
        }

        private class LevelLayout
        {
            public string Title;
            public LevelTheme Theme;
            public int KeptEnemies, BoostPads, HealPickups, Bumpers, Spinners, MovingWalls, TimeCoins, TimeTraps, Chasers;
        }

        // Few, well spread elements: more of them and new kinds only as the levels get harder.
        private static readonly Dictionary<string, LevelLayout> LevelLayouts = new ()
        {
            ["Level 1"] = new LevelLayout
            {
                Title = "Level 1 - Easy",
                KeptEnemies = 18, BoostPads = 4, HealPickups = 3, Bumpers = 3,
                Spinners = 0, MovingWalls = 0, TimeCoins = 3, TimeTraps = 0, Chasers = 0,
                Theme = new LevelTheme
                {
                    Name = "Meadow", Pattern = GroundPattern.Grass,
                    Ground = new Color(0.33f, 0.61f, 0.13f), Wall = new Color(0.8f, 0.34f, 0.11f), WallEmission = Color.black,
                    Light = new Color(1.0f, 0.97f, 0.9f), LightIntensity = 1.0f,
                    SkyTint = new Color(0.5f, 0.6f, 0.8f), SkyExposure = 1.3f, Ambient = new Color(0.55f, 0.6f, 0.62f),
                    UseFog = false
                }
            },
            ["Level 2"] = new LevelLayout
            {
                Title = "Level 2 - Medium",
                KeptEnemies = 22, BoostPads = 5, HealPickups = 3, Bumpers = 5,
                Spinners = 2, MovingWalls = 2, TimeCoins = 4, TimeTraps = 2, Chasers = 0,
                Theme = new LevelTheme
                {
                    Name = "Desert", Pattern = GroundPattern.Sand,
                    Ground = new Color(0.86f, 0.72f, 0.47f), Wall = new Color(0.65f, 0.33f, 0.2f), WallEmission = Color.black,
                    Light = new Color(1.0f, 0.88f, 0.7f), LightIntensity = 1.25f,
                    SkyTint = new Color(0.85f, 0.65f, 0.45f), SkyExposure = 1.4f, Ambient = new Color(0.65f, 0.56f, 0.45f),
                    UseFog = true, Fog = new Color(0.93f, 0.82f, 0.62f), FogStart = 50.0f, FogEnd = 150.0f
                }
            },
            ["Level 3"] = new LevelLayout
            {
                Title = "Level 3 - Hard",
                KeptEnemies = 34, BoostPads = 6, HealPickups = 3, Bumpers = 6,
                Spinners = 3, MovingWalls = 3, TimeCoins = 4, TimeTraps = 3, Chasers = 5,
                Theme = new LevelTheme
                {
                    Name = "Night", Pattern = GroundPattern.Stone,
                    Ground = new Color(0.3f, 0.33f, 0.45f), Wall = new Color(0.36f, 0.22f, 0.62f), WallEmission = new Color(0.18f, 0.08f, 0.4f),
                    Light = new Color(0.6f, 0.7f, 1.0f), LightIntensity = 0.75f,
                    SkyTint = new Color(0.1f, 0.1f, 0.25f), SkyExposure = 0.5f, Ambient = new Color(0.3f, 0.3f, 0.45f),
                    UseFog = true, Fog = new Color(0.12f, 0.12f, 0.22f), FogStart = 40.0f, FogEnd = 130.0f
                }
            }
        };


        // ---------- Look ----------

        private static void ApplyTheme(LevelTheme theme)
        {
            EnsureFolder("Assets/Materials", "Levels");
            EnsureFolder("Assets/Images", "Levels");

            Texture2D groundTexture = CreatePatternTexture(theme.Name + " Ground", theme.Pattern);
            Material ground = CreateLevelMaterial(theme.Name + " Ground", theme.Ground, groundTexture, Color.black);
            Material wall   = CreateLevelMaterial(theme.Name + " Wall",   theme.Wall,   null,          theme.WallEmission);

            // Walls, spinner bars and moving walls share the wall material, so they all take the level's colour.
            foreach (Renderer renderer in FindInScene<Renderer>())
            {
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null) continue;
                    if (IsLevelMaterial(materials[i], "Ground")) { materials[i] = ground; changed = true; }
                    else if (IsLevelMaterial(materials[i], "Wall")) { materials[i] = wall; changed = true; }
                }
                if (changed) renderer.sharedMaterials = materials;
            }

            foreach (Light light in FindInScene<Light>().Where(light => light.type == LightType.Directional))
            {
                light.color     = theme.Light;
                light.intensity = theme.LightIntensity;
            }

            RenderSettings.skybox       = CreateSkyMaterial(theme);
            RenderSettings.ambientMode  = AmbientMode.Flat;
            RenderSettings.ambientLight = theme.Ambient;
            RenderSettings.fog          = theme.UseFog;
            RenderSettings.fogMode      = FogMode.Linear;
            RenderSettings.fogColor     = theme.Fog;
            RenderSettings.fogStartDistance = theme.FogStart;
            RenderSettings.fogEndDistance   = theme.FogEnd;
        }

        private static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + name)) AssetDatabase.CreateFolder(parent, name);
        }

        // The original shared materials and every level's own copies count, so the setup can run again.
        private static bool IsLevelMaterial(Material material, string kind)
        {
            return material.name == kind || LevelLayouts.Values.Any(layout => material.name == layout.Theme.Name + " " + kind);
        }

        private static Material CreateLevelMaterial(string name, Color color, Texture2D texture, Color emission)
        {
            string path = LevelMaterialsFolder + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.2f);
            if (texture != null)
            {
                material.SetTexture("_BaseMap", texture);
                material.SetTextureScale("_BaseMap", Vector2.one * GroundTiling);
            }
            if (emission != Color.black)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", emission);
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            EditorUtility.SetDirty(material);

            return material;
        }

        private static Material CreateSkyMaterial(LevelTheme theme)
        {
            string path = LevelMaterialsFolder + theme.Name + " Sky.mat";
            Material sky = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (sky == null)
            {
                sky = new Material(Shader.Find("Skybox/Procedural"));
                AssetDatabase.CreateAsset(sky, path);
            }

            sky.SetColor("_SkyTint", theme.SkyTint);
            sky.SetColor("_GroundColor", theme.Ground * 0.6f);
            sky.SetFloat("_Exposure", theme.SkyExposure);
            EditorUtility.SetDirty(sky);

            return sky;
        }

        // Seamless grey-scale patterns; the material's colour tints them.
        private static Texture2D CreatePatternTexture(string name, GroundPattern pattern)
        {
            string path = LevelTexturesFolder + name + ".png";
            if (!File.Exists(path))
            {
                const int size = 256;
                Texture2D texture = new (size, size, TextureFormat.RGB24, true);
                System.Random random = new (name.GetHashCode());
                float[,] lattice = new float[16, 16];
                for (int x = 0; x < 16; x++) for (int y = 0; y < 16; y++) lattice[x, y] = (float) random.NextDouble();

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        float u = (float) x / size, v = (float) y / size;
                        float value = PatternValue(pattern, u, v, lattice, random);
                        texture.SetPixel(x, y, new Color(value, value, value));
                    }
                }
                File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);

                TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(path);
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static float PatternValue(GroundPattern pattern, float u, float v, float[,] lattice, System.Random random)
        {
            float noise = 0.6f * ValueNoise(u * 4, v * 4, lattice) + 0.4f * ValueNoise(u * 16, v * 16, lattice);

            switch (pattern)
            {
                case GroundPattern.Grass:
                    float speck = random.NextDouble() < 0.04 ? -0.12f : 0.0f;
                    return Mathf.Clamp01(0.8f + 0.2f * noise + speck);

                case GroundPattern.Sand:
                    float ripple = 0.5f + 0.5f * Mathf.Sin((v * 12.0f + noise * 1.5f) * 2.0f * Mathf.PI);
                    return Mathf.Clamp01(0.82f + 0.1f * ripple + 0.08f * noise);

                default:
                    float tileU = u * 4 % 1.0f, tileV = v * 4 % 1.0f;
                    bool grout = tileU < 0.04f || tileV < 0.04f;
                    float tile = lattice[(int) (u * 4) * 3 % 16, (int) (v * 4) * 5 % 16];
                    return grout ? 0.45f : Mathf.Clamp01(0.75f + 0.15f * tile + 0.1f * noise);
            }
        }

        // Value noise on a 16x16 lattice that wraps around, so the texture tiles without seams.
        private static float ValueNoise(float x, float y, float[,] lattice)
        {
            int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y);
            float fx = Mathf.SmoothStep(0, 1, x - x0), fy = Mathf.SmoothStep(0, 1, y - y0);

            float Corner(int cx, int cy) => lattice[((cx % 16) + 16) % 16, ((cy % 16) + 16) % 16];

            float bottom = Mathf.Lerp(Corner(x0, y0),     Corner(x0 + 1, y0),     fx);
            float top    = Mathf.Lerp(Corner(x0, y0 + 1), Corner(x0 + 1, y0 + 1), fx);
            return Mathf.Lerp(bottom, top, fy);
        }


        // ---------- Enemies ----------

        // Keeps the given number of enemies, spread as evenly as possible and farthest from the ball's start first,
        // so every level opens calmly. A level that already has few enough enemies is left as it is.
        private static void ThinEnemies(string levelName, int keep)
        {
            List<EnemyStartPoint> left = FindInScene<EnemyStartPoint>().ToList();
            if (left.Count <= keep) return;

            int before = left.Count;
            List<Vector3> kept = new () { GetBallStart() };

            for (int i = 0; i < keep; i++)
            {
                EnemyStartPoint farthest = left
                    .OrderByDescending(enemy => kept.Min(point => Flat(enemy.transform.position - point).sqrMagnitude))
                    .First();
                kept.Add(farthest.transform.position);
                left.Remove(farthest);
            }
            foreach (EnemyStartPoint enemy in left) Object.DestroyImmediate(enemy.gameObject);

            Debug.Log($"[Feature Setup] {levelName}: enemies {before} -> {keep}");
        }


        // ---------- New prefabs ----------

        private static GameObject CreateTimePickupPrefab(bool isPenalty)
        {
            string name = isPenalty ? "Time Trap" : "Time Coin";
            Color color = isPenalty ? TimeTrapColor : TimeCoinColor;
            Material face = CreateMaterial("Assets/Materials/Objects/" + name + ".mat", color);
            Material hand = CreateMaterial("Assets/Materials/Objects/Clock Hand.mat", Color.white);

            GameObject pickup = new (name);
            SphereCollider trigger = pickup.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius    = 0.7f;
            pickup.AddComponent<CoinRotation>();

            SerializedObject timePickup = new (pickup.AddComponent<TimePickup>());
            timePickup.FindProperty("_isPenalty").boolValue = isPenalty;
            timePickup.ApplyModifiedPropertiesWithoutUndo();

            // A clock face standing upright, like a coin.
            GameObject disc = CreatePrimitive(PrimitiveType.Cylinder, "Face", pickup.transform, new Vector3(0.9f, 0.06f, 0.9f), face);
            disc.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            GameObject minuteHand = CreatePrimitive(PrimitiveType.Cube, "Minute Hand", pickup.transform, new Vector3(0.07f, 0.34f, 0.15f), hand);
            minuteHand.transform.localPosition = new Vector3(0.0f, 0.13f, 0.0f);
            GameObject hourHand = CreatePrimitive(PrimitiveType.Cube, "Hour Hand", pickup.transform, new Vector3(0.24f, 0.07f, 0.15f), hand);
            hourHand.transform.localPosition = new Vector3(0.1f, 0.0f, 0.0f);
            AddMinimapIcon(pickup, color, 1.3f);

            return SavePrefab(pickup, ObjectsFolder + name + ".prefab");
        }

        private static GameObject CreateChaserPrefab()
        {
            Material body  = CreateMaterial("Assets/Materials/Objects/Chaser.mat", ChaserColor);
            Material spike = CreateMaterial("Assets/Materials/Objects/Chaser Spike.mat", ChaserColor * 0.5f);
            Material eye   = CreateMaterial("Assets/Materials/Objects/Clock Hand.mat", Color.white);

            GameObject chaser = new ("Chaser");
            AddKinematicBody(chaser);
            SphereCollider collider = chaser.AddComponent<SphereCollider>();
            collider.radius = 0.6f;
            collider.center = new Vector3(0.0f, 0.6f, 0.0f);
            chaser.AddComponent<Chaser>();

            GameObject sphere = CreatePrimitive(PrimitiveType.Sphere, "Body", chaser.transform, Vector3.one * 1.2f, body);
            sphere.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f);

            Vector3[] spikes = { Vector3.left, Vector3.right, Vector3.back, Vector3.up };
            foreach (Vector3 direction in spikes)
            {
                GameObject spikeCube = CreatePrimitive(PrimitiveType.Cube, "Spike", chaser.transform, Vector3.one * 0.32f, spike);
                spikeCube.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f) + direction * 0.62f;
                spikeCube.transform.localRotation = Quaternion.Euler(45.0f, 45.0f, 0.0f);
            }
            foreach (float side in new [] { -0.22f, 0.22f })
            {
                GameObject eyeSphere = CreatePrimitive(PrimitiveType.Sphere, "Eye", chaser.transform, Vector3.one * 0.2f, eye);
                eyeSphere.transform.localPosition = new Vector3(side, 0.82f, 0.5f);
            }
            AddMinimapIcon(chaser, EnemyIconColor, 1.6f);

            return SavePrefab(chaser, ObjectsFolder + "Chaser.prefab");
        }

        // Below the coin panel: how many coins are left, then that the finish is open.
        private static GameObject CreateObjectivePrefab(TMP_FontAsset font)
        {
            RectTransform objective = CreateRect
            (
                "Objective", null,
                new Vector2(0.0f, 1.0f), new Vector2(0.0f, 1.0f),
                new Vector2(10.0f, -64.0f), new Vector2(210.0f, 30.0f)
            );
            AddBackground(objective.gameObject, HudBackground);

            RectTransform textRect = CreateRect("Text", objective, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            Stretch(textRect, 4.0f);
            textRect.offsetMin = new Vector2(10.0f, 2.0f);
            TMP_Text text = AddText(textRect, "Coins left: 0", font, 18.0f, TextAlignmentOptions.MidlineLeft);

            SerializedObject objectiveText = new (objective.gameObject.AddComponent<ObjectiveText>());
            objectiveText.FindProperty("_text").objectReferenceValue = text;
            objectiveText.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab(objective.gameObject, UIFolder + "Objective.prefab");
        }

        // The rules screen shown before the level starts.
        private static GameObject CreateLevelIntroPrefab(TMP_FontAsset font)
        {
            RectTransform intro = CreateRect("Level Intro", null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            Stretch(intro, 0.0f);
            intro.gameObject.AddComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.65f);

            // Drawn above the minimap, timer and the rest of the HUD.
            // The prefab root is not nested yet, so Canvas ignores overrideSorting set as a property.
            SerializedObject introCanvas = new (intro.gameObject.AddComponent<Canvas>());
            introCanvas.FindProperty("m_OverrideSorting").boolValue = true;
            introCanvas.FindProperty("m_SortingOrder").intValue     = 100;
            introCanvas.ApplyModifiedPropertiesWithoutUndo();
            intro.gameObject.AddComponent<GraphicRaycaster>();

            RectTransform panel = CreateRect
            (
                "Panel", intro,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(760.0f, 490.0f)
            );
            AddBackground(panel.gameObject, new Color(0.12f, 0.14f, 0.18f, 1.0f)).raycastTarget = true;

            RectTransform titleRect = CreateRect
            (
                "Title", panel,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -10.0f), new Vector2(712.0f, 42.0f)
            );
            TMP_Text title = AddText(titleRect, "Level", font, 30.0f, TextAlignmentOptions.Center);

            RectTransform rulesRect = CreateRect
            (
                "Rules", panel,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -54.0f), new Vector2(712.0f, 48.0f)
            );
            TMP_Text rules = AddText(rulesRect, "Rules", font, 16.0f, TextAlignmentOptions.TopLeft);
            rules.enableWordWrapping = true;

            // The level's elements with their icons; rows of elements a level lacks are hidden in that level.
            AddIntroElements(panel, font);

            RectTransform levelRect = CreateRect
            (
                "Level Rules", panel,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -372.0f), new Vector2(712.0f, 44.0f)
            );
            TMP_Text levelRules = AddText(levelRect, "Level", font, 16.0f, TextAlignmentOptions.TopLeft);
            levelRules.enableWordWrapping = true;
            levelRules.color = new Color(1.0f, 0.85f, 0.4f);

            GameObject start = CreateStyledButton("Start Button", "Start", panel);
            RectTransform startRect = (RectTransform) start.transform;
            startRect.anchorMin        = new Vector2(0.5f, 0.0f);
            startRect.anchorMax        = new Vector2(0.5f, 0.0f);
            startRect.pivot            = new Vector2(0.5f, 0.0f);
            startRect.anchoredPosition = new Vector2(0.0f, 14.0f);
            startRect.sizeDelta        = new Vector2(180.0f, 50.0f);

            SerializedObject levelIntro = new (intro.gameObject.AddComponent<LevelIntro>());
            levelIntro.FindProperty("_title")      .objectReferenceValue = title;
            levelIntro.FindProperty("_text")       .objectReferenceValue = rules;
            levelIntro.FindProperty("_levelText")  .objectReferenceValue = levelRules;
            levelIntro.FindProperty("_startButton").objectReferenceValue = start.GetComponent<Button>();
            levelIntro.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab(intro.gameObject, UIFolder + "Level Intro.prefab");
        }

        // A copy of the finish window's Retry button, so new buttons match the game's style.
        private static GameObject CreateStyledButton(string name, string label, Transform parent)
        {
            GameObject finishWindow = AssetDatabase.LoadAssetAtPath<GameObject>(FinishWindowPath);
            Button retryButton = finishWindow.GetComponentsInChildren<Button>(true).First(button => button.name == "Retry Button");

            GameObject copy = Object.Instantiate(retryButton.gameObject, parent, false);
            copy.name = name;
            foreach (Retry retry in copy.GetComponentsInChildren<Retry>(true)) Object.DestroyImmediate(retry);
            copy.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

            TMP_Text text = copy.GetComponentInChildren<TMP_Text>(true);
            Stretch(text.rectTransform, 0.0f);
            text.text               = label;
            text.enableWordWrapping = false;
            text.fontSizeMax        = text.fontSize;
            text.fontSizeMin        = 14.0f;
            text.enableAutoSizing   = true;

            return copy;
        }
    }
}
