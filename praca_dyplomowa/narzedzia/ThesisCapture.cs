using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using R3;
using Infrastructure.MainMenu;
using MainMenu.Statistics;
using MainScene.Ball;
using MainScene.Finish.FinishTrigger;
using MainScene.LineRender;
using MainScene.Wall.WallsMarker;

namespace BallFinisher.Editor
{
    // Temporary tool used only in an isolated copy of the project: plays the game in batch mode
    // and saves screenshots for the thesis. It is never added to the real project.
    [InitializeOnLoad]
    public static class ThesisCapture
    {
        private const string ActiveKey = "ThesisCapture.Active";
        private const string DoneKey = "ThesisCapture.Done";
        private const int Width = 1920, Height = 1080;

        private static readonly List<Func<float>> Steps = new();
        private static int _step;
        private static double _resumeAt, _startedAt;
        private static string _outDir;
        private static readonly List<(Canvas canvas, RenderMode mode, Camera camera, float distance)> SavedCanvases = new();

        static ThesisCapture()
        {
            if (!SessionState.GetBool(ActiveKey, false)) return;
            if (SessionState.GetBool(DoneKey, false))
            {
                EditorApplication.delayCall += () => EditorApplication.Exit(0);
                return;
            }
            _outDir = SessionState.GetString("ThesisCapture.Out", "");
            BuildSteps();
            EditorApplication.update += Tick;
        }

        public static void Run()
        {
            string outDir = GetArgument("-thesisOut") ?? Path.GetFullPath("ThesisCapture");
            Directory.CreateDirectory(outDir);
            SessionState.SetString("ThesisCapture.Out", outDir);
            SessionState.SetBool(ActiveKey, true);
            SessionState.SetBool(DoneKey, false);
            EditorSceneManager.OpenScene("Assets/Scenes/Main Menu.unity");
            Debug.Log("[ThesisCapture] entering play mode");
            _outDir = outDir;
            BuildSteps();
            EditorApplication.update += Tick;
            EditorApplication.EnterPlaymode();
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;
            double now = EditorApplication.timeSinceStartup;
            if (_startedAt == 0) { _startedAt = now; _resumeAt = now + 3.0; }
            if (now - _startedAt > 900) { Finish("timeout"); return; }
            if (now < _resumeAt) return;
            if (_step >= Steps.Count) { Finish("all steps done"); return; }
            float wait;
            try
            {
                wait = Steps[_step]();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[ThesisCapture] step {_step} failed: {exception}");
                wait = 0.5f;
            }
            _step++;
            _resumeAt = EditorApplication.timeSinceStartup + wait;
        }

        private static void Finish(string reason)
        {
            Debug.Log($"[ThesisCapture] finished: {reason}");
            EditorApplication.update -= Tick;
            SessionState.SetBool(DoneKey, true);
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredEditMode) EditorApplication.Exit(0);
            };
            EditorApplication.ExitPlaymode();
        }

        private static void BuildSteps()
        {
            Steps.Clear();
            // Main menu and its statistics window.
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("menu_glowne"));
            Steps.Add(() => { SetActive<StatisticsWindow>(true); return 0.5f; });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("menu_statystyki"));
            Steps.Add(() => { SetActive<StatisticsWindow>(false); return 0.3f; });
            // Loading screen, entered the same way as from the level button.
            Steps.Add(() =>
            {
                LoadingLoader loader = FindAll<LoadingLoader>().OrderBy(l => l.name).First();
                Debug.Log($"[ThesisCapture] loading through {loader.name}");
                loader.LoadLevel();
                return 0.6f;
            });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("ekran_ladowania"));
            Steps.Add(() => WaitForScene("Level 1"));
            // Level 1: HUD, aiming line, rolling ball, win window, top view.
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom1_hud"));
            Steps.Add(() => ShowAimLine(true));
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom1_celowanie"));
            Steps.Add(() => { ShowAimLine(false); return LaunchBall(); });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom1_ruch"));
            Steps.Add(() => TopView("poziom1_widok_z_gory"));
            Steps.Add(() =>
            {
                UnityEngine.Object.FindObjectOfType<FinishTrigger>(true).BallCollidedFinished.OnNext(Unit.Default);
                return 1.0f;
            });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom1_wygrana"));
            // Level 2: damage taken, then the loss window.
            Steps.Add(() => { SceneManager.LoadScene("Level 2"); return 3.0f; });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom2_hud"));
            Steps.Add(() => TopView("poziom2_widok_z_gory"));
            Steps.Add(() => { Damage(29); return 0.4f; });
            Steps.Add(() => { Damage(29); return 1.0f; });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom2_obrazenia"));
            Steps.Add(() => { Damage(100); return 1.0f; });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom2_przegrana"));
            // Level 3.
            Steps.Add(() => { SceneManager.LoadScene("Level 3"); return 3.0f; });
            Steps.Add(() => Prepare());
            Steps.Add(() => Save("poziom3_hud"));
            Steps.Add(() => TopView("poziom3_widok_z_gory"));
        }

        private static float WaitForScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                _step--;
                return 0.25f;
            }
            return 3.0f;
        }

        // Screen-space overlay canvases are drawn by no camera, so for the capture they are moved in front of the main camera.
        private static float Prepare()
        {
            Camera camera = MainCamera();
            RenderTexture target = new(Width, Height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            camera.targetTexture = target;
            SavedCanvases.Clear();
            foreach (Canvas canvas in UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c => c.isRootCanvas))
            {
                SavedCanvases.Add((canvas, canvas.renderMode, canvas.worldCamera, canvas.planeDistance));
                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
                canvas.renderMode    = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera   = camera;
                canvas.planeDistance = camera.nearClipPlane + 0.05f;
            }
            Canvas.ForceUpdateCanvases();
            return 0.3f;
        }

        private static Camera MainCamera()
        {
            if (Camera.main != null) return Camera.main;
            Camera camera = new GameObject("Thesis Temp Camera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            return camera;
        }

        private static float Save(string fileName)
        {
            Camera camera = MainCamera();
            foreach (Camera other in Camera.allCameras.Where(c => c != camera && c.targetTexture != null)) other.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();
            WritePng(camera.targetTexture, fileName);
            RenderTexture target = camera.targetTexture;
            camera.targetTexture = null;
            target.Release();
            foreach (var saved in SavedCanvases.Where(s => s.canvas != null))
            {
                saved.canvas.renderMode    = saved.mode;
                saved.canvas.worldCamera   = saved.camera;
                saved.canvas.planeDistance = saved.distance;
            }
            SavedCanvases.Clear();
            return 0.3f;
        }

        private static float TopView(string fileName)
        {
            Bounds bounds = default;
            bool first = true;
            foreach (WallMarker wall in UnityEngine.Object.FindObjectsOfType<WallMarker>())
            {
                foreach (Renderer renderer in wall.GetComponentsInChildren<Renderer>())
                {
                    if (first) { bounds = renderer.bounds; first = false; }
                    else bounds.Encapsulate(renderer.bounds);
                }
            }
            if (first) throw new InvalidOperationException("no walls found for the top view");
            float aspect = Mathf.Clamp(bounds.size.x / bounds.size.z, 0.6f, 1.8f);
            int width = 1600, height = Mathf.RoundToInt(width / aspect);
            Camera camera = new GameObject("Thesis Top Camera").AddComponent<Camera>();
            camera.orthographic     = true;
            camera.orthographicSize = Mathf.Max(bounds.extents.z, bounds.extents.x / aspect) * 1.06f;
            camera.transform.SetPositionAndRotation(bounds.center + Vector3.up * 80.0f, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            camera.farClipPlane     = 200.0f;
            camera.clearFlags       = CameraClearFlags.SolidColor;
            camera.backgroundColor  = new Color(0.12f, 0.14f, 0.17f);
            camera.cullingMask      = ~LayerMask.GetMask("UI");
            RenderTexture target = new(width, height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            camera.targetTexture = target;
            camera.Render();
            WritePng(target, fileName);
            camera.targetTexture = null;
            target.Release();
            UnityEngine.Object.Destroy(camera.gameObject);
            Debug.Log($"[ThesisCapture] {fileName}: bounds {bounds.size}");
            return 0.3f;
        }

        private static void WritePng(RenderTexture source, string fileName)
        {
            RenderTexture resolved = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(source, resolved);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = resolved;
            Texture2D texture = new(source.width, source.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            texture.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(resolved);
            string path = Path.Combine(_outDir, fileName + ".png");
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.Destroy(texture);
            Debug.Log($"[ThesisCapture] saved {path}");
        }

        private static float ShowAimLine(bool show)
        {
            LineRenderDrawer drawer = FindAll<LineRenderDrawer>().First();
            drawer.enabled = false;
            drawer.gameObject.SetActive(show);
            if (!show) return 0.2f;
            Transform ball = UnityEngine.Object.FindObjectOfType<BallMovement>().transform;
            Vector3 back = -Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            LineRenderer line = drawer.GetComponent<LineRenderer>();
            Vector3 start = ball.position;
            start.y = 0.2f;
            line.SetPosition(0, start);
            line.SetPosition(1, start + back * 3.5f);
            return 0.3f;
        }

        private static float LaunchBall()
        {
            BallMovement ball = UnityEngine.Object.FindObjectOfType<BallMovement>();
            Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            ball.BallMovementLogic.Launch(forward, 14.0f, 25.0f);
            return 0.45f;
        }

        private static void Damage(int damage)
        {
            UnityEngine.Object.FindObjectOfType<BallGetDamage>().GetDamage(damage);
        }

        private static void SetActive<T>(bool active) where T : Component
        {
            foreach (T component in FindAll<T>()) component.gameObject.SetActive(active);
        }

        private static IEnumerable<T> FindAll<T>() where T : Component
        {
            return Resources.FindObjectsOfTypeAll<T>().Where(c => c.gameObject.scene.IsValid());
        }

        private static string GetArgument(string name)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, name);
            return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : null;
        }
    }
}
