using UnityEditor;
using UnityEngine.UIElements;

namespace JZ.Common.Editor
{
    /// <summary>
    /// Some helpful visual exlement extensions
    /// </summary>
    public static class VEExtensions
    {
        /// <summary>
        /// Adds a uxml visual tree and sets a style sheet from a given folder to the visual element
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="folderName"></param>
        /// <param name="uxmlName"></param>
        /// <param name="cssName"></param>
        public static void SetUxmlAndUss(this VisualElement ve, string folderName, string uxmlName, string cssName)
        {
            ve.SetUxml(folderName, uxmlName);
            ve.SetUss(folderName, cssName);
        }

        /// <summary>
        /// Adds a uxml visual tree from a given folder to the visual element
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="folderName"></param>
        /// <param name="uxmlName"></param>
        public static void SetUxml(this VisualElement ve, string folderName, string uxmlName)
        {
            var assetPath = GetPath(folderName);
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{assetPath}/{uxmlName}");
            uxml.CloneTree(ve);
        }

        /// <summary>
        /// Sets a stylesheet from a given folder to the visual element
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="folderName"></param>
        /// <param name="cssName"></param>
        public static void SetUss(this VisualElement ve, string folderName, string cssName)
        {
            var assetPath = GetPath(folderName);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{assetPath}/{cssName}");
            ve.styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Returns the asset path to the referenced folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private static string GetPath(string folderName)
        {
            return AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(folderName)[0]);
        }
    }
}