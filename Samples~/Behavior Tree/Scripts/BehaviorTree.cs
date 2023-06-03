using System.Collections.Generic;
using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    public class BehaviorTree : MonoBehaviour, ITreeViewer
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float chaseRange = 5;
        [SerializeField] private Transform player;
        [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
        private BTNode root;
        private float myAngle = 0;


        private void Awake()
        {
            var detect = new SelectorNode("Detect", this);
            root = new RepeaterNode("Root", this, detect);

            var found = new SequenceNode("Found", this);
            {
                var wait = new WaitForNode("Wait", this, 0.3f);
                found.AddChild(wait);
                var chase = new MoveToNode("Chase", this, player, 0f);
                var chaseLoop = new RepeaterNode("Chase Loop", this, chase);
                found.AddChild(chaseLoop);
                found.ChangeEnterCondition(InRange);
            }
            detect.AddChild(found);

            var route = new SequenceNode("Route", this);
            {
                var waitToPatrol = new WaitForNode("Wait to patrol", this, 1);
                route.AddChild(waitToPatrol);

                for(int ii = 0; ii < patrolPoints.Count; ii++)
                {
                    var rotate = new RotateToNode($"Face Point {ii}", this, patrolPoints[ii], 2f);
                    route.AddChild(rotate);

                    var patrolTo = new MoveToNode($"Move to Point {ii}", this, patrolPoints[ii], 0f); 
                    route.AddChild(patrolTo);
                }
            }
            var patrol = new RepeaterNode("Patrol", this, route);
            detect.AddChild(patrol);
            root.EnterNode();
        }

        private void Update()
        {
            root.NodeTick();
        }

        private void FixedUpdate()
        {
            root.FixedTick();
        }

        public bool RotateTowards(Transform target, float tolerance)
        {
            Vector2 targetDirection = target.position - transform.position;
            float angleToTarget = Vector2.SignedAngle(Vector2.up, targetDirection);
            float newAngle = Mathf.MoveTowardsAngle(myAngle, angleToTarget, rotateSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, newAngle);
            myAngle = newAngle;
            return Vector3.Angle(transform.up, targetDirection) <= tolerance;
        }

        public bool MoveTowards(Transform target, float tolerance)
        {
            RotateTowards(target, 0);
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.position = newPos;
            return Vector2.Distance(target.position, transform.position) <= tolerance;
        }

        private bool InRange()
        {
            return Vector2.Distance(player.position, transform.position) <= chaseRange;
        }

        #region //Interface specific
        public IEnumerable<ITreeNodeViewer> GetAllNodes()
        {
            Queue<BTNode> nodeQueue = new Queue<BTNode>();
            List<BTNode> returned = new List<BTNode>();

            if(root != null)
            {
                nodeQueue.Enqueue(root);
            }
            while(nodeQueue.TryDequeue(out BTNode dequeue))
            {
                if(returned.Contains(dequeue))
                {
                    continue;
                }
                returned.Add(dequeue);
                yield return dequeue;
                foreach(BTNode child in dequeue.GetChildren())
                {
                    nodeQueue.Enqueue(child);
                }
            }
        }

        public string GetTreeName()
        {
            return "Behavior Tree";
        }
        #endregion
    }
}