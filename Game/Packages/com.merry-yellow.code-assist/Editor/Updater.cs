using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;

#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor
{

    public static class Updater
    {
        const string EditorPrefsKey = "my_uca_update_disabled";
        const string SessionStateKey = "my_uca_update_checked_before";
        const string ItchApiUri = "https://itch.io/api/1/x/wharf/latest?target=meryel/unity-code-assist&channel_name=asset";
        const string ItchStoreUri = "https://meryel.itch.io/unity-code-assist";
        const string AssetStoreUri = "https://assetstore.unity.com/packages/tools/utilities/code-assist-216713";
        const string VSMarketplaceUri = "https://marketplace.visualstudio.com/items?itemName=MerryYellow.UCA-Lite";
        const string VSCodeMarketplaceUri = "https://marketplace.visualstudio.com/items?itemName=MerryYellow.uca-lite-vscode";
        

#pragma warning disable CS0162
        public static void CheckUpdateSilent()
        {
#if !MERYEL_UCA_ITCH_BUILD
            return;
#endif

            var updateDisabled = EditorPrefs.GetBool(EditorPrefsKey, false);
            if (updateDisabled)
                return;

            var updateCheckedBefore = SessionState.GetBool(SessionStateKey, false);
            if (updateCheckedBefore)
                return;

            EditorCoroutines.EditorCoroutineUtility.StartCoroutine(GetRequest(ItchApiUri, false), MQTTnetInitializer.Publisher);
        }
#pragma warning restore CS0162

        public static void CheckUpdateForced()
        {
            EditorCoroutines.EditorCoroutineUtility.StartCoroutine(GetRequest(ItchApiUri, true), MQTTnetInitializer.Publisher);
        }


        static void DisplayDialog()
        {
            SessionState.SetBool(SessionStateKey, true);

            int option = EditorUtility.DisplayDialogComplex(
                "Update Unity Code Assist",
                "Do you want to update asset: Unity Code Assist?",
                "Update", //"Save",
                "Cancel",
                "Never ask again"); //"Don't Save");

            switch (option)
            {
                // update - Save.
                case 0:
#if MERYEL_UCA_ITCH_BUILD
                    Application.OpenURL(ItchStoreUri);
#endif
#if MERYEL_UCA_ASSET_STORE_BUILD
                    Application.OpenURL(AssetStoreUri);
                    Serilog.Log.Information("It may take a few days for the Asset Store team to review updates. Thank you for your patience.");
#endif
#if MERYEL_UCA_VS_MARKETPLACE_BUILD
                    Application.OpenURL(VSMarketplaceUri);
#endif
#if MERYEL_UCA_VSC_MARKETPLACE_BUILD
                    Application.OpenURL(VSCodeMarketplaceUri);
#endif
                    break;

                // Cancel.
                case 1:
                    // do nothing
                    break;

                // never ask again - Don't Save.
                case 2:
                    EditorPrefs.SetBool(EditorPrefsKey, true);
                    break;

                default:
                    Serilog.Log.Error("Unrecognized option for {Location}.", nameof(DisplayDialog));
                    break;
            }

        }

        static void Compare(string response, bool isForced)
        {
            var regex = new System.Text.RegularExpressions.Regex("\\d+(\\.\\d+){2,4}");
            var match = regex.Match(response);
            if (!match.Success)
            {
                Serilog.Log.Error("Invalid update response, couldn't parse: {Response}", response);
                return;
            }

            var version = match.Value;

            if (Assister.VersionCompare(Assister.Version, version) >= 0)
            {
                if (isForced)
                    Serilog.Log.Information("Unity Code Assist is up to date, version: {Version}", Assister.Version);
                return;
            }

            DisplayDialog();
        }

        static IEnumerator GetRequest(string uri, bool isForced)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Serilog.Log.Error("Error while checking new version of UnityCodeAssist " + pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Serilog.Log.Error("Error while checking new version of UnityCodeAssist " + pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Serilog.Log.Debug("Checking new version of UnityCodeAssist " + pages[page] + ": Received: " + webRequest.downloadHandler.text);
                        Compare(webRequest.downloadHandler.text, isForced);
                        break;
                }
            }
        }
    }
}