using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
    public class CharacterHand : MonoBehaviour {

        [Header("Controlled by animation")]
        public int order_in_layer;
        public bool collider_enabled;

        private PlayerCharacter character;
        private SpriteRenderer weapon_render;
        private Collider2D weapon_collide;

        void Start() {
            character = GetComponentInParent<PlayerCharacter>();
            RefreshWeapon();
        }

        void Update() {
            if(weapon_render != null)
                weapon_render.sortingOrder = order_in_layer;
            if (weapon_collide != null)
                weapon_collide.enabled = collider_enabled;
        }

        //Call this after changing the weapon
        public void RefreshWeapon()
        {
            weapon_render = GetComponentInChildren<SpriteRenderer>();
            weapon_collide = GetComponentInChildren<Collider2D>();
        }
    }
}
