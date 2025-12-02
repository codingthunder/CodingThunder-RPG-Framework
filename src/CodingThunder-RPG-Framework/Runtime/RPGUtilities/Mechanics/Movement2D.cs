using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CodingThunder.RPGUtilities.Mechanics
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Movement2D : GameStateManaged
	{
		public float walkingSpeed = 3;
		public float runningSpeed = 10;

		public Vector2 m_direction;
		public float m_speed;

		Rigidbody2D rb;

		protected override void OnAwake()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		// Start is called before the first frame update
		//void Start()
		//   {

		//   }

		//   // Update is called once per frame
		//   void Update()
		//   {

		//   }

		protected override void OnFixedUpdate()
		{
			var velocity = m_direction.normalized * m_speed;

			if (velocity.magnitude >= 0.1)
			{
				rb.AddForce(velocity, ForceMode2D.Impulse);
			}
		}

		protected override void HandleGameStateChange(GameStateEnum gameState)
		{
			if (IsActive)
			{
				//rb.isKinematic = false;

				OnFixedUpdate();
				return;
			}

			rb.velocity = Vector2.zero;
			// m_speed = 0f;
			// OnFixedUpdate();
			//rb.isKinematic = true;

		}

		public void Walk2D(Vector2 direction)
		{
			if (!IsActive)
			{
				return;
			}

			if ((m_direction.normalized - direction.normalized).magnitude < 0.025f)
			{
				return;
			}

			m_direction = direction;
			m_speed = Mathf.Min(walkingSpeed * direction.magnitude, walkingSpeed);
		}
		
		public void Run2D(Vector2 direction)
		{
			if (!IsActive)
			{
				return;
			}

			if ((m_direction.normalized - direction.normalized).magnitude < 0.025f)
			{
				return;
			}

			m_direction = direction;
			m_speed = Mathf.Min(runningSpeed * direction.magnitude, runningSpeed);
		}

		public void Stop()
		{
			m_speed = 0;
		}
	}
}