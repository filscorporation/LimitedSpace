using System.Collections.Generic;
using SteelCustom.GameActions;

namespace SteelCustom.Buildings
{
    public class Market : Building
    {
        public override (int X, int Y) Size => (10, 10);
        public override string Name => "Market";
        protected override string SpritePath => "market.png";
        public override BuildingType Type => BuildingType.Market;
        public override float BuildingDuration => 45;
        public override List<GameAction> GameActions => new List<GameAction>();
    }
}