using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MainScene.StartPoints;
using MainScene.UI.HealthBar;
using MainScene.UI.Timer;
using Object = UnityEngine.Object;

namespace BallFinisher.Editor
{
    // Icons for the rules screen: every element of a level gets a small picture rendered from its own prefab,
    // and each level shows only the elements it really contains.
    public static partial class FeatureSetup
    {
        private const string IntroIconsFolder = "Assets/Images/Intro Icons/";
        private const string IntroElementsName = "Elements";
        private const int IntroIconSize = 128;
        private const int IntroRenderSize = 384;

        private class IntroElement
        {
            public string Name, PrefabPath, Text;
            public EnemyStartPoint.EnemyTypes? Enemy;
            public bool Always;
        }

        // Order on the screen: goal first, then enemies, then what helps and what gets in the way.
        private static readonly IntroElement[] IntroElements =
        {
            new () { Name = "Coin",        PrefabPath = ObjectsFolder + "Coin.prefab",        Text = "Coin - collect them all",            Always = true },
            new () { Name = "Finish",      PrefabPath = ObjectsFolder + "Finish .prefab",     Text = "Finish - opens after the last coin", Always = true },
            new () { Name = "Mushroom",    PrefabPath = ObjectsFolder + "Mushroom.prefab",    Text = "Mushroom - stands still, hurts",     Enemy = EnemyStartPoint.EnemyTypes.Mushroom },
            new () { Name = "Cactus",      PrefabPath = ObjectsFolder + "Cactus.prefab",      Text = "Cactus - patrols back and forth",    Enemy = EnemyStartPoint.EnemyTypes.Cactus },
            new () { Name = "Chaser",      PrefabPath = ObjectsFolder + "Chaser.prefab",      Text = "Chaser - follows you when close" },
            new () { Name = "Heal Pickup", PrefabPath = HealPickupPath,                       Text = "Green cross - heals you" },
            new () { Name = "Time Coin",   PrefabPath = ObjectsFolder + "Time Coin.prefab",   Text = "Blue clock - adds time" },
            new () { Name = "Time Trap",   PrefabPath = ObjectsFolder + "Time Trap.prefab",   Text = "Red clock - takes time away" },
            new () { Name = "Boost Pad",   PrefabPath = BoostPadPath,                         Text = "Orange pad - speeds you up" },
            new () { Name = "Bumper",      PrefabPath = BumperPath,                           Text = "Bumper - bounces you off" },
            new () { Name = "Spinner",     PrefabPath = SpinnerPath,                          Text = "Spinning bar - knocks the ball" },
            new () { Name = "Moving Wall", PrefabPath = MovingWallPath,                       Text = "Sliding wall - moves side to side" }
        };


        [MenuItem("Ball Finisher/Rebuild Level Intro With Icons")]
        public static void RebuildLevelIntro()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            _panelSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            EditorSceneManager.OpenScene(Levels[0], OpenSceneMode.Single);
            GameObject intro = CreateLevelIntroPrefab(GetHealthBarFont());

            foreach (string path in Levels)
            {
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Transform canvas = FindInScene<HealthBar>().First()
                    .GetComponentInParent<Canvas>().rootCanvas.transform;

                ReplaceHudElement(canvas, intro);
                ShowLevelIntroElements(scene);

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Feature Setup] Level intro rebuilt");
        }


        // One row per element: the icon on the left, a short description next to it.
        private static void AddIntroElements(RectTransform panel, TMP_FontAsset font)
        {
            RectTransform elements = CreateRect
            (
                IntroElementsName, panel,
                new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, -106.0f), new Vector2(712.0f, 260.0f)
            );
            GridLayoutGroup grid = elements.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize        = new Vector2(350.0f, 40.0f);
            grid.spacing         = new Vector2(12.0f, 4.0f);
            grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.childAlignment  = TextAnchor.UpperCenter;

            foreach (IntroElement element in IntroElements)
            {
                RectTransform row = CreateRect(element.Name, elements, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
                AddBackground(row.gameObject, new Color(1.0f, 1.0f, 1.0f, 0.06f));

                RectTransform iconRect = CreateRect
                (
                    "Icon", row,
                    new Vector2(0.0f, 0.5f), new Vector2(0.0f, 0.5f),
                    new Vector2(3.0f, 0.0f), new Vector2(36.0f, 36.0f)
                );
                Image icon = iconRect.gameObject.AddComponent<Image>();
                icon.sprite         = RenderIntroIcon(element);
                icon.preserveAspect = true;
                icon.raycastTarget  = false;

                RectTransform labelRect = CreateRect("Label", row, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
                Stretch(labelRect, 0.0f);
                labelRect.offsetMin = new Vector2(46.0f, 0.0f);
                labelRect.offsetMax = new Vector2(-6.0f, 0.0f);
                TMP_Text label = AddText(labelRect, element.Text, font, 16.0f, TextAlignmentOptions.MidlineLeft);
                label.enableWordWrapping = false;
                label.enableAutoSizing   = true;
                label.fontSizeMin        = 11.0f;
                label.fontSizeMax        = 16.0f;
            }
        }

        // Hides the rows of elements the open level does not contain.
        private static void ShowLevelIntroElements(Scene scene)
        {
            Transform elements = FindInScene<LevelIntro>().First().transform.Find("Panel/" + IntroElementsName);

            HashSet<string> placedPrefabs = new (scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .Where(transform => PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject))
                .Select(transform => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(transform.gameObject)));
            HashSet<EnemyStartPoint.EnemyTypes> enemies = new (FindInScene<EnemyStartPoint>().Select(point => point.EnemyType));

            foreach (IntroElement element in IntroElements)
            {
                bool present = element.Always
                            || (element.Enemy.HasValue ? enemies.Contains(element.Enemy.Value) : placedPrefabs.Contains(element.PrefabPath));
                elements.Find(element.Name).gameObject.SetActive(present);
            }
            Debug.Log($"[Feature Setup] {scene.name} intro: " +
                      string.Join(", ", elements.Cast<Transform>().Where(row => row.gameObject.activeSelf).Select(row => row.name)));
        }


        // ---------- Icons ----------

        // The prefab is rendered twice, on black and on white; the difference gives each pixel's transparency.
        private static Sprite RenderIntroIcon(IntroElement element)
        {
            EnsureFolder("Assets/Images", "Intro Icons");
            string path = IntroIconsFolder + element.Name + ".png";

            GameObject instance = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(element.PrefabPath));
            foreach (Transform child in instance.GetComponentsInChildren<Transform>(true).Where(child => child.name == MinimapIconName).ToList())
            {
                Object.DestroyImmediate(child.gameObject);
            }

            PreviewRenderUtility preview = new ();
            try
            {
                preview.AddSingleGO(instance);
                Bounds bounds = GetRenderBounds(instance);

                Camera camera = preview.camera;
                camera.fieldOfView   = 30.0f;
                camera.clearFlags    = CameraClearFlags.SolidColor;
                camera.allowHDR      = false;
                Quaternion view = Quaternion.Euler(28.0f, -35.0f, 0.0f);
                float distance = bounds.extents.magnitude / Mathf.Sin(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 1.02f;
                camera.transform.SetPositionAndRotation(bounds.center - view * Vector3.forward * distance, view);
                camera.nearClipPlane = distance * 0.05f;
                camera.farClipPlane  = distance * 4.0f;

                preview.lights[0].intensity          = 1.2f;
                preview.lights[0].transform.rotation = Quaternion.Euler(45.0f, -20.0f, 0.0f);
                preview.lights[1].intensity          = 0.6f;
                preview.lights[1].transform.rotation = Quaternion.Euler(20.0f, 160.0f, 0.0f);
                preview.ambientColor = new Color(0.35f, 0.35f, 0.35f);

                Texture2D onBlack = RenderPreview(preview, Color.black);
                Texture2D onWhite = RenderPreview(preview, Color.white);
                Texture2D icon = CropToContent(CombineToTransparent(onBlack, onWhite));

                File.WriteAllBytes(path, icon.EncodeToPNG());
                Object.DestroyImmediate(icon);
            }
            finally
            {
                preview.Cleanup();
            }

            AssetDatabase.ImportAsset(path);
            TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(path);
            importer.textureType         = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled       = false;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Bounds GetRenderBounds(GameObject instance)
        {
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>()
                .Where(renderer => renderer.enabled && renderer is MeshRenderer or SkinnedMeshRenderer)
                .ToArray();
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);

            return bounds;
        }

        private static Texture2D RenderPreview(PreviewRenderUtility preview, Color background)
        {
            preview.camera.backgroundColor = background;
            preview.BeginStaticPreview(new Rect(0, 0, IntroRenderSize, IntroRenderSize));
            preview.Render(true);
            Texture2D rendered = preview.EndStaticPreview();

            // The preview texture may be larger on high-DPI screens; read it back at the icon size.
            RenderTexture target = RenderTexture.GetTemporary(IntroRenderSize, IntroRenderSize, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(rendered, target);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            Texture2D result = new (IntroRenderSize, IntroRenderSize, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, IntroRenderSize, IntroRenderSize), 0, 0);
            result.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(target);

            return result;
        }

        // Some prefabs have parts that are not drawn (colliders' helpers, effects), so the framing can be loose;
        // the icon is cut to the visible pixels, kept square, and scaled back to the icon size.
        private static Texture2D CropToContent(Texture2D source)
        {
            int minX = source.width, minY = source.height, maxX = -1, maxY = -1;
            Color[] pixels = source.GetPixels();
            for (int y = 0; y < source.height; y++)
            {
                for (int x = 0; x < source.width; x++)
                {
                    if (pixels[y * source.width + x].a < 0.05f) continue;
                    minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y);
                }
            }
            if (maxX < 0) return source;

            float side = Mathf.Max(maxX - minX + 1, maxY - minY + 1) * 1.08f;
            float centerX = (minX + maxX + 1) * 0.5f, centerY = (minY + maxY + 1) * 0.5f;

            Texture2D cropped = new (IntroIconSize, IntroIconSize, TextureFormat.RGBA32, false);
            for (int y = 0; y < IntroIconSize; y++)
            {
                for (int x = 0; x < IntroIconSize; x++)
                {
                    float u = (centerX + ((x + 0.5f) / IntroIconSize - 0.5f) * side) / source.width;
                    float v = (centerY + ((y + 0.5f) / IntroIconSize - 0.5f) * side) / source.height;
                    bool inside = u >= 0.0f && u <= 1.0f && v >= 0.0f && v <= 1.0f;
                    cropped.SetPixel(x, y, inside ? source.GetPixelBilinear(u, v) : Color.clear);
                }
            }
            cropped.Apply();
            Object.DestroyImmediate(source);

            return cropped;
        }

        private static Texture2D CombineToTransparent(Texture2D onBlack, Texture2D onWhite)
        {
            Color[] black = onBlack.GetPixels(), white = onWhite.GetPixels();
            Color[] result = new Color[black.Length];

            for (int i = 0; i < black.Length; i++)
            {
                float alpha = 1.0f - ((white[i].r - black[i].r) + (white[i].g - black[i].g) + (white[i].b - black[i].b)) / 3.0f;
                alpha = Mathf.Clamp01(alpha);
                result[i] = alpha < 0.004f
                    ? Color.clear
                    : new Color(Mathf.Clamp01(black[i].r / alpha), Mathf.Clamp01(black[i].g / alpha), Mathf.Clamp01(black[i].b / alpha), alpha);
            }

            Texture2D combined = new (onBlack.width, onBlack.height, TextureFormat.RGBA32, false);
            combined.SetPixels(result);
            combined.Apply();
            Object.DestroyImmediate(onBlack);
            Object.DestroyImmediate(onWhite);

            return combined;
        }
    }
}
