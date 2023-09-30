namespace SteelCustom.Buildings
{
    public class House : Building
    {
        public override (int X, int Y) Size => (4, 4);
        protected override string SpritePath => "house.png";
        public override BuildingType Type => BuildingType.House;
    }
}