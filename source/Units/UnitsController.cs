using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.MapSystem;

namespace SteelCustom.Units
{
    public class UnitsController : ScriptComponent
    {
        public List<Unit> Units => new List<Unit>(_units);

        private List<Unit> _units = new List<Unit>();

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
            
            _units.Add(unit);

            GameController.Instance.Player.Population = _units.Count;

            return unit;
        }

        public void RemoveUnit(Unit unit)
        {
            _units.Remove(unit);
            
            GameController.Instance.Player.Population = _units.Count;
        }

        private void TryMoveSelectedUnits()
        {
            foreach (Unit unit in GameController.Instance.SelectionController.SelectedObjects.OfType<Unit>())
            {
                TryMoveUnit(unit);
            }
        }

        private void TryMoveUnit(Unit unit)
        {
            Map map = GameController.Instance.Map;
            Tile tile = map.GetTileAt(Camera.Main.ScreenToWorldPoint(Input.MousePosition));
            
            IResource targetResource = null;
            Building targetBuilding = null;
            if (tile != null && tile.IsOccupied)
            {
                if (tile.OnObject is IResource resource && unit is Worker worker && resource.CanBeGathered(worker))
                {
                    tile = map.GetClosestPassableTile(unit.OnTile, resource.ToMapObject(), false);
                    if (tile != null)
                    {
                        targetResource = resource;
                    }
                }
                else if (tile.OnObject is Building building && unit is Worker worker2 && worker2.ResourceAmount > 0 && building.IsStorage(worker2.ResourceType))
                {
                    tile = map.GetClosestPassableTile(unit.OnTile, building, false);
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

            CreateCommandEffect(tile, targetResource, targetBuilding);
            
            unit.MoveToTarget(tile, targetResource, targetBuilding);
        }

        private void CreateCommandEffect(Tile tile, IResource resource, Building targetBuilding)
        {
            Vector3 position;
            if (resource != null)
                position = resource.Position;
            else if (targetBuilding != null)
                position = targetBuilding.Transformation.Position;
            else
                position = GameController.Instance.Map.CoordsToPosition(tile.X, tile.Y);
            
            Entity effect = ResourcesManager.GetAsepriteData("command_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = position + new Vector3(0, 0, 1);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.7f);
        }
    }
}