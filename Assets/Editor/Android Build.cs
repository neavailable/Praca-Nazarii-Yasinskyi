using System;
using System.IO;
using System.Linq;
using Configuration.Configurations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEngine;

namespace BallFinisher.Editor
{
    public sealed class AndroidBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            ValidateConfigurations();
            if (report.summary.platform == BuildTarget.Android)
                PrepareFirebaseRepository();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes.Where(s => s.enabled))
            {
                if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(scene.path)))
                    throw new BuildFailedException($"Build scene is missing: {scene.path}");
            }
        }

        [MenuItem("Build/Android APK")]
        public static void BuildApk()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
                throw new BuildFailedException("Install Android Build Support, SDK/NDK and OpenJDK for Unity 2022.3.60f1.");

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                throw new BuildFailedException("Could not switch the active build target to Android.");

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            string output = GetArgument("-androidOutput") ?? "Builds/Android/BallFinisher.apk";
            output = Path.GetFullPath(output);
            Directory.CreateDirectory(Path.GetDirectoryName(output));

            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Android build {report.summary.result}: {report.summary.totalErrors} errors.");

            Debug.Log($"Android APK built successfully: {output} ({report.summary.totalSize} bytes)");
        }

        private static string GetArgument(string name)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, name);
            return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : null;
        }

        private static void ValidateConfigurations()
        {
            // Instantiate every JSON configuration so missing sections, resources or values fail before building.
            foreach (Type type in typeof(BallMovementConfiguration).Assembly.GetTypes()
                         .Where(t => !t.IsAbstract && t.BaseType != null && t.BaseType.IsGenericType &&
                                     t.BaseType.GetGenericTypeDefinition() == typeof(AbstractConfiguration<>)))
            {
                object configuration = Activator.CreateInstance(type);
                try
                {
                    type.GetMethod("Load").Invoke(configuration, null);
                }
                catch (Exception exception)
                {
                    throw new BuildFailedException($"Invalid configuration {type.Name}: {exception.GetBaseException().Message}");
                }
            }
        }

        private static void PrepareFirebaseRepository()
        {
            // Firebase App is a UPM package; its native .srcaar is not in Assets/Firebase.
            // Stage its Maven artifacts outside Assets so Unity does not import the AAR twice.
            PackageInfo package = PackageInfo.FindForAssetPath("Packages/com.google.firebase.app");
            if (package == null)
                throw new BuildFailedException("Firebase App package is missing.");

            string source = Path.Combine(package.resolvedPath, "Firebase/m2repository");
            string destination = Path.GetFullPath("Library/AndroidFirebaseRepository");
            if (!Directory.Exists(source))
                throw new BuildFailedException($"Firebase Maven repository is missing: {source}");

            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                    continue;

                string relative = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (relative.EndsWith(".srcaar", StringComparison.OrdinalIgnoreCase))
                    relative = Path.ChangeExtension(relative, ".aar");

                string target = Path.Combine(destination, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target, true);
            }
        }
    }
}
