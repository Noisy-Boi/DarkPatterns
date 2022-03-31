using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieMarc.TopDown
{

    public class GameplayUI : MonoBehaviour
    {
        public int player_id;
        public Text coins;
        public Text keys;

        void Start()
        {

        }

        void Update()
        {
            PlayerData pdata = PlayerData.Get();
            if (pdata != null)
            {
                coins.text = pdata.coins.ToString();
                keys.text = pdata.GetNbKeys(0).ToString();
            }
        }
    }

}