using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using CodeEditor = Unity.CodeEditor.CodeEditor;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{
    public class Assister
    {
        public const string Version = "1.4.19"; //do NOT modify this line, except the number value, its being used by VSCode/Typescript for version detection (in exporter.ts.getVersionOfUnitySide())

#if MERYEL_UCA_LITE_VERSION
        public const string Title = "Code Assist Lite";
#else
        public const string Title = "Code Assist";
#endif

        [MenuItem("Tools/" + Title + "/Status", false, 1)]
        static void DisplayStatusWindow()
        {
            StatusWindow.Display();
        }


        [MenuItem("Tools/" + Title + "/Synchronize", false, 2)]
        static void Sync()
        {
            EditorCoroutines.EditorCoroutineUtility.StartCoroutine(SyncAux(), MQTTnetInitializer.Publisher);

            //MQTTnetInitializer.Publisher.SendConnect();
            //Serilog.Log.Information("Code Assist is looking for more IDEs to connect to...");

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "Synchronize_MenuItem");
        }


        [MenuItem("Tools/" + Title + "/Report error", false, 91)]
        static void DisplayFeedbackWindow()
        {
            FeedbackWindow.Display();
        }

        [MenuItem("Tools/" + Title + "/About", false, 92)]
        static void DisplayAboutWindow()
        {
            AboutWindow.Display();
        }

#if MERYEL_UCA_LITE_VERSION
        [MenuItem("Tools/" + Title + "/Compare versions", false, 31)]
        static void CompareVersions()
        {
            Application.OpenURL("http://unitycodeassist.netlify.app/compare");

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "CompareVersions_MenuItem");
        }

        [MenuItem("Tools/" + Title + "/Get full version", false, 32)]
        static void GetFullVersion()
        {
            Application.OpenURL("https://unitycodeassist.netlify.app/purchase?utm_source=unity_getfull");

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "FullVersion_MenuItem");
        }
#endif // MERYEL_UCA_LITE_VERSION

        [MenuItem("Tools/" + Title + "/Setup/Upgrade to full version", false, 65)]
        public static void Upgrade()
        {
            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "Upgrade_MenuItem");

#if MERYEL_UCA_LITE_VERSION
            Serilog.Log.Information("Purchase <a href=\"https://unitycodeassist.netlify.app/purchase?utm_source=unity_upgrade\">Unity Code Assist</a> from the <a href=\"http://u3d.as/2N2H\">Asset Store</a> or <a href=\"https://meryel.itch.io/unity-code-assist\">itch.io</a> first. Then download it from the package manager or itch.io");
            return;
#else
            if (GetCodeEditor(true, out var isVisualStudio, out var isVisualStudioCode, out var error))
            {
                if (isVisualStudio)
                {
                    var vsixPath = CommonTools.GetInstallerPath("CodeAssist.Full.VisualStudio.Installer.vsix");
                    if (System.IO.File.Exists(vsixPath))
                    {
                        CallVisualStudioInstaller(vsixPath);
                        return;
                    }

                    var zipPath = CommonTools.GetInstallerPath("CodeAssist.Full.VisualStudio.Installer.zip");
                    if (System.IO.File.Exists(zipPath))
                    {
                        var tempVsixPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CodeAssist.Full.VisualStudio.Installer.vsix");
                        System.IO.File.Copy(zipPath, tempVsixPath, true);

                        CallVisualStudioInstaller(tempVsixPath);
                        return;
                    }
                    
                    Serilog.Log.Information("Installer for Visual Studio couldn't be found at {ZipPath}. Please try re-importing the asset from the package manager", zipPath);
                    return;
                }
                else if (isVisualStudioCode)
                {
                    var vsixPath = CommonTools.GetInstallerPath("CodeAssist.Full.VSCode.Installer.vsix");
                    if (System.IO.File.Exists(vsixPath))
                    {
                        CallVSCodeInstaller(vsixPath);
                        return;
                    }

                    var zipPath = CommonTools.GetInstallerPath("CodeAssist.Full.VSCode.Installer.zip");
                    if (System.IO.File.Exists(zipPath))
                    {
                        var tempVsixPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CodeAssist.Full.VSCode.Installer.vsix");
                        System.IO.File.Copy(zipPath, tempVsixPath, true);

                        CallVSCodeInstaller(tempVsixPath);
                        return;
                    }

                    Serilog.Log.Information("Installer for VS Code couldn't be found at {ZipPath}. Please try re-importing the asset from the package manager", zipPath);
                    return;
                }
            }
            else
            {
                Serilog.Log.Information(error!);
            }
#endif
            
        }

        /*
        [MenuItem("Tools/" + Title + "/Setup/Update", false, 61)]
        static void Update()
        {
            //UnityEditor.PackageManager.Client.
        }
        */

        [MenuItem("Tools/" + Title + "/Setup/Re-import package", false, 62)]
        static void RepairFiles()
        {
            if (MQTTnetInitializer.Publisher?.Clients.Any() != true)
                Serilog.Log.Information("No connected IDE found. Please start up Visual Studio or VS Code first");

            //var cleanupPath = CommonTools.GetToolPath("CleanupObsoleteFiles.bat");
            //Execute(cleanupPath);
            Cleanup.DoCleanup();

            MQTTnetInitializer.Publisher?.SendRequestUpdate("Unity", string.Empty, true);

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "Reimport_MenuItem");
        }

        [MenuItem("Tools/" + Title + "/Setup/Import files for .NET Standard 2.0", false, 63)]
        static void ImportSystemBinariesForDotNetStandard20()
        {
            var solutionDirectory = CommonTools.GetProjectPath();
            var cSharpVersion = Cleanup.GetCSharpVersionFromUnityProjectVersionFile(solutionDirectory);

            if (cSharpVersion >= 9)
            {
                if (!EditorUtility.DisplayDialog("Import files for .NET Standard 2.0",
                    "This is not required for versions of Unity 2021.2 and newer. Do you still want to continue?",
                    "Okay", "Cancel"))
                {
                    Serilog.Log.Debug("ImportNetStandard20_MenuItem cancelled via confirm dialog");
                    return;
                }
            }

            if (MQTTnetInitializer.Publisher?.Clients.Any() != true)
                Serilog.Log.Information("No connected IDE found. Please start up Visual Studio or VS Code first");

            MQTTnetInitializer.Publisher?.SendRequestUpdate("SystemBinariesForDotNetStandard20", string.Empty, true);

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "ImportNetStandard20_MenuItem");
        }

        [MenuItem("Tools/" + Title + "/Setup/Regenerate project files", false, 64)]
        public static void RegenerateProjectFiles() => RegenerateProjectFilesAux(true);

        public static void RegenerateProjectFilesAux(bool showError)
        {
            try
            {
                if (GetCodeEditor(true, out _, out _, out var error))
                {
                    CodeEditor.Editor.CurrentCodeEditor.SyncAll();
                }
                else
                {
                    if (showError && error != null)
                        Serilog.Log.Information(error);

                    // other similar approaches
                    // https://www.reddit.com/r/Unity3D/comments/s1joc6/help_with_generating_csproj_and_sln_for_github/
                    // https://discussions.unity.com/t/manually-generate-sln-and-csproj-files/648686/6
                    // https://discussions.unity.com/t/how-can-i-generate-csproj-files-during-continuous-integration-builds/842493/3
                    // https://github.com/Unity-Technologies/UnityCsReference/blob/f45f297f342239326ea865a57a1bb8ddf93e38c6/Editor/Mono/CodeEditor/SyncVS.cs#L22
                    var t = ScriptFinder.GetType123("Microsoft.Unity.VisualStudio.Editor.Cli");
                    var m = t!.GetMethod("GenerateSolution", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    m.Invoke(null, null);
                }
            }
            catch (System.Exception ex)
            {
                Serilog.Log.Error(ex, "Couldn't invoke GenerateSolution");
                Serilog.Log.Information("Please 'Regenerate project files' manually. 'Edit'->'Preferences'->'External Tools'->'Regenerate project files'");
            }
        }


        static IEnumerator CallShell(string command, string ide)
        {
            Serilog.Log.Debug("calling shell with command: {Command}", command);
            var task = Shell.UnityEditorShell.Execute(command);
            task.OnLog += (logType, log) =>
            {
                Serilog.Log.Debug("shell log: {Log}", log);
            };
            task.OnExit += (code) =>
            {
                Serilog.Log.Debug("shell exit: {Code}", code);
                if (code == 0)
                    Serilog.Log.Information($"{ide} extension installed successfully. Please restart {ide}");
                else
                    Serilog.Log.Information($"{ide} extension installation failed. Please try manual installition at {CommonTools.GetInstallerPath(string.Empty)}");
            };
            yield return new Shell.ShellCommandYieldable(task);
        }

        static void CallVisualStudioInstaller(string vsixPath)
        {
            EditorCoroutines.EditorCoroutineUtility.StartCoroutine(CallShell(
                $"@for /f \"usebackq delims=\" %i in (`\"%ProgramFiles(x86)%\\Microsoft Visual Studio\\Installer\\vswhere.exe\" -latest -prerelease -products * -property enginePath`) do @set enginePath=%i & if exist \"%i\\VSIXInstaller.exe\" call \"%i\\VSIXInstaller.exe\" /u:VSIXLite2.6815b720-6186-48a1-a405-1387e54b41c6 & call \"%i\\VSIXInstaller.exe\" \"{vsixPath}\"", "Visual Studio"), MQTTnetInitializer.Publisher);
        }

        static void CallVSCodeInstaller(string vsixPath)
        {
            string command;
#if UNITY_EDITOR_WIN
            command = $"code --uninstall-extension MerryYellow.uca-lite-vscode & code --install-extension \"{vsixPath}\"";
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            command = $"code --uninstall-extension MerryYellow.uca-lite-vscode ; code --install-extension \"{vsixPath}\"";
#else
            Serilog.Log.Error("invalid platform at {Location}", nameof(CallVSCodeInstaller));
            command = string.Empty;
#endif

            EditorCoroutines.EditorCoroutineUtility.StartCoroutine(CallShell(command, "VS Code"), MQTTnetInitializer.Publisher);
        }

        internal static string Execute(string vsixPath, bool isVisualStudio = false, bool isVSCode = false)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                //startInfo.FileName = GetExePath();
                FileName = vsixPath,
                //startInfo.Arguments = args;
                UseShellExecute = false,
                RedirectStandardOutput = true
                //startInfo.WorkingDirectory = workingDirectoryPath;
            };
            var process = new System.Diagnostics.Process
            {
                StartInfo = startInfo
            };

            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Serilog.Log.Error(ex, "Error at running bat file {File}", vsixPath);
            }

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }


        static IEnumerator SyncAux()
        {
            var clientCount = MQTTnetInitializer.Publisher?.Clients.Count() ?? 0;
            MQTTnetInitializer.Publisher?.SendConnect();
            Serilog.Log.Information("Code Assist is looking for more IDEs to connect to...");

            //yield return new WaitForSeconds(3);
            yield return new EditorCoroutines.EditorWaitForSeconds(3);

            var newClientCount = MQTTnetInitializer.Publisher?.Clients.Count() ?? 0;

            var dif = newClientCount - clientCount;

            if (dif <= 0)
                Serilog.Log.Information("Code Assist couldn't find any new IDE to connect to.");
            else
                Serilog.Log.Information("Code Assist is connected to {Dif} new IDE(s).", dif);
        }

#if MERYEL_DEBUG

        [MenuItem("Code Assist/Binary2Text")]
        static void Binary2Text()
        {
            var filePath = CommonTools.GetInputManagerFilePath();
            var hash = Input.UnityInputManager.GetMD5Hash(filePath);
            var convertedPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"UCA_IM_{hash}.txt");
            
            var b = new Input.Binary2TextExec();
            b.Exec(filePath, convertedPath, detailed: false, largeBinaryHashOnly: false, hexFloat: false);
        }

        [MenuItem("Code Assist/Bump InputManager")]
        static void BumpInputManager()
        {
            Input.InputManagerMonitor.Instance.Bump();
        }


        [MenuItem("Code Assist/Layer Check")]
        static void UpdateLayers()
        {
            var names = UnityEditorInternal.InternalEditorUtility.layers;
            var indices = names.Select(l => LayerMask.NameToLayer(l).ToString()).ToArray();
            MQTTnetInitializer.Publisher?.SendLayers(indices, names);

            var sls = SortingLayer.layers;
            var sortingNames = sls.Select(sl => sl.name).ToArray();
            var sortingIds = sls.Select(sl => sl.id.ToString()).ToArray();
            var sortingValues = sls.Select(sl => sl.value.ToString()).ToArray();

            MQTTnetInitializer.Publisher?.SendSortingLayers(sortingNames, sortingIds, sortingValues);

            /*
            for (var i = 0; i < 32; i++)
            {
                var name = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(name))
                {
                    Debug.Log(i + ":" + name);
                }
            }

            if (ScriptFinder.FindGameObjectOfType("Deneme", out var go))
                MQTTnetInitializer.Publisher.SendGameObject(go);
            */
        }

        [MenuItem("Code Assist/Tag Check")]
        static void UpdateTags()
        {
            Serilog.Log.Debug("Listing tags {Count}", UnityEditorInternal.InternalEditorUtility.tags.Length);

            foreach (var tag in UnityEditorInternal.InternalEditorUtility.tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    Serilog.Log.Debug("{Tag}", tag);
                }
            }

            MQTTnetInitializer.Publisher?.SendTags(UnityEditorInternal.InternalEditorUtility.tags);

        }

        [MenuItem("Code Assist/GO Check")]

        static void TestGO()
        {

            var go = GameObject.Find("Deneme");
            //var go = MonoBehaviour.FindObjectOfType<Deneme>().gameObject;

            MQTTnetInitializer.Publisher?.SendGameObject(go);
        }

        [MenuItem("Code Assist/Undo Records Test")]
        static void UndoTest()
        {
            var undos = new List<string>();
            var redos = new List<string>();

            var type = typeof(Undo);
            System.Reflection.MethodInfo dynMethod = type.GetMethod("GetRecords",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            dynMethod.Invoke(null, new object[] { undos, redos });

            Serilog.Log.Debug("undos:{UndoCount},redos:{RedoCount}", undos.Count, redos.Count);

            var last = undos.LastOrDefault();
            if (last != null)
            {
                Serilog.Log.Debug("last:{Last}", last);
                Serilog.Log.Debug("group:{UndoCurrentGroup},{UndoCurrentGroupName}",
                    Undo.GetCurrentGroup(), Undo.GetCurrentGroupName());
            }
        }


        [MenuItem("Code Assist/Undo List Test")]
        static void Undo2Test()
        {

            //List<string> undoList, out int undoCursor
            var undoList = new List<string>();
            int undoCursor = int.MaxValue;
            var type = typeof(Undo);
            System.Reflection.MethodInfo dynMethod = type.GetMethod("GetUndoList",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            dynMethod = type.GetMethod("GetUndoList",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                null,
                new System.Type[] { typeof(List<string>), typeof(int).MakeByRefType() },
                null);


            dynMethod.Invoke(null, new object[] { undoList, undoCursor });

            Serilog.Log.Debug("undo count: {Count}", undoList.Count);

        }

        [MenuItem("Code Assist/Reload Domain")]
        static void ReloadDomain()
        {
            EditorUtility.RequestScriptReload();
        }

        /*
        [MenuItem("Code Assist/TEST")]
        static void TEST()
        {
            //if (ScriptFinder.FindGameObjectOfType("Deneme_OtherScene", out var go))
            if (ScriptFinder.FindInstanceOfType("Deneme_SO", out var go, out var so))
            {
                MQTTnetInitializer.Publisher.SendScriptableObject(so);
            }

            ScriptFinder.DENEMEEEE();



        }
        */

#endif // MERYEL_DEBUG


        public static void SendTagsAndLayers()
        {
            Serilog.Log.Debug(nameof(SendTagsAndLayers));

            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            MQTTnetInitializer.Publisher?.SendTags(tags);

            var layerNames = UnityEditorInternal.InternalEditorUtility.layers;
            var layerIndices = layerNames.Select(l => LayerMask.NameToLayer(l).ToString()).ToArray();
            MQTTnetInitializer.Publisher?.SendLayers(layerNames, layerIndices);

            var sls = SortingLayer.layers;
            var sortingNames = sls.Select(sl => sl.name).ToArray();
            var sortingIds = sls.Select(sl => sl.id.ToString()).ToArray();
            var sortingValues = sls.Select(sl => sl.value.ToString()).ToArray();
            MQTTnetInitializer.Publisher?.SendSortingLayers(sortingNames, sortingIds, sortingValues);

#if UNITY_6000_0_OR_NEWER
            // Version 6+ only, 2022.3 doesn't have class RenderingLayerMask, even though some renderingLayerMask fields/properties are declared

            var renderingLayerCount = RenderingLayerMask.GetRenderingLayerCount();
            var renderingLayerIndices = new string[renderingLayerCount];
            var renderingLayerNames = new string[renderingLayerCount];
            for (var i = 0; i < renderingLayerCount; i++)
            {
                renderingLayerIndices[i] = i.ToString();
                renderingLayerNames[i] = RenderingLayerMask.RenderingLayerToName(i);
            }
            MQTTnetInitializer.Publisher?.SendRenderingLayers(renderingLayerNames, renderingLayerIndices);

#endif // UNITY_6000_0_OR_NEWER
        }

        public static bool GetCodeEditor(bool checkVersion, out bool isVisualStudio, out bool isVisualStudioCode, out string? error)
        {
            isVisualStudio = false;
            isVisualStudioCode = false;

            if (CodeEditor.Editor.CurrentCodeEditor.TryGetInstallationForPath(CodeEditor.CurrentEditorInstallation, out var installation))
            {
                if (installation.Name.StartsWith("Visual Studio Code"))
                    isVisualStudioCode = true;
                else if (installation.Name.StartsWith("Visual Studio"))
                    isVisualStudio = true;

                if (!isVisualStudioCode && !isVisualStudio)
                {
                    error = $"Unsupported code editor: {installation.Name}. Unity Code Assist only supports Visual Studio and Visual Studio Code";
                    return false;
                }

                if (installation.Name.Contains("(internal)"))
                {
                    error = "Code editor set but not working properly. Please try updating 'Visual Studio Editor' package";
                    return false;
                }

                if (!checkVersion)
                {
                    error = null;
                    return true;
                }

                var versionRegex = new System.Text.RegularExpressions.Regex(".*\\[([\\d\\.]+)\\]");
                var versionStr = versionRegex.Match(installation.Name).Groups.ElementAtOrDefault(1)?.Value;

                if (isVisualStudioCode && !string.IsNullOrEmpty(versionStr) && (VersionCompare(versionStr!, "1.76") < 0))
                {
                    error = $"Version {versionStr} of Visual Studio Code is not supported by Unity Code Assist. Please update Visual Studio Code";
                    return false;
                }

                if (isVisualStudio && !string.IsNullOrEmpty(versionStr) && (VersionCompare(versionStr!, "17") < 0))
                {
                    error = $"Version {versionStr} of Visual Studio is not supported by Unity Code Assist. Please update Visual Studio";
                    return false;
                }

                error = null;
                return true;
            }
            else
            {
                error = "No code editor found. Please set it through 'Edit'->'Preferences'->'External Tools'->'External Script Editor'";
                return false;
            }


        }


        //https://www.geeksforgeeks.org/compare-two-version-numbers/amp/
        public static int VersionCompare(string v1, string v2)
        {
            // vnum stores each numeric

            // part of version

            int vnum1 = 0, vnum2 = 0;

            // loop until both string are
            // processed

            for (int i = 0, j = 0; (i < v1.Length || j < v2.Length);)

            {
                // storing numeric part of
                // version 1 in vnum1
                while (i < v1.Length && v1[i] != '.')
                {

                    vnum1 = vnum1 * 10 + (v1[i] - '0');

                    i++;
                }
                // storing numeric part of

                // version 2 in vnum2

                while (j < v2.Length && v2[j] != '.')
                {
                    vnum2 = vnum2 * 10 + (v2[j] - '0');
                    j++;
                }
                if (vnum1 > vnum2)
                    return 1;

                if (vnum2 > vnum1)
                    return -1;

                // if equal, reset variables and

                // go for next numeric part
                vnum1 = vnum2 = 0;
                i++;
                j++;
            }

            return 0;
        }

    }
}
