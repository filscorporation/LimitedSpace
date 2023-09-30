using Steel;

namespace SteelCustom.MapSystem
{
    public abstract class MapObject : ScriptComponent
    {
        public Tile OnBottomLeftTile { get; protected set; }
        public virtual (int X, int Y) Size => (2, 2);
        public virtual bool IsBlocking => true;
    }
}