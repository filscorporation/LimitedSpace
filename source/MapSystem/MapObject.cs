using System.Collections.Generic;

namespace SteelCustom.MapSystem
{
    public abstract class MapObject : SelectableObject
    {
        public Tile OnBottomLeftTile { get; protected set; }
        public virtual (int X, int Y) Size => (2, 2);
        public virtual bool IsBlocking => true;

        protected List<Tile> _onTiles = new List<Tile>();

        public List<Tile> OnTiles() => new List<Tile>(_onTiles);
    }
}