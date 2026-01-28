using Code.Entities;
using System;
using System.Collections.Generic;

namespace Code.FSM
{
    public class StateMachine
    {
        private Dictionary<string, State> states = new Dictionary<string, State>();

        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }

        public void AddState(string stateName, State state)
        {
            if (!states.ContainsKey(stateName))
            {
                states.Add(stateName, state);
            }
        }

        public void ChangeState(string stateName, bool isForcing = false)
        {
            if (CurrentState != null && !isForcing && CurrentState == states[stateName])
                return;

            if (states.ContainsKey(stateName))
            {
                CurrentState?.Exit();
                PreviousState = CurrentState;
                CurrentState = states[stateName];
                CurrentState?.Enter();
            }
            else
            {
                throw new Exception($"State '{stateName}' not found in the state machine.");
            }
        }

        public void Update()
        {
            CurrentState?.Update();
        }

        internal void TriggerEvent(AnimationEventType eventType)
        {
            CurrentState?.OnTriggerEnter(eventType);
        }
    }
}
