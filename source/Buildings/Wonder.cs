using System.Collections.Generic;
using SteelCustom.GameActions;

namespace SteelCustom.Buildings
{
    public class Wonder : Building
    {
        public override (int X, int Y) Size => (16, 16);
        protected override string SpritePath => "wonder.png";
        public override BuildingType Type => BuildingType.Wonder;
        public override float BuildingDuration => 120f;

        public override string Name => "Wonder";
        public override List<GameAction> GameActions => new List<GameAction>();

        protected override void OnConstructed()
        {
            base.OnConstructed();
            
            GameController.Instance.WinGame();
        }
    }
}