using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{

    public enum PowerupType
    {
        None=0,
        Life=5,
        Coin=10,
    }

    public class Powerup : MonoBehaviour
    {
        public PowerupType type;
        public float value;

        public AudioClip collect_sound;

        void Start()
        {

        }
        
        void Update()
        {

        }

        public void Take(PlayerCharacter character)
        {
            PlayerData pdata = PlayerData.Get();
            if (type == PowerupType.Life)
            {
                character.HealDamage(value);
            }
            if (type == PowerupType.Coin)
            {
                pdata.coins += Mathf.RoundToInt(value);
            }

            if (TheAudio.Instance)
                TheAudio.Instance.PlaySound("powerup", collect_sound, 0.5f);

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerCharacter>())
                Take(collision.GetComponent<PlayerCharacter>());
        }
    }

}