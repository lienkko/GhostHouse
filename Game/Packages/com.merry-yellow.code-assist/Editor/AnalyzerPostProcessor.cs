using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{
    // according to documentation, https://docs.unity3d.com/2023.2/Documentation/Manual/roslyn-analyzers.html
    // if analyzers are under any asmdef file, they are bound to the asmdef's scope
    // to declare out analyzers globally, had to write custom AssetPostprocessor and don't use "RoslynAnalyzer" asset label

    public class AnalyzerPostProcessor : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            // do not add roslyn analyzers to Visual Studio projects for performance
            if (Assister.GetCodeEditor(false, out var isVisualStudio, out _, out _) && isVisualStudio)
                return content;

            var analyzerGroup = new StringBuilder();
            analyzerGroup.Append(NewLine);
            analyzerGroup.Append("  <!--This section is added by Unity Code Assist-->");
            analyzerGroup.Append(NewLine);
            analyzerGroup.Append("  <ItemGroup>");

            var prefix = $"{NewLine}    <Analyzer Include=\"{CommonTools.GetExternalReferencesPath().Replace('/', '\\')}\\";
            var suffix = $"\" />";

            foreach (var analyzer in Analyzers)
            {
                analyzerGroup.Append(prefix);
                analyzerGroup.Append(analyzer);
                analyzerGroup.Append(suffix);
            }

            analyzerGroup.Append(NewLine);
            analyzerGroup.Append("  </ItemGroup>");
            //analyzerGroup.Append(NewLine);


            //content = Regex.Replace(content, $"[{NewLine}]*</Project>[{NewLine}]*", $"{analyzerGroup.ToString()}$&");
            var matches = Regex.Matches(content, $"[{NewLine}]*</Project>");
            var index = matches.LastOrDefault(m => m.Success)?.Index ?? -1;
            //var index = content.LastIndexOf("</Project>");

            if (index >= 0)
                content = content.Insert(index, analyzerGroup.ToString());

            return content;
        }

        private const string NewLine = "\r\n";

        private readonly static string[] Analyzers = new string[]
        {
#if MERYEL_UCA_LITE_VERSION
            "Meryel.UnityCodeAssist.AnalyzersLite.dll",
#else
            "Meryel.UnityCodeAssist.Analyzers.dll",
#endif
            "Meryel.UnityCodeAssist.Common.dll",
            "Meryel.UnityCodeAssist.Completion.dll",
            "Meryel.UnityCodeAssist.CompletionInternals.dll",
            "Meryel.UnityCodeAssist.Logger.dll",
            "Meryel.UnityCodeAssist.MQTTnet.dll",
            "Meryel.UnityCodeAssist.MQTTnet.Extensions.ManagedClient.dll",
            "Meryel.UnityCodeAssist.Newtonsoft.Json.dll",
            "Meryel.UnityCodeAssist.ProjectData.dll",
            "Meryel.UnityCodeAssist.RoslynAnalyzerManager.dll",
            "Meryel.UnityCodeAssist.Synchronizer.dll",
            "Meryel.UnityCodeAssist.SynchronizerModel.dll",
            "Meryel.UnityCodeAssist.VSIXLibrary.dll",
        };
    }
}
