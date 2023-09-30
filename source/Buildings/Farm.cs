namespace SteelCustom.Buildings
{
    public class Farm : Building
    {
        protected override string SpritePath { get; }
        public override bool IsBlocking => false;
    }
}