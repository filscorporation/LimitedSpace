namespace SteelCustom.Buildings
{
    public class Farm : Building
    {
        public override (int X, int Y) Size => (6, 6);
        protected override string SpritePath => "farm.png";
        public override BuildingType Type => BuildingType.Farm;
        public override bool IsBlocking => false;
    }
}