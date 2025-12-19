using System;
using System.Collections.Generic;

namespace KinopiStudio.Core.FSM
{
    public abstract class StateMachine<TOwner, TState> where TState : struct, Enum
    {
        public StateMachine(TOwner owner)
        {
            _owner = owner;    
        }
     
        protected readonly TOwner _owner;
        private Dictionary<TState, State<TOwner>> _states = new Dictionary<TState, State<TOwner>>();
        
        public State<TOwner> CurrentState { get; private set; }
        public TState CurrentStateType { get; private set; }
        
        private TState? _pendingState;
        private bool _isInTransition = false;
        
        public void AddState(TState stateType, State<TOwner> state)
        {
            _states.TryAdd(stateType, state);
        }

        public void ChangeState(TState stateType)
        {
            if (CurrentState != null && EqualityComparer<TState>.Default.Equals(CurrentStateType, stateType))
            {
                return;   
            }
            
            if(CurrentState != null && !CurrentState.CanExit())
            {
                return;
            }

            _pendingState = stateType;
            
            if (_isInTransition)
            {
                return;
            }
            
            ProcessStateChange();
        }
        
        private void ProcessStateChange()
        {
            _isInTransition = true;
            

            while (_pendingState.HasValue)
            {
                var nextStateType = _pendingState.Value;
                _pendingState = null;

                if (CurrentState != null && EqualityComparer<TState>.Default.Equals(CurrentStateType, nextStateType))
                {
                    continue;
                }

                if (!_states.TryGetValue(nextStateType, out var nextState))
                {
                    continue;
                }
                
                if(CurrentState != null && !CurrentState.CanTransitionTo(nextState))
                {
                    continue;
                }
                
                CurrentState?.OnExit();
                CurrentState = nextState;
                CurrentStateType = nextStateType;
                CurrentState.OnEnter();
            }

            _isInTransition = false;
        }
        
        public void OnUpdate(float deltaTime)
        {
            CurrentState?.OnUpdate(deltaTime);
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            CurrentState?.OnFixedUpdate(fixedDeltaTime);
        }   
    }
}
