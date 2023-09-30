namespace SteelCustom.Buildings
{
    public class Wonder : Building
    {
        protected override string SpritePath { get; }
        public override BuildingType Type => BuildingType.Wonder;
    }
}