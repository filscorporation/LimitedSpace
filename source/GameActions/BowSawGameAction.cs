using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public class BowSawGameAction : TechnologyGameAction
    {
        public override string Name => "Bow saw";
        public override string Description => "Workers chop wood 50% faster";
        public override string Icon => "ui_bow_saw.png";
        public override float Duration => 30;
        public override ResourceCost Cost => new ResourceCost(200, 100, 0);

        public override bool IsVisible()
        {
            return base.IsVisible() && !GameController.Instance.Player.Effects.BowSawGameActionReserved;
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
            GameController.Instance.Player.Effects.BowSawGameActionReserved = true;
        }

        public override void DoAction(SelectableObject source)
        {
            Player player = GameController.Instance.Player;
            player.Effects.WorkerWoodGatherSpeedBonus *= 1.5f;
        }
    }
}