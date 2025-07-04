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


#if IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace CustomGameModes
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
#if MONO
    internal class Plugin : BaseUnityPlugin
#elif IL2CPP
    internal class Plugin : BasePlugin
#endif
    {
        public const string ModName = "CustomGameModes";

        public static Plugin Instance;
        private Harmony _harmony;
        public new static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<string> ConfigAssetLocation;

#if MONO
        private void Awake()
#elif IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if MONO
            Log = Logger;
#elif IL2CPP
            Log = base.Log;
#endif

            SetupConfig(Config, Path.Combine("BepInEx", "data", ModName));

            InitializeDaniDojoSceneAssetBundle();

            SetupHarmony();

        }

        private void SetupConfig(ConfigFile config, string saveFolder, bool isSaveManager = false)
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            if (!isSaveManager)
            {
                ConfigEnabled = config.Bind("General",
                   "Enabled",
                   true,
                   "Enables the mod.");
            }

            ConfigAssetLocation = Config.Bind("Data",
                "AssetLocation",
                Path.Combine(dataFolder),
                "The file location for all of this mod's asset data.");
        }

        private const string ASSETBUNDLE_NAME = "CustomGameModes.scene";
        private const string ALT_ASSETBUNDLE_NAME = "danidojo.scene";
        public static AssetBundle Assets;
        private void InitializeDaniDojoSceneAssetBundle()
        {
            ModLogger.Log("CustomGameMode scene load start");
            string assetBundlePath = Path.Combine(ConfigAssetLocation.Value, ASSETBUNDLE_NAME);
            if (File.Exists(assetBundlePath))
            {
                Assets = AssetBundle.LoadFromFile(assetBundlePath);
                ModLogger.Log("CustomGameMode scene loaded");
            }
            else if (File.Exists(Path.Combine(ConfigAssetLocation.Value, ALT_ASSETBUNDLE_NAME)))
            {
                Assets = AssetBundle.LoadFromFile(Path.Combine(ConfigAssetLocation.Value, ALT_ASSETBUNDLE_NAME));
                ModLogger.Log("CustomGameMode scene loaded");
            }
            else
            {
                Assets = null;
                ModLogger.Log("CustomGameMode scene asset not found at location: " + assetBundlePath);
                return;
            }
        }


        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            LoadPlugin(ConfigEnabled.Value);
        }

        public static void LoadPlugin(bool enabled)
        {
            if (enabled)
            {
                bool result = true;
                if (!Directory.Exists(Instance.ConfigAssetLocation.Value))
                {
                    Directory.CreateDirectory(Instance.ConfigAssetLocation.Value);
                }
                // If any PatchFile fails, result will become false
                result &= Instance.PatchFile(typeof(CustomModeSelectMenu));
                if (result)
                {
                    ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
                }
                else
                {
                    ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.", LogType.Error);
                    // Unload this instance of Harmony
                    // I hope this works the way I think it does
                    Instance._harmony.UnpatchSelf();
                }
            }
            else
            {
                ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
#if DEBUG
                ModLogger.Log("File patched: " + type.FullName);
#endif
                return true;
            }
            catch (Exception e)
            {
                ModLogger.Log("Failed to patch file: " + type.FullName);
                ModLogger.Log(e.Message);
                return false;
            }
        }

        public static void UnloadPlugin()
        {
            Instance._harmony.UnpatchSelf();
            ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} has been unpatched.");
        }

        public static void ReloadPlugin()
        {
            // Reloading will always be completely different per mod
            // You'll want to reload any config file or save data that may be specific per profile
            // If there's nothing to reload, don't put anything here, and keep it commented in AddToSaveManager
            //SwapSongLanguagesPatch.InitializeOverrideLanguages();
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.Reload();
        }



        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCoroutine(IEnumerator enumerator)
        {
#if MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }
    }
}