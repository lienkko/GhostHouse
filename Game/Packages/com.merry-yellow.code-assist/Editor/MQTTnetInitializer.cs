using System;
using UnityEditor;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
using MQTTnet = Meryel.UnityCodeAssist.MQTTnet;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{
    //[InitializeOnLoad]
    public static class MQTTnetInitializer
    {
        public static MQTTnetPublisher? Publisher;

        static MQTTnetInitializer()
        {
            EditorApplication.quitting += EditorApplication_quitting;
            AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEvents_beforeAssemblyReload;
            //AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEvents_afterAssemblyReload;

            RunOnceOnUpdate(Initialize);
        }

        /// <summary>
        /// Empty method for invoking static class ctor
        /// </summary>
        public static void Bump() { }

        /// <summary>
        /// false for profiler standalone process
        /// </summary>
        /// <returns></returns>
        public static bool IsMainUnityEditorProcess()
        {
#if UNITY_2020_2_OR_NEWER
            if (UnityEditor.AssetDatabase.IsAssetImportWorkerProcess())
                return false;
#elif UNITY_2019_3_OR_NEWER
			if (UnityEditor.Experimental.AssetDatabaseExperimental.IsAssetImportWorkerProcess())
				return false;
#endif

#if UNITY_2021_1_OR_NEWER
            if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Secondary)
                return false;
#elif UNITY_2020_2_OR_NEWER
			if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Slave)
				return false;
#elif UNITY_2020_1_OR_NEWER
			if (global::Unity.MPE.ProcessService.level == global::Unity.MPE.ProcessLevel.UMP_SLAVE)
				return false;
#endif

            return true;
        }

        public static void Initialize()
        {
            if (!IsMainUnityEditorProcess())
            {
                // if try to creaate NetMQ, will recieve AddressAlreadyInUseException during binding
                Serilog.Log.Debug("MQTTnet won't initialize on secondary processes");
                return;
            }

            Serilog.Log.Debug("MQTTnet initializing");

            //Serilog.Log.Debug("MQTTnet constructing");
            Publisher = new MQTTnetPublisher();
            
            RunOnShutdown(OnShutDown);
            Serilog.Log.Debug("MQTTnet initialized");
        }

        private static void OnShutDown()
        {
            Serilog.Log.Debug("MQTTnet OnShutDown");
            Clear();
        }

        //private static void AssemblyReloadEvents_afterAssemblyReload()
        //{
        //    Serilog.Log.Debug("MQTTnet AssemblyReloadEvents_afterAssemblyReload");
        //}

        private static void AssemblyReloadEvents_beforeAssemblyReload()
        {
            Serilog.Log.Debug("MQTTnet AssemblyReloadEvents_beforeAssemblyReload");

            Clear();
        }

        private static void EditorApplication_quitting()
        {
            Serilog.Log.Debug("MQTTnet EditorApplication_quitting");

            Publisher?.SendDisconnect();
            Clear();
        }

        static void Clear() => Publisher?.Clear();


        private static void RunOnceOnUpdate(Action action)
        {
            void callback()
            {
                EditorApplication.update -= callback;
                action();
            }

            EditorApplication.update += callback;
        }

        private static void RunOnShutdown(Action action)
        {
            // Mono on OSX has all kinds of quirks on AppDomain shutdown
            //if (!VisualStudioEditor.IsWindows)
            //return;
#if !UNITY_EDITOR_WIN
            return;
#else
            AppDomain.CurrentDomain.DomainUnload += (_, __) => action();
#endif

        }
    }
}
