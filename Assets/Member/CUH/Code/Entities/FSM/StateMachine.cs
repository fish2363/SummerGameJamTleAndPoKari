using System;
using System.Collections.Generic;
using UnityEngine;

namespace Member.CUH.Code.Entities.FSM
{
    public class StateMachine
    {
        public EntityState currentState { get; private set; }
        
        private Dictionary<StateName, EntityState> _stateDictionary;

        public StateMachine(EntityFsmso entityStates, Entity entity)
        {
            _stateDictionary = new Dictionary<StateName, EntityState>();
        
            foreach (StateSO state in entityStates.states)
            {
                try
                {
                    Type type = Type.GetType(state.className);
                    var entityState = Activator.CreateInstance(type, entity, state.animParam) as EntityState;
                    _stateDictionary.Add(state.stateName, entityState);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{state.className} loading error, Message : {e.Message}");
                }
            }
        }
    
        public EntityState GetState(StateName state)
        {
            return _stateDictionary.GetValueOrDefault(state);
        }
    
        public void Initialize(StateName startState)
        {
            currentState = GetState(startState);
            currentState.Enter();
        }

        public void UpdateStateMachine()
        {
            currentState.Update();
        }
    
        public void ChageState(StateName newState)
        {
            currentState.Exit();
            EntityState nextState = GetState(newState);
            Debug.Assert(nextState != null, $"State : {newState} not found");
        
            currentState = nextState;
            currentState.Enter();
        }

    }
}
