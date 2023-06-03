using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using JZ.Common;
using JZ.Common.Editor;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using UnityEditor.UIElements;
using Unity.EditorCoroutines.Editor;


namespace JZ.TreeViewer.Editor
{

    /// <summary>
    /// Displays a tree system, their branches, and which nodes are currently active
    /// </summary>
    public class TreeViewerTool : EditorWindow
    {
        private PanAndZoomView blockContainer;
        private Label treeName;
        private ToolbarMenu saveMenu;
        private VisualElement settingsMenu;
        private TreeSettingManager settingManager;
        private TransitionLog transitionLog;

        private const string layoutKey = "Saved Tree Layouts";
        private ITreeViewer activeTree;
        private string treeTypeName => activeTree.GetType().ToString();
        private TNodeBlock rootBlock => TNodeBlock.currentRoot;
        private List<TNodeBlock> nodeBlocks = new List<TNodeBlock>();
        public Dictionary<string, TreeLayout> savedLayouts = new Dictionary<string, TreeLayout>();
        private bool created = false;


        #region //Set up Window
        [MenuItem("Tools/Tree Viewer")]
        private static void ShowWindow()
        {
            TreeViewerTool window = GetWindow<TreeViewerTool>();

            //Closes the active instance in the event of a failure to create the window
            if(!window.created) 
            {
                window.Close();
            }
            else
            {
                window.titleContent = new GUIContent("Tree Viewer");
                EditorUtility.SetDirty(window);
            }
        }

        private void CreateGUI()
        {
            try
            {
                //Set up UI
                rootVisualElement.SetUxmlAndUss("Tree viewer uxml and uss", "Tree Viewer.uxml", "Tree Viewer Tool Style.uss");
                treeName = rootVisualElement.Q<Label>("tree-name");
                blockContainer = rootVisualElement.Q<PanAndZoomView>("block-container");
                var log = rootVisualElement.Q<ListView>("transition-log");
                var maxCount = rootVisualElement.Q<IntegerField>("max-log-count");
                transitionLog = new TransitionLog(log, maxCount);

                //Initialize View
                LoadLayoutPrefs();
                InitializeToolbar();
                InitializeSettings();

                //Must be done on a delay to give the ui time to update itself
                EditorCoroutineUtility.StartCoroutine(DelayedSetUp(), this);
                created = true;
            }
            catch(Exception e)
            {
                Debug.LogError($"{e.Message}, {e.StackTrace}");
            }
        }

        
        private IEnumerator DelayedSetUp()
        {
            yield return null;
            CheckSelection();
            if(nodeBlocks.Count == 0)
            {
                ShowTestTree();
            }
        }
        #endregion

        private void OnGUI()
        {
            if(Event.current.keyCode == KeyCode.R)
            {
                ResetLayout();
            }
            else if(Event.current.keyCode == KeyCode.F)
            {
                blockContainer.ResetView();
            }
        }

        private void Update()
        {
            if(rootBlock != null && rootBlock.IsNodeDirty())
            {
                UpdateAllBlocks();
                transitionLog.AddEntry(rootBlock.FindNewActive());
            }
        }

        private void OnFocus()
        {
            CheckSelection();
        }

        #region //Creating the Node Diagram
        private void ShowTestTree()
        {
            SetNewTree(new TestTree("Test Tree", blockContainer));
        }

        // Called when clicking on an object in the editor
        private void OnSelectionChange()
        {
            CheckSelection();
        }

        private void CheckSelection()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }

            if (Selection.activeGameObject.TryGetComponent<ITreeViewer>(out ITreeViewer tree))
            {
                if(tree != activeTree)
                {
                    SetNewTree(tree);
                }
            }
        }

        /// <summary>
        /// Creates view for the designated Tree in the tool window
        /// </summary>
        /// <param name="tree"></param>
        private void SetNewTree(ITreeViewer tree)
        {
            //Safety check for trees that initialize in play mode
            IEnumerable<ITreeNodeViewer> nodes = tree.GetAllNodes();
            if(nodes == null || nodes.Count() == 0)
            {
                return;
            }

            //Reset State
            nodeBlocks.Clear();
            blockContainer.ClearElements();
            transitionLog.ResetLog();

            //Set up new tree
            activeTree = tree;
            treeName.text = tree.GetTreeName();
            foreach(ITreeNodeViewer node in nodes)
            {
                //Determine number of nodes and blocks that share this name
                int nameCount = 0;
                foreach(TNodeBlock block in nodeBlocks)
                {
                    string post = nameCount > 0 ? $" {nameCount + 1}" : "";
                    if(block.SharesNameWith(node.GetNodeName() + post))
                    {
                        nameCount++;
                    }
                }

                //Create block
                bool isRoot = nodeBlocks.Count == 0;
                TNodeBlock newBlock = new TNodeBlock(node, settingManager, isRoot, nameCount);
                blockContainer.AddElement(newBlock);
                nodeBlocks.Add(newBlock);

                //Set parent
                if(!isRoot)
                {
                    TNodeBlock parent = nodeBlocks.Where((block) => newBlock.IsChildOf(block)).First();
                    parent?.AddChildBlock(newBlock);
                }
            }
            UpdateSaveMenu();

            //Must be done on a delay to give the ui time to update itself
            EditorCoroutineUtility.StartCoroutine(DelayedReset(), this);
        }

        private IEnumerator DelayedReset()
        {
            yield return null;
            ResetLayout();
        }

        /// <summary>
        /// Shows the current currently saved layout if one exists. Otherwise shows the default layout.
        /// </summary>
        private void ResetLayout()
        {
            if(savedLayouts.ContainsKey(treeTypeName))
            {
                savedLayouts[treeTypeName].LoadLayout(blockContainer, nodeBlocks);
            }
            else
            {
                blockContainer.ResetView();
                DefaultPositionNodes();
            }

            UpdateAllBlocks();
        }

        private void DefaultPositionNodes()
        {
            Vector2 centerOffset = new Vector2(0, 150);
            rootBlock.transform.position = (blockContainer.layout.size/2 - centerOffset) - rootBlock.layout.min - rootBlock.layout.size/2;
            rootBlock.ResetChildren();
            ShowAllChildren();
        }

        private void UpdateAllBlocks()
        {
            rootBlock.UpdateBlock();
        }

        #endregion

        #region //Toolbar
        private void InitializeToolbar()
        {
            saveMenu = rootVisualElement.Q<ToolbarMenu>("save-menu");
            saveMenu.menu.AppendAction("Load Saved Layout", LoadSavedLayout);
            saveMenu.menu.AppendAction("Save Current Layout", SaveCurrentLayout);
            saveMenu.menu.AppendAction("Delete Current Layout", DeleteCurrentLayout);
            saveMenu.menu.AppendAction("Delete All Layouts", DeleteAllLayouts);

            var viewMenu = rootVisualElement.Q<ToolbarMenu>("view-menu");
            viewMenu.menu.AppendAction("Show Test Tree", (action) => ShowTestTree());
            viewMenu.menu.AppendAction("Show all nodes", ShowAllChildren);
            viewMenu.menu.AppendAction("Hide all nodes", HideAllChildren);
        }

        /// <summary>
        /// Deactivates the option to show a saved layout if one does not exist for the current tree
        /// </summary>
        private void UpdateSaveMenu()
        {
            DropdownMenuAction.Status showSaveStatus;
            if(savedLayouts.ContainsKey(treeTypeName))
            {
                showSaveStatus = DropdownMenuAction.Status.Normal;
            }
            else
            {
                showSaveStatus = DropdownMenuAction.Status.Disabled;
            }
            saveMenu.menu.AppendAction("Load Saved Layout", LoadSavedLayout, showSaveStatus);
        }

        /// <summary>
        /// Shows all child blocks in the window
        /// </summary>
        /// <param name="action"></param>
        private void ShowAllChildren(DropdownMenuAction action = null)
        {
            foreach(TNodeBlock block in nodeBlocks)
            {
                block.SetShowButton(true);
            }
            rootBlock.UpdateBlock();
        }

        /// <summary>
        /// Hides all child blocks in the window
        /// </summary>
        /// <param name="action"></param>
        private void HideAllChildren(DropdownMenuAction action)
        {
            foreach(TNodeBlock block in nodeBlocks)
            {
                block.SetShowButton(false);
            }
            rootBlock.UpdateBlock();
        }
        #endregion

        #region //Settings
        private void InitializeSettings()
        {
            var settingsButton = rootVisualElement.Q<Button>("settings-button");
            var resetSettingsButton = rootVisualElement.Q<Button>("reset-settings-button");
            settingsMenu = rootVisualElement.Q<VisualElement>("settings-menu");
            settingsButton.clicked += ToggleSettingsMenu;
            resetSettingsButton.clicked += ResetSettings;
            ToggleSettingsMenu();

            settingManager = new TreeSettingManager(this);
            settingManager.AddSettingEvent(TreeSetting.xGap, PositionSettingsChange);
            settingManager.AddSettingEvent(TreeSetting.yGap, PositionSettingsChange);
            settingManager.AddSettingEvent(TreeSetting.showActive, UpdateAllBlocks);
        }

        private void ResetSettings()
        {
            settingManager.ResetSettings();
        }

        private void ToggleSettingsMenu()
        {
            settingsMenu.visible = !settingsMenu.visible;
        }

        private void PositionSettingsChange()
        {
            rootBlock.ResetChildren();
            ShowAllChildren();
        }
        #endregion

        #region //Saved layouts
        private void LoadLayoutPrefs()
        {
            if (EditorPrefs.HasKey(layoutKey))
            {
                string json = EditorPrefs.GetString(layoutKey);
                savedLayouts = JsonConvert.DeserializeObject<Dictionary<string, TreeLayout>>(json);
            }
        }

        private void LoadSavedLayout(DropdownMenuAction action)
        {
            savedLayouts[treeTypeName].LoadLayout(blockContainer, nodeBlocks);
            UpdateAllBlocks();
        }

        private void SaveCurrentLayout(DropdownMenuAction action)
        {
            savedLayouts[treeTypeName] = new TreeLayout(blockContainer);
            SaveLayoutsToPrefs();
            UpdateSaveMenu();
        }

        private void DeleteCurrentLayout(DropdownMenuAction action)
        {
            savedLayouts.Remove(treeTypeName);
            SaveLayoutsToPrefs();
            ResetLayout();
            UpdateSaveMenu();
        }

        private void DeleteAllLayouts(DropdownMenuAction action)
        {
            savedLayouts = new Dictionary<string, TreeLayout>();
            SaveLayoutsToPrefs();
            ResetLayout();
            UpdateSaveMenu();
        }

        private void SaveLayoutsToPrefs()
        {
            string json = JsonConvert.SerializeObject(savedLayouts, Formatting.Indented);
            EditorPrefs.SetString(layoutKey, json);
        }
        #endregion
    }
}