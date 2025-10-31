using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{

    // copied from Exporter.cs in VSIX
    public static class Cleanup
    {
        public static bool DoCleanup()
        {
            var assetsPath = UnityEngine.Application.dataPath;

            var _solutionDirectory = CommonTools.GetProjectPath();
            var destination = _solutionDirectory;
            var oldDestination = assetsPath;


            var succeed = true;
            try
            {
                Cleanup1(oldDestination);
                Cleanup2(oldDestination);
                Cleanup3(oldDestination, _solutionDirectory);
                Cleanup4(oldDestination);
            }
            catch (Exception ex)
            {
                succeed = false;
                Serilog.Log.Error(ex, "DoCleanup failed at {Destination}", destination);
            }

            return succeed;
        }


        private static void DeleteFileAndItsMeta(string filePath)
        {
            if (File.Exists(filePath))
            {
                Serilog.Log.Debug("Deleting file {File}", filePath);
                //File.Delete(filePath);
                DeleteFileAux(filePath);
                Serilog.Log.Debug("Deleted file {File} {Exists}", filePath, File.Exists(filePath));
            }
            var metaFilePath = filePath + ".meta";
            if (File.Exists(metaFilePath))
            {
                Serilog.Log.Debug("Deleting meta file {File}", metaFilePath);
                //File.Delete(metaFilePath);
                DeleteFileAux(metaFilePath);
                Serilog.Log.Debug("Deleted file {File} {Exists}", metaFilePath, File.Exists(metaFilePath));
            }
        }

        private static bool IsDirectoryExistsAndEmpty(string path)
        {
            return Directory.Exists(path) && !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private static void DeleteDirectoryAndItsMeta(string directoryPath)
        {
            if (IsDirectoryExistsAndEmpty(directoryPath))
            {
                Serilog.Log.Debug("Deleting directory {Dir}", directoryPath);
                Directory.Delete(directoryPath);
                Serilog.Log.Debug("Deleted directory {Dir} {Exists}", directoryPath, IsDirectoryExistsAndEmpty(directoryPath));

                var metaFilePath = directoryPath + ".meta";
                if (File.Exists(metaFilePath))
                {
                    Serilog.Log.Debug("Deleting directory meta file {File}", metaFilePath);
                    //File.Delete(metaFilePath);
                    DeleteFileAux(metaFilePath);
                    Serilog.Log.Debug("Deleted directory meta file {File} {Exists}", metaFilePath, File.Exists(metaFilePath));
                }
            }
        }

        private static void DeleteFileAux(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                var fileDirectoryPath = Path.GetDirectoryName(filePath);
                SetEveryoneAccessToDirectory(fileDirectoryPath, out _);
                TakeOwnership(filePath);
                File.Delete(filePath);
            }
        }


        /// <summary>
        /// Set Everyone Full Control permissions for selected directory
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        static bool SetEveryoneAccessToDirectory(string dirName, out string _lastError)
        {

            try
            {
                // Make sure directory exists
                if (Directory.Exists(dirName) == false)
                    throw new Exception(string.Format("Directory {0} does not exist, so permissions cannot be set.", dirName));

                // Get directory access info
                DirectoryInfo dinfo = new DirectoryInfo(dirName);
                DirectorySecurity dSecurity = dinfo.GetAccessControl();

                // Add the FileSystemAccessRule to the security settings. 
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                // Set the access control
                dinfo.SetAccessControl(dSecurity);

                _lastError = String.Format("Everyone FullControl Permissions were set for directory {0}", dirName);

                return true;

            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solutionDirectory"></param>
        /// <param name="cs_7_3_orLower"></param>
        /// <param name="cs_8_0"></param>
        /// <param name="cs_9_0_orHigher"></param>
        /// <returns>
        /// 7 if C# 7.3 or lower,
        /// 8 if C# 8.0 (.netstandard2.0),
        /// 9 if C# 9.0 (.netstandard2.1),
        /// -1 if error
        /// </returns>
        public static int GetCSharpVersionFromUnityProjectVersionFile(string? solutionDirectory)
        {
            if (string.IsNullOrEmpty(solutionDirectory))
                return -1;

            var projectVersionFilePath = System.IO.Path.Combine(solutionDirectory, "ProjectSettings/ProjectVersion.txt");
            if (!System.IO.File.Exists(projectVersionFilePath))
                return -1;


            string? version = null;
            string[]? readText = null;
            try
            {
                readText = System.IO.File.ReadAllLines(projectVersionFilePath);
                // format is m_EditorVersion: 2018.2.0b7
                string[] versionText = readText[0].Split(' ');
                version = versionText[1];
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Project version file parsing error {FirstLine}", readText?.FirstOrDefault());
                return -1;
            }

            if (version == null)
            {
                Serilog.Log.Error("Parsed project version is null");
                return -1;
            }

            // see my personal notes for Unity version X C# version table
            // which is at OneNote->ShinSekai->CEPostRelease->UnityCompilerC#VersionTable

            if (version.StartsWith("5.") || version.StartsWith("2017.") ||
                version.StartsWith("2018.") || version.StartsWith("2019.") || version.StartsWith("2020.1."))
                return 7;

            if (version.StartsWith("2020.") || version.StartsWith("2021.1."))
                return 8;

            if (version.StartsWith("2021.") || version.StartsWith("2022.") ||
                version.StartsWith("2023.") || version.StartsWith("6000."))
                return 9;

            Serilog.Log.Error("Parsed project version is unknown {Version}", version);
            return -1;
        }

        private static void TakeOwnership(string filename)
        {
            // Remove read-only attribute
            File.SetAttributes(filename, File.GetAttributes(filename) & ~FileAttributes.ReadOnly);

            FileSecurity security = new FileSecurity();

            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
            security.SetOwner(sid);
            security.SetAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow));

            File.SetAccessControl(filename, security);
        }

        private static void Cleanup1(string destination)
        {
            // prior to version UCA.v.1.1.9, syncronizerModel and yamlDotNet dll files were located at ProjectPath/Assets/Plugins/CodeAssist/Editor/ExternalReferences/Release/netstandard2.0
            // with version UCA.v.1.1.9 and newer versions, they are located at ProjectPath/Assets/Plugins/CodeAssist/Editor/ExternalReferences
            // delete ProjectPath/Assets/Plugins/CodeAssist/Editor/ExternalReferences/Release
            var oldBinariesDirectory = Path.Combine(destination, "Plugins/CodeAssist/Editor/ExternalReferences/Release/netstandard2.0");
            if (Directory.Exists(oldBinariesDirectory))
            {
                // dont just delete the directory for security reasons, instead delete binary files one by one

                Serilog.Log.Debug("Old binaries directory exists at {Location}", oldBinariesDirectory);

                var files = new string[]
                {
                    "UnityCodeAssistSynchronizerModel.deps.json",
                    "UnityCodeAssistSynchronizerModel.dll",
                    "UnityCodeAssistSynchronizerModel.pdb",
                    "UnityCodeAssistYamlDotNet.deps.json",
                    "UnityCodeAssistYamlDotNet.dll",
                    "UnityCodeAssistYamlDotNet.pdb",
                    "UnityCodeAssistYamlDotNet.xml",
                };

                foreach (var file in files)
                {
                    var filePath = Path.Combine(oldBinariesDirectory, file);
                    DeleteFileAndItsMeta(filePath);
                }

                DeleteDirectoryAndItsMeta(oldBinariesDirectory);
            }

            var oldBinariesDirectory2 = Path.Combine(destination, "Plugins/CodeAssist/Editor/ExternalReferences/Release");
            DeleteDirectoryAndItsMeta(oldBinariesDirectory2);


            // also delete old vsix, it's now renamed as Meryel.UnityCodeAssist.VSIX.vsix
            var oldVsixFilePath = Path.Combine(destination, "Plugins/CodeAssist/UnityCodeAssistVSIX.vsix");
            DeleteFileAndItsMeta(oldVsixFilePath);
        }

        private static void Cleanup2(string destination)
        {
            // with version 1.1.12, dll files has been customized (renamed and changed their namespace) (so that they dont conflict with user's other dll files, if he tries to use them)
            // delete AsyncIO.dll, and use Meryel.UnityCodeAssist.AsyncIO.dll instead
            var files = new string[]
            {
                "AsyncIO.dll",
                "NaCl.dll",
                "NetMQ.dll",
                "Serilog.dll",
                "Serilog.Sinks.PersistentFile.dll",
            };

            var binariesDirectory = Path.Combine(destination, "Plugins/CodeAssist/Editor/ExternalReferences");

            foreach (var file in files)
            {
                var filePath = Path.Combine(binariesDirectory, file);
                DeleteFileAndItsMeta(filePath);
            }
        }

        private static void Cleanup3(string destination, string solutionDirectory)
        {
            // as it turns out, .netstandard2.1 does not need system binaries (for C#9.0, unity versions 2021.2 and newer)

            //var cSharpVersion = CommonVS.VSCommonTools.GetCSharpVersionFromUnityProjectVersionFile(solutionDirectory);
            var cSharpVersion = GetCSharpVersionFromUnityProjectVersionFile(solutionDirectory);
            if (cSharpVersion < 9)
                return;

            var systemBinaryFiles = new string[]
            {
                "System.Buffers.dll",
                "System.Memory.dll",
                "System.Runtime.CompilerServices.Unsafe.dll",
                "System.Threading.Tasks.Extensions.dll",
            };

            var binariesDirectory = Path.Combine(destination, "Plugins/CodeAssist/Editor/ExternalReferences");

            foreach (var file in systemBinaryFiles)
            {
                var filePath = Path.Combine(binariesDirectory, file);
                DeleteFileAndItsMeta(filePath);
            }

        }

        private static void Cleanup4(string destination)
        {
            // with version 1.2, asset directory moved from Assets/Plugins/CodeAssist to Packages/com.merry-yellow.code-assist
            // so remove all files from the old directory

            var directory = Path.Combine(destination, "Plugins/CodeAssist/Editor");

            var content = new string[]
            {
@"TinyJson/JsonWriter.cs",
@"TinyJson/JsonParser.cs",
@"Preferences/RegistryMonitor.cs",
@"Preferences/PreferenceStorageAccessor.cs",
@"Preferences/PreferenceMonitor.cs",
@"Preferences/PreferenceEntryHolder.cs",
@"Logger/UnitySink.cs",
@"Logger/MemorySink.cs",
@"Logger/ELogger.cs",
@"Logger/DomainHashEnricher.cs",
@"Logger/CommonTools.cs",
@"Logger/Attributes.cs",
@"Input/UnityInputManager.cs",
@"Input/Text2Yaml.cs",
@"Input/InputManagerMonitor.cs",
@"Input/Binary2TextExec.cs",
@"ExternalReferences/Meryel.UnityCodeAssist.YamlDotNet.xml",
@"ExternalReferences/Meryel.UnityCodeAssist.YamlDotNet.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.YamlDotNet.dll",
@"ExternalReferences/Meryel.UnityCodeAssist.YamlDotNet.deps.json",
@"ExternalReferences/Meryel.UnityCodeAssist.SynchronizerModel.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.SynchronizerModel.dll",
@"ExternalReferences/Meryel.UnityCodeAssist.SynchronizerModel.deps.json",
@"ExternalReferences/Meryel.UnityCodeAssist.Serilog.xml",
@"ExternalReferences/Meryel.UnityCodeAssist.Serilog.Sinks.PersistentFile.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.Serilog.Sinks.PersistentFile.dll",
@"ExternalReferences/Meryel.UnityCodeAssist.Serilog.Sinks.PersistentFile.deps.json",
@"ExternalReferences/Meryel.UnityCodeAssist.Serilog.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.Serilog.dll",
@"ExternalReferences/Meryel.UnityCodeAssist.NetMQ.xml",
@"ExternalReferences/Meryel.UnityCodeAssist.NetMQ.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.NetMQ.dll",
@"ExternalReferences/Meryel.UnityCodeAssist.NetMQ.deps.json",
@"ExternalReferences/Meryel.UnityCodeAssist.NaCl.xml",
@"ExternalReferences/Meryel.UnityCodeAssist.NaCl.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.NaCl.dll",
@"ExternalReferences/Meryel.UnityCodeAssist.AsyncIO.pdb",
@"ExternalReferences/Meryel.UnityCodeAssist.AsyncIO.dll",
@"EditorCoroutines/EditorWindowCoroutineExtension.cs",
@"EditorCoroutines/EditorWaitForSeconds.cs",
@"EditorCoroutines/EditorCoroutineUtility.cs",
@"EditorCoroutines/EditorCoroutine.cs",
@"UnityClassExtensions.cs",
@"StatusWindow.cs",
@"ScriptFinder.cs",
@"NetMQPublisher.cs",
@"NetMQInitializer.cs",
@"Monitor.cs",
@"MerryYellow.CodeAssist.Editor.asmdef",
@"MainThreadDispatcher.cs",
@"LazyInitializer.cs",
@"FeedbackWindow.cs",
@"Assister.cs",
@"AboutWindow.cs",
//@"TinyJson",
//@"Preferences",
//@"Logger",
//@"Input",
//@"ExternalReferences",
//@"EditorCoroutines",
            };

            foreach (var c in content)
            {
                var path = Path.Combine(directory, c);
                DeleteFileAndItsMeta(path);
            }
        }

    }
}