Code Assist improves your coding experience with Visual Studio and VS Code. Coding for Unity projects becomes easier and more efficient with fewer bugs.
Connect your game editor and IDE, let Unity and Visual Studio/VS Code share data between each other. Enrich your coding experience with the new data available.



Contact:
merryyellow@outlook.com

Website:
https://unitycodeassist.netlify.app/

Discord:
https://discord.gg/2CgKHDq

Forum:
https://forum.unity.com/threads/code-assist-empower-your-ide.1384731/

GitHub:
https://github.com/merryyellow/Unity-Code-Assist

Asset Store:
http://u3d.as/2N2H

itch.io:
https://meryel.itch.io/unity-code-assist

Visual Studio Marketplace:
https://marketplace.visualstudio.com/items?itemName=MerryYellow.UCA-Lite

VS Code Marketplace:
https://marketplace.visualstudio.com/items?itemName=MerryYellow.uca-lite-vscode



Restrictions:
* Unity 2021.2 and newer.
* Visual Studio 2022 or VS Code (1.75+). Does not work with Visual Studio for Mac, Rider or any other IDE or text editor.



How to Setup:
* Import the Unity asset with the package manager into your Unity project
* In Unity, from top menu bar, select "Tools"->"Code Assist"->"Setup"->"Upgrade to full version" (You can skip this step if you have installed it before for another project). This will uninstall the lite version of extension from Visual Studio/VS Code, and then install full version of extension to Visual Studio/VS Code. To install the extension manually, see "YOUR_PROJECT_PATH/Packages/com.merry-yellow.code-assist/Installers~/Extension Installation.md"
* Open your Unity project with Visual Studio/VS Code. Code Assist loads in background with low priority, so it will be ready in a minute when Visual Studio/VS Code first starts up.
* To check if setup is successfull, in Unity, from top menu bar choose "Tools"->"Code Assist"->"Status". The windows should display "Code Assist is working!".



How to Use:
Code Assist is always active while you are coding. The following samples will give you a glimpse of Code Assist.
* Code Completion: In a class which derives MonoBehaviour, type "CompareTag(". When you press double quotes("), code completion popup should appear with the available tags of the scene.
* Inline Visuals: In a class which derives MonoBehaviour, type "gameObject.layer = 0;" and save the document. "Default" text should appear on top or right of "0".
* Code Analyzers: Define a Vector3 (let's say vec3). When used like "if (vec3.magnitude < 10)", a warning will appear on that line. Press Alt+Enter to convert it to "if (vec3.sqrMagnitude < 10 * 10)", which has better performance.



Generative AI:
Generative AI is working only in Visual Studio (for now). It does not work in VS Code.
Generative AI is one of the features of Code Assist. If you have any ethical or legal concerns, you can use Code Assist without this feature by disabling generative AI in the options.

To use generative AI with your comments:
* Type "//gpt "
* Type your prompt
* Press Enter key

To use generative AI with CodeLens:
* Click Gpt Codelens which is above class or method declaration

Requirements (OpenAI ChatGPT): You need an OpenAI account and provide it's API key to this asset.
https://platform.openai.com/account/api-keys

Provide key by one of the below
* Create an environment variable called OPENAI_API_KEY with the API key
* Create a file with name .openai under user profile directory with the API key
* Set API key in Visual Studio options, Tools->Options->Unity Code Assist->Generative AI->Authentication

Requirements (Google Gemini): You need a Google account and provide it's API key to this asset.
https://ai.google.dev/gemini-api/docs/api-key

Provide key by one of the below
* Create an environment variable called GOOGLE_API_KEY with the API key
* Create a file with name .gemini under user profile directory with the API key
* Set API key in options, Tools->Options->Unity Code Assist->Generative AI->Authentication

Requirements (Anthropic Claude): You need an Anthropic account and provide it's API key to this asset.
https://console.anthropic.com/settings/keys

Provide key by one of the below
* Create an environment variable called ANTHROPIC_API_KEY with the API key
* Create a file with name .anthropic under user profile directory with the API key
* Set API key in options, Tools->Options->Comment2GPT->Authentication

Requirements (DeepSeek): You need a DeepSeek account and provide it's API key to this asset.
https://platform.deepseek.com/api_keys

Provide key by one of the below
* Create an environment variable called DEEPSEEK_API_KEY with the API key
* Create a file with name .deepseek under user profile directory with the API key
* Set API key in options, Tools->Options->Comment2GPT->Authentication

This asset has one-time purchase fee, but you may need to pay extra for OpenAI, Google AI, Anthropic or DeepSeek services to use Generative AI feature.
https://openai.com/pricing
https://ai.google.dev/pricing
https://www.anthropic.com/pricing#anthropic-api
https://api-docs.deepseek.com/quick_start/pricing



Troubleshooting:
* Make sure in Unity, "Tools"->"Code Assist"->"Status" is available from the top menu bar. If not, reimport the asset.
* Make sure in Visual Studio, "Extensions"->"Unity Code Assist"->Status is available from the top menu bar. If not, install the extension manually, see "YOUR_PROJECT_PATH/Packages/com.merry-yellow.code-assist/Installers~/Extension Installation.md"
* Make sure in VS Code,"Unity Code Assist: Status" command is avaible in the commands palette (shortcut: Ctrl+Shift+P) . If not, install the extension manually, see "YOUR_PROJECT_PATH/Packages/com.merry-yellow.code-assist/Installers~/Extension Installation.md"
* If Visual Studio/VS Code has just been started, just wait; it usually takes around one minute to fully load Code Assist.
* If still not working, in Unity try "Tools"->"Code Assist"->"Synchronize" from the top menu bar. This will attempt a new connection with Visual Studio/VS Code.
* If none works above, try restarting both Unity and Visual Studio/VS Code; sometimes an unclosed background thread blocks communication channels (network ports).



Error Reporting:
* In Unity, choose "Tools"->"Code Assist"->"Report error" from the menu bar, to open feedback window.
* Or in Visual Studio, choose "Extensions"->"Unity Code Assist"->"Report error" from the menu bar, to open feedback window. (There is no feedback window in VS Code, log files are under system temp folder)
* In the feedback window, you can view the most recent logs, or view full log of Unity or Visual Studio.
* You can report the error via email ( merryyellow@outlook.com ) or Discord ( https://discord.gg/2CgKHDq ) or GitHub ( https://github.com/merryyellow/Unity-Code-Assist/issues/new )
* Attaching full logs is recommended when reporting an error.



Thank you for using Code Assist. We hope this will help you in your crazy journey called game development :)