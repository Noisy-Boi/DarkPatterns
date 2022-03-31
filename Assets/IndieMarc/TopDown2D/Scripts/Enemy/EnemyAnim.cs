using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyAnim : MonoBehaviour
    {
        public GameObject death_fx_prefab;
        
        private Animator animator;
        private SpriteRenderer render;
        private Enemy enemy;
        private float flash_fx_timer;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            render = GetComponentInChildren<SpriteRenderer>();
            enemy = GetComponent<Enemy>();

            enemy.onDeath += OnDeath;
            enemy.onHit += OnDamage;
        }

        void Update()
        {
            if (enemy.IsDead())
            {
                float alpha = render.color.a - 1f * Time.deltaTime;
                render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("Move", enemy.IsMoving());
                }

                //Hit flashing
                render.color = new Color(render.color.r, render.color.g, render.color.b, 1f);
                if (flash_fx_timer > 0f)
                {
                    flash_fx_timer -= Time.deltaTime;
                    float alpha = Mathf.Abs(Mathf.Sin(flash_fx_timer * Mathf.PI * 4f));
                    render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
                }
            }
        }

        void OnDamage()
        {
            if (!enemy.IsDead())
                flash_fx_timer = 1f;
        }

        private void OnDeath()
        {
            if(death_fx_prefab)
                Instantiate(death_fx_prefab, transform.position, death_fx_prefab.transform.rotation);

            if (animator != null)
                animator.SetTrigger("Death");
        }
    }
}
