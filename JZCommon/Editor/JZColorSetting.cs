using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace JZ.Common.Editor
{
    
    /// <summary>
    /// Editor tool color settings
    /// </summary>
    public class JZColorSetting : ColorField, IJZToolSetting
    {
        #region //Uxml Factory
        public new class UxmlFactory : UxmlFactory<JZColorSetting, UxmlTraits> { }

        public new class UxmlTraits : ColorField.UxmlTraits
        {
            UxmlStringAttributeDescription m_Key =
                new UxmlStringAttributeDescription { name = "prefs-key", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as JZColorSetting;
                ate.prefsKey = m_Key.GetValueFromBag(bag, cc);
            }
        }
        #endregion


        public string prefsKey { get; set; }
        public event Action OnValueChange;
        private Color defaultValue;

        public void Init()
        {
            defaultValue = value;
            RegisterCallback<ChangeEvent<Color>>(ValueChanged);
            LoadSetting();
        }

        public void ValueChanged(ChangeEvent<Color> evt)
        {
            SaveSetting();
            OnValueChange?.Invoke();
        }

        public void LoadSetting()
        {
            Color color;
            if(EditorPrefs.HasKey(prefsKey))
            {
                ColorUtility.TryParseHtmlString(EditorPrefs.GetString(prefsKey), out color);
            }
            else
            {
                color = defaultValue;
            }
            value = color;
        }

        public void SaveSetting()
        {
            EditorPrefs.SetString(prefsKey, $"#{ColorUtility.ToHtmlStringRGBA(value)}");
        }
    }
}