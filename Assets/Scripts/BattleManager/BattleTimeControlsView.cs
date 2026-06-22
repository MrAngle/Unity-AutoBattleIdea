using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MageFactory.BattleManager {
    internal sealed class BattleTimeControlsView : MonoBehaviour {
        private static readonly float[] SpeedPresets = {
            BattleTimeController.MinSpeedMultiplier,
            0.5f,
            BattleTimeController.DefaultSpeedMultiplier,
            2f,
            BattleTimeController.MaxSpeedMultiplier
        };

        private BattleTimeController timeController;
        private Button slowerButton;
        private Button pauseButton;
        private Button fasterButton;
        private Slider speedSlider;
        private TextMeshProUGUI pauseButtonText;
        private TextMeshProUGUI speedLabel;
        private bool updatingView;

        internal static BattleTimeControlsView create(
            RectTransform parent,
            BattleTimeController timeController) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            if (timeController == null) {
                throw new ArgumentNullException(nameof(timeController));
            }

            GameObject root = new GameObject(
                "BattleTimeControls",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(BattleTimeControlsView));

            root.transform.SetParent(parent, false);

            BattleTimeControlsView view = root.GetComponent<BattleTimeControlsView>();
            view.initialize(timeController);
            return view;
        }

        private void initialize(BattleTimeController controller) {
            timeController = controller;

            configureRoot();
            fasterButton = createButton("FasterButton", "+", 40f);
            speedSlider = createSlider();
            slowerButton = createButton("SlowerButton", "-", 40f);
            pauseButton = createButton("PauseButton", "||", 40f);
            speedLabel = createLabel("SpeedLabel", 40f);

            slowerButton.onClick.AddListener(() => moveToSpeedPreset(-1));
            pauseButton.onClick.AddListener(togglePause);
            fasterButton.onClick.AddListener(() => moveToSpeedPreset(1));
            speedSlider.onValueChanged.AddListener(setSpeedFromSlider);

            refreshView();
        }

        private void OnDestroy() {
            slowerButton?.onClick.RemoveAllListeners();
            pauseButton?.onClick.RemoveAllListeners();
            fasterButton?.onClick.RemoveAllListeners();
            speedSlider?.onValueChanged.RemoveAllListeners();
        }

        private void configureRoot() {
            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(8f, -24f);
            rectTransform.sizeDelta = new Vector2(52f, 226f);

            Image background = GetComponent<Image>();
            background.color = new Color(0.07f, 0.08f, 0.09f, 0.88f);

            VerticalLayoutGroup layout = GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 6, 6);
            layout.spacing = 6f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        private Button createButton(string name, string label, float width) {
            GameObject buttonObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));

            buttonObject.transform.SetParent(transform, false);

            LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.minWidth = width;
            layoutElement.preferredHeight = 30f;
            layoutElement.minHeight = 30f;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.19f, 0.22f, 0.25f, 0.95f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            TextMeshProUGUI text = createText(buttonObject.transform, "Text", label, 18f);
            text.fontStyle = FontStyles.Bold;

            if (name == "PauseButton") {
                pauseButtonText = text;
            }

            return button;
        }

        private Slider createSlider() {
            GameObject sliderObject = new GameObject(
                "SpeedSlider",
                typeof(RectTransform),
                typeof(Slider),
                typeof(LayoutElement));

            sliderObject.transform.SetParent(transform, false);

            LayoutElement layoutElement = sliderObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = 40f;
            layoutElement.minWidth = 40f;
            layoutElement.preferredHeight = 84f;
            layoutElement.minHeight = 84f;

            RectTransform backgroundRect = createSliderImage(
                sliderObject.transform,
                "Background",
                new Color(0.16f, 0.18f, 0.2f, 1f));
            backgroundRect.anchorMin = new Vector2(0.42f, 0f);
            backgroundRect.anchorMax = new Vector2(0.58f, 1f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            RectTransform fillAreaRect = (RectTransform)fillArea.transform;
            fillAreaRect.anchorMin = new Vector2(0.42f, 0f);
            fillAreaRect.anchorMax = new Vector2(0.58f, 1f);
            fillAreaRect.offsetMin = new Vector2(0f, 8f);
            fillAreaRect.offsetMax = new Vector2(0f, -8f);

            RectTransform fillRect = createSliderImage(
                fillArea.transform,
                "Fill",
                new Color(0.34f, 0.73f, 0.92f, 1f));
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderObject.transform, false);
            RectTransform handleAreaRect = (RectTransform)handleArea.transform;
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(0f, 8f);
            handleAreaRect.offsetMax = new Vector2(0f, -8f);

            RectTransform handleRect = createSliderImage(
                handleArea.transform,
                "Handle",
                new Color(0.93f, 0.96f, 0.98f, 1f));
            handleRect.sizeDelta = new Vector2(28f, 16f);

            Slider slider = sliderObject.GetComponent<Slider>();
            slider.minValue = BattleTimeController.MinSpeedMultiplier;
            slider.maxValue = BattleTimeController.MaxSpeedMultiplier;
            slider.wholeNumbers = false;
            slider.direction = Slider.Direction.BottomToTop;
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleRect.GetComponent<Image>();
            slider.SetValueWithoutNotify(timeController.getSpeedMultiplier());

            return slider;
        }

        private RectTransform createSliderImage(Transform parent, string name, Color color) {
            GameObject imageObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.color = color;
            return (RectTransform)imageObject.transform;
        }

        private TextMeshProUGUI createLabel(string name, float width) {
            GameObject labelObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(LayoutElement),
                typeof(TextMeshProUGUI));

            labelObject.transform.SetParent(transform, false);

            LayoutElement layoutElement = labelObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.minWidth = width;
            layoutElement.preferredHeight = 24f;

            TextMeshProUGUI text = labelObject.GetComponent<TextMeshProUGUI>();
            configureText(text, string.Empty, 16f);
            text.alignment = TextAlignmentOptions.Center;
            return text;
        }

        private static TextMeshProUGUI createText(
            Transform parent,
            string name,
            string value,
            float fontSize) {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);

            RectTransform rectTransform = (RectTransform)textObject.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            configureText(text, value, fontSize);
            text.alignment = TextAlignmentOptions.Center;
            return text;
        }

        private static void configureText(TextMeshProUGUI text, string value, float fontSize) {
            text.text = value;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
        }

        private void setSpeedFromSlider(float speedMultiplier) {
            if (updatingView) {
                return;
            }

            timeController.setSpeedMultiplier(speedMultiplier);
            refreshView();
        }

        private void togglePause() {
            timeController.togglePause();
            refreshView();
        }

        private void moveToSpeedPreset(int direction) {
            float currentSpeed = timeController.getSpeedMultiplier();
            int targetPresetIndex = getTargetPresetIndex(currentSpeed, direction);
            timeController.setSpeedMultiplier(SpeedPresets[targetPresetIndex]);
            refreshView();
        }

        private static int getTargetPresetIndex(float currentSpeed, int direction) {
            if (direction > 0) {
                for (int i = 0; i < SpeedPresets.Length; i++) {
                    if (SpeedPresets[i] > currentSpeed + 0.001f) {
                        return i;
                    }
                }

                return SpeedPresets.Length - 1;
            }

            for (int i = SpeedPresets.Length - 1; i >= 0; i--) {
                if (SpeedPresets[i] < currentSpeed - 0.001f) {
                    return i;
                }
            }

            return 0;
        }

        private void refreshView() {
            updatingView = true;
            speedSlider.SetValueWithoutNotify(timeController.getSpeedMultiplier());
            updatingView = false;

            pauseButtonText.text = timeController.isPaused() ? ">" : "||";
            speedLabel.text = timeController.isPaused()
                ? "PAUSE"
                : $"{timeController.getSpeedMultiplier():0.0}x";
        }
    }
}