namespace KinopiStudio.Core.FSM
{
    public abstract class State<TOwner>
    {
        protected readonly TOwner _owner;
        
        public State(TOwner owner)
        {
            _owner = owner;    
        }
        
        public virtual void OnEnter() { }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnFixedUpdate(float fixedDeltaTime) { }
        public virtual void OnExit() { }
        
        public virtual bool CanTransitionTo(State<TOwner> newState)
        {
            return true;
        }
        
        public virtual bool CanExit()
        {
            return true;
        }
    }
}
