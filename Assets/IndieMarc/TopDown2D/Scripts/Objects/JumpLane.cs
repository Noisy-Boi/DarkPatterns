using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{

    public class JumpLane : MonoBehaviour
    {

        public float start_radius = 1f;
        public Transform start_pos;
        public Transform end_pos;

        [HideInInspector]
        public Vector3 jump_dir;

        private Collider2D collide;

        private static List<JumpLane> lane_list = new List<JumpLane>();

        void Awake()
        {
            lane_list.Add(this);
            collide = GetComponent<Collider2D>();
            jump_dir = (end_pos.position - start_pos.position).normalized;
        }

        void OnDestroy()
        {
            lane_list.Remove(this);
        }

        public static JumpLane GetOverlap(GameObject other, float radius = 1f)
        {
            foreach (JumpLane lane in lane_list)
            {
                if (lane.collide.OverlapPoint(other.transform.position)
                    || lane.collide.OverlapPoint(other.transform.position + Vector3.up * radius)
                    || lane.collide.OverlapPoint(other.transform.position + Vector3.right * radius)
                    || lane.collide.OverlapPoint(other.transform.position + Vector3.left * radius)
                    || lane.collide.OverlapPoint(other.transform.position + Vector3.down * radius))
                    return lane;
            }
            return null;
        }

        public bool IsInRadius(GameObject obj)
        {
            float dist = (obj.transform.position - start_pos.position).magnitude;
            return dist < start_radius;
        }
    }

}