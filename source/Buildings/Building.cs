using System.Linq;
using Steel;
using SteelCustom.MapSystem;
using SteelCustom.Units;

namespace SteelCustom.Buildings
{
    public abstract class Building : MapObject
    {
        protected abstract string SpritePath { get; }

        public virtual void Init()
        {
            Sprite sprite = ResourcesManager.GetImage(SpritePath);
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;
        }
        
        public virtual void Dispose()
        {
            
        }

        public void Place(Tile bottomLeftTile)
        {
            OnBottomLeftTile = bottomLeftTile;
        }

        public T SpawnUnit<T>() where T : Unit, new()
        {
            var tile = GameController.Instance.Map.GetPassableTilesAround(this).FirstOrDefault();
            if (tile == null)
                return null;

            return GameController.Instance.UnitsController.SpawnUnit<T>(tile);
        }
    }
}