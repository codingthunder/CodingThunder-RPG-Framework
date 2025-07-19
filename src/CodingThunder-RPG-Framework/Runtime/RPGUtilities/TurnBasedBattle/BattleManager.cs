using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using CodingThunder.RPGUtilities.DataManagement;

namespace CodingThunder.RPGUtilities.TurnBasedBattle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        public BattleState BattleState { get; private set; }
        public Action<BattleState, Action<string, string>> DecideActionNPC;
        public Action<BattleState, Action<string, string>> DecideActionPlayer;

        // private bool waitingForAction = false;
        // private string selectedAction;
        // private string selectedTarget;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            LookupResolver.Instance.RegisterRootKeyword("Battle", Lookup);
        }

        public void Initialize(Action<BattleState, Action<string, string>> decideActionNPC, Action<BattleState, Action<string, string>> decideActionPlayer)
        {
            BattleState = new();
            DecideActionNPC = decideActionNPC;
            DecideActionPlayer = decideActionPlayer;
        }

        public object Lookup(List<string> idChain)
        {
            if (idChain == null || idChain.Count == 0)
            {
                return null;
            }

            if (idChain[0] == "Battle")
            {
                idChain.RemoveAt(0);
            }

            if (idChain.Count == 0)
            {
                return null;
            }

            if (idChain[0] == "Combatants")
            {
                idChain.RemoveAt(0);
                
                if (idChain.Count == 0)
                {
                    return null;
                }

                string combatantKey = idChain[0];
                idChain.RemoveAt(0);

                if (BattleState.Combatants.ContainsKey(combatantKey))
                {
                    return BattleState.Combatants[combatantKey];
                }
            }

            return null;
        }

        public void AddCombatant(Combatant combatant)
        {
            if (combatant != null && !string.IsNullOrEmpty(combatant.Name))
            {
                BattleState.Combatants[combatant.Name] = combatant;
            }
        }

        public void SelectAction(string actionName, string actionTarget)
        {
            BattleState.SelectedAction = actionName;
            BattleState.SelectedTarget = actionTarget;
            BattleState.WaitingForAction = false;
        }

        public IEnumerator RunBattle()
        {
            if (BattleState == null)
            {
                Debug.LogError("You need to Initialize your BattleManager before calling RunBattle.");
                yield break;
            }

            BattleState.Round = 0;

            while (GetTeamsWithLivingCombatants().Count > 1)
            {
                BattleState.Round++;

                var turnOrder = BattleState.Combatants.Values
                    .Where(c => c.HP > 0)
                    .OrderByDescending(c => c.Agility)
                    .ToList();

                foreach (var combatant in turnOrder)
                {
                    if (combatant.HP <= 0) continue;

                    BattleState.Turn = combatant.Name;
                    BattleState.WaitingForAction = true;
                    BattleState.SelectedAction = null;
                    BattleState.SelectedTarget = null;

                    if (combatant.IsPlayer)
                    {
                        DecideActionPlayer?.Invoke(BattleState, SelectAction);
                    }
                    else
                    {
                        DecideActionNPC?.Invoke(BattleState, SelectAction);
                    }

                    yield return new WaitUntil(() => !BattleState.WaitingForAction);
                    if (!string.IsNullOrEmpty(BattleState.SelectedAction) && !string.IsNullOrEmpty(BattleState.SelectedTarget))
                    {
                        Debug.Log($"Combatant {combatant.Name} Uses Action {BattleState.SelectedAction} on Target {BattleState.SelectedTarget}");
                        
                        if (BattleState.Combatants.ContainsKey(BattleState.SelectedTarget))
                        {
                            BattleState.Combatants[BattleState.SelectedTarget].HP -= 1;
                        }
                    }
                }
            }

            var winningTeams = GetTeamsWithLivingCombatants();
            if (winningTeams.Count == 1)
            {
                Debug.Log($"Battle Ended, Team {winningTeams.First()} won!");
            }
        }

        private List<int> GetTeamsWithLivingCombatants()
        {
            return BattleState.Combatants.Values
                .Where(c => c.HP > 0)
                .Select(c => c.Team)
                .Distinct()
                .ToList();
        }
    }
}