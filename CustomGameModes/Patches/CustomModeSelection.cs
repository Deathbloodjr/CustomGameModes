using CustomGameModes.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if IL2CPP
using Il2CppInterop.Runtime.Injection;
#endif

namespace CustomGameModes.Patches
{
    internal class CustomModeSelection : MonoBehaviour
    {
#if IL2CPP
        static CustomModeSelection() => ClassInjector.RegisterTypeInIl2Cpp<CustomModeSelection>();
#endif

        int currentIndex = 0;

        GameObject selectedImage = null;

        void Start()
        {
            if (selectedImage == null)
            {
                selectedImage = AssetUtility.CreateImageChild(this.gameObject, "SelectedItem", new Rect(90, 890, 320, 120), Color.white);
            }

            CustomModeSelectApi.CustomModeButtons = CustomModeSelectApi.CustomModeButtons.OrderBy((CustomModeButtonData x) => x.Name).ToList();

            var list7 = GameObject.Find("List1");
            var tmpGui = list7.GetComponentInChildren<TextMeshProUGUI>();
            var font = tmpGui.font;
            var fontMat = tmpGui.fontMaterial;

            for (int i = 0; i < CustomModeSelectApi.CustomModeButtons.Count; i++)
            {
                var buttonData = CustomModeSelectApi.CustomModeButtons[i];
                var buttonObj = AssetUtility.CreateButton(this.gameObject, buttonData.Name, GetRectFromIndex(i), buttonData.ButtonText, buttonData.Color);
                var button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(buttonData.ClickEvent);

                var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.font = font;
                buttonText.fontMaterial = fontMat;
                buttonText.color = Color.white;
            }
        }

        void Update()
        {
            if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetCancelDown(ControllerManager.ControllerPlayerNo.Player1))
            {
                this.gameObject.SetActive(false);
            }
            else if (TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1))
            {
                if (CustomModeSelectApi.CustomModeButtons.Count <= currentIndex)
                {
                    ModLogger.Log("CustomModeButtons index out of bounds.", LogType.Warning);
                }
                else
                {
                    CustomModeSelectApi.CustomModeButtons[currentIndex].ClickEvent.Invoke();
                }
            }
            else
            {
                ControllerManager.Dir dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionDown(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
                if (dir == ControllerManager.Dir.None)
                {
                    dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionButton(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
                    if (!TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionRepeatTrigger(ControllerManager.ControllerPlayerNo.Player1, dir, out var flag2))
                    {
                        dir = ControllerManager.Dir.None;
                    }
                }
                switch (dir)
                {
                    case ControllerManager.Dir.Right:
                        // Move to the next column. I don't have multiple columns set up yet.
                        currentIndex = (currentIndex + 6) % CustomModeSelectApi.CustomModeButtons.Count;
                        break;
                    case ControllerManager.Dir.Left:
                        // Move to the previous column. I don't have multiple columns set up yet.
                        currentIndex = (currentIndex - 6 + CustomModeSelectApi.CustomModeButtons.Count) % CustomModeSelectApi.CustomModeButtons.Count;
                        break;
                    case ControllerManager.Dir.Up:
                        currentIndex = (currentIndex - 1 + CustomModeSelectApi.CustomModeButtons.Count) % CustomModeSelectApi.CustomModeButtons.Count;
                        break;
                    case ControllerManager.Dir.Down:
                        currentIndex = (currentIndex + 1) % CustomModeSelectApi.CustomModeButtons.Count;
                        break;
                }
                AssetUtility.SetRect(selectedImage, GetRectFromIndex(currentIndex, true));

            }
            
        }

        Rect GetRectFromIndex(int index, bool highlighted = false)
        {
            // 6 rows by 3 columns
            int row = index % 6;
            int column = index / 6; // this should drop the decimal
            var rect = new Rect(100 + (column * 350), 900 - (row * 150), 300, 100);
            if (highlighted)
            {
                rect.y -= 10;
                rect.x -= 10;
                rect.height += 20;
                rect.width += 20;
            }
            return rect;
        }
    }
}
