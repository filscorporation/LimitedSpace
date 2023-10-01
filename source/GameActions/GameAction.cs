using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public abstract class GameAction
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Icon { get; }
        public abstract float Duration { get; }
        public abstract ResourceCost Cost { get; }
        
        public abstract NotAvailableReason IsAvailable();
        public virtual bool IsVisible() => true;
        public virtual bool CanDequeue() => true;
        public abstract void ReserveAction();
        public abstract void DoAction(SelectableObject source);
    }
}