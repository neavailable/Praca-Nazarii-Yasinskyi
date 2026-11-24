using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Infrastructure.MainScene.Installers;
using MainMenu;
using MainMenu.Statistics;
using MainScene.BoostPad;
using MainScene.Coin;
using MainScene.Finish.FinishTrigger;
using MainScene.Ground;
using MainScene.Heal;
using MainScene.Obstacles;
using MainScene.StartPoints;
using MainScene.UI.HealthBar;
using MainScene.UI.Buttons;
using MainScene.UI.Minimap;
using MainScene.UI.Push;
using MainScene.UI.Timer;
using MainScene.Wind;
using Object = UnityEngine.Object;

namespace BallFinisher.Editor
{
    // Builds the prefabs for the minimap, level timer, wind, boost pad and heal pickup features,
    // adds them to the level scenes and adds the control settings window to the main menu.
    // Running it again rebuilds the prefabs and HUD but keeps boost pads and pickups already placed in levels.
    public static partial class FeatureSetup
    {
        private const string ObjectsFolder = "Assets/Prefabs/Main Scene/Objects/";
        private const string UIFolder = "Assets/Prefabs/Main Scene/UI/";
        private const string BoostPadPath = ObjectsFolder + "Boost Pad.prefab";
        private const string HealPickupPath = ObjectsFolder + "Heal Pickup.prefab";
        private const string BumperPath = ObjectsFolder + "Bumper.prefab";
        private const string SpinnerPath = ObjectsFolder + "Spinner.prefab";
        private const string MovingWallPath = ObjectsFolder + "Moving Wall.prefab";
        private const string LevelTimerPath = UIFolder + "Level Timer.prefab";
        private const string WindIndicatorPath = UIFolder + "Wind Indicator.prefab";
        private const string MinimapPath = UIFolder + "Minimap.prefab";
        private const string PushButtonPath = UIFolder + "Push Button.prefab";
        private const string FinishWindowPath = UIFolder + "Finish Window.prefab";
        private const string ArrowPath = "Assets/Images/UI/Arrow.png";
        private const string BoostPadMaterialPath = "Assets/Materials/Objects/Boost Pad.mat";
        private const string HealMaterialPath = "Assets/Materials/Objects/Heal Pickup.mat";
        private const string BumperMaterialPath = "Assets/Materials/Objects/Bumper.mat";
        private const string BumperCapMaterialPath = "Assets/Materials/Objects/Bumper Cap.mat";
        private const string SpinnerPostMaterialPath = "Assets/Materials/Objects/Spinner Post.mat";
        private const string MainMenuScene = "Assets/Scenes/Main Menu.unity";
        private const string LevelFeaturesName = "Level Features";
        private const string MinimapIconName = "Minimap Icon";
        private const string SettingsButtonName = "Settings Button";
        private const string SettingsWindowName = "Settings Window";
        private const string StatisticsButtonName = "Statistics Button";
        private const string StatisticsWindowName = "Statistics Window";
        private const float MinimapIconHeight = 6.0f;
        private const float BallRadius = 0.6f;
        private const float SpinnerRadius = 2.7f;
        private const float MovingWallLength = 4.0f;
        // Half the wall plus how far it slides to each side (Moving Wall Distance in the obstacles configuration).
        private const float MovingWallReach = MovingWallLength / 2.0f + 2.5f;

        private static readonly string[] Levels =
        {
            "Assets/Scenes/Level 1.unity",
            "Assets/Scenes/Level 2.unity",
            "Assets/Scenes/Level 3.unity"
        };

        // The left menu column, top to bottom, evenly spaced.
        private static readonly string[] MenuColumn =
        {
            "Level 1", "Level 2", "Level 3", StatisticsButtonName, "Quit"
        };
        private const float MenuColumnTop = 200.0f, MenuColumnSpacing = 100.0f;
        private static readonly Vector2 MenuWideButtonSize = new (200.0f, 60.0f);

        private static readonly Color BallIconColor   = new (0.3f, 0.75f, 1.0f);
        private static readonly Color EnemyIconColor  = new (0.95f, 0.2f, 0.2f);
        private static readonly Color FinishIconColor = new (1.0f, 0.85f, 0.2f);
        private static readonly Color HealColor       = new (0.25f, 0.85f, 0.35f);
        private static readonly Color BoostPadColor   = new (1.0f, 0.55f, 0.1f);
        private static readonly Color BumperColor     = new (0.8f, 0.2f, 0.7f);
        private static readonly Color BumperCapColor  = new (1.0f, 0.85f, 0.95f);
        private static readonly Color SpinnerPostColor = new (0.25f, 0.25f, 0.3f);
        private static readonly Color HudBackground   = new (0.0f, 0.0f, 0.0f, 0.45f);

        private static int _minimapLayer;
        private static Sprite _arrowSprite, _dotSprite, _panelSprite;
        private static Material _spriteMaterial;


        [MenuItem("Ball Finisher/Install New Features")]
        public static void Run() => RunSetup(false);

        [MenuItem("Ball Finisher/Install New Features and Re-place Pads and Pickups")]
        public static void RunAndReplaceLevelFeatures() => RunSetup(true);


        private static void RunSetup(bool replaceLevelFeatures)
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            _minimapLayer   = EnsureLayer(MinimapView.LayerName);
            _arrowSprite    = CreateArrowSprite();
            _dotSprite      = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            _panelSprite    = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            _spriteMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");

            EditObjectPrefabs();
            Material wallMaterial = AssetDatabase.LoadAssetAtPath<GameObject>(ObjectsFolder + "Wall.prefab")
                .GetComponentInChildren<MeshRenderer>().sharedMaterial;
            LevelPrefabs levelPrefabs = new ()
            {
                BoostPad   = CreateBoostPadPrefab(),
                HealPickup = CreateHealPickupPrefab(),
                Bumper     = CreateBumperPrefab(),
                Spinner    = CreateSpinnerPrefab(wallMaterial),
                MovingWall = CreateMovingWallPrefab(wallMaterial),
                TimeCoin   = CreateTimePickupPrefab(false),
                TimeTrap   = CreateTimePickupPrefab(true),
                Chaser     = CreateChaserPrefab()
            };
            EditFinishWindowPrefab();

            // The HUD uses the same font as the health bar in the levels.
            EditorSceneManager.OpenScene(Levels[0], OpenSceneMode.Single);
            TMP_FontAsset hudFont = GetHealthBarFont();
            GameObject levelTimer    = CreateLevelTimerPrefab(hudFont);
            GameObject windIndicator = CreateWindIndicatorPrefab();
            GameObject minimap       = CreateMinimapPrefab();
            GameObject pushButton    = CreatePushButtonPrefab();
            GameObject objective     = CreateObjectivePrefab(hudFont);
            GameObject feedback      = CreateFeedbackPrefab();
            GameObject intro         = CreateLevelIntroPrefab(hudFont);

            foreach (string path in Levels)
            {
                SetUpLevel
                (
                    path,
                    new [] { levelTimer, windIndicator, minimap, pushButton, objective, feedback, intro },
                    levelPrefabs,
                    replaceLevelFeatures
                );
            }
            SetUpMainMenu();

            AssetDatabase.SaveAssets();
            Debug.Log("[Feature Setup] Finished");
        }


        private static int EnsureLayer(string layerName)
        {
            int existing = LayerMask.NameToLayer(layerName);
            if (existing >= 0) return existing;

            SerializedObject tagManager = new (AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 6; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);
                if (!string.IsNullOrEmpty(layer.stringValue)) continue;

                layer.stringValue = layerName;
                tagManager.ApplyModifiedPropertiesWithoutUndo();
                return i;
            }
            throw new InvalidOperationException("No free layer for " + layerName);
        }

        private static Sprite CreateArrowSprite()
        {
            if (!File.Exists(ArrowPath))
            {
                const int size = 64;
                Texture2D texture = new (size, size, TextureFormat.RGBA32, false);

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float fromAxis = Mathf.Abs(x + 0.5f - size / 2.0f);
                        bool stem = fromAxis < 7.0f && y >= 4 && y < 38;
                        bool head = y >= 34 && y < 61 && fromAxis <= (60 - y) * 1.05f;

                        texture.SetPixel(x, y, stem || head ? Color.white : Color.clear);
                    }
                }
                File.WriteAllBytes(ArrowPath, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(ArrowPath);

                TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(ArrowPath);
                importer.textureType         = TextureImporterType.Sprite;
                importer.spriteImportMode    = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled       = false;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(ArrowPath);
        }

        private static Material CreateMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;

            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);

            return material;
        }


        // ---------- Object prefabs ----------

        private static void EditObjectPrefabs()
        {
            EditPrefab(ObjectsFolder + "Ball.prefab", ball =>
            {
                // Tilt control was removed, its component is left on the ball as a missing script.
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(ball);
                if (!ball.GetComponent<BallWind>()) ball.AddComponent<BallWind>();
                AddMinimapIcon(ball, BallIconColor, 1.8f);
            });
            EditPrefab(ObjectsFolder + "Cactus.prefab",   cactus   => AddMinimapIcon(cactus,   EnemyIconColor, 1.5f));
            EditPrefab(ObjectsFolder + "Mushroom.prefab", mushroom => AddMinimapIcon(mushroom, EnemyIconColor, 1.5f));
            EditPrefab(ObjectsFolder + "Finish .prefab",  finish   => AddMinimapIcon(finish,   FinishIconColor, 2.0f));
        }

        private static GameObject CreateBoostPadPrefab()
        {
            Material material = CreateMaterial(BoostPadMaterialPath, BoostPadColor);
            GameObject pad = new ("Boost Pad");

            BoxCollider trigger = pad.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center    = new Vector3(0.0f, 0.6f, 0.0f);
            trigger.size      = new Vector3(2.5f, 1.2f, 4.0f);
            pad.AddComponent<BoostPadTrigger>();

            GameObject plate = CreatePrimitive(PrimitiveType.Cube, "Plate", pad.transform, new Vector3(2.5f, 0.04f, 4.0f), material);
            plate.transform.localPosition = new Vector3(0.0f, 0.02f, 0.0f);

            for (int i = 0; i < 3; i++)
            {
                GameObject arrow = new ("Arrow " + (i + 1));
                arrow.transform.SetParent(pad.transform, false);
                arrow.transform.localPosition = new Vector3(0.0f, 0.05f, -1.2f + 1.2f * i);
                arrow.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                arrow.transform.localScale    = Vector3.one * 2.2f;

                SpriteRenderer spriteRenderer = arrow.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite         = _arrowSprite;
                spriteRenderer.sharedMaterial = _spriteMaterial;
            }

            return SavePrefab(pad, BoostPadPath);
        }

        private static GameObject CreateHealPickupPrefab()
        {
            Material material = CreateMaterial(HealMaterialPath, HealColor);
            GameObject pickup = new ("Heal Pickup");

            SphereCollider trigger = pickup.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius    = 0.8f;
            pickup.AddComponent<HealPickup>();
            pickup.AddComponent<CoinRotation>();

            CreatePrimitive(PrimitiveType.Cube, "Vertical",   pickup.transform, new Vector3(0.3f, 0.9f, 0.3f), material);
            CreatePrimitive(PrimitiveType.Cube, "Horizontal", pickup.transform, new Vector3(0.9f, 0.3f, 0.3f), material);
            AddMinimapIcon(pickup, HealColor, 1.3f);

            return SavePrefab(pickup, HealPickupPath);
        }

        // A round bumper; the collider is a tall capsule, so its side is straight at the ball's height.
        private static GameObject CreateBumperPrefab()
        {
            GameObject bumper = new ("Bumper");

            CapsuleCollider collider = bumper.AddComponent<CapsuleCollider>();
            collider.radius = 0.8f;
            collider.height = 3.0f;
            collider.center = new Vector3(0.0f, 0.8f, 0.0f);

            Transform body = new GameObject("Body").transform;
            body.SetParent(bumper.transform, false);

            GameObject ring = CreatePrimitive(PrimitiveType.Cylinder, "Ring", body, new Vector3(1.6f, 0.35f, 1.6f), CreateMaterial(BumperMaterialPath, BumperColor));
            ring.transform.localPosition = new Vector3(0.0f, 0.35f, 0.0f);
            GameObject cap = CreatePrimitive(PrimitiveType.Cylinder, "Cap", body, new Vector3(1.1f, 0.06f, 1.1f), CreateMaterial(BumperCapMaterialPath, BumperCapColor));
            cap.transform.localPosition = new Vector3(0.0f, 0.74f, 0.0f);

            SerializedObject bumperComponent = new (bumper.AddComponent<Bumper>());
            bumperComponent.FindProperty("_body").objectReferenceValue = body;
            bumperComponent.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab(bumper, BumperPath);
        }

        // A wall-coloured bar turning around a dark post.
        private static GameObject CreateSpinnerPrefab(Material wallMaterial)
        {
            GameObject spinner = new ("Spinner");
            AddKinematicBody(spinner);
            spinner.AddComponent<Spinner>();
            spinner.AddComponent<ObstacleKnocker>();

            GameObject post = CreatePrimitive(PrimitiveType.Cylinder, "Post", spinner.transform, new Vector3(0.6f, 0.75f, 0.6f), CreateMaterial(SpinnerPostMaterialPath, SpinnerPostColor));
            post.transform.localPosition = new Vector3(0.0f, 0.75f, 0.0f);
            GameObject bar = CreatePrimitive(PrimitiveType.Cube, "Bar", spinner.transform, new Vector3(SpinnerRadius * 2.0f - 0.2f, 0.9f, 0.4f), wallMaterial, true);
            bar.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f);

            return SavePrefab(spinner, SpinnerPath);
        }

        // A short wall that slides along its own length (local X).
        private static GameObject CreateMovingWallPrefab(Material wallMaterial)
        {
            GameObject wall = new ("Moving Wall");
            AddKinematicBody(wall);
            wall.AddComponent<MovingWall>();
            wall.AddComponent<ObstacleKnocker>();

            GameObject body = CreatePrimitive(PrimitiveType.Cube, "Body", wall.transform, new Vector3(MovingWallLength, 1.4f, 0.5f), wallMaterial, true);
            body.transform.localPosition = new Vector3(0.0f, 0.7f, 0.0f);

            return SavePrefab(wall, MovingWallPath);
        }

        private static void AddKinematicBody(GameObject gameObject)
        {
            Rigidbody body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic   = true;
            body.useGravity    = false;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private static GameObject CreatePrimitive
        (
            PrimitiveType type,
            string name,
            Transform parent,
            Vector3 scale,
            Material material,
            bool keepCollider = false
        )
        {
            GameObject primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localScale = scale;

            if (!keepCollider) Object.DestroyImmediate(primitive.GetComponent<Collider>());
            primitive.GetComponent<MeshRenderer>().sharedMaterial = material;

            return primitive;
        }

        private static void AddMinimapIcon(GameObject owner, Color color, float size)
        {
            Transform existing = owner.transform.Find(MinimapIconName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            float ownerScale = Mathf.Max(owner.transform.lossyScale.x, 0.01f);
            GameObject icon = new (MinimapIconName) { layer = _minimapLayer };

            icon.transform.SetParent(owner.transform, false);
            icon.transform.localPosition = Vector3.up * (MinimapIconHeight / ownerScale);
            icon.transform.rotation      = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            icon.transform.localScale    = Vector3.one * (size / _dotSprite.bounds.size.x / ownerScale);

            SpriteRenderer spriteRenderer = icon.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite         = _dotSprite;
            spriteRenderer.color          = color;
            spriteRenderer.sharedMaterial = _spriteMaterial;

            SerializedObject minimapIcon = new (icon.AddComponent<MinimapIcon>());
            minimapIcon.FindProperty("_size")  .floatValue = size;
            minimapIcon.FindProperty("_height").floatValue = MinimapIconHeight;
            minimapIcon.ApplyModifiedPropertiesWithoutUndo();
        }


        // ---------- UI prefabs ----------

        private static void EditFinishWindowPrefab()
        {
            EditPrefab(FinishWindowPath, window =>
            {
                Transform panel = window.transform.Find("Panel");

                Transform oldTimePanel = panel.Find("Time Panel");
                if (oldTimePanel != null) Object.DestroyImmediate(oldTimePanel.gameObject);
                FinishTimeText oldTimeText = panel.GetComponent<FinishTimeText>();
                if (oldTimeText != null) Object.DestroyImmediate(oldTimeText);

                TMP_FontAsset font = window.GetComponentInChildren<TMP_Text>(true).font;

                // Below the panel, so the existing window layout stays untouched.
                RectTransform timePanel = CreateRect
                (
                    "Time Panel", panel,
                    new Vector2(0.5f, 0.0f), new Vector2(0.5f, 1.0f),
                    new Vector2(0.0f, -10.0f), new Vector2(320.0f, 70.0f)
                );
                AddBackground(timePanel.gameObject, new Color(0.0f, 0.0f, 0.0f, 0.55f));

                RectTransform textRect = CreateRect("Time Text", timePanel, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
                Stretch(textRect, 6.0f);
                TMP_Text text = AddText(textRect, "Time: 00:00.00\nBest: 00:00.00", font, 24.0f, TextAlignmentOptions.Center);

                SerializedObject finishTimeText = new (panel.gameObject.AddComponent<FinishTimeText>());
                finishTimeText.FindProperty("_panel").objectReferenceValue = timePanel.gameObject;
                finishTimeText.FindProperty("_text") .objectReferenceValue = text;
                finishTimeText.ApplyModifiedPropertiesWithoutUndo();
            });
        }

        private static TMP_FontAsset GetHealthBarFont()
        {
            HealthBar healthBar = FindInScene<HealthBar>().First();
            Object healthText = new SerializedObject(healthBar).FindProperty("_healthText").objectReferenceValue;

            return healthText is TMP_Text text && text.font != null ? text.font : TMP_Settings.defaultFontAsset;
        }

        private static GameObject CreateLevelTimerPrefab(TMP_FontAsset font)
        {
            RectTransform timer = CreateRect
            (
                "Level Timer", null,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -10.0f), new Vector2(150.0f, 40.0f)
            );
            AddBackground(timer.gameObject, HudBackground);

            RectTransform textRect = CreateRect("Time Text", timer, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            Stretch(textRect, 4.0f);
            TMP_Text text = AddText(textRect, "00:00.00", font, 28.0f, TextAlignmentOptions.Center);

            SerializedObject levelTimer = new (timer.gameObject.AddComponent<LevelTimer>());
            levelTimer.FindProperty("_timeText").objectReferenceValue = text;
            levelTimer.ApplyModifiedPropertiesWithoutUndo();
            timer.gameObject.AddComponent<LevelStatisticsRecorder>();

            return SavePrefab(timer.gameObject, LevelTimerPath);
        }

        private static GameObject CreateWindIndicatorPrefab()
        {
            RectTransform indicator = CreateRect
            (
                "Wind Indicator", null,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -56.0f), new Vector2(44.0f, 44.0f)
            );
            AddBackground(indicator.gameObject, HudBackground);

            RectTransform arrow = CreateRect
            (
                "Arrow", indicator,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(34.0f, 34.0f)
            );
            Image arrowImage = arrow.gameObject.AddComponent<Image>();
            arrowImage.sprite         = _arrowSprite;
            arrowImage.preserveAspect = true;
            arrowImage.raycastTarget  = false;

            SerializedObject windIndicator = new (indicator.gameObject.AddComponent<WindIndicator>());
            windIndicator.FindProperty("_arrow").objectReferenceValue = arrow;
            windIndicator.ApplyModifiedPropertiesWithoutUndo();

            return SavePrefab(indicator.gameObject, WindIndicatorPath);
        }

        // Styled like the finish window's buttons, at the bottom middle between the health bar and the info panel.
        private static GameObject CreatePushButtonPrefab()
        {
            GameObject finishWindow = AssetDatabase.LoadAssetAtPath<GameObject>(FinishWindowPath);
            Button retryButton = finishWindow.GetComponentsInChildren<Button>(true).First(button => button.name == "Retry Button");

            GameObject push = Object.Instantiate(retryButton.gameObject);
            push.name = "Push Button";
            foreach (Retry retry in push.GetComponentsInChildren<Retry>(true)) Object.DestroyImmediate(retry);
            push.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

            RectTransform rect = (RectTransform) push.transform;
            rect.anchorMin        = new Vector2(0.5f, 0.0f);
            rect.anchorMax        = new Vector2(0.5f, 0.0f);
            rect.pivot            = new Vector2(0.5f, 0.0f);
            rect.anchoredPosition = new Vector2(0.0f, 14.0f);
            rect.sizeDelta        = new Vector2(150.0f, 50.0f);

            TMP_Text text = push.GetComponentInChildren<TMP_Text>(true);
            Stretch(text.rectTransform, 0.0f);
            text.text               = "Push";
            text.enableWordWrapping = false;
            text.fontSizeMax        = text.fontSize;
            text.fontSizeMin        = 14.0f;
            text.enableAutoSizing   = true;

            push.AddComponent<PushButton>();

            return SavePrefab(push, PushButtonPath);
        }

        private static GameObject CreateMinimapPrefab()
        {
            RectTransform minimap = CreateRect
            (
                "Minimap", null,
                Vector2.one, Vector2.one,
                new Vector2(-10.0f, -10.0f), new Vector2(150.0f, 150.0f)
            );
            AddBackground(minimap.gameObject, new Color(0.05f, 0.06f, 0.08f, 0.85f));

            RectTransform map = CreateRect("Map", minimap, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            Stretch(map, 4.0f);
            map.gameObject.AddComponent<RawImage>().raycastTarget = false;
            map.gameObject.AddComponent<MinimapView>();

            return SavePrefab(minimap.gameObject, MinimapPath);
        }

        private static RectTransform CreateRect
        (
            string name,
            Transform parent,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 position,
            Vector2 size
        )
        {
            RectTransform rect = (RectTransform) new GameObject(name, typeof(RectTransform)).transform;

            rect.SetParent(parent, false);
            rect.anchorMin        = anchor;
            rect.anchorMax        = anchor;
            rect.pivot            = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta        = size;

            return rect;
        }

        private static void Stretch(RectTransform rect, float padding)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }

        private static Image AddBackground(GameObject gameObject, Color color)
        {
            Image image = gameObject.AddComponent<Image>();
            image.sprite        = _panelSprite;
            image.type          = Image.Type.Sliced;
            image.color         = color;
            image.raycastTarget = false;

            return image;
        }

        private static TMP_Text AddText
        (
            RectTransform rect,
            string value,
            TMP_FontAsset font,
            float fontSize,
            TextAlignmentOptions alignment
        )
        {
            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            if (font != null) text.font = font;

            text.text          = value;
            text.fontSize      = fontSize;
            text.alignment     = alignment;
            text.color         = Color.white;
            text.raycastTarget = false;

            return text;
        }


        // ---------- Level scenes ----------

        private static void SetUpLevel
        (
            string scenePath,
            IEnumerable<GameObject> hudPrefabs,
            LevelPrefabs levelPrefabs,
            bool replaceLevelFeatures
        )
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Transform canvas = FindInScene<HealthBar>().First().GetComponentInParent<Canvas>().rootCanvas.transform;

            LevelLayout layout = LevelLayouts[scene.name];

            foreach (GameObject hudPrefab in hudPrefabs) ReplaceHudElement(canvas, hudPrefab);
            ApplyTheme(layout.Theme);
            ThinEnemies(scene.name, layout.KeptEnemies);
            PlaceLevelFeatures(scene, levelPrefabs, layout, replaceLevelFeatures);
            ShowLevelIntroElements(scene);
            ExportLevelPlan(scene.name, layout);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ReplaceHudElement(Transform canvas, GameObject prefab)
        {
            foreach (Transform child in canvas.Cast<Transform>().ToList())
            {
                if (child.name == prefab.name) Object.DestroyImmediate(child.gameObject);
            }
            PrefabUtility.InstantiatePrefab(prefab, canvas);
        }

        // Spreads obstacles, pickups and chasers over the part of the level the ball can reach from its start,
        // so nothing lands behind a wall. Every object keeps a ball-wide gap to walls and to the others,
        // so it can always be rolled around. How many of each comes from the level's difficulty.
        // Without replacing, a later run keeps whatever was placed or moved by hand.
        private static void PlaceLevelFeatures(Scene scene, LevelPrefabs prefabs, LevelLayout layout, bool replace)
        {
            GameObject existing = scene.GetRootGameObjects().FirstOrDefault(root => root.name == LevelFeaturesName);
            if (existing != null)
            {
                if (!replace)
                {
                    Debug.Log($"[Feature Setup] {scene.name}: '{LevelFeaturesName}' already exists, placement kept");
                    return;
                }
                Object.DestroyImmediate(existing);
            }
            Physics.SyncTransforms();

            Vector3 ballStart     = GetBallStart();
            float ballHeight      = ballStart.y;
            List<Vector3> coins   = FindInScene<CoinsStartPointMarker>().Select(coin => coin.transform.position).ToList();
            List<Vector3> enemies = FindInScene<EnemyStartPoint>().Select(enemy => enemy.transform.position).ToList();
            Vector3 finish        = FindInScene<FinishTrigger>().First().transform.position;

            PlayArea playArea = new (ballStart, coins.Concat(enemies).Append(finish));
            Debug.Log($"[Feature Setup] {scene.name}: reachable area {playArea.Area:0} m2");

            LevelPlacer placer = new (scene.name, playArea, coins, enemies, ballHeight)
            {
                Parent = new GameObject(LevelFeaturesName).transform
            };
            placer.Taken.Add((ballStart, 2.0f));
            placer.Taken.Add((finish, 2.0f));

            // The pickiest and biggest first, while there is still room for them.
            placer.Place(prefabs.MovingWall, layout.MovingWalls, MovingWallReach, 2.5f, 1.0f, 2.5f, true,
                spot => placer.GetMovingWallRotation(spot));
            placer.Place(prefabs.Spinner, layout.Spinners, SpinnerRadius, 3.4f, 3.0f, 4.2f, true,
                _ => Quaternion.identity);
            // Chasers start well away from the ball, so the player has a moment before meeting them.
            placer.Place(prefabs.Chaser, layout.Chasers, 1.0f, 1.5f, 1.0f, 3.0f, true,
                spot => IsFarFrom(spot, ballStart, 15.0f) ? Quaternion.identity : null);
            placer.Place(prefabs.Bumper, layout.Bumpers, 0.9f, 2.2f, 2.0f, 3.0f, true,
                _ => Quaternion.identity);
            placer.Place(prefabs.BoostPad, layout.BoostPads, 2.4f, 2.0f, 2.5f, 3.0f, true,
                spot => Quaternion.LookRotation(playArea.GetLongestRunDirection(spot)));
            placer.Place(prefabs.TimeCoin, layout.TimeCoins, 0.6f, 0.75f, 1.5f, 3.0f, false,
                _ => Quaternion.identity);
            placer.Place(prefabs.TimeTrap, layout.TimeTraps, 0.6f, 0.75f, 1.5f, 3.0f, false,
                _ => Quaternion.identity);
            placer.Place(prefabs.HealPickup, layout.HealPickups, 0.6f, 0.75f, 1.5f, 3.0f, false,
                _ => Quaternion.identity);
        }

        private static Vector3 GetBallStart()
        {
            BallInstaller ballInstaller = FindInScene<BallInstaller>().First();
            Transform ballStart = (Transform) new SerializedObject(ballInstaller)
                .FindProperty("_ballStartPoint").objectReferenceValue;

            return ballStart.position;
        }

        private static IEnumerable<(Vector3 Point, float Distance)> Avoid(IEnumerable<Vector3> points, float distance)
        {
            return points.Select(point => (point, distance));
        }

        private static bool HasGroundBelow(Vector3 point)
        {
            RaycastHit[] hits = Physics.RaycastAll(point + Vector3.up * 50.0f, Vector3.down, 200.0f);
            RaycastHit firstSolid = hits.Where(hit => !hit.collider.isTrigger).OrderBy(hit => hit.distance).FirstOrDefault();

            return firstSolid.collider != null && IsGround(firstSolid.collider);
        }

        private static float GetGroundHeight(Vector3 point, float fallback)
        {
            RaycastHit[] hits = Physics.RaycastAll(point + Vector3.up * 50.0f, Vector3.down, 200.0f);
            RaycastHit ground = hits.Where(hit => IsGround(hit.collider)).OrderBy(hit => hit.distance).FirstOrDefault();

            return ground.collider != null ? ground.point.y : fallback;
        }

        private static bool IsGround(Collider collider)
        {
            return collider.GetComponentInParent<GroundMarker>() != null || collider.GetComponent<Friction>() != null;
        }

        private static bool IsFarFrom(Vector3 point, Vector3 other, float distance)
        {
            return Flat(point - other).sqrMagnitude >= distance * distance;
        }

        private static Vector3 Flat(Vector3 vector) => new (vector.x, 0.0f, vector.z);

        private static void Place(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.SetPositionAndRotation(position, rotation);

            Debug.Log($"[Feature Setup] {instance.scene.name}: {prefab.name} at {position}");
        }


        // ---------- Main menu ----------

        // Rebuilds the statistics window, removes the old settings (tilt control is gone)
        // and lines the left column up evenly: levels, statistics, quit.
        private static void SetUpMainMenu()
        {
            Scene scene = EditorSceneManager.OpenScene(MainMenuScene, OpenSceneMode.Single);
            Button quitButton = FindInScene<Button>().First(button => button.name == "Quit");
            Transform canvas = quitButton.GetComponentInParent<Canvas>().rootCanvas.transform;

            string[] oldObjects = { SettingsButtonName, SettingsWindowName, StatisticsButtonName, StatisticsWindowName };
            foreach (string oldName in oldObjects)
            {
                Transform old = canvas.Find(oldName);
                if (old != null) Object.DestroyImmediate(old.gameObject);
            }
            TMP_FontAsset font = quitButton.GetComponentInChildren<TMP_Text>(true).font;
            RectTransform quitRect = (RectTransform) quitButton.transform;
            quitRect.sizeDelta = MenuWideButtonSize;

            RectTransform statisticsWindow = CreateStatisticsWindow(canvas, quitButton, font);
            AddMenuButton(quitButton, canvas, StatisticsButtonName, "Statistics", statisticsWindow);

            for (int i = 0; i < MenuColumn.Length; i++)
            {
                RectTransform button = (RectTransform) canvas.Find(MenuColumn[i]);
                button.anchoredPosition = new Vector2(button.anchoredPosition.x, MenuColumnTop - MenuColumnSpacing * i);
            }

            statisticsWindow.SetAsLastSibling();
            statisticsWindow.gameObject.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // A table: one row per level with completed, time played, best time and max coins.
        private static RectTransform CreateStatisticsWindow(Transform canvas, Button quitButton, TMP_FontAsset font)
        {
            RectTransform window = CreateWindow(StatisticsWindowName, canvas, "Statistics", new Vector2(620.0f, 340.0f), font, out RectTransform panel);

            string[] headers = { "Level", "Completed", "Time played", "Best time", "Max coins" };
            float[] columnX     = { -230.0f, -120.0f, 5.0f, 130.0f, 235.0f };
            float[] columnWidth = {  100.0f,  110.0f, 130.0f, 120.0f, 100.0f };
            const float headerY = 85.0f, rowHeight = 45.0f;

            for (int column = 0; column < headers.Length; column++)
            {
                AddCell(panel, headers[column], font, 20.0f, new Vector2(columnX[column], headerY), columnWidth[column], new Color(1.0f, 1.0f, 1.0f, 0.6f));
            }

            string[] levelNames = Levels.Select(level => Path.GetFileNameWithoutExtension(level)).ToArray();
            TMP_Text[,] values = new TMP_Text[headers.Length - 1, levelNames.Length];

            for (int row = 0; row < levelNames.Length; row++)
            {
                float y = headerY - rowHeight * (row + 1);
                AddCell(panel, levelNames[row], font, 24.0f, new Vector2(columnX[0], y), columnWidth[0], Color.white);

                for (int column = 1; column < headers.Length; column++)
                {
                    values[column - 1, row] = AddCell(panel, "-", font, 24.0f, new Vector2(columnX[column], y), columnWidth[column], Color.white);
                }
            }
            AddBackButton(quitButton, panel, "Statistics Back Button", window);

            SerializedObject statistics = new (window.gameObject.AddComponent<StatisticsWindow>());
            SetArray(statistics.FindProperty("_levelNames"), levelNames.Length, (element, i) => element.stringValue = levelNames[i]);

            string[] valueFields = { "_completedTexts", "_timePlayedTexts", "_bestTimeTexts", "_maxCoinsTexts" };
            for (int field = 0; field < valueFields.Length; field++)
            {
                int column = field;
                SetArray
                (
                    statistics.FindProperty(valueFields[field]),
                    levelNames.Length,
                    (element, row) => element.objectReferenceValue = values[column, row]
                );
            }
            statistics.ApplyModifiedPropertiesWithoutUndo();

            return window;
        }

        private static RectTransform CreateWindow
        (
            string name,
            Transform canvas,
            string titleText,
            Vector2 panelSize,
            TMP_FontAsset font,
            out RectTransform panel
        )
        {
            RectTransform window = CreateRect(name, canvas, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            Stretch(window, 0.0f);
            window.gameObject.AddComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.6f);

            panel = CreateRect
            (
                "Panel", window,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, panelSize
            );
            AddBackground(panel.gameObject, new Color(0.12f, 0.14f, 0.18f, 0.95f)).raycastTarget = true;

            RectTransform title = CreateRect
            (
                "Title", panel,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -14.0f), new Vector2(panelSize.x - 40.0f, 44.0f)
            );
            AddText(title, titleText, font, 30.0f, TextAlignmentOptions.Center);

            return window;
        }

        private static TMP_Text AddCell
        (
            RectTransform panel,
            string value,
            TMP_FontAsset font,
            float fontSize,
            Vector2 position,
            float width,
            Color color
        )
        {
            RectTransform cell = CreateRect
            (
                "Cell", panel,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                position, new Vector2(width, 40.0f)
            );
            TMP_Text text = AddText(cell, value, font, fontSize, TextAlignmentOptions.Center);
            text.color              = color;
            text.enableWordWrapping = false;
            text.fontSizeMax        = fontSize;
            text.fontSizeMin        = 12.0f;
            text.enableAutoSizing   = true;

            return text;
        }

        private static void AddMenuButton(Button quitButton, Transform canvas, string name, string label, RectTransform window)
        {
            Vector2 position = ((RectTransform) quitButton.transform).anchoredPosition;
            Button button = CopyButton(quitButton, canvas, name, label, position, MenuWideButtonSize, false);

            UnityEventTools.AddBoolPersistentListener(button.onClick, window.gameObject.SetActive, true);
        }

        private static void AddBackButton(Button quitButton, RectTransform panel, string name, RectTransform window)
        {
            float y = -panel.sizeDelta.y / 2.0f + 45.0f;
            Button back = CopyButton(quitButton, panel, name, "Back", new Vector2(0.0f, y), new Vector2(160.0f, 50.0f), true);

            UnityEventTools.AddBoolPersistentListener(back.onClick, window.gameObject.SetActive, false);
        }

        private static void SetArray(SerializedProperty array, int size, Action<SerializedProperty, int> setElement)
        {
            array.arraySize = size;
            for (int i = 0; i < size; i++) setElement(array.GetArrayElementAtIndex(i), i);
        }


        // Labels never wrap; a long one shrinks a little instead.
        private static Button CopyButton
        (
            Button source,
            Transform parent,
            string name,
            string label,
            Vector2 position,
            Vector2 size,
            bool centered
        )
        {
            GameObject copy = Object.Instantiate(source.gameObject, parent, false);
            copy.name = name;
            foreach (Quit quit in copy.GetComponents<Quit>()) Object.DestroyImmediate(quit);

            Button button = copy.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();

            RectTransform rect = (RectTransform) copy.transform;
            if (centered)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
            }
            rect.anchoredPosition = position;
            rect.sizeDelta        = size;

            TMP_Text text = copy.GetComponentInChildren<TMP_Text>(true);
            Stretch(text.rectTransform, 0.0f);
            text.text               = label;
            text.enableWordWrapping = false;
            text.fontSizeMax        = text.fontSize;
            text.fontSizeMin        = 18.0f;
            text.enableAutoSizing   = true;

            return button;
        }

        // ---------- Helpers ----------

        private static IEnumerable<T> FindInScene<T>() where T : Object
        {
            return Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        private static void EditPrefab(string path, Action<GameObject> edit)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                edit(root);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static GameObject SavePrefab(GameObject root, string path)
        {
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);

            return prefab;
        }


        private class LevelPrefabs
        {
            public GameObject BoostPad, HealPickup, Bumper, Spinner, MovingWall, TimeCoin, TimeTrap, Chaser;
        }

        // Places one kind of object after another, remembering the room each one takes.
        private class LevelPlacer
        {
            // Room left between objects, a bit more than the ball's diameter.
            private const float Passage = 1.5f;

            public readonly List<(Vector3 Position, float Radius)> Taken = new ();
            public Transform Parent;

            private readonly string _levelName;
            private readonly PlayArea _playArea;
            private readonly List<Vector3> _coins, _enemies;
            private readonly float _ballHeight;


            public LevelPlacer
            (
                string levelName,
                PlayArea playArea,
                List<Vector3> coins,
                List<Vector3> enemies,
                float ballHeight
            )
            {
                _levelName  = levelName;
                _playArea   = playArea;
                _coins      = coins;
                _enemies    = enemies;
                _ballHeight = ballHeight;
            }

            public void Place
            (
                GameObject prefab,
                int count,
                float radius,
                float minClearance,
                float coinDistance,
                float enemyDistance,
                bool onGround,
                Func<Vector3, Quaternion?> getRotation
            )
            {
                List<(Vector3 Point, float Distance)> avoid = Avoid(_coins, coinDistance)
                    .Concat(Avoid(_enemies, enemyDistance))
                    .ToList();
                int placed = 0;

                for (int attempt = 0; placed < count && attempt < count * 40; attempt++)
                {
                    if (!_playArea.TryFindSpot(Taken, radius + Passage, minClearance, avoid, out Vector3 spot)) break;

                    Quaternion? rotation = getRotation(spot);
                    if (rotation == null)
                    {
                        avoid.Add((spot, 1.0f));
                        continue;
                    }

                    Vector3 position = spot;
                    if (onGround) position.y = GetGroundHeight(spot, _ballHeight - BallRadius);

                    FeatureSetup.Place(prefab, Parent, position, rotation.Value);
                    Taken.Add((spot, radius));
                    placed++;
                }
                Debug.Log($"[Feature Setup] {_levelName}: {prefab.name} {placed} of {count}");
            }

            // The wall slides along its length, so it needs that much free room on both sides
            // and must not sweep through enemies; passing over coins is fine, they are only triggers.
            public Quaternion? GetMovingWallRotation(Vector3 spot)
            {
                Vector3 bestAxis = Vector3.zero;
                float bestRun = 0.0f;

                for (int i = 0; i < 4; i++)
                {
                    Vector3 axis = Quaternion.Euler(0.0f, i * 45.0f, 0.0f) * Vector3.right;
                    float run = Mathf.Min(_playArea.GetRun(spot, axis), _playArea.GetRun(spot, -axis));

                    if (run <= bestRun) continue;
                    if (_enemies.Any(enemy => DistanceToSegment(enemy, spot, axis) < 2.0f)) continue;

                    bestRun  = run;
                    bestAxis = axis;
                }

                if (bestRun < MovingWallReach + 0.5f) return null;
                return Quaternion.FromToRotation(Vector3.right, bestAxis);
            }


            private static float DistanceToSegment(Vector3 point, Vector3 center, Vector3 axis)
            {
                Vector3 offset = Flat(point - center);
                float along = Mathf.Clamp(Vector3.Dot(offset, axis), -MovingWallReach, MovingWallReach);

                return (offset - axis * along).magnitude;
            }
        }

        // Ball-sized grid cells the ball can roll to from its start without passing through walls or leaving the ground.
        // The flood fill stays near the level's own objects, so a gap in the walls cannot spill it over the whole ground.
        // Cells are small and the probe a bit thinner than the ball, otherwise narrow passages between walls,
        // which the real ball rolls through, would cut the level in half.
        private class PlayArea
        {
            private const float CellSize = 0.25f, ProbeRadius = 0.5f, BoundsMargin = 6.0f;

            public float Area => _clearance.Count * CellSize * CellSize;

            private readonly float _height;
            private readonly Rect _bounds;
            // Reachable cell -> distance to the edge of the reachable area.
            private readonly Dictionary<Vector2Int, float> _clearance = new ();


            public PlayArea(Vector3 start, IEnumerable<Vector3> levelObjects)
            {
                _height = start.y;

                List<Vector3> points = levelObjects.Append(start).ToList();
                float minX = points.Min(point => point.x) - BoundsMargin;
                float minZ = points.Min(point => point.z) - BoundsMargin;
                float maxX = points.Max(point => point.x) + BoundsMargin;
                float maxZ = points.Max(point => point.z) + BoundsMargin;
                _bounds = Rect.MinMaxRect(minX, minZ, maxX, maxZ);

                ComputeClearance(FloodFill(ToCell(start)));
            }

            public bool TryFindSpot
            (
                List<(Vector3 Position, float Radius)> taken,
                float radius,
                float minClearance,
                List<(Vector3 Point, float Distance)> avoid,
                out Vector3 spot
            )
            {
                spot = default;
                float bestScore = float.MinValue;

                foreach (KeyValuePair<Vector2Int, float> cell in _clearance)
                {
                    if (cell.Value < minClearance) continue;

                    Vector3 position = ToWorld(cell.Key);
                    float gap = taken.Min(other => Flat(position - other.Position).magnitude - other.Radius);
                    if (gap < radius) continue;

                    // As far as possible from everything placed so far, which spreads them over the level,
                    // but not squeezed into a tight corner.
                    float score = gap + 0.5f * Mathf.Min(cell.Value, 3.0f);
                    if (score <= bestScore) continue;
                    if (avoid.Any(item => !IsFarFrom(position, item.Point, item.Distance))) continue;

                    bestScore = score;
                    spot      = position;
                }
                return bestScore > float.MinValue;
            }

            // Of 8 directions, the one with the longest free run, so a pad sends the ball into open space.
            public Vector3 GetLongestRunDirection(Vector3 position)
            {
                Vector3 bestDirection = Vector3.forward;
                float bestRun = -1.0f;

                for (int i = 0; i < 8; i++)
                {
                    Vector3 direction = Quaternion.Euler(0.0f, i * 45.0f, 0.0f) * Vector3.forward;
                    float run = GetRun(position, direction);
                    if (run <= bestRun) continue;

                    bestRun       = run;
                    bestDirection = direction;
                }
                return bestDirection;
            }

            // How far the ball could roll from the position in a straight line.
            public float GetRun(Vector3 position, Vector3 direction)
            {
                float run = 0.0f;
                while (run < 40.0f && _clearance.ContainsKey(ToCell(position + direction * (run + CellSize))))
                {
                    run += CellSize;
                }
                return run;
            }


            private HashSet<Vector2Int> FloodFill(Vector2Int start)
            {
                HashSet<Vector2Int> reachable = new ();
                HashSet<Vector2Int> visited   = new () { start };
                Queue<Vector2Int> queue       = new ();
                queue.Enqueue(start);

                while (queue.Count > 0)
                {
                    Vector2Int cell = queue.Dequeue();
                    if (!IsFree(cell)) continue;

                    reachable.Add(cell);
                    foreach (Vector2Int neighbour in Neighbours(cell, false))
                    {
                        if (visited.Add(neighbour)) queue.Enqueue(neighbour);
                    }
                }
                return reachable;
            }

            // Breadth-first from the edge cells inwards gives each cell its distance to the edge.
            private void ComputeClearance(HashSet<Vector2Int> reachable)
            {
                Queue<Vector2Int> queue = new ();

                foreach (Vector2Int cell in reachable)
                {
                    if (Neighbours(cell, true).All(reachable.Contains)) continue;

                    _clearance[cell] = CellSize;
                    queue.Enqueue(cell);
                }
                while (queue.Count > 0)
                {
                    Vector2Int cell = queue.Dequeue();
                    float next = _clearance[cell] + CellSize;

                    foreach (Vector2Int neighbour in Neighbours(cell, true))
                    {
                        if (!reachable.Contains(neighbour) || _clearance.ContainsKey(neighbour)) continue;

                        _clearance[neighbour] = next;
                        queue.Enqueue(neighbour);
                    }
                }
            }

            private bool IsFree(Vector2Int cell)
            {
                Vector3 center = ToWorld(cell);
                if (!_bounds.Contains(new Vector2(center.x, center.z)) || !HasGroundBelow(center)) return false;

                return Physics.OverlapSphere(center, ProbeRadius).All(found => found.isTrigger || IsGround(found));
            }

            private static IEnumerable<Vector2Int> Neighbours(Vector2Int cell, bool withDiagonals)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && z == 0) continue;
                        if (!withDiagonals && x != 0 && z != 0) continue;

                        yield return new Vector2Int(cell.x + x, cell.y + z);
                    }
                }
            }

            private Vector2Int ToCell(Vector3 position)
            {
                return new Vector2Int
                (
                    Mathf.RoundToInt(position.x / CellSize),
                    Mathf.RoundToInt(position.z / CellSize)
                );
            }

            private Vector3 ToWorld(Vector2Int cell) => new (cell.x * CellSize, _height, cell.y * CellSize);
        }
    }
}
