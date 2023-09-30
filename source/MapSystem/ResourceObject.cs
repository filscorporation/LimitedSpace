using System.Collections.Generic;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.MapSystem
{
    public abstract class ResourceObject : MapObject
    {
        public abstract float GatherDuration { get; }
        public abstract ResourceType ResourceType { get; }
        public int ResourceAmount { get; protected set; }
        
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

            if (ResourceAmount <= 0)
                OnResourceDepleted();
            
            return true;
        }

        public void Destroy()
        {
            foreach (var tile in _onTiles)
            {
                tile.ClearOnObject();
            }
            
            Entity.Destroy();
        }

        protected void OnResourceDepleted()
        {
            Destroy();
        }
    }
}