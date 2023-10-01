using Steel;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.Units;

namespace SteelCustom
{
    public interface IResource
    {
        ResourceType ResourceType { get; }
        int ResourceAmount { get; }
        float GatherDuration { get; }
        
        Vector2 Position { get; }
        bool IsDestroyed { get; }
        
        bool TryGather(Worker worker);
        bool CanBeGathered(Worker worker);
        MapObject ToMapObject();
        bool CanGatherFrom(Tile onTile);
    }
}