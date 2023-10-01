using SteelCustom.Buildings;
using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.Units;

namespace SteelCustom.GameActions
{
    public class CreateWorkerGameAction : GameAction
    {
        public override string Name => "Worker";
        public override string Description => "Create new worker that can gather resources. Requires free housing.";
        public override string Icon => "ui_create_worker.png";
        public override float Duration => 30;
        public override ResourceCost Cost => new ResourceCost(0, 50, 0);

        public override NotAvailableReason IsAvailable()
        {
            Player player = GameController.Instance.Player;
            if (!player.Resources.HasAmount(Cost))
                return NotAvailableReason.NoResources;
            if (!player.HasPopulationSpace)
                return NotAvailableReason.NoPopulationSpace;
            return NotAvailableReason.None;
        }

        public override bool CanDequeue()
        {
            Player player = GameController.Instance.Player;
            return base.CanDequeue() && player.HasPopulationSpace;
        }

        public override void ReserveAction()
        {
            
        }

        public override void DoAction(SelectableObject source)
        {
            TownCenter tc = (TownCenter)source;
            tc.SpawnUnit<Worker>();
        }
    }
}