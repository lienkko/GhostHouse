using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.IO.Path;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{
    public static class CommonTools
    {
        public static string GetScriptPath(string script)
        {
            var projectPath = GetProjectPathRaw();
            var toolPath = Combine(projectPath, "Packages/com.merry-yellow.code-assist/Editor/", script);
            return toolPath;
        }

        /// <summary>
        /// does NOT include the trailing slash
        /// </summary>
        /// <returns></returns>
        public static string GetExternalReferencesPath()
        {
            var projectPath = GetProjectPathRaw();
            var extRefPath = Combine(projectPath, "Packages/com.merry-yellow.code-assist/Editor/ExternalReferences");
            return extRefPath;
        }

        [Obsolete]
        public static string GetToolPath(string tool)
        {
            var projectPath = GetProjectPathRaw();
            var toolPath = Combine(projectPath, "Packages/com.merry-yellow.code-assist/Tools~/", tool);
            return toolPath;
        }

        public static string GetInstallerPath(string installer)
        {
            var projectPath = GetProjectPathRaw();
            var installerPath = Combine(projectPath, "Packages/com.merry-yellow.code-assist/Installers~/", installer);
            return installerPath;
        }

        public static string GetTagManagerFilePath()
        {
            var projectPath = GetProjectPathRaw();
            var tagManagerPath = Combine(projectPath, "ProjectSettings/TagManager.asset");
            return tagManagerPath;
        }

        public static string GetInputManagerFilePath()
        {
            var projectPath = GetProjectPathRaw();
            var inputManagerPath = Combine(projectPath, "ProjectSettings/InputManager.asset");
            return inputManagerPath;
        }

        public static string GetProjectPath()
        {
            var rawPath = GetProjectPathRaw();
            //var pathWithoutWhiteSpace = rawPath.Trim(); // this is done in OSPath ctor
            var osPath = new OSPath(rawPath);
            var unixPath = osPath.Unix;
            var trimmed = unixPath.TrimEnd('\\', '/');
            var capitalized = FirstCharToUpper(trimmed); // this is required for TypeScript, so doing it here as well just in case
            return capitalized!;
        }

        static string? FirstCharToUpper(string? input)
        {
            switch (input)
            {
                case null: return null;
                case "": return "";
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }

        /// <summary>
        /// Get the path to the project folder.
        /// </summary>
        /// <returns>The project folder path</returns>
        static string GetProjectPathRaw()
        {
            // Application.dataPath returns the path including /Assets, which we need to strip off
            var path = UnityEngine.Application.dataPath;
            var directory = new DirectoryInfo(path);
            var parent = directory.Parent;
            if (parent != null)
                return parent.FullName;

            return path;
        }

        public static string GetHashForLogFile(string path) => Synchronizer.Model.Utilities.GetHashForLogFile(path);
    }

    // https://github.com/dmitrynogin/cdsf/blob/master/Cds.Folders/OSPath.cs
    internal class OSPath
    {
        public static readonly OSPath Empty = "";

        public static bool IsWindows => DirectorySeparatorChar == '\\';

        public OSPath(string text)
        {
            Text = text.Trim();
        }

        public static implicit operator OSPath(string text) => new OSPath(text);
        public static implicit operator string(OSPath path) => path.Normalized;
        public override string ToString() => Normalized;

        protected string Text { get; }

        public string Normalized => IsWindows ? Windows : Unix;
        public string Windows => Text.Replace('/', '\\');
        //public string Unix => Simplified.Text.Replace('\\', '/');
        public string Unix => Text.Replace('\\', '/');

        public OSPath Relative => Simplified.Text.TrimStart('/', '\\');
        public OSPath Absolute => IsAbsolute ? this : "/" + Relative;

        public bool IsAbsolute => IsRooted || HasVolume;
        public bool IsRooted => Text.Length >= 1 && (Text[0] == '/' || Text[0] == '\\');
        public bool HasVolume => Text.Length >= 2 && Text[1] == ':';
        public OSPath Simplified => HasVolume ? Text.Substring(2) : Text;

        public OSPath Parent => GetDirectoryName(Text);

        public bool Contains(OSPath path) =>
            Normalized.StartsWith(path);

        public static OSPath operator +(OSPath left, OSPath right) =>
            new OSPath(Combine(left, right.Relative));

        public static OSPath operator -(OSPath left, OSPath right) =>
            left.Contains(right)
            ? new OSPath(left.Normalized.Substring(right.Normalized.Length)).Relative
            : left;
    }
}
