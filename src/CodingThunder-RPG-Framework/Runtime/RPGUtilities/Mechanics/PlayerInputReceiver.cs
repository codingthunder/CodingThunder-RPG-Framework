using System;
using CodingThunder.RPGUtilities.GameState;
using System.Collections;
using System.Collections.Generic;
using CodingThunder.RPGUtilities.InputChannels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CodingThunder.RPGUtilities.Mechanics
{
    public class PlayerInputReceiver : GameStateManaged
	{

		public List<WhitelistedInputReceiver> inputReceivers = new();
		
		protected override void HandleGameStateChange(GameStateEnum state)
		{
			DecoupleFromInputChannels();
			foreach (var receiver in inputReceivers)
			{
				if (!receiver.whitelistedStates.Contains(state))
				{
					continue;
				}
				
				foreach (var route in receiver.vector2InputRoutes)
				{
					if (receiver.trackedV2Actions.ContainsKey(route.inputChannel))
						continue; // already subscribed for this receiver
					
					Action<Vector2> action = (Vector2 v) =>
					{
						if (route.autoBroadcastChannelName)
							BroadcastMessage(route.inputChannel.name, v);
						
						foreach (var message in route.broadcastMessages)
							BroadcastMessage(message, v);
						
						route.uEvent?.Invoke(v);
					};

					route.inputChannel.OnRaised += action;
					receiver.trackedV2Actions.Add(route.inputChannel, action);
				}
				
				foreach (var route in receiver.boolInputRoutes)
				{
					if (receiver.trackedBoolActions.ContainsKey(route.inputChannel))
						continue; // already subscribed for this receiver
					
					Action<bool> action = (bool b) =>
					{
						if (route.autoBroadcastChannelName)
							BroadcastMessage(route.inputChannel.name, b);
						
						foreach (var message in route.broadcastMessages)
							BroadcastMessage(message, b);
						
						route.uEvent?.Invoke(b);
					};

					route.inputChannel.OnRaised += action;
					receiver.trackedBoolActions.Add(route.inputChannel, action);
				}
				
				foreach (var route in receiver.voidInputRoutes)
				{
					if (receiver.trackedVoidActions.ContainsKey(route.inputChannel))
						continue; // already subscribed for this receiver
					
					Action action = () =>
					{
						if (route.autoBroadcastChannelName)
							BroadcastMessage(route.inputChannel.name);

						foreach (var message in route.broadcastMessages)
							BroadcastMessage(message);
						
						route.uEvent?.Invoke();
					};

					route.inputChannel.OnRaised += action;
					receiver.trackedVoidActions.Add(route.inputChannel, action);
				}
			}
		}
		
		protected override void OnAwake()
		{
			
		}

		protected override void OnStart()
		{
			
		}

		protected override void HandleDestroy()
		{
			// global cleanup (covers scene unload, disable, etc.)
			DecoupleFromInputChannels();
		}

		private void DecoupleFromInputChannels()
		{
			foreach (var receiver in inputReceivers)
			{
				foreach (var kvp in receiver.trackedV2Actions)
					kvp.Key.OnRaised -= kvp.Value;
				foreach (var kvp in receiver.trackedVoidActions)
					kvp.Key.OnRaised -= kvp.Value;
				foreach (var kvp in receiver.trackedBoolActions)
					kvp.Key.OnRaised -= kvp.Value;

				receiver.trackedV2Actions.Clear();
				receiver.trackedVoidActions.Clear();
				receiver.trackedBoolActions.Clear();
			}
		}
	}
    
    [Serializable]
	public class WhitelistedInputReceiver
	{
		public string receiverName;
		public List<GameStateEnum> whitelistedStates = new List<GameStateEnum>();
		public List<VoidInputRouter> voidInputRoutes = new();
		public List<Vector2InputRouter> vector2InputRoutes = new();
		public List<BoolInputRouter> boolInputRoutes = new();
		[NonSerialized] public Dictionary<VoidInputChannel, Action> trackedVoidActions = new();
		[NonSerialized] public Dictionary<Vector2InputChannel, Action<Vector2>> trackedV2Actions = new();
		[NonSerialized] public Dictionary<BoolInputChannel, Action<bool>> trackedBoolActions = new();
	}

	[Serializable]
	public class VoidInputRouter
	{
		public VoidInputChannel inputChannel;
		public bool autoBroadcastChannelName = true;
		public List<string> broadcastMessages = new List<string>();
		public UnityEvent uEvent = new UnityEvent();
	}
	[Serializable]
	public class BoolInputRouter
	{
		public BoolInputChannel inputChannel;
		public bool autoBroadcastChannelName = true;
		public List<string> broadcastMessages = new List<string>();
		public UnityEvent<bool> uEvent = new();
	}
	
	[Serializable]
	public class Vector2InputRouter
	{
		public Vector2InputChannel inputChannel;
		public bool autoBroadcastChannelName = true;
		public List<string> broadcastMessages = new List<string>();
		public UnityEvent<Vector2> uEvent = new();
	}
}