using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace JZ.Common.Editor
{
    
    /// <summary>
    /// Editor tool float settings
    /// </summary>
    public class JZFloatSetting : FloatField, IJZToolSetting
    {
        #region //Uxml Factory
        public new class UxmlFactory : UxmlFactory<JZFloatSetting, UxmlTraits> { }

        public new class UxmlTraits : FloatField.UxmlTraits
        {
            UxmlStringAttributeDescription m_Key =
                new UxmlStringAttributeDescription { name = "prefs-key", defaultValue = "" };

            UxmlFloatAttributeDescription m_Min =
                new UxmlFloatAttributeDescription { name = "min-value", defaultValue = 0 };

            UxmlFloatAttributeDescription m_Max =
                new UxmlFloatAttributeDescription { name = "max-value", defaultValue = 1 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as JZFloatSetting;
                ate.prefsKey = m_Key.GetValueFromBag(bag, cc);
                ate.minValue = m_Min.GetValueFromBag(bag, cc);
                ate.maxValue = m_Max.GetValueFromBag(bag, cc);
            }
        }
        #endregion


        public event Action OnValueChange;
        public string prefsKey { get; set; }
        public float minValue { get; set; }
        public float maxValue { get; set; }
        private float defaultValue;

        public void Init()
        {
            defaultValue = value;
            RegisterCallback<ChangeEvent<float>>(ValueChanged);
            LoadSetting();
        }

        public void ValueChanged(ChangeEvent<float> evt)
        {
            SetValueWithoutNotify(Mathf.Clamp(evt.newValue, minValue, maxValue));
            SaveSetting();
            OnValueChange?.Invoke();
        }

        public void LoadSetting()
        {
            value = EditorPrefs.GetFloat(prefsKey, defaultValue);
        }

        public void SaveSetting()
        {
            EditorPrefs.SetFloat(prefsKey, value);
        }
    }
}