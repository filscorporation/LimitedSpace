using Steel;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom
{
    public interface IResource
    {
        ResourceType ResourceType { get; }
        int ResourceAmount { get; }
        float GatherDuration { get; }
        bool CanBeGathered { get; }
        
        Vector2 Position { get; }
        bool IsDestroyed { get; }
        
        bool TryGather();
        MapObject ToMapObject();
        bool CanGatherFrom(Tile onTile);
    }
}