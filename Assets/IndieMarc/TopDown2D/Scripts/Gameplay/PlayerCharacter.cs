using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Top Down character movement
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{
    public enum PlayerCharacterState
    {
        Normal=0,
        Climb = 10,
        Jump=20,
        Dead=50,
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class PlayerCharacter : MonoBehaviour
    {
        public int player_id;

        [Header("Stats")]
        public float max_hp = 100f;
        public float attack_damage = 1f;
        
        [Header("Movement")]
        public float move_accel = 20f;
        public float move_deccel = 20f;
        public float move_max = 5f;
        public float attack_duration = 0.2f;
        public float climb_speed = 2f;
        public float jump_speed = 5f;
        public LayerMask raycast_mask = ~(0); //All bit 1
        public float raycast_dist = 0.1f;

        [Header("References")]
        public Transform center;
        public Transform hand;

        public UnityAction onDeath;
        public UnityAction onHit;
        public UnityAction onAttack;
        public UnityAction<GameObject> onAttackHit;
        public UnityAction onJump;

        private Rigidbody2D rigid;
        private Collider2D collide;
        private CapsuleCollider2D capsule_coll;
        private ContactFilter2D contact_filter;
        private Vector2 coll_start_h;
        private Vector2 coll_start_off;
        private Vector3 start_scale;

        private PlayerCharacterState state;
        private Vector2 move;
        private Vector2 facing = Vector2.down;
        private Vector3 jump_target;

        private float hp;

        private float state_timer = 0f;
        private float attack_timer = 0f;
        private float hit_timer = 0f;

        private static List<PlayerCharacter> character_list = new List<PlayerCharacter>();

        void Awake()
        {
            character_list.Add(this);
            rigid = GetComponent<Rigidbody2D>();
            collide = GetComponent<Collider2D>();
            capsule_coll = GetComponent<CapsuleCollider2D>();
            coll_start_h = capsule_coll.size;
            coll_start_off = capsule_coll.offset;
            start_scale = transform.localScale;
            hp = max_hp;

            contact_filter = new ContactFilter2D();
            contact_filter.layerMask = raycast_mask;
            contact_filter.useLayerMask = true;
            contact_filter.useTriggers = false;

        }

        void OnDestroy()
        {
            character_list.Remove(this);
        }

        void Start()
        {
            PlayerData pdata = PlayerData.Get();
            if (pdata != null)
            {
                if (pdata.hp > 0)
                    hp = pdata.hp;

                pdata.hp = hp;
            }
        }

        //Handle physics
        void FixedUpdate()
        {
            if (TheGame.IsGamePaused())
                return;

            PlayerControls controls = PlayerControls.Get(player_id);

            //Movement velocity
            Vector3 move_input = controls.GetMove();
            Vector3 desiredSpeed = move_input * move_max;
            if (state == PlayerCharacterState.Climb)
                desiredSpeed = move_input * climb_speed;

            float acceleration = move_input.magnitude > 0.1f ? move_accel : move_deccel;
            move = Vector3.MoveTowards(move, desiredSpeed, acceleration * Time.fixedDeltaTime);

            if (attack_timer < 0f)
                move = Vector2.zero;

            if (move.magnitude > 0.1f)
                facing = move.normalized;

            if (state == PlayerCharacterState.Normal)
            {
                //Move
                rigid.velocity = move;
            }

            if (state == PlayerCharacterState.Climb)
            {
                move.x = 0f;
                rigid.velocity = move;
            }

            if (state == PlayerCharacterState.Jump)
            {
                Vector3 jdir = jump_target - transform.position;
                facing = jdir.normalized;
                move = Vector2.zero;
                rigid.velocity = Vector3.zero;
            }
            
            if (state == PlayerCharacterState.Dead)
            {
                move = Vector3.zero;
                rigid.velocity = move;
            }
            
        }

        //Handle render and controls
        void Update()
        {
            if (TheGame.IsGamePaused())
                return;

            if (IsDead())
                return;

            hit_timer += Time.deltaTime;
            state_timer += Time.deltaTime;
            attack_timer += Time.deltaTime;

            //Controls
            PlayerControls controls = PlayerControls.Get(player_id);

            if (controls.GetAttackDown())
                Attack();

            Ladder ladder = Ladder.GetOverlap(gameObject, 0.5f);
            JumpLane lane = JumpLane.GetOverlap(gameObject, 0.5f);

            if (state == PlayerCharacterState.Normal)
            {
                //Start climb
               
                if (ladder != null && state_timer > 1f)
                {
                    Vector3 dir = ladder.transform.position - transform.position;
                    if (dir.y * controls.GetMove().y > 0.01f)
                    {
                        state = PlayerCharacterState.Climb;
                        collide.enabled = false;
                    }
                }
                
                if (lane != null && state_timer > 1f)
                {
                    float move_dot = Vector3.Dot(lane.jump_dir, GetMove());
                    if (lane.IsInRadius(gameObject) && move_dot > 0.5f&& move.magnitude > 0.1f)
                        Jump(lane);
                }
            }

            if (state == PlayerCharacterState.Climb)
            {
                if (ladder == null)
                {
                    state = PlayerCharacterState.Normal;
                    collide.enabled = true;
                }
                else
                {
                    transform.position = new Vector3(ladder.transform.position.x, transform.position.y, 0f);
                }
            }

            if (state == PlayerCharacterState.Jump)
            {
                Vector3 jdir = jump_target - transform.position;
                float dist = Mathf.Min(jump_speed * Time.deltaTime, jdir.magnitude);

                if (state_timer > 0.2f)
                    transform.position += jdir.normalized * dist;
                
                if (state_timer > 2f || jdir.magnitude < 0.1f)
                {
                    state = PlayerCharacterState.Normal;
                    state_timer = 0f;
                    collide.enabled = true;
                }
            }
        }
        
        private bool IsFronted()
        {
            bool obstacle = DetectObstacle(GetFacing());
            bool box = RaycastObstacle<Box>(GetCapsulePos(GetFacing()), GetFacing());
            bool enemy = RaycastObstacle<Enemy>(GetCapsulePos(GetFacing()), GetFacing());
            return obstacle && !box && !enemy;
        }

        private bool DetectObstacle(Vector3 dir)
        {
            bool grounded = false;
            Vector2[] raycastPositions = new Vector2[3];

            Vector2 raycast_start = rigid.position;
            Vector2 orientation = dir.normalized;
            bool is_up_down = Mathf.Abs(orientation.y) > Mathf.Abs(orientation.x);
            Vector2 perp_ori = is_up_down ? Vector2.right : Vector2.up;
            float radius = GetCapsuleRadius();

            if (capsule_coll != null && is_up_down)
            {
                //Adapt raycast to collider
                raycast_start = GetCapsulePos(dir);
            }

            float ray_size = radius + raycast_dist;
            raycastPositions[0] = raycast_start - perp_ori * radius / 2f;
            raycastPositions[1] = raycast_start;
            raycastPositions[2] = raycast_start + perp_ori * radius / 2f;
            
            for (int i = 0; i < raycastPositions.Length; i++)
            {
                Debug.DrawRay(raycastPositions[i], orientation * ray_size);
                if (RaycastObstacle(raycastPositions[i], orientation * ray_size))
                    grounded = true;
            }
            return grounded;
        }

        public bool RaycastObstacle(Vector2 pos, Vector2 dir)
        {
            RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
            Physics2D.Raycast(pos, dir.normalized, contact_filter, hitBuffer, dir.magnitude);
            for (int j = 0; j < hitBuffer.Length; j++)
            {
                if (hitBuffer[j].collider != null && hitBuffer[j].collider != capsule_coll && !hitBuffer[j].collider.isTrigger)
                {
                    return true;
                }
            }
            return false;
        }

        public GameObject RaycastObstacle<T>(Vector2 pos, Vector2 dir)
        {
            RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
            Physics2D.Raycast(pos, dir.normalized, contact_filter, hitBuffer, dir.magnitude);
            for (int j = 0; j < hitBuffer.Length; j++)
            {
                if (hitBuffer[j].collider != null && hitBuffer[j].collider != capsule_coll && !hitBuffer[j].collider.isTrigger)
                {
                    if (hitBuffer[j].collider.GetComponent<T>() != null)
                        return hitBuffer[j].collider.gameObject;
                }
            }
            return null;
        }

        public void Attack()
        {
            attack_timer = -attack_duration;
            if (onAttack != null)
                onAttack.Invoke();
        }

        public void Jump(JumpLane lane)
        {
            if (state == PlayerCharacterState.Normal)
            {
                jump_target = lane.end_pos.position;
                state = PlayerCharacterState.Jump;
                collide.enabled = false;
                state_timer = 0f;

                if (onJump != null)
                    onJump.Invoke();
            }
        }

        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
            move = Vector2.zero;
        }
        
        public void HealDamage(float heal)
        {
            if (!IsDead())
            {
                hp += heal;
                hp = Mathf.Min(hp, max_hp);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsDead() && hit_timer > 0f)
            {
                hp -= damage;
                hit_timer = -1f;

                PlayerData pdata = PlayerData.Get();
                if(pdata != null)
                    pdata.hp = hp;

                if (hp <= 0f)
                {
                    Kill();
                }
                else
                {
                    if (onHit != null)
                        onHit.Invoke();
                }
            }
        }

        public void Kill()
        {
            if (!IsDead())
            {
                state = PlayerCharacterState.Dead;
                rigid.velocity = Vector2.zero;
                move = Vector2.zero;
                state_timer = 0f;
                collide.enabled = false;

                if (onDeath != null)
                    onDeath.Invoke();
            }
        }
        
        public PlayerCharacterState GetState() {
            return state;
        }

        public Vector2 GetMove()
        {
            return move;
        }

        public Vector2 GetFacing()
        {
            return facing;
        }
        
        //Return facing but locked to 1 of the 4 sides
        public Vector3 GetAnimFacing()
        {
            if (Mathf.Abs(facing.x) > Mathf.Abs(facing.y))
                return new Vector3(Mathf.Sign(facing.x), 0f, 0f);
            else
                return new Vector3(0f, Mathf.Sign(facing.y), 0f);
        }
        
        public bool IsClimbing()
        {
            return state == PlayerCharacterState.Climb;
        }

        public float GetHP()
        {
            return hp;
        }

        public bool IsDead()
        {
            return state == PlayerCharacterState.Dead;
        }

        public Vector2 GetSize()
        {
            if (capsule_coll != null)
                return new Vector2(Mathf.Abs(transform.localScale.x) * capsule_coll.size.x, Mathf.Abs(transform.localScale.y) * capsule_coll.size.y);
            return new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        }

        private Vector3 GetCapsulePos(Vector3 dir)
        {
            Vector2 orientation = dir.normalized;
            Vector2 raycast_offset = capsule_coll.offset + orientation * Mathf.Abs(capsule_coll.size.y * 0.5f - capsule_coll.size.x * 0.5f);
            return rigid.position + raycast_offset * capsule_coll.transform.lossyScale.y;
        }

        private float GetCapsuleRadius()
        {
            return GetSize().x * 0.5f;
        }

        private void TouchEnemy(Enemy enemy)
        {
            TakeDamage(enemy.damage);
        }

        public void OnSwordHit(Collider2D other)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attack_damage);
                enemy.Push(other.transform.position - transform.position);

                if (onAttackHit != null)
                    onAttackHit.Invoke(enemy.gameObject);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsDead())
                return;

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                TouchEnemy(enemy);
            }
        }

        public static PlayerCharacter GetNearest(Vector3 pos, float range = 99999f, bool alive_only=false)
        {
            PlayerCharacter nearest = null;
            float min_dist = range;
            foreach (PlayerCharacter character in GetAll())
            {
                if (!alive_only || !character.IsDead())
                {
                    float dist = (pos - character.transform.position).magnitude;
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        nearest = character;
                    }
                }
            }
            return nearest;
        }

        public static PlayerCharacter Get(int player_id)
        {
            foreach (PlayerCharacter character in GetAll())
            {
                if (character.player_id == player_id)
                {
                    return character;
                }
            }
            return null;
        }

        public static List<PlayerCharacter> GetAll()
        {
            return character_list;
        }
    }

}