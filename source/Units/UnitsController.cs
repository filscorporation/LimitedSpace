using Steel;
using SteelCustom.MapSystem;

namespace SteelCustom.Units
{
    public class UnitsController : ScriptComponent
    {
        public void Init()
        {
            
        }

        public T SpawnUnit<T>(Tile tile) where T : Unit, new()
        {
            var entity = new Entity($"Unit {nameof(T)}", Entity);
            var unit = entity.AddComponent<T>();
            unit.Transformation.Position = (Vector3)GameController.Instance.Map.CoordsToPosition(tile.X, tile.Y) + new Vector3(0, 0, 0.3f);
            unit.Init(tile);

            return unit;
        }
    }
}