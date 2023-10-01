using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public class HeavyPlowGameAction : TechnologyGameAction
    {
        public override string Name => "Heavy plow";
        public override string Description => "Triple farm food capacity.";
        public override string Icon => "ui_heavy_plow.png";
        public override float Duration => 60;
        public override ResourceCost Cost => new ResourceCost(100, 100, 0);

        public override bool IsVisible()
        {
            return base.IsVisible() && !GameController.Instance.Player.Effects.HeavyPlowGameActionReserved;
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
            GameController.Instance.Player.Effects.HeavyPlowGameActionReserved = true;
        }

        public override void DoAction(SelectableObject source)
        {
            Player player = GameController.Instance.Player;
            player.Effects.FarmResourcesBonus += 200;
        }
    }
}