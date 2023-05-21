using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace JZ.Common.Editor
{
    /// <summary>
    /// Editor tool toggle settings
    /// </summary>
    public class JZToggleSetting : Toggle, IJZToolSetting
    {
        #region //Uxml Factory
        public new class UxmlFactory : UxmlFactory<JZToggleSetting, UxmlTraits> { }

        public new class UxmlTraits : Toggle.UxmlTraits
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
                var ate = ve as JZToggleSetting;
                ate.prefsKey = m_Key.GetValueFromBag(bag, cc);
            }
        }
        #endregion


        public event Action OnValueChange;
        public string prefsKey { get; set; }
        private bool defaultValue;

        public void Init()
        {
            defaultValue = value;
            RegisterCallback<ChangeEvent<bool>>(ValueChanged);
            LoadSetting();
        }

        public void ValueChanged(ChangeEvent<bool> evt)
        {
            SaveSetting();
            OnValueChange?.Invoke();
        }

        public void LoadSetting()
        {
            value = EditorPrefs.GetBool(prefsKey, defaultValue);
        }

        public void SaveSetting()
        {
            EditorPrefs.SetBool(prefsKey, value);
        }
    }
}