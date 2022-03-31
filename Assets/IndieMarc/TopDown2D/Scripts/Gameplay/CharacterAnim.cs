using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
    [RequireComponent(typeof(PlayerCharacter))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnim : MonoBehaviour
    {
        public GameObject attack_fx;
        public float attack_fx_offset = 1f;
        public GameObject attack_hit_fx;
        public GameObject death_fx;

        private PlayerCharacter character;
        private SpriteRenderer render;
        private Animator animator;
        private float flash_fx_timer;

        void Awake()
        {
            character = GetComponent<PlayerCharacter>();
            render = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponent<Animator>();
            
            character.onHit += OnDamage;
            character.onDeath += OnDeath;
            character.onAttack += OnAttack;
            character.onAttackHit += onAttackHit;
            character.onJump += OnJump;
        }

        private void Start()
        {
            TheGame.Get().onPause += (bool paused) => { animator.Rebind(); };
        }

        void Update()
        {
            if (TheGame.Get().IsPaused())
                return;

            //Anims
            animator.SetBool("Move", character.GetMove().magnitude > 0.1f);
            animator.SetFloat("MoveX", character.GetAnimFacing().x);
            animator.SetFloat("MoveY", character.GetAnimFacing().y);
            animator.SetBool("Climb", character.IsClimbing());

            //Hit flashing
            render.color = new Color(render.color.r, render.color.g, render.color.b, 1f);
            if (flash_fx_timer > 0f)
            {
                flash_fx_timer -= Time.deltaTime;
                float alpha = Mathf.Abs(Mathf.Sin(flash_fx_timer * Mathf.PI * 4f));
                render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
            }
        }

        void OnAttack()
        {
            animator.SetTrigger("Attack");
            Vector3 facing = character.GetFacing();
            float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
            Instantiate(attack_fx, character.center.transform.position + facing * attack_fx_offset, Quaternion.Euler(0f, 0f, angle));
        }

        void onAttackHit(GameObject enemy)
        {
            Vector3 facing = character.GetFacing();
            float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
            Instantiate(attack_hit_fx, enemy.transform.position, Quaternion.Euler(0f, 0f, angle));
        }

        void OnJump()
        {
            animator.SetTrigger("Jump");
        }

        void OnDamage()
        {
            if (!character.IsDead())
                flash_fx_timer = 1f;
        }

        void OnDeath()
        {
            render.sortingOrder = 1;
            animator.SetTrigger("Death");
            if(death_fx != null)
                Instantiate(death_fx, transform.position, death_fx.transform.rotation);
        }
    }

}