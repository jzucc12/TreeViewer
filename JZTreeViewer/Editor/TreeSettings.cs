using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using JZ.Common.Editor;


namespace JZ.TreeViewer.Editor
{
    /// <summary>
    /// Holds all settings for the Tree viewer
    /// </summary>
    public class TreeSettingManager
    {
        public Dictionary<TreeSetting, IJZToolSetting> settings { get; private set; } = new Dictionary<TreeSetting, IJZToolSetting>();
        private JZFloatSetting xGapField;
        private JZFloatSetting yGapField;
        private JZToggleSetting showActiveNodeField;
        private JZColorSetting inactiveLineColorField;
        private JZColorSetting activeLineColorField;
        private JZFloatSetting lineThicknessField;


        public TreeSettingManager(TreeViewerTool window)
        {
            SerializedObject so = new SerializedObject(window);

            //Get setting fields
            xGapField = window.rootVisualElement.Q<JZFloatSetting>("x-gap");
            yGapField = window.rootVisualElement.Q<JZFloatSetting>("y-gap");
            showActiveNodeField = window.rootVisualElement.Q<JZToggleSetting>("show-active-node");
            inactiveLineColorField = window.rootVisualElement.Q<JZColorSetting>("inactive-line-color");
            activeLineColorField = window.rootVisualElement.Q<JZColorSetting>("active-line-color");
            lineThicknessField = window.rootVisualElement.Q<JZFloatSetting>("line-thickness");

            //Initialize settings
            settings.Add(TreeSetting.xGap, xGapField);
            settings.Add(TreeSetting.yGap, yGapField);
            settings.Add(TreeSetting.showActive, showActiveNodeField);
            settings.Add(TreeSetting.inactiveColor, inactiveLineColorField);
            settings.Add(TreeSetting.activeColor, activeLineColorField);
            settings.Add(TreeSetting.lineThickness, lineThicknessField);
            foreach(var setting in settings.Values)
            {
                setting.Init();
            }
        }

        public void ResetSettings()
        {
            foreach(IJZToolSetting setting in settings.Values)
            {
                setting.ResetSetting();
            }
        }

        #region //Setting events
        public void AddSettingEvent(TreeSetting key, Action action)
        {
            settings[key].OnValueChange += action;
        }

        public void RemoveSettingEvent(TreeSetting key, Action action)
        {
            settings[key].OnValueChange -= action;
        }
        #endregion

        #region //Getters
        public float GetXGap()
        {
            return xGapField.value;
        }

        public float GetYGap()
        {
            return yGapField.value;
        }

        public Color GetActiveColor(bool active)
        {
            return active ? activeLineColorField.value : inactiveLineColorField.value;
        }

        public float GetLineThickness()
        {
            return lineThicknessField.value;
        }

        public bool GetShowActiveNode()
        {
            return showActiveNodeField.value;
        }
        #endregion
    }

    /// <summary>
    /// Setting options for the tool
    /// </summary>
    public enum TreeSetting
    {
        xGap,
        yGap,
        inactiveColor,
        activeColor,
        lineThickness,
        showActive
    }
}