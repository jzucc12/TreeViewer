using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// UI tracker tree. Tree viewable in play mode only.
    /// </summary>
    public class UIStateTracker : MonoBehaviour, ITreeViewer
    {
        [Header("General")]
        [SerializeField] private ToggleButton modeButton;
        [SerializeField] private Slider alphaSlider;
        [SerializeField] private Toggle negativeToggle;
        [SerializeField] private Image outputShape;
        private bool inColorMode = true;

        [Header("Color Mode")]
        [SerializeField] private GameObject colorPanel;
        [SerializeField] private Slider hueSlider;
        private Color storedColor;

        [Header("Greyscale Mode")]
        [SerializeField] private GameObject greyscalePanel;
        [SerializeField] private Slider brightnessSlider;

        UIBaseState root;


        #region //Set up
        private void Awake()
        {
            storedColor = outputShape.color;
            
            root = new UIBaseState("Root");
            var colorModeState = new UIToggleState("Color Mode", modeButton, "Greyscale", "Color");
            var alphaState = new UISliderState("Alpha", alphaSlider, "Transparent", "Translucent", "Opaque");
            var negativeState = new UIToggleState("Negative", negativeToggle, "Off", "On");
            var hueState = new UISliderState("Hue", hueSlider, new (float, string)[] { (0.25f, "Low Warm"), (0.94f, "Cool"), (1, "High Warm") });
            var brightnessState = new UISliderState("Brightness", brightnessSlider, new (float, string)[] { (0.15f, "Black"), (0.79f, "Gray"), (1, "White") });

            root.AddChild(colorModeState);
            root.AddChild(alphaState);
            root.AddChild(negativeState);
            colorModeState.onState.AddChild(hueState);
            colorModeState.offState.AddChild(brightnessState);
            root.EnterState();
        }

        private void OnEnable()
        {
            modeButton.onToggle += ChangeMode;
            alphaSlider.onValueChanged.AddListener((_) => UpdateUI());
            negativeToggle.onValueChanged.AddListener((_) => UpdateUI());
            hueSlider.onValueChanged.AddListener((_) => UpdateUI());
            brightnessSlider.onValueChanged.AddListener((_) => UpdateUI());
        }
        #endregion

        #region //Update state
        private void ChangeMode(bool nowInColor)
        {
            inColorMode = nowInColor;
            colorPanel.SetActive(inColorMode);
            greyscalePanel.SetActive(!inColorMode);
            if(!inColorMode)
            {
                storedColor = outputShape.color;
            }
            UpdateUI();
        }
    
        private void UpdateUI()
        {
            Color color;
            if(inColorMode)
            {
                Color.RGBToHSV(storedColor, out float h, out float s, out float v);
                color = Color.HSVToRGB(hueSlider.value, s, v);
            }
            else
            {
                float brightness = brightnessSlider.value;
                color = new Color(brightness, brightness, brightness);
            }

            if(negativeToggle.isOn)
            {
                color = new Color(1 - color.r, 1 - color.g, 1 - color.b);
            }

            color.a = alphaSlider.value;
            outputShape.color = color;
        }
        #endregion

        #region //Interface specific
        public string GetTreeName()
        {
            return "UI Tracker";
        }

        public IEnumerable<ITreeNodeViewer> GetAllNodes()
        {
            if(root != null)
            {
                List<UIBaseState> allChildren = new List<UIBaseState>(root.GetAllChildren());
                allChildren.Insert(0, root);
                return allChildren;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
