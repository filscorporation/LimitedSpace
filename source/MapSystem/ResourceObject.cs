using System.Collections.Generic;
using Steel;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.MapSystem
{
    public abstract class ResourceObject : MapObject, IResource
    {
        public abstract float GatherDuration { get; }
        public Vector2 Position => Transformation.Position;
        public bool IsDestroyed => Entity.IsDestroyed();
        public abstract ResourceType ResourceType { get; }
        public int ResourceAmount { get; protected set; }
        public bool CanBeGathered => ResourceAmount > 0;

        public virtual void Init()
        {
            ReplaceShader();
        }

        public void Dispose()
        {
            foreach (var tile in _onTiles)
            {
                tile.ClearOnObject();
            }
        }

        public void Place(Tile bottomLeftTile, List<Tile> allTiles)
        {
            OnBottomLeftTile = bottomLeftTile;
            _onTiles = allTiles;
            
            foreach (var tile in _onTiles)
            {
                tile.SetOnObject(this);
            }
        }

        public bool TryGather()
        {
            if (ResourceAmount <= 0)
                return false;

            ResourceAmount--;
            OnGathered();

            if (ResourceAmount <= 0)
                OnResourceDepleted();
            
            return true;
        }

        public MapObject ToMapObject()
        {
            return this;
        }

        public bool CanGatherFrom(Tile onTile)
        {
            return GameController.Instance.Map.IsNextTo(onTile, ToMapObject());
        }

        protected virtual void OnGathered()
        {
            OverrideColor(new Color(240, 233, 201), 0.1f);
        }

        public void Destroy()
        {
            Dispose();
            
            Entity.Destroy();
        }

        protected void OnResourceDepleted()
        {
            Destroy();
        }
    }
}