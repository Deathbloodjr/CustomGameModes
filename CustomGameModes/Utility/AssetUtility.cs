using CustomGameModes;
using PlayFab.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomGameModes.Utility
{
    internal class AssetUtility
    {
        static Dictionary<string, Sprite> LoadedSprites;

        public static UIButton AddButtonComponent(GameObject newObject)
        {
            newObject.GetOrAddComponent<GraphicRaycaster>();
            return newObject.GetOrAddComponent<UIButton>();
        }

        private static GameObject CreateButton(GameObject parent, string name, Rect rect, string text)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);
            var button = newObject.GetOrAddComponent<UIButton>();

            var raycaster = newObject.GetOrAddComponent<GraphicRaycaster>();

            var rectTransform = SetRect(newObject, rect);

            var buttonText = CreateTextChild(newObject, "Text", new Rect(0, 0, rect.width, rect.height), "Button").GetOrAddComponent<TextMeshProUGUI>();
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.verticalAlignment = VerticalAlignmentOptions.Middle;
            buttonText.text = text;
            buttonText.color = new Color32(0, 0, 0, 255);

            return newObject;
        }

        public static GameObject CreateButton(GameObject parent, string name, Rect rect, string text, Color32 color)
        {
            var newObject = CreateButton(parent, name, rect, text);

            var image = newObject.GetOrAddComponent<Image>();
            image.color = color;

            var coverImage = CreateImageChild(newObject, "Cover", new Rect(0, 0, rect.width, rect.height), new Color32(0, 0, 0, 0));

            return newObject;
        }

        public static GameObject CreateButton(GameObject parent, string name, Rect rect, string text, Sprite sprite)
        {
            var newObject = CreateButton(parent, name, rect, text);

            var image = newObject.GetOrAddComponent<Image>();
            image.color = new Color32(255, 255, 255, 255);
            image.sprite = sprite;

            var coverImage = CreateImageChild(newObject, "Cover", new Rect(0, 0, rect.width, rect.height), new Color32(0, 0, 0, 0));

            return newObject;
        }

        public static void ChangeButtonTransparency(GameObject button, bool isOn)
        {
            var images = button.GetComponentsInChildren<Image>();
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                var imageColor = image.color;
                if (isOn)
                {
                    imageColor.a = 1;
                }
                else
                {
                    imageColor.a = 0.5f;
                }
                image.color = imageColor;
            }
            for (int i = 0; i < images.Length; i++)
            {
                var imageColor = images[i].color;
                if (isOn)
                {
                    imageColor.a = 1;
                }
                else
                {
                    imageColor.a = 0.5f;
                }
                images[i].color = imageColor;
            }
            var texts = button.GetComponentsInChildren<TextMeshProUGUI>();
            var text = button.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                var imageColor = text.color;
                if (isOn)
                {
                    imageColor.a = 1;
                }
                else
                {
                    imageColor.a = 0.5f;
                }
                text.color = imageColor;
            }
            for (int i = 0; i < texts.Length; i++)
            {
                var imageColor = texts[i].color;
                if (isOn)
                {
                    imageColor.a = 1;
                }
                else
                {
                    imageColor.a = 0.5f;
                }
                texts[i].color = imageColor;
            }
        }

        public static void ChangeButtonAndTextTransparency(GameObject button, bool isOn)
        {
            var image = button.GetOrAddComponent<Image>();
            var text = GetChildByName(button, "Text").GetComponent<TextMeshProUGUI>();
            var imageColor = image.color;
            var textColor = text.color;
            if (isOn)
            {
                imageColor.a = 1;
                textColor.a = 1;
            }
            else
            {
                imageColor.a = 0.5f;
                textColor.a = 0.5f;
            }
            image.color = imageColor;
            text.color = textColor;
        }

        public static void ChangeDifficultyButtonAndTextTransparency(GameObject button, bool isOn)
        {
            var image = GetChildByName(button, "IconDiff").GetComponent<Image>();
            var text = GetChildByName(button, "Text").GetComponent<TextMeshProUGUI>();
            var imageColor = image.color;
            var textColor = text.color;
            if (isOn)
            {
                imageColor.a = 1;
                textColor.a = 1;
            }
            else
            {
                imageColor.a = 0.5f;
                textColor.a = 0.5f;
            }
            image.color = imageColor;
            text.color = textColor;
        }

        static public GameObject CreateInputField(GameObject parent, string name, Rect rect)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);

            var rectTransform = SetRect(newObject, rect);

            var image = newObject.GetOrAddComponent<Image>();
            image.color = new Color32(255, 255, 255, 255);
            var inputField = newObject.AddComponent<TMP_InputField>();
            var raycaster = newObject.AddComponent<GraphicRaycaster>();


            var textArea = CreateEmptyObject(newObject, "Text Area", new Rect(0, 0, rect.width, rect.height));
            var rectMask = textArea.AddComponent<RectMask2D>();

            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
            var titleFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
            var titleFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.KanbanSelect);

            var text = CreateTextChild(textArea, "Text", new Rect(0, 0, rect.width, rect.height), "");
            var textComponent = text.GetComponent<TextMeshProUGUI>();
            textComponent.font = titleFont;
            textComponent.fontMaterial = titleFontMaterial;
            textComponent.alignment = TextAlignmentOptions.Left;

            var placeholder = CreateTextChild(textArea, "Placeholder", new Rect(0, 0, rect.width, rect.height), "Enter text...");
            var placeHolderText = placeholder.GetComponent<TextMeshProUGUI>();

            placeHolderText.font = titleFont;
            placeHolderText.fontMaterial = titleFontMaterial;

            inputField.textViewport = textArea.GetComponent<RectTransform>();
            inputField.textComponent = text.GetComponent<TextMeshProUGUI>();
            inputField.placeholder = placeholder.GetComponent<TextMeshProUGUI>();

            inputField.enabled = false;
            inputField.enabled = true;

            return newObject;
        }

        static public Sprite LoadSprite(string spriteFilePath)
        {
            if (LoadedSprites == null)
            {
                LoadedSprites = new Dictionary<string, Sprite>();
            }
            if (LoadedSprites.ContainsKey(spriteFilePath))
            {
                return LoadedSprites[spriteFilePath];
            }
            else if (File.Exists(spriteFilePath))
            {
                LoadedSprites.Add(spriteFilePath, LoadSpriteFromFile(spriteFilePath));
                return LoadedSprites[spriteFilePath];
            }
            // otherwise, the file doesn't exist, log an error, and return null (or hopefully a small transparent sprite
            else
            {
                Plugin.LogInfo(LogType.Error, "Could not find file: " + spriteFilePath);
                // Instead of null, could I have this return just a 1x1 transparent sprite or something?

                // Creates a transparent 2x2 texture, and returns that as the sprite
                Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                Color fillColor = Color.clear;
                Color[] fillPixels = new Color[tex.width * tex.height];
                for (int i = 0; i < fillPixels.Length; i++)
                {
                    fillPixels[i] = fillColor;
                }
                tex.SetPixels(fillPixels);
                tex.Apply();

                Rect rect = new Rect(0, 0, tex.width, tex.height);
                LoadedSprites.Add(spriteFilePath, Sprite.Create(tex, rect, new Vector2(0, 0)));
                return LoadedSprites[spriteFilePath];
            }
        }
        static public GameObject GetOrCreateEmptyObject(GameObject parent, string name, Vector2 position)
        {
            var newObject = GameObject.Find(name);
            if (newObject == null)
            {
                newObject = CreateEmptyObject(parent, name, position);
            }
            return newObject;
        }

        static public GameObject GetOrCreateEmptyChild(GameObject parent, string name, Vector2 position)
        {
            var child = GetChildByName(parent, name);
            if (child == null)
            {
                child = CreateEmptyObject(parent, name, position);
            }
            return child;
        }

        static public GameObject CreateEmptyObject(GameObject parent, string name, Vector2 position)
        {
            Rect rect = new Rect(position, Vector2.zero);
            return CreateEmptyObject(parent, name, rect);
        }

        public static GameObject GetChildByName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }

        static public GameObject CreateEmptyObject(GameObject parent, string name, Rect rect)
        {
            GameObject newObject = new GameObject(name);
            if (parent != null)
            {
                newObject.transform.SetParent(parent.transform);
            }
            SetRect(newObject, rect);
            return newObject;
        }


        static public Canvas AddCanvasComponent(GameObject gameObject)
        {
            var canvasObject = gameObject.GetOrAddComponent<Canvas>();
            canvasObject.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.worldCamera = null;
            canvasObject.overrideSorting = true;

            var canvasScalerObject = gameObject.GetOrAddComponent<CanvasScaler>();
            canvasScalerObject.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScalerObject.referenceResolution = new Vector2(1920, 1080);
            canvasScalerObject.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScalerObject.matchWidthOrHeight = 0;

            return canvasObject;
        }



        #region Text

        static public GameObject CreateTextChild(GameObject parent, string name, Rect rect, string text)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var textComponent = newObject.GetOrAddComponent<TextMeshProUGUI>();
            ChangeText(newObject, text);
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMax = 1000;
            textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;

            SetRect(newObject, rect);

            return newObject;
        }

        static public void ChangeText(TextMeshProUGUI textComponent, string text)
        {
            if (textComponent == null)
            {
                return;
            }
            textComponent.text = text;
        }

        static public void ChangeText(GameObject gameObject, string text)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();

            ChangeText(textComponent, text);

            return;
        }
        static public void SetTextFontAndMaterial(GameObject gameObject, TMP_FontAsset font, Material material)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            SetTextFontAndMaterial(textComponent, font, material);
        }

        static public void SetTextFontAndMaterial(TextMeshProUGUI text, TMP_FontAsset font, Material material)
        {
            if (text != null)
            {
                text.font = font;
                text.fontSharedMaterial = material;
            }
        }

        static public void SetTextAlignment(GameObject gameObject, HorizontalAlignmentOptions horizAlignment = HorizontalAlignmentOptions.Left,
                                                                     VerticalAlignmentOptions vertAlignment = VerticalAlignmentOptions.Top)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            SetTextAlignment(textComponent, horizAlignment, vertAlignment);
        }

        static public void SetTextAlignment(TextMeshProUGUI text, HorizontalAlignmentOptions horizAlignment = HorizontalAlignmentOptions.Left,
                                                                             VerticalAlignmentOptions vertAlignment = VerticalAlignmentOptions.Top)
        {
            if (text != null)
            {
                text.horizontalAlignment = horizAlignment;
                text.verticalAlignment = vertAlignment;
            }
        }

        static public void SetTextColor(GameObject gameObject, Color color)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            textComponent.color = color;
        }

        #endregion


        #region Slider

        static public GameObject CreateSlider(GameObject parent, string name, Rect rect, int min, int max)
        {
            var newObject = CreateSlider(name, rect, parent);
            var slider = newObject.GetComponent<Slider>();

            slider.wholeNumbers = true;

            slider.minValue = min;
            slider.maxValue = max;

            return newObject;
        }

        static public GameObject CreateSlider(string name, float min, float max, Rect rect, GameObject parent)
        {
            var newObject = CreateSlider(name, rect, parent);
            var slider = newObject.GetComponent<Slider>();

            slider.wholeNumbers = false;

            slider.minValue = min;
            slider.maxValue = max;

            return newObject;
        }

        static private GameObject CreateSlider(string name, Rect rect, GameObject parent)
        {
            // Create the object
            GameObject newObject = CreateEmptyObject(parent, name, rect);

            // Add a Slider component to it
            var slider = newObject.AddComponent<Slider>();
            var raycaster = newObject.AddComponent<GraphicRaycaster>();


            // Create background child
            GameObject backgroundObject = CreateEmptyObject(newObject, "Background", new Rect(0, 0, 0, 0));
            var backgroundRect = backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);

            // Add Image component to background child
            var bgImage = backgroundObject.GetOrAddComponent<Image>();
            bgImage.color = new Color32(214, 214, 214, 255);

            // Create Fill Area object
            GameObject fillAreaObject = CreateEmptyObject(newObject, "Fill Area", new Rect(-5f, 0, -20, 0));
            var fillAreaRect = fillAreaObject.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.pivot = new Vector2(0.5f, 0.5f);

            // Create Fill object
            GameObject fillObject = CreateEmptyObject(fillAreaObject, "Fill", new Rect(0, 0, 10, 0));
            var fillImage = fillObject.GetOrAddComponent<Image>();
            fillImage.color = new Color32(255, 255, 255, 255);

            // Create Handle Slide Area object
            GameObject handleSlideAreaObject = CreateEmptyObject(newObject, "Handle Slide Area", new Rect(0, 0, -20, 0));
            var handleSlideAreaRect = handleSlideAreaObject.GetComponent<RectTransform>();
            handleSlideAreaRect.anchorMin = new Vector2(0, 0);
            handleSlideAreaRect.anchorMax = new Vector2(1, 1);
            handleSlideAreaRect.pivot = new Vector2(0.5f, 0.5f);

            // Create Handle object
            GameObject handleObject = CreateEmptyObject(handleSlideAreaObject, "Handle", new Rect(0, 0, 20, 0));
            var handleImage = handleObject.GetOrAddComponent<Image>();
            handleImage.color = new Color32(255, 255, 255, 255);

            // Assign Slider variables
            slider.targetGraphic = handleImage;
            slider.fillRect = fillObject.GetComponent<RectTransform>();
            slider.handleRect = handleObject.GetComponent<RectTransform>();

            return newObject;
        }

        #endregion

        #region Dropdown

        static public GameObject CreateDropdown(GameObject parent, string name, Rect rect, List<string> options)
        {
            GameObject dropdownObject = CreateEmptyObject(parent, name, rect);
            var dropdownImage = dropdownObject.GetOrAddComponent<Image>();
            dropdownImage.color = new Color32(255, 255, 255, 255);
            var dropdownDropdown = dropdownObject.AddComponent<TMP_Dropdown>();
            dropdownObject.AddComponent<GraphicRaycaster>();

            // Create Label
            GameObject labelObject = CreateEmptyObject(dropdownObject, "Label", new Rect(-7.5f, -0.5f, -35, -13));
            AdjustAnchors(labelObject, new Vector2(0, 0), new Vector2(1, 1));
            var labelText = labelObject.GetOrAddComponent<TextMeshProUGUI>();
            labelText.text = "Option A";
            labelText.alignment = TextAlignmentOptions.Left;
            labelText.enableAutoSizing = false;
            labelText.color = new Color32(0, 0, 0, 255);

            // Create Arrow
            GameObject arrowObject = CreateEmptyObject(dropdownObject, "Arrow", new Rect(-15, 0, 20, 20));
            AdjustAnchors(arrowObject, new Vector2(1, 0.5f), new Vector2(1, 0.5f));
            var arrowImage = arrowObject.GetOrAddComponent<Image>();
            arrowImage.color = new Color32(20, 20, 20, 40);

            // Create Template
            GameObject templateObject = CreateEmptyObject(dropdownObject, "Template", new Rect(0, 2, 0, 150));
            AdjustAnchors(templateObject, new Vector2(0, 0), new Vector2(1, 0));
            AdjustPivot(templateObject, new Vector2(0.5f, 1));
            templateObject.SetActive(false);
            var templateImage = templateObject.GetOrAddComponent<Image>();
            templateImage.color = new Color32(255, 255, 255, 255);
            var templateScrollRect = templateObject.AddComponent<ScrollRect>();
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            // Create Viewport
            GameObject viewportObject = CreateEmptyObject(templateObject, "Viewport", new Rect(0, 0, -18, 0));
            AdjustAnchors(viewportObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(viewportObject, new Vector2(0, 1));
            var viewportMask = viewportObject.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            var viewportImage = viewportObject.GetOrAddComponent<Image>();
            viewportImage.color = new Color32(255, 255, 255, 255);

            // Create Content
            GameObject contentObject = CreateEmptyObject(viewportObject, "Content", new Rect(0, 0, 0, 28));
            AdjustAnchors(contentObject, new Vector2(0, 1), new Vector2(1, 1));
            AdjustPivot(contentObject, new Vector2(0.5f, 1));

            // Create Item
            GameObject itemObject = CreateEmptyObject(contentObject, "Item", new Rect(0, 0, 0, 20));
            AdjustAnchors(itemObject, new Vector2(0, 0.5f), new Vector2(1, 0.5f));
            AdjustPivot(itemObject, new Vector2(0.5f, 0.5f));
            var itemToggle = itemObject.AddComponent<Toggle>();
            itemToggle.isOn = true;

            // Create Item Background
            GameObject itemBackgroundObject = CreateEmptyObject(itemObject, "Item Background", new Rect(0, 0, 0, 0));
            AdjustAnchors(itemBackgroundObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(itemBackgroundObject, new Vector2(0.5f, 0.5f));
            var itemBackgroundImage = itemBackgroundObject.GetOrAddComponent<Image>();
            itemBackgroundImage.color = new Color32(255, 255, 255, 255);

            // Create Item Checkmark
            GameObject itemCheckmarkObject = CreateEmptyObject(itemObject, "Item Checkmark", new Rect(10, 0, 20, 20));
            AdjustAnchors(itemCheckmarkObject, new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            AdjustPivot(itemCheckmarkObject, new Vector2(0.5f, 0.5f));
            var itemCheckmarkImage = itemCheckmarkObject.GetOrAddComponent<Image>();
            itemBackgroundImage.color = new Color32(20, 20, 20, 255);


            // Create Item Label
            GameObject itemLabelObject = CreateEmptyObject(itemObject, "Item Label", new Rect(5, -0.5f, -30, -3));
            AdjustAnchors(itemLabelObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(itemLabelObject, new Vector2(0.5f, 0.5f));
            var itemLabelText = itemLabelObject.GetOrAddComponent<TextMeshProUGUI>();
            itemLabelText.text = "Option A";
            itemLabelText.alignment = TextAlignmentOptions.Left;
            itemLabelText.enableAutoSizing = true;
            itemLabelText.color = new Color32(0, 0, 0, 255);

            // Create Scrollbar
            GameObject scrollbarObject = CreateEmptyObject(templateObject, "Scrollbar", new Rect(0, 0, 20, 0));
            AdjustAnchors(scrollbarObject, new Vector2(1, 0), new Vector2(1, 1));
            AdjustPivot(scrollbarObject, new Vector2(1, 1));
            var scrollbarImage = scrollbarObject.GetOrAddComponent<Image>();
            itemBackgroundImage.color = new Color32(255, 255, 255, 255);
            var scrollbarScrollbar = scrollbarObject.AddComponent<Scrollbar>();
            scrollbarScrollbar.direction = Scrollbar.Direction.BottomToTop;

            // Create Sliding Area
            GameObject slidingAreaObject = CreateEmptyObject(scrollbarObject, "Sliding Area", new Rect(0, 0, -20, -20));
            AdjustAnchors(slidingAreaObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(slidingAreaObject, new Vector2(0.5f, 0.5f));


            // Create Handle
            GameObject handleObject = CreateEmptyObject(slidingAreaObject, "Handle", new Rect(0, 0, 20, 20));
            AdjustAnchors(handleObject, new Vector2(0, 0), new Vector2(1, 0.2f));
            AdjustPivot(handleObject, new Vector2(0.5f, 0.5f));
            var handleImage = handleObject.GetOrAddComponent<Image>();
            itemBackgroundImage.color = new Color32(255, 255, 255, 255);


            // Connect it all together
            dropdownDropdown.template = templateObject.GetComponent<RectTransform>();
            dropdownDropdown.captionText = labelText;
            dropdownDropdown.itemText = itemLabelText;

            templateScrollRect.content = contentObject.GetComponent<RectTransform>();
            templateScrollRect.viewport = viewportObject.GetComponent<RectTransform>();
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;

            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;

            scrollbarScrollbar.handleRect = handleObject.GetComponent<RectTransform>();


            // Fill Content
            dropdownDropdown.AddOptions(options);

            return dropdownObject;
        }

        #endregion


        #region Image

        static public GameObject GetOrCreateImageChild(GameObject parent, string name, Vector2 position, string spriteFilePath)
        {
            var imageChild = GetChildByName(parent, name);
            if (imageChild == null)
            {
                imageChild = CreateImageChild(parent, name, position, spriteFilePath);
            }
            else
            {
                imageChild.GetOrAddComponent<Image>().sprite = LoadSprite(spriteFilePath);
            }
            return imageChild;
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Color32 color)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = newObject.GetOrAddComponent<Image>();
            image.color = color;

            return newObject;
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Vector2 position, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            return CreateImageChild(parent, name, position, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            return CreateImageChild(parent, name, rect, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Vector2 position, Sprite sprite)
        {
            Rect rect = new Rect(position, new Vector2(sprite.rect.width, sprite.rect.height));
            return CreateImageChild(parent, name, rect, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Sprite sprite)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = newObject.GetOrAddComponent<Image>();
            image.sprite = sprite;

            return newObject;
        }

        static public void ChangeImageColor(GameObject gameObject, Color32 color)
        {
            var image = GetOrAddImageComponent(gameObject);
            image.color = color;
        }

        static public Image GetOrAddImageComponent(GameObject gameObject)
        {
            var imageObject = gameObject.GetComponent<Image>();
            if (imageObject == null)
            {
                imageObject = gameObject.AddComponent<Image>();
            }

            return imageObject;
        }

        static private Sprite LoadSpriteFromFile(string spriteFilePath)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            if (!File.Exists(spriteFilePath))
            {
                Plugin.Log.LogError("Could not find file: " + spriteFilePath);
                //return null;
            }
            else
            {
                tex.LoadImage(File.ReadAllBytes(spriteFilePath));
            }


            Rect rect = new Rect(0, 0, tex.width, tex.height);
            return Sprite.Create(tex, rect, new Vector2(0, 0));
        }

        static public Image ChangeImageSprite(GameObject gameObject, string spriteFilePath)
        {
            var image = GetOrAddImageComponent(gameObject);
            return ChangeImageSprite(image, spriteFilePath);
        }

        static public Image ChangeImageSprite(GameObject gameObject, Sprite sprite)
        {
            var image = GetOrAddImageComponent(gameObject);
            return ChangeImageSprite(image, sprite);
        }

        static public Image ChangeImageSprite(Image image, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            if (sprite == null)
            {
                return image;
            }
            return ChangeImageSprite(image, sprite);
        }

        static public Image ChangeImageSprite(Image image, Sprite sprite)
        {
            image.sprite = sprite;
            return image;
        }



        #endregion


        #region RectTransform

        // This feels kinda repetitive, but I think it's fine
        static public RectTransform SetRect(GameObject gameObject, Vector2 position)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            return rectTransform;
        }
        static public RectTransform SetRect(GameObject gameObject, Rect rect)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            return rectTransform;
        }
        static public RectTransform SetRect(GameObject gameObject, Rect rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rectTransform = SetRect(gameObject, rect);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            return rectTransform;
        }
        static public void SetRect(GameObject gameObject, Rect rect, Vector2 pivot)
        {
            var rectTransform = SetRect(gameObject, rect);
            rectTransform.pivot = pivot;
        }
        static public void SetRect(GameObject gameObject, Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            var rectTransform = SetRect(gameObject, rect, anchorMin, anchorMax);
            rectTransform.pivot = pivot;
        }

        static private void AdjustAnchors(GameObject gameObject, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        static private void AdjustPivot(GameObject gameObject, Vector2 pivot)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.pivot = pivot;
        }

        #endregion
    }

    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T t))
            {
                return t;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
    }

    public class UIButton : Button
    {
        public UnityEngine.Events.UnityEvent onRightClick = new UnityEngine.Events.UnityEvent();
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Invoke the left click event
                base.OnPointerClick(eventData);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Invoke the right click event
                onRightClick.Invoke();
            }
        }
    }
}
