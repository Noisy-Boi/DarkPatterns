using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace IndieMarc.DialogueSystem
{
    public class Region : MonoBehaviour
    {
        public string trigger_tag = ""; //Keep empty for any tag
        public int trigger_limit = 0; //0 means infinite, trigger limit is per session (not saved)

        [HideInInspector]
        public event UnityAction<GameObject> OnEnterRegion;
        [HideInInspector]
        public event UnityAction<GameObject> OnExitRegion;
        
        private int nb_in_region;
        private Bounds bounds;
        private bool in_region;

        private static List<Region> region_list = new List<Region>();

        private void Awake()
        {
            region_list.Add(this);
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
            bounds = GetComponent<BoxCollider2D>().bounds;
            in_region = false;
            nb_in_region = 0;
        }
        
        private void OnDestroy()
        {
            region_list.Remove(this);
        }

        private void TriggerRegionEffects(GameObject triggerer)
        {
            //Custom event
            if (OnEnterRegion != null)
            {
                OnEnterRegion.Invoke(triggerer);
            }
        }

        private void TriggerRegionEffectsExit(GameObject triggerer)
        {
            //Custom events
            if (OnExitRegion != null)
            {
                OnExitRegion.Invoke(triggerer);
            }
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.tag == trigger_tag || trigger_tag == "")
            {
                nb_in_region++;
                in_region = (nb_in_region >= trigger_limit);

                if (in_region)
                {
                    TriggerRegionEffects(coll.gameObject);
                }
            }
        }

        void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.gameObject.tag == trigger_tag || trigger_tag == "")
            {
                nb_in_region--;
                in_region = (nb_in_region >= trigger_limit);

                if (!in_region)
                {
                    TriggerRegionEffectsExit(coll.gameObject);
                }
            }
        }

        public bool IsActivated()
        {
            return in_region;
        }

        public bool IsInside(GameObject obj)
        {
            return IsInside(obj.transform.position);
        }

        public bool IsInside(Vector3 position)
        {
            return (position.x > bounds.min.x && position.y > bounds.min.y && position.x < bounds.max.x && position.y < bounds.max.y);
        }

        public Vector2 GetCoords(Vector3 point)
        {
            //Return coords inside the region from 0 to 1 
            float x = (point.x - bounds.min.x) / (bounds.max.x - bounds.min.x);
            float y = (point.y - bounds.min.y) / (bounds.max.y - bounds.min.y);
            return new Vector2(x, y);
        }

        public Vector3 PickRandomPosInside()
        {
            float pos_x = Random.Range(bounds.min.x, bounds.max.x);
            float pos_y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector3(pos_x, pos_y, 0f);
        }

        public static Region[] GetAll()
        {
            return region_list.ToArray();
        }
    }

}