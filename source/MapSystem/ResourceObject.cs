namespace SteelCustom.MapSystem
{
    public class ResourceObject : MapObject
    {
        public void Place(Tile bottomLeftTile)
        {
            OnBottomLeftTile = bottomLeftTile;
        }
    }
}