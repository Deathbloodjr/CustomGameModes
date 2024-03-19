using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using BepInEx.Configuration;
using CustomGameModes.Patches;
using System.Collections.Generic;
using System.IO;
using UnityEngine.U2D;
using System.Diagnostics;
using System.Reflection;
using System.Linq;


#if TAIKO_IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace CustomGameModes
{
    internal enum LogType
    {
        Info,
        Warning,
        Error,
        Fatal,
        Message,
        Debug
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, ModName, PluginInfo.PLUGIN_VERSION)]
#if TAIKO_MONO
    internal class Plugin : BaseUnityPlugin
#elif TAIKO_IL2CPP
    internal class Plugin : BasePlugin
#endif
    {
        const string ModName = "CustomGameModes";

        public static Plugin Instance;
        private Harmony _harmony;
        public new static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<string> ConfigAssetLocation;

        public ConfigEntry<bool> ConfigLoggingEnabled;
        public ConfigEntry<int> ConfigLoggingDetailLevelEnabled;

#if TAIKO_MONO
        private void Awake()
#elif TAIKO_IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if TAIKO_MONO
            Log = Logger;
#elif TAIKO_IL2CPP
            Log = base.Log;
#endif

            SetupConfig();

            InitializeDaniDojoSceneAssetBundle();

            SetupHarmony();
        }

        private void SetupConfig()
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");

            ConfigLoggingEnabled = Config.Bind("Debug",
                "LoggingEnabled",
                true,
                "Enables logs to be sent to the console.");

            ConfigLoggingDetailLevelEnabled = Config.Bind("Debug",
                "LoggingDetailLevelEnabled",
                0,
                "Enables more detailed logs to be sent to the console. The higher the number, the more logs will be displayed. Mostly for my own debugging.");

            ConfigAssetLocation = Config.Bind("Data",
                "AssetLocation",
                Path.Combine(dataFolder, "Assets"),
                "The file location for all of this mod's asset data.");
        }

        private const string ASSETBUNDLE_NAME = "danidojo.scene";
        public static AssetBundle Assets;
        private void InitializeDaniDojoSceneAssetBundle()
        {
            Plugin.Log.LogInfo("CustomGameMode scene load start");
            string assetBundlePath = Path.Combine(ConfigAssetLocation.Value, ASSETBUNDLE_NAME);
            if (!File.Exists(assetBundlePath))
            {
                Assets = null;
                Plugin.Log.LogInfo("CustomGameMode scene asset not found?");
                return;
            }
            Assets = AssetBundle.LoadFromFile(assetBundlePath);
            Plugin.Log.LogInfo("CustomGameMode scene loaded?");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                _harmony.PatchAll(typeof(CustomModeSelectMenu));

                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        

        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCustomCoroutine(IEnumerator enumerator)
        {
#if TAIKO_MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif TAIKO_IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }

        public void LogInfoInstance(LogType type, string value, int detailLevel = 0)
        {
            if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
            {
                switch (type)
                {
                    case LogType.Info:
                        Log.LogInfo("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Warning:
                        Log.LogWarning("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Error:
                        Log.LogError("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Fatal:
                        Log.LogFatal("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Message:
                        Log.LogMessage("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Debug:
                        // I'm not sure if I should make this only happen in DEBUG mode
                        // Seems like a decent idea, I'll keep it until it seems like a bad idea
#if DEBUG
                        Log.LogDebug("[" + detailLevel + "] " + value);
#endif
                        break;
                    default:
                        break;
                }
            }
        }
        public static void LogInfo(LogType type, string value, int detailLevel = 0)
        {
            Instance.LogInfoInstance(type, value, detailLevel);
        }
        public static void LogInfo(LogType type, List<string> value, int detailLevel = 0)
        {
            if (value.Count == 0)
            {
                return;
            }
            string sendValue = value[0];
            for (int i = 1; i < value.Count; i++)
            {
                sendValue += "\n" + value[i];
            }
            Instance.LogInfoInstance(type, sendValue, detailLevel);
        }

    }
}