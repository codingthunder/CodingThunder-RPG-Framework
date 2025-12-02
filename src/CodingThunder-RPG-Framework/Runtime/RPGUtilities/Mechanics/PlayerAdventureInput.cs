using CodingThunder.RPGUtilities.GameState;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Mechanics
{
    public class PlayerAdventureInput: GameStateManaged
    {
	    private Movement2D movement2D;
	    private Interactor interactor;

	    private float interactorDistance;
	    private bool runInputPressed;
	    
        protected override void OnAwake()
		{
			movement2D = GetComponent<Movement2D>();
			interactor = GetComponentInChildren<Interactor>();
		}

		protected override void OnStart()
		{
			//PlayerInput playerInput = GetComponent<PlayerInput>();
			//EventSystem.current.currentInputModule
			if (interactor != null)
			{
				interactorDistance = (interactor.transform.position - transform.position).magnitude;
			}
		}

		protected override void HandleGameStateChange(GameStateEnum state)
		{
			base.HandleGameStateChange(state);

			if (movement2D == null)
			{
				return;
			}

			if (!IsActive)
			{
				movement2D.Stop();
			}
			
		}

		public void OnWalk2D(Vector2 value)
		{

			if (!IsActive) { return; }
			if (movement2D == null) { return; }

			var dir = value;

			if (runInputPressed)
			{
				movement2D.Run2D(dir);
				return;
			}

			movement2D.Walk2D(dir);

			//Little did I know, I'd already tried to control the interactor here. Whoops. Gonna probably remove this because
			//while virtually every game is going to have an interactor, how that interactor is manipulated will depend upon
			//the individual game. As such, the package should provide the interactor, but not tell the game how to arrange it.
			//if (interactor == null) { return; }

			////Are the parentheses overkill? Yes. Do they make it VERY clear what's happening? Also yes.
			//interactor.transform.position = ((Vector2) transform.position) + (dir.normalized * interactorDistance);
		}

		public void OnRun2D(bool isPressed)
		{
			runInputPressed = isPressed;
		}

		public void OnInteract()
		{
			Debug.Log("OnInteract pressed, baby!");
			if (!IsActive)
			{
				return;
			}
			if (interactor == null)
			{
				return;
			}

			interactor.Interact();
		}
    }
}