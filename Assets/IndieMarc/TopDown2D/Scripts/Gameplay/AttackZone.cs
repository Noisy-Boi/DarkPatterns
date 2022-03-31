using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
    public class AttackZone : MonoBehaviour
    {

        private PlayerCharacter character;

        private void Awake()
        {
            character = GetComponentInParent<PlayerCharacter>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            character.OnSwordHit(collision);

        }
    }
}
