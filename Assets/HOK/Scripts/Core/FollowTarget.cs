using UnityEngine;

namespace HOK.Core
{
    /// <summary>
    /// Simple script to follow a target transform with an offset.
    /// Used for characters that need to move with the raft without being children.
    /// </summary>
    [ExecuteAlways]
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private bool followRotation;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = target.position + offset;

            if (followRotation)
            {
                transform.rotation = target.rotation;
            }
        }

        /// <summary>
        /// Sets the target to follow.
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Sets the offset from the target.
        /// </summary>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
    }
}
