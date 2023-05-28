using System;
using UnityEngine;
using UnityEngine.UI;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Button that toggles between two states
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private bool startValue = true;
        [SerializeField] private Color onColor;
        [SerializeField] private string onText;
        [SerializeField] private Color offColor;
        [SerializeField] private string offText;
        private Button button;
        private Text text;
        public bool isOn { get; private set; }
        public event Action<bool> onToggle;


        private void Awake()
        {
            isOn = startValue;
            button = GetComponent<Button>();
            text = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            onToggle?.Invoke(isOn);
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ToggleState);
        }

        private void ToggleState()
        {
            isOn = !isOn;
            if(isOn)
            {
                button.image.color = onColor;
                text.text = onText;
            }
            else
            {
                button.image.color = offColor;
                text.text = offText;
            }
            onToggle?.Invoke(isOn);
        }
    }
}