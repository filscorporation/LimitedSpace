using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.GameActions
{
    public class TreadmillCraneGameAction : TechnologyGameAction
    {
        public override string Name => "Treadmill crane";
        public override string Description => "Double construction speed.\nRequires advanced era.";
        public override string Icon => "ui_treadmill_crane.png";
        public override float Duration => 60;
        public override ResourceCost Cost => new ResourceCost(500, 300, 200);

        public override bool IsVisible()
        {
            return base.IsVisible() && !GameController.Instance.Player.Effects.TreadmillCraneGameActionReserved;
        }

        public override NotAvailableReason IsAvailable()
        {
            Player player = GameController.Instance.Player;
            if (!player.Resources.HasAmount(Cost))
                return NotAvailableReason.NoResources;
            if (!player.IsInAdvancedEra)
                return NotAvailableReason.NotAdvancedEra;
            return NotAvailableReason.None;
        }

        public override void ReserveAction()
        {
            GameController.Instance.Player.Effects.TreadmillCraneGameActionReserved = true;
        }

        public override void DoAction(SelectableObject source)
        {
            Player player = GameController.Instance.Player;
            player.Effects.ConstructionSpeedBonus *= 2;
        }
    }
}