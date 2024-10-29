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
			rb.velocity = m_direction.normalized * m_speed;
		}

		protected override void HandleGameStateChange(GameStateEnum gameState)
		{
			if (IsActive)
			{
				rb.isKinematic = false;
				OnFixedUpdate();
				return;
			}
			rb.isKinematic = true;

		}

		public void Walk2D(Vector2 direction)
		{
			if (!IsActive)
			{
				return;
			}

			m_direction = direction;
			m_speed = walkingSpeed;
		}
	}
}