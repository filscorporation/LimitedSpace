using System.Collections.Generic;
using SteelCustom.GameActions;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Buildings
{
    public class LumberMill : Building
    {
        public override (int X, int Y) Size => (4, 4);
        protected override string SpritePath => "lumber_mill.png";
        public override BuildingType Type => BuildingType.LumberMill;
        public override float BuildingDuration => 10;

        public override string Name => "Lumber mill";
        public override List<GameAction> GameActions => new List<GameAction> { new BowSawGameAction() };

        public override bool IsStorage(ResourceType resourceType)
        {
            return resourceType == ResourceType.Wood;
        }
    }
}