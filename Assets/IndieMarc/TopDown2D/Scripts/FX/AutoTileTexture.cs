using UnityEngine;
using System.Collections;

namespace IndieMarc.TopDown
{

    public class AutoTileTexture : MonoBehaviour
    {
        public float scale = 1f;
        public string sorting_layer = "Default";
        public int sorting_order = 0;

        private MeshRenderer render;
        private Material material;
        
        void Awake()
        {
            render = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            ResizeMaterial();
        }

        void Update()
        {

        }

        void ResizeMaterial()
        {
            render.sortingLayerName = sorting_layer;
            render.sortingOrder = sorting_order;
            if (material == null)
            {
                material = new Material(render.sharedMaterial);
                render.material = material;
            }
            material.SetTextureScale("_MainTex", new Vector2(this.gameObject.transform.lossyScale.x * scale, this.gameObject.transform.lossyScale.y * scale));
        }

        void OnDrawGizmos()
        {
            if(render == null)
                render = GetComponent<MeshRenderer>();
            ResizeMaterial();
        }
    }
}