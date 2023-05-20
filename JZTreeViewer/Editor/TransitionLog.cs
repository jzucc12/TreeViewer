using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace JZ.TreeViewer.Editor
{
    using UnityEditor.UIElements;

    /// <summary>
    /// Keeps track of active node history of the currently shown tree view
    /// </summary>
    public class TransitionLog
    {
        private ListView log;
        private IntegerField maxCountField;
        private int maxCount;
        private int absoluteMax = 999;


        public TransitionLog(ListView log, IntegerField maxCountField)
        {
            this.log = log;
            this.maxCountField = maxCountField;
            maxCountField.RegisterCallback<ChangeEvent<int>>(UpdateMaxCount);
            ResetLog();
            SetMaxCount();
        }

        private void UpdateMaxCount(ChangeEvent<int> evt)
        {
            SetMaxCount();
        }

        private void SetMaxCount()
        {
            int newMax = maxCountField.value;
            if(newMax > absoluteMax)
            {
                maxCount = absoluteMax;
                maxCountField.SetValueWithoutNotify(absoluteMax);
            }
            else if(newMax < 0)
            {
                maxCount = 0;
                maxCountField.SetValueWithoutNotify(0);
            }
            else
            {
                maxCount = newMax;
            }
            ResizeLog(log.itemsSource);
        }

        public void ResetLog()
        {
            log.itemsSource = new List<TransitionLogData>();
            log.RefreshItems();
        }

        public void AddEntry(string nodeName)
        {
            IList items = log.itemsSource;
            items.Insert(0, new TransitionLogData(nodeName));
            ResizeLog(items);
        }

        private void ResizeLog(IList items)
        {
            while (items.Count > maxCount)
            {
                items.RemoveAt(maxCount);
            }
            log.itemsSource = items;
            log.RefreshItems();
        }



        /// <summary>
        /// Data displayed in the node history log of the tool
        /// </summary>
        private struct TransitionLogData
        {
            private string nodeName;
            private double timeInNode;
            private static double lastTime = 0;

            public TransitionLogData(string nodeName)
            {
                this.nodeName = nodeName;

                //Time is determined by taking the difference between the creation
                //of this and the most previous log data
                if(lastTime == 0)
                {
                    timeInNode = 0;
                }
                else
                {
                    timeInNode = EditorApplication.timeSinceStartup - lastTime;
                }
                lastTime = EditorApplication.timeSinceStartup;
            }

            public override string ToString()
            {
                if(timeInNode == 0)
                {
                    return $"Started in {nodeName}";
                }
                else
                {
                    return $"{nodeName} after {timeInNode.ToString("F2")}s";
                }
            }
        }
    }
}
#endif