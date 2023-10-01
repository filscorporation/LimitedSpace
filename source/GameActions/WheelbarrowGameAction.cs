using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public class WheelbarrowGameAction : TechnologyGameAction
    {
        public override string Name => "Wheelbarrow";
        public override string Description => "Increase workers speed and double their capacity";
        public override string Icon => "ui_wheelbarrow.png";
        public override float Duration => 60;
        public override ResourceCost Cost => new ResourceCost(200, 200, 100);

        public override bool IsVisible()
        {
            return base.IsVisible() && !GameController.Instance.Player.Effects.WheelbarrowGameActionReserved;
        }

        public override NotAvailableReason IsAvailable()
        {
            Player player = GameController.Instance.Player;
            if (!player.Resources.HasAmount(Cost))
                return NotAvailableReason.NoResources;
            return NotAvailableReason.None;
        }

        public override void ReserveAction()
        {
            GameController.Instance.Player.Effects.WheelbarrowGameActionReserved = true;
        }

        public override void DoAction(SelectableObject source)
        {
            Player player = GameController.Instance.Player;
            player.Effects.WorkerCapacityBonus += 10;
            player.Effects.WorkerSpeedBonus *= 1.3f;
        }
    }
}