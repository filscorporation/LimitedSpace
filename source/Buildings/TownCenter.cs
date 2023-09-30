using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Buildings
{
    public class TownCenter : Building
    {
        public override (int X, int Y) Size => (10, 10);
        protected override string SpritePath => "town_center.png";
        public override BuildingType Type => BuildingType.TownCenter;

        public override bool IsStorage(ResourceType resourceType)
        {
            return true;
        }
    }
}