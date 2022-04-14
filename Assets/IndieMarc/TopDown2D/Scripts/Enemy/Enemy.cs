using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace IndieMarc.TopDown
{
    public enum EnemyBehavior
    {
        None=0,
        WanderOnly=5,
        WanderFollow=10,
        PatrolOnly=20,
        PatrolFollow=25,
    }

    public enum EnemyRotateType {
        None=0,
        FlipX=2,
        Rotate=5,
    }

    public enum EnemyState
    {
        None=0,
        Wander=5,
        Patrol=10,
        Follow=15,
        Dead=50,
    }

    public class Enemy : MonoBehaviour
    {
        public EnemyBehavior behavior;
        public float move_speed = 2f;
        public EnemyRotateType rotate_type;
        public float rotate_speed = 120f;
        public bool use_physics = true;
        public float push_force = 3f;
        public float damage = 1f;
        public float hp_max = 3f;
        public bool invulnerable = false;
        public GameObject[] loot_prefab;

        [Header("Wander")]
        public float wander_range = 5f;

        [Header("Patrol")]
        public Transform[] patrol_points;
        public float wait_time = 1f;
        public bool loop = false;

        [Header("Follow")]
        public float detect_range = 5f;
        public float follow_range = 5f;
        public float follow_speed_mult = 1f;

        [Header("Obstacle")]
        public LayerMask raycast_mask = ~(0); //All bit 1
        public float raycast_dist = 0.1f;

        public UnityAction onHit;
        public UnityAction onDeath;

        private Rigidbody2D rigid;
        private Collider2D collide;
        private CircleCollider2D circle_coll;
        private ContactFilter2D contact_filter;
        private Vector3 start_pos;
        private Vector3 start_scale;
        private Vector3 wander_targ;
        private Vector3 follow_targ;
        private Vector3 move_vect;
        private Vector3 face_vect;
        private Quaternion face_rot;
        private float rot_angle = 0f;
        private Vector3 push_vect = Vector3.zero;

        private EnemyState state = EnemyState.None;
        private Vector2 current_target;
        private float current_mult = 1f;
        private Vector3 current_rot_target;
        private float current_rot_mult = 1f;
        public float hp;

        private bool waiting = false;
        private int current_path = 0;
        private bool path_rewind = false;

        private float wait_timer = 0f;
        private float hit_timer = 0f;
        private float wander_timer = 0f;

        private List<Vector3> path_list = new List<Vector3>();

        private static List<Enemy> enemy_list = new List<Enemy>();

        private void Awake()
        {
            enemy_list.Add(this);
            rigid = GetComponent<Rigidbody2D>();
            collide = GetComponent<Collider2D>();
            circle_coll = GetComponent<CircleCollider2D>();
            move_vect = Vector3.zero;
            current_target = transform.position;
            current_rot_target = transform.position + transform.forward;
            start_pos = transform.position;
            start_scale = transform.localScale;
            wander_targ = start_pos;
            follow_targ = start_pos;
            face_rot = Quaternion.identity;
            rot_angle = -90f;
            move_vect = Mathf.Sign(start_scale.x) > 0f ? Vector3.right : Vector3.left;
            hp = hp_max;

            //Raycast init
            contact_filter = new ContactFilter2D();
            contact_filter.layerMask = raycast_mask;
            contact_filter.useLayerMask = true;
            contact_filter.useTriggers = false;

            //Patrol init
            path_list.Add(transform.position);
            foreach (Transform patrol in patrol_points)
            {
                if (patrol)
                    path_list.Add(patrol.position);
            }

            current_path = 0;
            if (path_list.Count >= 2)
                current_path = 1; //Dont start at start pos
        }

        private void OnDestroy()
        {
            enemy_list.Remove(this);
        }

        void Start()
        {
            
        }

        private void FixedUpdate()
        {
            if (TheGame.IsGamePaused())
                return;

            if (state== EnemyState.Dead)
                return;

            if (use_physics)
            {
                Vector2 dist_vect = (current_target - rigid.position);
                move_vect = dist_vect.normalized * move_speed * current_mult * Mathf.Min(dist_vect.magnitude, 1f);

                rigid.velocity = move_vect + push_vect;

                push_vect = Vector3.MoveTowards(push_vect, Vector3.zero, push_force * Time.fixedDeltaTime);
            }
        }

        private void Update()
        {
            if (TheGame.IsGamePaused())
                return;

            if (state == EnemyState.Dead)
                return;

            hit_timer += Time.deltaTime;
            wait_timer += Time.deltaTime;

            //State
            if (state == EnemyState.None)
            {
                if (behavior == EnemyBehavior.PatrolOnly || behavior == EnemyBehavior.PatrolFollow)
                    state = EnemyState.Patrol;
                if (behavior == EnemyBehavior.WanderFollow || behavior == EnemyBehavior.WanderOnly)
                    state = EnemyState.Wander;
            }

            if (state == EnemyState.Wander)
            {
                MoveTo(wander_targ);
                FaceToward(GetMoveTarget(), 2f);

                wander_timer += Time.deltaTime;
                if (wander_timer > 5f)
                {
                    wander_timer = Random.Range(0f, 1f);
                    Vector3 wtarg = FindWanderTarget();
                    if (!RaycastObstacle(transform.position, (wtarg - transform.position)))
                        wander_targ = wtarg;
                }

                if (behavior == EnemyBehavior.PatrolFollow || behavior == EnemyBehavior.WanderFollow)
                {
                    if (IsPlayerInRange(detect_range))
                        state = EnemyState.Follow;
                }
            }

            if (state == EnemyState.Patrol)
            {
                //If still in starting path
                if (!waiting && path_list.Count > 0)
                {
                    //Move
                    Vector3 targ = path_list[current_path];
                    MoveTo(targ);
                    FaceToward(targ);

                    //Check if reached target
                    Vector3 dist_vect = (targ - transform.position);
                    dist_vect.z = 0f;
                    if (dist_vect.magnitude < 0.1f)
                    {
                        waiting = true;
                        wait_timer = 0f;
                    }

                    //Check if obstacle ahead
                    bool fronted = CheckFronted(dist_vect.normalized);
                    if (fronted && wait_timer > 2f)
                    {
                        RewindPath();
                    }
                }

                if (waiting)
                {
                    //Wait a bit
                    if (wait_timer > wait_time)
                    {
                        GoToNextPath();
                        waiting = false;
                        wait_timer = 0f;
                    }
                }

                if (behavior == EnemyBehavior.PatrolFollow || behavior == EnemyBehavior.WanderFollow)
                {
                    if (IsPlayerInRange(detect_range))
                        state = EnemyState.Follow;
                }
            }

            if (state == EnemyState.Follow)
            {
                if (IsPlayerInRange(follow_range))
                {
                    PlayerCharacter character = PlayerCharacter.GetNearest(transform.position, follow_range, true);
                    if (character != null)
                    {
                        MoveTo(character.transform.position, follow_speed_mult);
                        FaceToward(GetMoveTarget(), 2f);
                    }
                }
                else
                {
                    state = EnemyState.None;
                }
            }

            //Movement (non-physics based)
            if (!use_physics)
            {
                Vector3 dist_vect = (current_target - rigid.position);
                float dist_length = Mathf.Min(move_speed * Time.deltaTime, dist_vect.magnitude);
                transform.position += dist_vect.normalized * dist_length;
            }

            //Angle and facing
            Vector3 dir = current_rot_target - transform.position;
            dir.z = 0f;
            
            if (dir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x * GetSide()) * Mathf.Rad2Deg;
                rot_angle = angle;
                Quaternion target = Quaternion.AngleAxis(angle, Vector3.forward);
                face_rot = Quaternion.RotateTowards(face_rot, target, rotate_speed * current_rot_mult * Time.deltaTime);
                face_vect = face_rot * Vector3.right;
                face_vect.x = face_vect.x * GetSide();
                face_vect.Normalize();
            }

            if (rotate_type == EnemyRotateType.FlipX)
            {
                if (Mathf.Abs(dir.x) > 0.1f)
                {
                    float side = (dir.x < 0f) ? -1f : 1f;
                    transform.localScale = new Vector3(Mathf.Abs(start_scale.x) * side, start_scale.y, start_scale.z);
                }
            }

            if (rotate_type == EnemyRotateType.Rotate)
            {
                rigid.rotation = rot_angle + 90f;
            }

            Debug.DrawRay(transform.position, face_vect);
        }
        
        private Vector3 FindWanderTarget()
        {
            float radius = Random.Range(0f, wander_range);
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            return start_pos + offset;
        }

        private void RewindPath()
        {
            path_rewind = !path_rewind;
            current_path += path_rewind ? -1 : 1;
            current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
            wait_timer = 0f;
        }

        private void GoToNextPath()
        {
            if (!loop && (current_path <= 0 || current_path >= path_list.Count - 1))
                path_rewind = !path_rewind;

            current_path += path_rewind ? -1 : 1;

            if (loop && current_path >= path_list.Count)
                current_path = 0;

            current_path = Mathf.Clamp(current_path, 0, path_list.Count - 1);
        }
        
        private bool DetectObstacle(Vector3 dir)
        {
            bool grounded = false;
            Vector2[] raycastPositions = new Vector2[3];

            Vector2 raycast_start = rigid.position;
            Vector2 orientation = dir.normalized;
            bool is_up_down = Mathf.Abs(orientation.y) > Mathf.Abs(orientation.x);
            Vector2 perp_ori = is_up_down ? Vector2.right : Vector2.up;
            float radius = GetSize().x * 0.5f;

            if (circle_coll != null && is_up_down)
            {
                //Adapt raycast to collider
                Vector2 raycast_offset = circle_coll.offset + orientation * radius;
                raycast_start = rigid.position + raycast_offset * circle_coll.transform.lossyScale.y;
            }

            float ray_size = radius + raycast_dist;
            raycastPositions[0] = raycast_start - perp_ori * radius / 2f;
            raycastPositions[1] = raycast_start;
            raycastPositions[2] = raycast_start + perp_ori * radius / 2f;


            for (int i = 0; i < raycastPositions.Length; i++)
            {
                Debug.DrawRay(raycastPositions[i], orientation * ray_size, Color.green);
                if (RaycastObstacle(raycastPositions[i], orientation * ray_size))
                    grounded = true;
            }
            return grounded;
        }

        public bool RaycastObstacle(Vector2 pos, Vector2 dir, GameObject skip_object=null)
        {
            RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
            Physics2D.Raycast(pos, dir.normalized, contact_filter, hitBuffer, dir.magnitude);
            for (int j = 0; j < hitBuffer.Length; j++)
            {
                if (hitBuffer[j].collider != null && hitBuffer[j].collider != collide && !hitBuffer[j].collider.isTrigger)
                {
                    if(hitBuffer[j].collider.gameObject != skip_object)
                        return true;
                }
            }
            return false;
        }

        public bool CheckFronted(Vector3 dir)
        {
            return RaycastObstacle(transform.position, dir);
        }

        public bool IsPlayerInRange(float range)
        {
            PlayerCharacter character = PlayerCharacter.GetNearest(transform.position, range, true);
            if (character != null)
            {
                Vector3 dir_vect = character.transform.position - transform.position;
                return dir_vect.magnitude < range && !RaycastObstacle(transform.position, dir_vect, character.gameObject);
            }
            return false;
        }

        public Vector2 GetSize()
        {
            if (circle_coll != null)
                return new Vector2(Mathf.Abs(circle_coll.transform.lossyScale.x) * circle_coll.radius, Mathf.Abs(circle_coll.transform.lossyScale.y) * circle_coll.radius);
            if (collide != null)
                return new Vector2(collide.bounds.size.x, collide.bounds.size.y);
            return new Vector2(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        }

        public void MoveTo(Vector3 pos, float speed_mult = 1f)
        {
            current_target = pos;
            current_mult = speed_mult;
        }

        public void FaceToward(Vector3 pos, float speed_mult = 1f)
        {
            current_rot_target = pos;
            current_rot_mult = speed_mult;
        }

        public void Push(Vector3 dir)
        {
            push_vect = dir.normalized * push_force;
        }

        public void TakeDamage(float damage)
        {
            if (!IsDead() && !invulnerable && hit_timer > 0f)
            {
                hp -= damage;
                hit_timer = -1f;

                if (onHit != null)
                    onHit.Invoke();

                if (hp <= 0f)
                    Kill();
            }
        }
        
        public void Kill()
        {
            state = EnemyState.Dead;
            collide.enabled = false;
            if (onDeath != null)
                onDeath.Invoke();

            Destroy(gameObject, 1f);

			for (int i = 0; i < loot_prefab.Length; i++)
            if (loot_prefab[i] != null)
                Instantiate(loot_prefab[i], transform.position, Quaternion.identity);
        }

        public bool IsDead()
        {
            return state == EnemyState.Dead;
        }

        public Vector3 GetMove()
        {
            return move_vect;
        }

        public Vector3 GetFacing()
        {
            return face_vect;
        }

        public bool IsMoving()
        {
            return !IsDead() && move_vect.magnitude > 0.1f;
        }

        public float GetFacingAngle()
        {
            return Mathf.Atan2(face_vect.y, face_vect.x * GetSide()) * Mathf.Rad2Deg;
        }

        public float GetSide()
        {
            return Mathf.Sign(transform.localScale.x);
        }

        public Vector3 GetMoveTarget()
        {
            return current_target;
        }
        
        public static Enemy GetNearest(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            Enemy nearest = null;
            foreach (Enemy point in enemy_list)
            {
                float dist = (point.transform.position - pos).magnitude;
                if (dist < min_dist)
                {
                    min_dist = dist;
                    nearest = point;
                }
            }
            return nearest;
        }
        
        public static List<Enemy> GetAll()
        {
            return enemy_list;
        }

        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 prev_pos = transform.position;

            foreach (Transform patrol in patrol_points)
            {
                if (patrol)
                {
                    Gizmos.DrawLine(prev_pos, patrol.transform.position);
                    prev_pos = patrol.transform.position;
                }
            }

            if(loop && patrol_points.Length >= 2)
                Gizmos.DrawLine(prev_pos, transform.position);
        }
    }

}