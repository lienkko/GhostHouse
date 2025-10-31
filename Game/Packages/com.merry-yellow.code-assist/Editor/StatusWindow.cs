using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{
    public class StatusWindow : EditorWindow
    {
        GUIStyle? styleLabel;

        public static void Display()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<StatusWindow>();
            window.Show();

            MQTTnetInitializer.Publisher?.SendConnectionInfo();

            Serilog.Log.Debug("Displaying status window");

            MQTTnetInitializer.Publisher?.SendAnalyticsEvent("Gui", "StatusWindow_Display");
        }

        private void OnEnable()
        {
            //**--icon
            //var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/Gear.png");
            //titleContent = new GUIContent("Code Assist", icon);
            titleContent = new GUIContent(Assister.Title);
        }

        private void OnGUI()
        {
            var hasAnyClient = MQTTnetInitializer.Publisher?.Clients.Any() == true;

            styleLabel ??= new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
            };

            if (hasAnyClient)
            {
                EditorGUILayout.LabelField($"Code Assist is working!", styleLabel, GUILayout.ExpandWidth(true));

                foreach (var client in MQTTnetInitializer.Publisher!.Clients)
                {
                    EditorGUILayout.LabelField($"Connected to {client.ContactInfo}", styleLabel, GUILayout.ExpandWidth(true));
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Code Assist isn't working!", styleLabel, GUILayout.ExpandWidth(true));

                EditorGUILayout.LabelField($"No IDE found", styleLabel, GUILayout.ExpandWidth(true));
            }

#if MERYEL_UCA_LITE_VERSION

            EditorGUILayout.LabelField($"", styleLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField($"This is the lite version of Code Assist with limited features.", styleLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField($"To unlock all of the features, get the full version.", styleLabel, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Get full version"))
            {
                Application.OpenURL("https://unitycodeassist.netlify.app/purchase?utm_source=unity_getfullbutton");
            }

#endif // MERYEL_UCA_LITE_VERSION


        }
    }

}