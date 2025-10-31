using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEditor;


#pragma warning disable IDE0005
//using Serilog = Meryel.UnityCodeAssist.Serilog;
using Serilog = Meryel.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor.Setup
{

#if !MERYEL_UCA_LITE_VERSION
    [InitializeOnLoad]
    public static class SetupManager
    {
        static SetupManager()
        {
            //var cleanupPath = CommonTools.GetToolPath("CleanupObsoleteFiles.bat");
            //Assister.Execute(cleanupPath);
			Cleanup.DoCleanup();

            //var installerPath = CommonTools.GetToolPath("InstallFullVersionOfVsix.bat");
            //Assister.Execute(installerPath);
            Assister.Upgrade();

            // delete itself (file), so these cleanup and install only called once
            var scriptMeta = CommonTools.GetScriptPath("SetupManager.cs.meta");
            System.IO.File.Delete(scriptMeta);
            var script = CommonTools.GetScriptPath("SetupManager.cs");
            System.IO.File.Delete(script);
        }

    }
#endif
}