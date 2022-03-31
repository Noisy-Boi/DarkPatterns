using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
	
	public class Box : MonoBehaviour
	{
		public bool respawn_on_death = true;
		public float bounciness = 2f;
		public LayerMask raycast_mask = ~(0); //All bit 1
		public float raycast_dist = 0.1f;

		private Rigidbody2D rigid;
		private SpriteRenderer render;
		private BoxCollider2D collide;
		private ContactFilter2D contact_filter;

		private Vector3 start_pos;
		private float kill_timer = 0f;
		private bool dead = false;

		private Vector2 bounce_velocity = Vector2.zero;

		void Awake()
		{
			rigid = GetComponent<Rigidbody2D>();
			render = GetComponentInChildren<SpriteRenderer>();
			collide = GetComponent<BoxCollider2D>();
			start_pos = transform.position;

			contact_filter = new ContactFilter2D();
			contact_filter.layerMask = raycast_mask;
			contact_filter.useLayerMask = true;
			contact_filter.useTriggers = false;
		}

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			rigid.velocity = bounce_velocity;
			bounce_velocity = Vector2.MoveTowards(bounce_velocity, Vector2.zero, 2f * Time.fixedDeltaTime);
		}

		void Update()
		{
			//Reset after death
			if (dead && respawn_on_death)
			{
				kill_timer += Time.deltaTime;
				if (kill_timer > 5f)
				{
					transform.position = start_pos;
					dead = false;
					render.enabled = true;
					collide.enabled = true;
					kill_timer = 0f;
				}
			}

			//Bounce 
			float radiusX = GetSize().x * 0.5f + raycast_dist;
			float radiusY = GetSize().y * 0.5f + raycast_dist;
			Debug.DrawRay(transform.position, Vector2.right * radiusX);
			if (RaycastObstacle(transform.position, Vector2.right * radiusX))
				bounce_velocity = Vector2.left * bounciness;
			if (RaycastObstacle(transform.position, Vector2.left * radiusX))
				bounce_velocity = Vector2.right * bounciness;
			if (RaycastObstacle(transform.position, Vector2.up * radiusY))
				bounce_velocity = Vector2.down * bounciness;
			if (RaycastObstacle(transform.position, Vector2.down * radiusY))
				bounce_velocity = Vector2.up * bounciness;

			
		}

		public bool RaycastObstacle(Vector2 pos, Vector2 dir)
		{
			RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
			Physics2D.Raycast(pos, dir.normalized, contact_filter, hitBuffer, dir.magnitude);
			for (int j = 0; j < hitBuffer.Length; j++)
			{
				if (hitBuffer[j].collider != null && hitBuffer[j].collider != collide && !hitBuffer[j].collider.isTrigger)
				{
					return true;
				}
			}
			return false;
		}

		public Vector2 GetSize()
		{
			if (collide != null)
				return new Vector2(Mathf.Abs(transform.localScale.x) * collide.size.x, Mathf.Abs(transform.localScale.y) * collide.size.y);
			return new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
		}

		public void Kill()
		{
			dead = true;
			render.enabled = false;
			collide.enabled = false;
			kill_timer = 0f;
		}
	}
	
}
