using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CodingThunder.RPGUtilities.Mechanics
{
	public class PlayerInputReceiver : GameStateManaged
	{

		private Movement2D movement2D;
		protected override void OnAwake()
		{
			movement2D = GetComponent<Movement2D>();
		}

		protected override void OnStart()
		{
			//PlayerInput playerInput = GetComponent<PlayerInput>();
			//EventSystem.current.currentInputModule
		}

		public void OnWalk2D(InputValue value)
		{

			if (!IsActive) { return; }
			if (movement2D == null) { return; }

			var dir = value.Get<Vector2>();

			movement2D.Walk2D(value.Get<Vector2>());
		}
	}
}