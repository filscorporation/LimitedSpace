using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public class AdvanceGameAction : TechnologyGameAction
    {
        public override string Name => "Advance";
        public override string Description => "Advance to new era, unlocking new technologies and a wonder, required to win.";
        public override string Icon => "ui_advance.png";
        public override float Duration => 90;
        public override ResourceCost Cost => new ResourceCost(0, 500, 0);

        public override bool IsVisible()
        {
            return base.IsVisible() && !GameController.Instance.Player.Effects.AdvanceGameActionReserved && !GameController.Instance.Player.IsInAdvancedEra;
        }

        public override void ReserveAction()
        {
            GameController.Instance.Player.Effects.AdvanceGameActionReserved = true;
        }

        public override NotAvailableReason IsAvailable()
        {
            Player player = GameController.Instance.Player;
            if (!player.Resources.HasAmount(Cost))
                return NotAvailableReason.NoResources;
            return NotAvailableReason.None;
        }

        public override void DoAction(SelectableObject source)
        {
            Player player = GameController.Instance.Player;
            player.Advance();
        }
    }
}