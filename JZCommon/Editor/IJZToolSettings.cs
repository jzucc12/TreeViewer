using System;
using UnityEditor;

namespace JZ.Common.Editor
{
    /// <summary>
    /// Interface for saving editor tool settings to EditorPrefs
    /// </summary>
    public interface IJZToolSetting
    {
        string prefsKey { get; set; }
        event Action OnValueChange;

        void Init();
        void LoadSetting();
        void SaveSetting();
        void ResetSetting()
        {
            EditorPrefs.DeleteKey(prefsKey);
            LoadSetting();
        }
    }
}