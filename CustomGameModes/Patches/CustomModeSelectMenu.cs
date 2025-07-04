using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomGameModes.Patches
{
    internal class CustomModeSelectMenu
    {
        static GameObject CustomModeSelectButton;

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void ModeSelectMenu_Start_Postfix(ModeSelectMenu __instance)
        {
            var tmpItems = __instance.listItems;
            __instance.listItems = new ModeSelectMenu.ListItem[tmpItems.Length + 1];
            for (int i = 0; i < tmpItems.Length; i++)
            {
                __instance.listItems[i] = tmpItems[i];
                var pos = tmpItems[i].ListAnim.gameObject.transform.position;
                pos.y = 800 - (100 * i);
                tmpItems[i].ListAnim.gameObject.transform.position = pos;
            }

            var list1 = GameObject.Find("List1");
            var list7 = GameObject.Instantiate(list1);
            list7.name = "List7";
            list7.transform.SetParent(list1.transform.parent);
            var position = list7.transform.position;
            position.y = 900;
            list7.transform.position = position;

            var tmpGui = list7.GetComponentInChildren<TextMeshProUGUI>();
            tmpGui.text = "Custom Modes";

            ModeSelectMenu.ListItem newItem = new ModeSelectMenu.ListItem();
            var animators = list7.GetComponentsInChildren<Animator>();
            for (int i = 0; i < animators.Length; i++)
            {
                if (animators[i].gameObject.name == "List7")
                {
                    newItem.ListAnim = animators[i];
                }
                else if (animators[i].gameObject.name == "Button")
                {
                    newItem.ButtonAnim = animators[i];
                    newItem.ListButton = animators[i].gameObject.GetComponent<Button>();
                }
            }

            newItem.ButtonText = tmpGui;
            __instance.listItems[tmpItems.Length] = newItem;

            if (CustomModeSelectButton == null)
            {
                CustomModeSelectButton = new GameObject("CustomModeSelection");
                CustomModeSelectButton.transform.SetParent(__instance.gameObject.transform);
                CustomModeSelectButton.AddComponent<CustomModeSelection>();
                CustomModeSelectButton.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.SelectButton))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_SelectButton_Prefix(ModeSelectMenu __instance, int buttonIndex, bool playAnim = true)
        {
            if (__instance.CurrentState != ModeSelectMenu.State.ModeSelecting)
            {
                return true;
            }
            if (buttonIndex == 6)
            {
                __instance.selectedItem = buttonIndex;
                __instance.UpdateButtonDisplay(playAnim);

                var text = __instance.helpText;
                var key = "mode_select_desc_dani_dojo";
#if MONO
                __instance.SetText(text, key, DataConst.DescriptionFontMaterialType.Plane);
#else
                __instance.SetText(ref text, ref key, DataConst.DescriptionFontMaterialType.Plane);
#endif

                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.DecideItem))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static void ModeSelectMenu_DecideItem_Prefix(ModeSelectMenu __instance)
        {
            if (__instance.CurrentState != ModeSelectMenu.State.ModeSelecting)
            {
                return;
            }
            string source = __instance.GetSceneName(__instance.sourceScene);
            if (__instance.selectedItem == 6)
            {
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);

                CustomModeSelectButton.SetActive(!CustomModeSelectButton.active);
            }
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.UpdateClosing))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static void ModeSelectMenu_UpdateClosing_Prefix(ModeSelectMenu __instance)
        {
            if (CustomModeSelectButton != null)
            {
                CustomModeSelectButton.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.UpdateModeSelect))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_UpdateModeSelect_Prefix(ModeSelectMenu __instance)
        {
            if (CustomModeSelectButton != null && CustomModeSelectButton.activeSelf)
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.SetText))]
        [HarmonyPatch([typeof(TMP_Text), typeof(string), typeof(DataConst.DefaultFontMaterialType)],
                      [ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal])]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_SetText_Prefix(ModeSelectMenu __instance, in TMP_Text tmpText, in string key, DataConst.DefaultFontMaterialType fontMaterialType)
        {
            if (key == "mode_select_desc_dani_dojo")
            {
                WordDataManager.WordListKeysInfo wordListInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr.GetWordListInfo(key);
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
                tmpText.fontSharedMaterial = fontTMPMgr.GetDefaultFontMaterial(wordListInfo.FontType, fontMaterialType);
                tmpText.font = fontTMPMgr.GetDefaultFontAsset(wordListInfo.FontType);
                tmpText.text = "This leads to custom game modes!";
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ModeSelectMenu))]
        [HarmonyPatch(nameof(ModeSelectMenu.SetText))]
        [HarmonyPatch([typeof(TMP_Text), typeof(string), typeof(DataConst.DescriptionFontMaterialType)],
            [ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal])]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool ModeSelectMenu_SetText_Prefix(ModeSelectMenu __instance, in TMP_Text tmpText, in string key, DataConst.DescriptionFontMaterialType fontMaterialType)
        {
            if (key == "mode_select_desc_dani_dojo")
            {
                WordDataManager.WordListKeysInfo wordListInfo = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr.GetWordListInfo("mode_select_desc_rank_match");
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
                tmpText.fontSharedMaterial = fontTMPMgr.GetDescriptionFontMaterial(wordListInfo.FontType, fontMaterialType);
                tmpText.font = fontTMPMgr.GetDescriptionFontAsset(wordListInfo.FontType);
                tmpText.text = "This leads to custom game modes!";
                return false;
            }
            return true;
        }
    }
}
