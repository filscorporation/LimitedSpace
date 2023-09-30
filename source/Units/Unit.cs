using Steel;
using SteelCustom.MapSystem;

namespace SteelCustom.Units
{
    public abstract class Unit : ScriptComponent
    {
        public Tile OnTile { get; protected set; }
        
        protected abstract string SpritePath { get; }

        public virtual void Init(Tile tile)
        {
            Sprite sprite = ResourcesManager.GetImage(SpritePath);
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;

            OnTile = tile;
            tile.SetOnReservingUnit(this);
        }
        
        public virtual void Dispose()
        {
            
        }
    }
}