using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player controls for platformer demo
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{

    public class PlayerControls : MonoBehaviour
    {
        public int player_id;
        public KeyCode attack_key;
        public KeyCode attack_key2;
        public KeyCode action_key;
        public KeyCode action_key2;
        public KeyCode menu_key;

        [HideInInspector]
        public bool disable_controls = false;

        private Vector2 move = Vector2.zero;
        private bool attack_press = false;
        private bool attack_hold = false;
        private bool action_press = false;
        private bool action_hold = false;
        private bool menu_press = false;

        private static Dictionary<int, PlayerControls> controls = new Dictionary<int, PlayerControls>();

        void Awake()
        {
            controls[player_id] = this;
        }

        void OnDestroy()
        {
            controls.Remove(player_id);
        }

        void Update()
        {
            move = Vector2.zero;
            attack_hold = false;
            attack_press = false;
            action_hold = false;
            action_press = false;
            menu_press = false;

            if (disable_controls)
                return;

            if (Input.GetKey(KeyCode.A))
                move += -Vector2.right;
            if (Input.GetKey(KeyCode.D))
                move += Vector2.right;
            if (Input.GetKey(KeyCode.W))
                move += Vector2.up;
            if (Input.GetKey(KeyCode.S))
                move += -Vector2.up;

            if (Input.GetKey(KeyCode.LeftArrow))
                move += -Vector2.right;
            if (Input.GetKey(KeyCode.RightArrow))
                move += Vector2.right;
            if (Input.GetKey(KeyCode.UpArrow))
                move += Vector2.up;
            if (Input.GetKey(KeyCode.DownArrow))
                move += -Vector2.up;

            if (Input.GetKey(attack_key))
                attack_hold = true;
            if (Input.GetKeyDown(attack_key))
                attack_press = true;
            if (Input.GetKey(attack_key2))
                attack_hold = true;
            if (Input.GetKeyDown(attack_key2))
                attack_press = true;

            if (Input.GetKey(action_key))
                action_hold = true;
            if (Input.GetKeyDown(action_key))
                action_press = true;
            if (Input.GetKey(action_key2))
                action_hold = true;
            if (Input.GetKeyDown(action_key2))
                action_press = true;
            if (Input.GetKeyDown(menu_key))
                menu_press = true;

            float move_length = Mathf.Min(move.magnitude, 1f);
            move = move.normalized * move_length;
        }


        //------ These functions should be called from the Update function, not FixedUpdate
        public Vector2 GetMove()
        {
            return move;
        }

        public bool GetAttackDown()
        {
            return attack_press;
        }

        public bool GetAttackHold()
        {
            return attack_hold;
        }

        public bool GetActionDown()
        {
            return action_press;
        }

        public bool GetActionHold()
        {
            return action_hold;
        }

        public bool GetMenuDown()
        {
            return menu_press;
        }

        //-----------

        public static PlayerControls Get(int player_id)
        {
            foreach (PlayerControls control in GetAll())
            {
                if (control.player_id == player_id)
                {
                    return control;
                }
            }
            return null;
        }

        public static PlayerControls[] GetAll()
        {
            PlayerControls[] list = new PlayerControls[controls.Count];
            controls.Values.CopyTo(list, 0);
            return list;
        }

    }

}