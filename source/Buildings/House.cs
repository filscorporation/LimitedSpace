using System.Collections.Generic;
using Steel;
using SteelCustom.GameActions;

namespace SteelCustom.Buildings
{
    public class House : Building
    {
        public override (int X, int Y) Size => (4, 4);
        protected override string SpritePath => "house.png";
        public override BuildingType Type => BuildingType.House;
        public override float BuildingDuration => 10f;

        public override string Name => "House";
        public override List<GameAction> GameActions => new List<GameAction>();

        protected override void OnConstructed()
        {
            base.OnConstructed();

            GameController.Instance.Player.PopulationSpace += 5;
            
            Log.LogInfo($"House increase pop space to {GameController.Instance.Player.Population}/{GameController.Instance.Player.PopulationSpace}");
        }

        public override void Dispose()
        {
            base.Dispose();
            
            if (IsBuilt)
                GameController.Instance.Player.PopulationSpace -= 5;
        }
    }
}