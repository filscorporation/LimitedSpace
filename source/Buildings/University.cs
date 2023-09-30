namespace SteelCustom.Buildings
{
    public class University : Building
    {
        protected override string SpritePath { get; }
        public override BuildingType Type => BuildingType.University;
    }
}