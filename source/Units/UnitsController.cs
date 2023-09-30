using System.Linq;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.MapSystem;

namespace SteelCustom.Units
{
    public class UnitsController : ScriptComponent
    {
        public Unit HoveredUnit { get; set; }

        public override void OnUpdate()
        {
            if (Input.IsMouseJustPressed(MouseCodes.ButtonRight))
                TryMoveSelectedUnits();
        }

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

        private void TryMoveSelectedUnits()
        {
            foreach (Unit unit in GameController.Instance.Player.SelectedObjects.OfType<Unit>())
            {
                TryMoveUnit(unit);
            }
        }

        private void TryMoveUnit(Unit unit)
        {
            Map map = GameController.Instance.Map;
            Tile tile = map.GetTileAt(Camera.Main.ScreenToWorldPoint(Input.MousePosition));
            
            ResourceObject targetResourceObject = null;
            Building targetBuilding = null;
            if (tile != null && tile.IsOccupied)
            {
                if (tile.OnObject is ResourceObject resourceObject)
                {
                    tile = map.GetClosestPassableTileAround(unit.OnTile, resourceObject, false);
                    if (tile != null)
                    {
                        targetResourceObject = resourceObject;
                    }
                }
                else if (tile.OnObject is Building building && unit is Worker worker && worker.ResourceAmount > 0 && building.IsStorage(worker.ResourceType))
                {
                    tile = map.GetClosestPassableTileAround(unit.OnTile, building, false);
                    if (tile != null)
                    {
                        targetBuilding = building;
                    }
                }
                else
                    tile = null;
            }

            if (tile != null && tile.IsReserved)
            {
                if (tile.OnReservingUnit == unit)
                    tile = null;
                else
                    tile = map.GetPassableTilesAround(tile).FirstOrDefault();
            }
            
            if (tile == null)
                return;
            
            unit.MoveToTarget(tile, targetResourceObject, targetBuilding);
        }
    }
}