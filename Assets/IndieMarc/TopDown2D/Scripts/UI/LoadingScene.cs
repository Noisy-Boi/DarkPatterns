using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{

    public class LoadingScene : MonoBehaviour
    {
        void Start()
        {
            SceneNav.GoTo(SceneNav.load_scene);
        }

        void Update()
        {

        }
    }
}