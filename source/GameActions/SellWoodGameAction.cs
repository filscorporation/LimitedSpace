using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public class SellWoodGameAction : GameAction
    {
        public override string Name => "Sell Wood";
        public override string Description => "Sell 500 Wood for 100 Gold.";
        public override string Icon => "ui_sell_wood.png";
        public override float Duration => 0;
        public override ResourceCost Cost => new ResourceCost(500, 0, 0);

        public override NotAvailableReason IsAvailable()
        {
            Player player = GameController.Instance.Player;
            if (!player.Resources.HasAmount(Cost))
                return NotAvailableReason.NoResources;
            return NotAvailableReason.None;
        }

        public override void ReserveAction()
        {
            
        }

        public override void DoAction(SelectableObject source)
        {
            GameController.Instance.Player.Resources.Gold += 100;
        }
    }
}