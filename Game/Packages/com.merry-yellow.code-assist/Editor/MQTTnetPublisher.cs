using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Application = UnityEngine.Application;

using Meryel.UnityCodeAssist.MQTTnet;
using Meryel.UnityCodeAssist.MQTTnet.Server;
using Meryel.UnityCodeAssist.MQTTnet.Protocol;
using Meryel.UnityCodeAssist.MQTTnet.Adapter;
using Meryel.UnityCodeAssist.MQTTnet.Implementations;
using Meryel.UnityCodeAssist.MQTTnet.Diagnostics;


#pragma warning disable IDE0005
using Serilog = Meryel.Serilog;
using MQTTnet = Meryel.UnityCodeAssist.MQTTnet;
using Newtonsoft = Meryel.UnityCodeAssist.Newtonsoft;
#pragma warning restore IDE0005


#nullable enable


//**--
// can also do this for better clear, sometimes it gets locked
// https://answers.unity.com/questions/704066/callback-before-unity-reloads-editor-assemblies.html#

namespace Meryel.UnityCodeAssist.Editor
{
    public class MQTTnetPublisher : Synchronizer.Model.IProcessor
    {
        MqttServer? broker;

        CancellationTokenSource? cancellationTokenSource;

        readonly Synchronizer.Model.Manager syncMngr;

        //public readonly List<Synchronizer.Model.Connect> clients;
        readonly System.Collections.Concurrent.ConcurrentDictionary<string, Synchronizer.Model.Connect> _clients;

        public IEnumerable<Synchronizer.Model.Connect> Clients => _clients.Values.Where(c => c.NodeKind != Synchronizer.Model.NodeKind.SemiClient_RoslynAnalyzer.ToString());

        Synchronizer.Model.Connect? _self;

        Synchronizer.Model.Connect Self => _self!;

        void InitializeSelf()
        {
            var projectPath = CommonTools.GetProjectPath();
            _self = new Synchronizer.Model.Connect()
            {
                ModelVersion = Synchronizer.Model.Utilities.Version,
                ProjectPath = projectPath,
                ProjectName = getProjectName(),
                ContactInfo = $"Unity {Application.unityVersion}",
                AssemblyVersion = Assister.Version,
#if MERYEL_UCA_LITE_VERSION
                LiteOrFull = "Lite",
#else
                LiteOrFull = "Full",
#endif
                NodeKind = Synchronizer.Model.NodeKind.Server.ToString(),
                ClientId = "",
            };

            string getProjectName()
            {
                string[] s = projectPath.Split('/');
#pragma warning disable IDE0056
                string projectName = s[s.Length - 1];
#pragma warning restore IDE0056
                //Logg("project = " + projectName);
                return projectName;
            }
        }


        public static void LogContext()
        {
        }

        public MQTTnetPublisher()
        {
            // LogContext();

            Serilog.Log.Debug("MQTTnet server initializing, begin");

            InitializeSelf();

            _clients = new System.Collections.Concurrent.ConcurrentDictionary<string, Synchronizer.Model.Connect>();
            syncMngr = new Synchronizer.Model.Manager(this);

            var port = Synchronizer.Model.Utilities.GetPortForMQTTnet(Self!.ProjectPath);


            // Create the options for our MQTT Broker
            MqttServerOptionsBuilder options = new MqttServerOptionsBuilder()
                                                 // set endpoint to localhost
                                                 .WithDefaultEndpoint()
                                                 // port used will be 707
                                                 .WithDefaultEndpointPort(port)
                                                 // handler for new connections
                                                 //.WithConnectionValidator(OnNewConnection)
                                                 // handler for new messages
                                                 //.WithApplicationMessageInterceptor(OnNewMessage)

                                                 // disable ipv6 for linux (and possibly macos too), otherwise socket exception is thrown
                                                 .WithDefaultEndpointBoundIPV6Address(System.Net.IPAddress.None)

                                                 // for preventing socket ex after server restart https://github.com/dotnet/MQTTnet/issues/494
                                                 // System.Net.Sockets.SocketException (0x80004005): Only one usage of each socket address (protocol/network address/port) is normally permitted.
                                                 .WithTlsEndpointReuseAddress()
                                                 ;

            IList<IMqttServerAdapter> DefaultServerAdapters = new List<IMqttServerAdapter>()
            {
                new MqttTcpServerAdapter(),
            };
            var logger = new MqttNetNullLogger();



            broker = new MqttServer(options.Build(), DefaultServerAdapters, logger);

            broker.InterceptingPublishAsync += Broker_InterceptingPublishAsync;
            broker.ClientDisconnectedAsync += Broker_ClientDisconnectedAsync;

            Serilog.Log.Debug("MQTTnet server initializing, constructed broker, port: {Port}", port);

            try
            {
                //broker.StartAsync().GetAwaiter().GetResult();

                var startTask = Task.Run(() => broker.StartAsync());
                if (!startTask.Wait(TimeSpan.FromSeconds(5)))
                {
                    Serilog.Log.Error("MQTTnet broker.StartAsync timed out.");
                    return;
                }


                Serilog.Log.Debug("MQTTnet server initializing, started broker");
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                Serilog.Log.Error(socketEx, "Socket exception");
                LogContext();
                //Serilog.Log.Warning("Socket exception disposing pubSocket");
                //broker.Dispose();
                //broker = null;
                return;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "MQTTnet broker.StartAsync failed.");
                return;
            }


            //pubSocket.SendReady += PubSocket_SendReady;
            //SendConnect();

            cancellationTokenSource = new CancellationTokenSource();
            //pullThread = new System.Threading.Thread(async () => await PullAsync(conn.pushPull, pullThreadCancellationTokenSource.Token));
            //pullThread = new System.Threading.Thread(() => InitPull(conn.pushPull, pullTaskCancellationTokenSource.Token));
            //pullThread.Start();
            //Task.Run(() => InitPullAsync());


            Serilog.Log.Debug("MQTTnet server initializing, initialized");

            // need to sleep here, clients will take some time to start subscribing
            // https://github.com/zeromq/netmq/issues/482#issuecomment-182200323
            Thread.Sleep(1000);
            SendConnect();

            Serilog.Log.Debug("MQTTnet server initializing, initialized at {port} with {projectPath}", port, Self!.ProjectPath);
        }

        private Task Broker_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        {
            try
            {
                var removed = _clients.TryRemove(arg.ClientId, out _);
                Serilog.Log.Debug("Broker_ClientDisconnectedAsync {ClientId} {Result}", arg.ClientId, removed);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "async exception at {Location}", nameof(Broker_ClientDisconnectedAsync));
            }
            return Task.CompletedTask;
        }

        private Task Broker_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            try
            {
                // if server message
                if (string.IsNullOrEmpty(arg.ClientId))
                    return Task.CompletedTask;

                Serilog.Log.Verbose("mqttnet consume {topic} {content}", arg.ApplicationMessage.Topic, arg.ApplicationMessage.ConvertPayloadToString());

                var topic = arg.ApplicationMessage.Topic;
                var header = topic.Substring(3); // for "cs/" prefix
                var content = arg.ApplicationMessage.ConvertPayloadToString();

                MainThreadDispatcher.Add(() => syncMngr.ProcessMessage(header, content));
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "async exception at {Location}", nameof(Broker_InterceptingPublishAsync));
            }
            
            return Task.CompletedTask;
        }

        public void Clear()
        {
            // LogContext();

            Serilog.Log.Verbose("MQTTnet clearing {HasBroker}", (broker != null));

            var server = broker;
            if (server != null)
            {
                server.InterceptingPublishAsync -= Broker_InterceptingPublishAsync;
                Serilog.Log.Verbose("MQTTnet clearing, removed events");
            }

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;
            Serilog.Log.Verbose("MQTTnet clearing, cancelled async token");

            if (server == null)
                return;

            // broker?.StopAsync().GetAwaiter().GetResult(); // this line was freezing Unity editor, so calling Task.Run().Wait() instead
            try
            {
                var stopTask = Task.Run(() => server.StopAsync());
                if (!stopTask.Wait(TimeSpan.FromSeconds(5))) // give it five secs to complete
                {
                    Serilog.Log.Error("MQTTnet broker.StopAsync timed out.");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "MQTTnet broker.StopAsync failed.");
            }

            Serilog.Log.Verbose("MQTTnet clearing, stopped broker");
            try
            {
                server.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "MQTTnet broker.Dispose failed.");
            }

            server = null;
            broker = null;

            Serilog.Log.Debug("MQTTnet clearing, cleared");
        }

        string SerializeObject<T>(T obj)
            where T : class
        {
            // Odin cant serialize string arrays, https://github.com/TeamSirenix/odin-serializer/issues/26
            //var buffer = OdinSerializer.SerializationUtility.SerializeValue<T>(obj, OdinSerializer.DataFormat.JSON);
            //var str = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            // Newtonsoft works fine, but needs package reference
            //var str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            // not working
            //var str = EditorJsonUtility.ToJson(obj);

            // needs nuget
            //System.Text.Json.JsonSerializer;

            //var str = TinyJson.JsonWriter.ToJson(obj);
            //var str = Meryel.UnityCodeAssist.ProjectData.LitJson.JsonMapper.ToJson(obj);
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            return str;
        }

        void SendAux(Synchronizer.Model.IMessage message, bool logContent = true)
        {
            if (message == null)
                return;

            SendAux(message.GetType().Name, message, logContent);
        }

        void SendAux(string messageType, object content, bool logContent = true)
        {
            if (logContent)
                Serilog.Log.Debug("Publishing {MessageType} {@Content}", messageType, content);
            else
                Serilog.Log.Debug("Publishing {MessageType}", messageType);

            var publisher = broker;
            if (publisher != null)
                //publisher.SendMoreFrame(messageType).SendFrame(SerializeObject(content));
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("sc/" + messageType) // sc/ => server->client message
                    .WithRetainFlag(false)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .WithPayload(SerializeObject(content))
                    .Build();

                //broker?.InjectApplicationMessage(new InjectedMqttApplicationMessage(applicationMessage), cancellationTokenSource?.Token ?? default).GetAwaiter().GetResult();
                broker?.InjectApplicationMessage(new InjectedMqttApplicationMessage(applicationMessage), cancellationTokenSource?.Token ?? default);
            }
            else
                Serilog.Log.Error("Publisher socket is null");
        }

        public void SendConnect()
        {
            var connect = Self;

            SendAux(connect);
        }

        public void SendDisconnect()
        {
            var disconnect = new Synchronizer.Model.Disconnect()
            {
                ModelVersion = Self.ModelVersion,
                ProjectPath = Self.ProjectPath,
                ProjectName = Self.ProjectName,
                ContactInfo = Self.ContactInfo,
                AssemblyVersion = Self.AssemblyVersion,
                LiteOrFull = Self.LiteOrFull,
                NodeKind = Self.NodeKind,
                ClientId = Self.ClientId,
            };

            SendAux(disconnect);
        }

        public void SendConnectionInfo()
        {
            var connectionInfo = new Synchronizer.Model.ConnectionInfo()
            {
                ModelVersion = Self.ModelVersion,
                ProjectPath = Self.ProjectPath,
                ProjectName = Self.ProjectName,
                ContactInfo = Self.ContactInfo,
                AssemblyVersion = Self.AssemblyVersion,
                LiteOrFull = Self.LiteOrFull,
                NodeKind = Self.NodeKind,
                ClientId = Self.ClientId,
            };

            SendAux(connectionInfo);
        }

        public void SendHandshake()
        {
            var handshake = new Synchronizer.Model.Handshake();

            SendAux(handshake);
        }

        public void SendRequestInternalLog()
        {
            var requestInternalLog = new Synchronizer.Model.RequestInternalLog();

            SendAux(requestInternalLog);
        }

        public void SendRequestUpdate(string app, string path, bool isInteractive)
        {
            var requestUpdate = new Synchronizer.Model.RequestUpdate()
            {
                App = app,
                Path = path,
                IsInteractive = isInteractive,
            };

            SendAux(requestUpdate);
        }

        public void SendInternalLog()
        {
            var internalLog = new Synchronizer.Model.InternalLog()
            {
                LogContent = Logger.ELogger.GetInternalLogContent(),
            };

            SendAux(internalLog, logContent: false);
        }


        void SendStringArrayAux(string id, string[] array)
        {
            var stringArray = new Synchronizer.Model.StringArray()
            {
                Id = id,
                Array = array,
            };

            SendAux(stringArray);
        }

        void SendStringArrayContainerAux(params (string id, string[] array)[] container)
        {
            var stringArrayContainer = new Synchronizer.Model.StringArrayContainer()
            {
                Container = new Synchronizer.Model.StringArray[container.Length],
            };

            for (int i = 0; i < container.Length; i++)
            {
                stringArrayContainer.Container[i] = new Synchronizer.Model.StringArray
                {
                    Id = container[i].id,
                    Array = container[i].array
                };
            }

            SendAux(stringArrayContainer);
        }

        public void SendTags(string[] tags) =>
            SendStringArrayAux(Synchronizer.Model.Ids.Tags, tags);

        public void SendLayers(string[] layerNames, string[] layerIndices)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.Layers, layerNames),
                (Synchronizer.Model.Ids.LayerIndices, layerIndices));
        }

        public void SendSortingLayers(string[] sortingLayers, string[] sortingLayerIds, string[] sortingLayerValues)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.SortingLayers, sortingLayers),
                (Synchronizer.Model.Ids.SortingLayerIds, sortingLayerIds),
                (Synchronizer.Model.Ids.SortingLayerValues, sortingLayerValues));
        }

        public void SendRenderingLayers(string[] renderingLayers, string[] renderingLayerIndices)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.RenderingLayers, renderingLayers),
                (Synchronizer.Model.Ids.RenderingLayerIndices, renderingLayerIndices));
        }

        public void SendPlayerPrefs(string[] playerPrefKeys, string[] playerPrefValues,
            string[] playerPrefStringKeys, string[] playerPrefIntegerKeys, string[] playerPrefFloatKeys)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.PlayerPrefKeys, playerPrefKeys),
                (Synchronizer.Model.Ids.PlayerPrefValues, playerPrefValues),
                (Synchronizer.Model.Ids.PlayerPrefStringKeys, playerPrefStringKeys),
                (Synchronizer.Model.Ids.PlayerPrefIntegerKeys, playerPrefIntegerKeys),
                (Synchronizer.Model.Ids.PlayerPrefFloatKeys, playerPrefFloatKeys)
                );
        }

        public void SendEditorPrefs(string[] editorPrefKeys, string[] editorPrefValues,
            string[] editorPrefStringKeys, string[] editorPrefIntegerKeys, string[] editorPrefFloatKeys,
            string[] editorPrefBooleanKeys)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.EditorPrefKeys, editorPrefKeys),
                (Synchronizer.Model.Ids.EditorPrefValues, editorPrefValues),
                (Synchronizer.Model.Ids.EditorPrefStringKeys, editorPrefStringKeys),
                (Synchronizer.Model.Ids.EditorPrefIntegerKeys, editorPrefIntegerKeys),
                (Synchronizer.Model.Ids.EditorPrefFloatKeys, editorPrefFloatKeys),
                (Synchronizer.Model.Ids.EditorPrefBooleanKeys, editorPrefBooleanKeys)
                );
        }

        public void SendInputManager(string[] axisNames, string[] axisInfos, string[] buttonKeys, string[] buttonAxis, string[] joystickNames)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.InputManagerAxes, axisNames),
                (Synchronizer.Model.Ids.InputManagerAxisInfos, axisInfos),
                (Synchronizer.Model.Ids.InputManagerButtonKeys, buttonKeys),
                (Synchronizer.Model.Ids.InputManagerButtonAxis, buttonAxis),
                (Synchronizer.Model.Ids.InputManagerJoystickNames, joystickNames)
                );
        }

        public void SendSceneList(string[] sceneNames, string[] scenePaths, string[] sceneBuildIndices,
            string[] sceneNamesAndPaths, string[] scenePathsAndNames)
        {
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.SceneNames, sceneNames),
                (Synchronizer.Model.Ids.ScenePaths, scenePaths),
                (Synchronizer.Model.Ids.SceneBuildIndices, sceneBuildIndices),
                (Synchronizer.Model.Ids.SceneNamesAndPaths, sceneNamesAndPaths),
                (Synchronizer.Model.Ids.ScenePathsAndNames, scenePathsAndNames)
                );
        }

        public void SendScriptMissing(string component)
        {
            var scriptMissing = new Synchronizer.Model.ScriptMissing()
            {
                Component = component,
            };

            SendAux(scriptMissing);
        }

        public void SendComponentHumanTrait(string[] bones, string[] muscles)
        {
            //var humanTrait = new Synchronizer.Model.Components.HumanTrait();

            var boneIndices = new string[bones.Length];
            var boneNames = new string[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                boneIndices[i] = i.ToString();
                boneNames[i] = bones[i];
            }

            var muscleIndices = new string[muscles.Length];
            var muscleNames = new string[muscles.Length];
            for (int i = 0; i < muscles.Length; i++)
            {
                muscleIndices[i] = i.ToString();
                muscleNames[i] = muscles[i];
            }
            SendStringArrayContainerAux(
                (Synchronizer.Model.Ids.AnimationHumanBones, boneNames),
                (Synchronizer.Model.Ids.AnimationHumanBoneIndices, boneIndices),
                (Synchronizer.Model.Ids.AnimationHumanMuscles, muscleNames),
                (Synchronizer.Model.Ids.AnimationHumanMuscleIndices, muscleIndices)
                );
        }

        public void SendShaderGlobalKeywords()
        {
            SendStringArrayAux(Synchronizer.Model.Ids.ShaderGlobalKeywords, Shader.globalKeywords.Select(k => k.name).ToArray());
        }

        public void SendGameObject(GameObject go)
        {
            if (!go)
                return;

            Serilog.Log.Debug("SendGO: {GoName}", go.name);

            var dataOfSelf = go.ToSyncModel(priority:10000);
            if (dataOfSelf != null)
                SendAux(dataOfSelf);

            var dataOfHierarchy = go.ToSyncModelOfHierarchy();
            if (dataOfHierarchy != null)
            {
                foreach (var doh in dataOfHierarchy)
                    SendAux(doh);
            }

            var dataOfComponents = go.ToSyncModelOfComponents();
            if (dataOfComponents != null)
            {
                foreach (var doc in dataOfComponents)
                    SendAux(doc);
            }

            var dataOfComponentAnimator = go.ToSyncModelOfComponentAnimator();
            if (dataOfComponentAnimator != null)
                SendAux(dataOfComponentAnimator);

            var dataOfComponentAnimation = go.ToSyncModelOfComponentAnimation();
            if (dataOfComponentAnimation != null)
                SendAux(dataOfComponentAnimation);

            var dataOfComponentMaterial = go.ToSyncModelOfComponentMaterial();
            if (dataOfComponentMaterial != null)
                SendAux(dataOfComponentMaterial);
        }

        public void SendScriptableObject(ScriptableObject so)
        {
            Serilog.Log.Debug("SendSO: {SoName}", so.name);

            var dataOfSo = so.ToSyncModel();
            if (dataOfSo != null)
                SendAux(dataOfSo);
        }

        public void SendAnalyticsEvent(string type, string content)
        {
            var analyticsEvent = new Synchronizer.Model.AnalyticsEvent()
            {
                EventType = type,
                EventContent = content
            };
            SendAux(analyticsEvent);
        }

        public void SendErrorReport(string errorMessage, string stack, string type)
        {
            var errorReport = new Synchronizer.Model.ErrorReport()
            {
                ErrorMessage = errorMessage,
                ErrorStack = stack,
                ErrorType = type,
            };
            SendAux(errorReport);
        }

        public void SendRequestVerboseType(string type, string docPath)
        {
            var requestVerboseType = new Synchronizer.Model.RequestVerboseType()
            {
                Type = type,
                DocPath = docPath,
            };
            SendAux(requestVerboseType);
        }

        public void ForwardRelayMessage(Synchronizer.Model.IRelayMessage relayMessage)
        {
            SendAux(relayMessage);
        }


        string Synchronizer.Model.IProcessor.Serialize<T>(T value)
        {
            //return System.Text.Json.JsonSerializer.Serialize<T>(value);
            //return Newtonsoft.Json.JsonConvert.SerializeObject(value);
            return SerializeObject(value);
        }
        T Synchronizer.Model.IProcessor.Deserialize<T>(string data)
        {
            //return System.Text.Json.JsonSerializer.Deserialize<T>(data)!;
            //return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data)!;
            //return TinyJson.JsonParser.FromJson<T>(data)!;
            //return Meryel.UnityCodeAssist.ProjectData.LitJson.JsonMapper.ToObject<T>(data);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data)!;

            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
            //T val = OdinSerializer.SerializationUtility.DeserializeValue<T>(buffer, OdinSerializer.DataFormat.JSON);
            //return val;
        }

        //**--make sure all Synchronizer.Model.IProcessor.Process methods are thread-safe

        // a new client has connected
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Connect connect)
        {
            if (connect.ModelVersion != Self.ModelVersion)
            {
                Serilog.Log.Error("Version mismatch with {ContactInfo}. Please update your Unity asset and reinstall the Visual Studio/VS Code extension. {ContactModel} != {SelfModel}", connect.ContactInfo, connect.ModelVersion, Self.ModelVersion);
                return;
            }

            if (connect.ProjectPath != Self.ProjectPath)
            {
                Serilog.Log.Error("Project mismatch with {ProjectName}. '{ConnectPath}' != '{SelfPath}'", connect.ProjectName, connect.ProjectPath, Self.ProjectPath);
                return;
            }

            if (!string.IsNullOrEmpty(connect.LiteOrFull) && connect.LiteOrFull != Self.LiteOrFull)
            {
                if (connect.LiteOrFull == "Lite")
                {
                    //**-- upgrade vsix to full here //**--//**--
                }
            }

            var hasClient = _clients.TryGetValue(connect.ClientId, out var client);
            if (!hasClient)
                _clients[connect.ClientId] = connect;
            else
            {
                // LiteOrFull field might be updated
                client.ModelVersion = connect.ModelVersion;
                client.ProjectPath = connect.ProjectPath;
                client.ProjectName = connect.ProjectName;
                client.ContactInfo = connect.ContactInfo;
                client.AssemblyVersion = connect.AssemblyVersion;
                client.LiteOrFull = connect.LiteOrFull;
                client.NodeKind = connect.NodeKind;
                client.ClientId = connect.ClientId;
            }

            SendHandshake();
            if (ScriptFinder.GetActiveGameObject(out var activeGO))
                SendGameObject(activeGO);
            Assister.SendTagsAndLayers();
        }

        // a new client is online and requesting connection
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestConnect requestConnect)
        {
            SendConnect();
        }
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Disconnect disconnect)
        {
            var removed = _clients.TryRemove(disconnect.ClientId, out var client);
            Serilog.Log.Debug("Synchronizer.Model.Disconnect {ClientId} {Removed}", disconnect.ClientId, removed);
        }
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ConnectionInfo connectionInfo)
        {
            if (connectionInfo.ModelVersion != Self.ModelVersion)
            {
                Serilog.Log.Error("Version mismatch with {ContactInfo}. Please update your Unity asset and reinstall the Visual Studio/VS Code extension. {ContactModel} != {SelfModel}", connectionInfo.ContactInfo, connectionInfo.ModelVersion, Self.ModelVersion);
                return;
            }

            if (connectionInfo.ProjectPath != Self.ProjectPath)
            {
                Serilog.Log.Error("Project mismatch with {ProjectName}. '{ConnectPath}' != '{SelfPath}'", connectionInfo.ProjectName, connectionInfo.ProjectPath, Self.ProjectPath);
                return;
            }

            if (!_clients.TryGetValue(connectionInfo.ClientId, out _))
            {
                SendConnect();
            }
            else
            {
                SendHandshake();
                if (ScriptFinder.GetActiveGameObject(out var activeGO))
                    SendGameObject(activeGO);
                Assister.SendTagsAndLayers();
            }
        }
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestConnectionInfo requestConnectionInfo)
        {
            SendConnectionInfo();
        }
        /*
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Layers layers)
        {

        }
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Tags tags)
        {

        }
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.SortingLayers sortingLayers)
        {

        }*/
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.StringArray stringArray)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.StringArray)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.StringArrayContainer stringArrayContainer)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.StringArrayContainer)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.GameObject gameObject)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.GameObject)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ComponentData component)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ComponentData)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Component_Animator component_Animator)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Component_Animator)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Component_Animation component_Animation)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Component_Animation)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Component_Material component_Material)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Component_Material)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestScript requestScript)
        {
            if (requestScript.DeclaredTypes == null || requestScript.DeclaredTypes.Length == 0)
                return;

            var documentPath = requestScript.DocumentPath;

            foreach (var declaredType in requestScript.DeclaredTypes)
            {
                if (ScriptFinder.FindInstanceOfType(declaredType, documentPath, out var go, out var so))
                {
                    if (go != null)
                        SendGameObject(go);
                    else if (so != null)
                        SendScriptableObject(so);
                    else
                        Serilog.Log.Warning("Invalid instance of type");
                }
                else
                {
                    SendScriptMissing(declaredType);
                }
            }
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestScriptFast requestScriptFast)
        {
            var documentPath = requestScriptFast.DocumentPath;

            //**--namespace?
            var possiblyDeclaredType = Path.GetFileNameWithoutExtension(documentPath);

            if (ScriptFinder.FindInstanceOfType(possiblyDeclaredType, documentPath, out var go, out var so))
            {
                if (go != null)
                    SendGameObject(go);
                else if (so != null)
                    SendScriptableObject(so);
                else
                    Serilog.Log.Warning("Invalid instance of type");
            }
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ScriptMissing scriptMissing)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ScriptMissing)");
        }


        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.Handshake handshake)
        {
            // Do nothing
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestInternalLog requestInternalLog)
        {
            SendInternalLog();
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.InternalLog internalLog)
        {
            Logger.ELogger.VsInternalLog = internalLog.LogContent;
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.AnalyticsEvent analyticsEvent)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.AnalyticsEvent)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ErrorReport errorReport)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.ErrorReport)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestVerboseType requestVerboseType)
        {
            Serilog.Log.Warning("Unity/Server shouldn't call Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestVerboseType)");
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestLazyLoad requestLazyLoad)
        {
            Monitor.LazyLoad(requestLazyLoad.Category);
        }

        internal Synchronizer.Model.RequestUpdate? DelayedRequestUpdate { get; private set; }
        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RequestUpdate requestUpdate)
        {
            if (requestUpdate.App != "Unity" && requestUpdate.App != "SystemBinariesForDotNetStandard20")
                return;

            // cannot import package in play mode, so delay it
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Serilog.Log.Information("Cannot import package in play mode, please exit play mode to update");
                DelayedRequestUpdate = requestUpdate;
                return;
            }
            DelayedRequestUpdate = null;

            // let unity update the package, don't unzip it, to prevent file already in use and other issues
            AssetDatabase.ImportPackage(requestUpdate.Path, requestUpdate.IsInteractive);
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RelayDocumentShow relayDocumentShow)
        {
            ForwardRelayMessage(relayDocumentShow);
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RelayDocumentSave relayDocumentSave)
        {
            ForwardRelayMessage(relayDocumentSave);
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RelayDocumentViewportChanged relayDocumentViewportChanged)
        {
            ForwardRelayMessage(relayDocumentViewportChanged);
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RelayLogMessage relayLogMessage)
        {
            ForwardRelayMessage(relayLogMessage);
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RelayUpdateExport relayUpdateExport)
        {
            ForwardRelayMessage(relayUpdateExport);
        }

        void Synchronizer.Model.IProcessor.Process(Synchronizer.Model.RelayAdornmentText relayAdornmentText)
        {
            ForwardRelayMessage(relayAdornmentText);
        }

    }
}

