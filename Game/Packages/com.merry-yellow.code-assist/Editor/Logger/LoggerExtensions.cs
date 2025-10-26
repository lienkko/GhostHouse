//using Meryel.UnityCodeAssist.Serilog;
//using Meryel.UnityCodeAssist.Serilog.Core;
using UnityEngine;
using UnityEditor;
using System.Linq;

using Meryel.Serilog;
using Meryel.Serilog.Core;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor.Logger
{

    //[InitializeOnLoad]
    public static class ELogger
    {
        public static event System.Action? OnVsInternalLogChanged;


        // Change 'new LoggerConfiguration().MinimumLevel.Debug();' if you change these values
        const Serilog.Events.LogEventLevel fileMinLevel = Serilog.Events.LogEventLevel.Debug;
        const Serilog.Events.LogEventLevel outputWindowMinLevel = Serilog.Events.LogEventLevel.Information;
        static LoggingLevelSwitch? fileLevelSwitch, outputWindowLevelSwitch;

        //static bool IsInitialized { get; set; }

        static ILogEventSink? _outputWindowSink;
        static ILogEventSink? _memorySink;


        public static string GetInternalLogContent() => _memorySink == null ? string.Empty : ((Meryel.UnityCodeAssist.Logger.MemorySink)_memorySink).Export();
        public static int GetErrorCountInInternalLog() => _memorySink == null ? 0 : ((Meryel.UnityCodeAssist.Logger.MemorySink)_memorySink).ErrorCount;
        public static int GetWarningCountInInternalLog() => _memorySink == null ? 0 : ((Meryel.UnityCodeAssist.Logger.MemorySink)_memorySink).WarningCount;

        public static string? FilePath => Meryel.UnityCodeAssist.Logger.ELogger.UnityFilePath;
        public static string? VSFilePath => Meryel.UnityCodeAssist.Logger.ELogger.VisualStudioFilePath;

        //**-- make it work with multiple clients
        static string? _vsInternalLog;
        public static string? VsInternalLog
        {
            get => _vsInternalLog;
            set
            {
                _vsInternalLog = value;
                OnVsInternalLogChanged?.Invoke();
            }
        }



        static ELogger()
        {
            fileLevelSwitch = null;
            outputWindowLevelSwitch = null;
            _memorySink = null;

            var isFirst = false;
            const string stateName = "isFirst";
            if (!SessionState.GetBool(stateName, false))
            {
                isFirst = true;
                SessionState.SetBool(stateName, true);
            }

            var projectPath = CommonTools.GetProjectPath();
            var outputWindowSink = new System.Lazy<ILogEventSink>(() => new UnityOutputWindowSink(null));

            Init(isFirst, projectPath, outputWindowSink);

            if (isFirst)
                LogHeader(Application.unityVersion, projectPath);
        }

        /// <summary>
        /// Empty method for invoking static class ctor
        /// </summary>
        public static void Bump() { }


        static void LogHeader(string unityVersion, string solutionDir)
        {
            var os = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            var assisterVersion = Assister.Version;
            var syncModel = Synchronizer.Model.Utilities.Version;
            var hash = CommonTools.GetHashForLogFile(solutionDir);
            var port = Synchronizer.Model.Utilities.GetPortForMQTTnet(solutionDir);
            Serilog.Log.Debug(
                "Beginning logging {OS}, Unity {U}, Unity Code Assist {A}, Communication Protocol {SM}, Project: '{Dir}', Project Hash: {Hash}, Port: {Port}",
                os, unityVersion, assisterVersion, syncModel, solutionDir, hash, port);
        }


        public static void Init(bool isFirst, string solutionDir, System.Lazy<ILogEventSink> outputWindowSink)
        {
            //var solutionHash = Common.CommonTools.GetHashOfPath(solutionDir);
            var solutionHash = CommonTools.GetHashForLogFile(solutionDir); // dir is osSafePath
            _outputWindowSink ??= outputWindowSink.Value;
            var sinkWrapper = new System.Lazy<Meryel.Serilog.Core.ILogEventSink>(() => _outputWindowSink);

            Meryel.UnityCodeAssist.Logger.ELogger.Init(
                UnityCodeAssist.Logger.ELogger.State.FullyInitialized,
                UnityCodeAssist.Logger.ELogger.PackagePriority.High,
                solutionDir, solutionHash, "UnityCodeAssist", ProjectData.Domain.Unity,
                sinkWrapper, null, null, null, null);
        }

        public static void OnOptionsChanged()
        {
            // Since we don't use LogEventLevel.Fatal, we can use it for disabling sinks

            var isLoggingToFile = OptionsIsLoggingToFile;
            var targetFileLevel = isLoggingToFile ? fileMinLevel : Serilog.Events.LogEventLevel.Fatal;
            if (fileLevelSwitch != null)
                fileLevelSwitch.MinimumLevel = targetFileLevel;

            var isLoggingToOutputWindow = OptionsIsLoggingToOutputWindow;
            var targetOutputWindowLevel = isLoggingToOutputWindow ? outputWindowMinLevel : Serilog.Events.LogEventLevel.Fatal;
            if (outputWindowLevelSwitch != null)
                outputWindowLevelSwitch.MinimumLevel = targetOutputWindowLevel;
        }

        //**-- UI for these two
        static bool OptionsIsLoggingToFile => true;
        static bool OptionsIsLoggingToOutputWindow => true;
    }

}

