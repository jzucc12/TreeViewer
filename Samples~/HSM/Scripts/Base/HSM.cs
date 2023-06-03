using System;
using System.Collections.Generic;
using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base HSM for the character scene
    /// </summary>
    public class HSM : MonoBehaviour, ITreeViewer
    {
        [SerializeField] private string hsmName;
        public Animator animator;
        protected BaseState root;
        public event Action<StateEvent> OnStateEvent;


        private void Update()
        {
            root.StateTick();
        }

        public void InvokeStateEvent(StateEvent evt)
        {
            OnStateEvent?.Invoke(evt);
        }

        public IEnumerable<ITreeNodeViewer> GetAllNodes()
        {
            return root;
        }

        public string GetTreeName()
        {
            return hsmName;
        }
    }
}