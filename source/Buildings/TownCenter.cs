namespace SteelCustom.Buildings
{
    public class TownCenter : Building
    {
        public override (int X, int Y) Size => (10, 10);
        protected override string SpritePath => "town_center.png";
    }
}