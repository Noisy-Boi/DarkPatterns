using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{

    public class Ladder : MonoBehaviour
    {

        private Collider2D collide;

        private static List<Ladder> ladder_list = new List<Ladder>();

        void Awake()
        {
            ladder_list.Add(this);
            collide = GetComponent<Collider2D>();
        }

        void OnDestroy()
        {
            ladder_list.Remove(this);
        }

        void Update()
        {

        }

        public static Ladder GetOverlap(GameObject other, float radius=1f)
        {
            foreach (Ladder ladder in ladder_list)
            {
                if (ladder.collide.OverlapPoint(other.transform.position)
                    || ladder.collide.OverlapPoint(other.transform.position + Vector3.up * radius)
                    || ladder.collide.OverlapPoint(other.transform.position + Vector3.right * radius)
                    || ladder.collide.OverlapPoint(other.transform.position + Vector3.left * radius)
                    || ladder.collide.OverlapPoint(other.transform.position + Vector3.down * radius))
                    return ladder;
            }
            return null;
        }

        public static List<Ladder> GetAll()
        {
            return ladder_list;
        }
    }

}