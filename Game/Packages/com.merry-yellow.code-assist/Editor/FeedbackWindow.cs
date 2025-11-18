using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{
    public class FeedbackWindow : EditorWindow
    {

        GUIStyle? styleLabel;

        public static void Display()
        {
            MQTTnetInitializer.Publisher?.SendRequestInternalLog();

            // Get existing open window or if none, make a new one:
            var window = GetWindow<FeedbackWindow>();
            window.Show();

            Serilog.Log.Debug("Displaying feedback window");

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "FeedbackWindow_Display");
        }


        private void OnEnable()
        {
            //**--icon
            //var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/Gear.png");
            //titleContent = new GUIContent("Code Assist", icon);
            titleContent = new GUIContent("Code Assist Feedback");
        }

        private void OnGUI()
        {
            var errorCount = Logger.ELogger.GetErrorCountInInternalLog();
            var warningCount = Logger.ELogger.GetWarningCountInInternalLog();
            var logContent = Logger.ELogger.GetInternalLogContent();
            if (!string.IsNullOrEmpty(Logger.ELogger.VsInternalLog))
                logContent += Logger.ELogger.VsInternalLog;

            styleLabel ??= new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleCenter,
            };
            
            if (errorCount > 0)
                EditorGUILayout.LabelField($"{errorCount} error(s) found in logs. Please submit a feedback (via e-mail, Discord or GitHub) with the logs if possible.", styleLabel, GUILayout.ExpandWidth(true));
            else if (warningCount > 0)
                EditorGUILayout.LabelField($"{warningCount} warnings(s) found in logs. Please submit a feedback (via e-mail, Discord or GitHub) with the logs if possible.", styleLabel, GUILayout.ExpandWidth(true));
            else
                EditorGUILayout.LabelField("No errors found in logs. Please submit a feedback (via e-mail, Discord or GitHub) describing what went wrong or unexpected.", styleLabel, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Send e-mail"))
            {
                var uri = "mailto:merryyellow@outlook.com";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri));
            }

            if (GUILayout.Button("Send Discord message"))
            {
                //var uri = "discord://invites/2CgKHDq";
                var uri = "https://discord.gg/2CgKHDq";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri));
            }

            if (GUILayout.Button("Submit GitHub issue"))
            {
                var uri = "https://github.com/merryyellow/Unity-Code-Assist/issues/new/choose";
                Application.OpenURL(uri);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Open Unity full log"))
            {
                var filePath = Logger.ELogger.FilePath;
                System.Diagnostics.Process.Start(filePath);
            }
            if (GUILayout.Button("Reveal Unity full log"))
            {
                var filePath = Logger.ELogger.FilePath;
                ShowInFileExplorer(filePath);
            }

            if (GUILayout.Button("Open Visual Studio full log"))
            {
                var filePath = Logger.ELogger.VSFilePath;
                System.Diagnostics.Process.Start(filePath);
            }
            if (GUILayout.Button("Reveal Visual Studio full log"))
            {
                var filePath = Logger.ELogger.VSFilePath;
                ShowInFileExplorer(filePath);
            }

            if (GUILayout.Button("Copy recent logs to clipboard"))
            {
                GUIUtility.systemCopyBuffer = logContent;
            }

            EditorGUILayout.LabelField("Recent logs:", styleLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.SelectableLabel(logContent, EditorStyles.textArea, GUILayout.ExpandHeight(true));
        }

        public static void ShowInFileExplorer(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Serilog.Log.Error("Argument {Arg} is null or empty at {Location}", nameof(filePath), nameof(ShowInFileExplorer));
                return;
            }

            filePath = System.IO.Path.GetFullPath(filePath);

            if (!System.IO.File.Exists(filePath) && !System.IO.Directory.Exists(filePath))
            {
                Serilog.Log.Error("Argument {Arg} is not found at {Location}, value: {Value}", nameof(filePath), nameof(ShowInFileExplorer), filePath);
                return;
            }

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                // Windows: highlight the file in Explorer
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                // macOS: reveal in Finder
                System.Diagnostics.Process.Start("open", $"-R \"{filePath}\"");
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                // Linux: open containing folder in a common file manager
                string? directory = System.IO.Path.GetDirectoryName(filePath);
                if (directory == null)
                    return;

                string[] managers = { "xdg-open", "nautilus", "dolphin", "nemo", "thunar" };

                foreach (string manager in managers)
                {
                    if (TryStart(manager, directory))
                        return; // success
                }

                Serilog.Log.Error("No supported file manager found to open the directory at {Location}", nameof(ShowInFileExplorer));
            }
            else
            {
                Serilog.Log.Error("Unsupported OS platform at {Location}", nameof(ShowInFileExplorer));
            }

            // static local function for cleaner structure
            static bool TryStart(string command, string args)
            {
                try
                {
                    System.Diagnostics.Process.Start(command, $"\"{args}\"");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

    }
}