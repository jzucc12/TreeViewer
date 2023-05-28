using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    public class DialogueOption : ITreeNodeViewer
    {
        private string optionName;
        public string playerResponse { get; private set; }
        public string bannerText { get; private set; }

        private DialogueOption parent;
        public List<DialogueOption> children { get; private set; } = new List<DialogueOption>();

        public bool IsActive { get; private set; }
        private bool dirty;


        #region //Constructors
        public DialogueOption(string optionName, string playerResponse, string bannerText)
        {
            this.optionName = optionName;
            this.playerResponse = playerResponse;
            this.bannerText = bannerText;
            IsActive = false;
        }

        public DialogueOption(string optionName, string bannerText)
        {
            this.optionName = optionName;
            this.bannerText = bannerText;
            IsActive = false;
        }
        #endregion

        #region //Node specific
        public void EnterOption()
        {
            dirty = true;
            IsActive = true;
            parent?.EnterOption();
        }


        public void ExitOption()
        {
            dirty = true;
            IsActive = false;
            foreach(DialogueOption child in children)
            {
                child.ExitOption();
            }
        }

        public void AddChild(DialogueOption child)
        {
            child.parent = this;
            children.Add(child);
        }
        #endregion

        #region //Interface specific
        public string GetNodeName()
        {
            return optionName;
        }

        public bool IsChildOf(ITreeNodeViewer node)
        {
            return parent == node;
        }

        public bool IsNodeDirty()
        {
            bool wasDirty = dirty;
            dirty = false;
            return wasDirty;
        }
        #endregion
    }
}