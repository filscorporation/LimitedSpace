using System;
using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.MapSystem;

namespace SteelCustom.Buildings
{
    public class BuilderController : ScriptComponent
    {
        public bool HasDraft => _draftBuilding != null && !_draftBuilding.Entity.IsDestroyed();
        
        public AudioSource AudioSource { get; private set; }
        
        private Building _draftBuilding;
        
        public override void OnUpdate()
        {
            UpdateBuildingHotkeys();
            UpdateDraft();
        }

        public void Init()
        {
            AudioSource = Entity.AddComponent<AudioSource>();
        }

        public Building PlaceBuildingInstant(BuildingType buildingType, Vector2 position)
        {
            var building = CreateBuilding(buildingType);
            
            var map = GameController.Instance.Map;
            if (!map.IsValid(position, building.Size.X, building.Size.Y))
                Log.LogError("PlaceBuildingInstant position is not valid");
            
            building.Transformation.Position = (Vector3)map.GetCenterPosition(position, building.Size.X, building.Size.Y) + new Vector3(0, 0, building.GetZ());
            Log.LogInfo($"TC position {building.Transformation.Position}");
            building.Init();
            var bottomLeftTile = map.GetBottomLeftTile(position, building.Size.X, building.Size.Y);
            var allTiles = map.GetTiles(position, building.Size.X, building.Size.Y).ToList();
            building.Place(bottomLeftTile, allTiles, true);

            return building;
        }

        public void TryStartPlacing(BuildingType buildingType)
        {
            if (CanStartPlacing(buildingType) != NotAvailableReason.None)
                return;
            
            CreateDraft(buildingType);
            UpdateDraft();
        }

        public NotAvailableReason CanStartPlacing(BuildingType buildingType)
        {
            if (!GameController.Instance.Player.Resources.HasAmount(BuildingsInfo.GetCost(buildingType)))
                return NotAvailableReason.NoResources;
            
            if (buildingType == BuildingType.Wonder && !GameController.Instance.Player.IsInAdvancedEra)
                return NotAvailableReason.NotAdvancedEra;
            
            return NotAvailableReason.None;
        }

        private void UpdateBuildingHotkeys()
        {
            if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft) && !UI.IsPointerOverUI())
            {
                PlaceDraft();
            }
            if (Input.IsMouseJustPressed(MouseCodes.ButtonRight))
            {
                ClearDraft();
            }
            
            if (Input.IsKeyJustPressed(KeyCode.D1))
            {
                TryStartPlacing(BuildingType.House);
            }
            if (Input.IsKeyJustPressed(KeyCode.D2))
            {
                TryStartPlacing(BuildingType.Farm);
            }
            if (Input.IsKeyJustPressed(KeyCode.D3))
            {
                TryStartPlacing(BuildingType.TownCenter);
            }
            if (Input.IsKeyJustPressed(KeyCode.D4))
            {
                TryStartPlacing(BuildingType.LumberMill);
            }
            if (Input.IsKeyJustPressed(KeyCode.D5))
            {
                TryStartPlacing(BuildingType.Mill);
            }
            if (Input.IsKeyJustPressed(KeyCode.D6))
            {
                TryStartPlacing(BuildingType.Market);
            }
            if (Input.IsKeyJustPressed(KeyCode.D9))
            {
                TryStartPlacing(BuildingType.Wonder);
            }
            
            if (Input.IsKeyJustPressed(KeyCode.Delete))
            {
                foreach (Building building in GameController.Instance.SelectionController.SelectedObjects.OfType<Building>())
                {
                    building.Destroy();
                }
            }
        }

        private void CreateDraft(BuildingType buildingType)
        {
            ClearDraft();

            _draftBuilding = CreateBuilding(buildingType);
            _draftBuilding.Init();

            _draftBuilding.SetIsDraft(true);
        }

        private void UpdateDraft()
        {
            if (_draftBuilding == null)
                return;
            
            var map = GameController.Instance.Map;

            Vector2 position = Camera.Main.ScreenToWorldPoint(Input.MousePosition);
            _draftBuilding.Transformation.Position = (Vector3)map.GetCenterPosition(position, _draftBuilding.Size.X, _draftBuilding.Size.Y) + new Vector3(0, 0, _draftBuilding.GetZ());
            
            _draftBuilding.SetCanPlace(CanPlaceDraft());
        }

        private void PlaceDraft()
        {
            if (!CanPlaceDraft())
                return;
            
            var map = GameController.Instance.Map;
            
            Vector2 position = _draftBuilding.Transformation.Position;

            var cost = BuildingsInfo.GetCost(_draftBuilding.Type);
            GameController.Instance.Player.Resources.SpendAmount(cost);

            _draftBuilding.SetIsDraft(false);
            
            _draftBuilding.Transformation.Position = (Vector3)map.GetCenterPosition(position, _draftBuilding.Size.X, _draftBuilding.Size.Y) + new Vector3(0, 0, _draftBuilding.GetZ());
            var bottomLeftTile = map.GetBottomLeftTile(position, _draftBuilding.Size.X, _draftBuilding.Size.Y);
            var allTiles = map.GetTiles(position, _draftBuilding.Size.X, _draftBuilding.Size.Y).ToList();
            _draftBuilding.Place(bottomLeftTile, allTiles);

            _draftBuilding = null;
        }

        private bool CanPlaceDraft()
        {
            if (_draftBuilding == null)
                return false;
            
            var map = GameController.Instance.Map;
            
            Vector2 position = _draftBuilding.Transformation.Position;
            
            var allTiles = map.GetTiles(position, _draftBuilding.Size.X, _draftBuilding.Size.Y).ToList();
            if (allTiles.Any(t => t.IsOccupied) || allTiles.Count != _draftBuilding.Size.X * _draftBuilding.Size.Y)
                return false;
            
            var allTilesHashSet = new HashSet<Tile>(allTiles);
            if (GameController.Instance.UnitsController.Units.Any(u => allTilesHashSet.Contains(u.OnTile)))
                return false;

            var cost = BuildingsInfo.GetCost(_draftBuilding.Type);
            if (!GameController.Instance.Player.Resources.HasAmount(cost))
                return false;

            return true;
        }

        private void ClearDraft()
        {
            if (_draftBuilding != null)
            {
                _draftBuilding.Destroy();
                _draftBuilding = null;
            }
        }

        private Building CreateBuilding(BuildingType buildingType)
        {
            var entity = new Entity($"Building {buildingType}", Entity);

            Building building;
            switch (buildingType)
            {
                case BuildingType.TownCenter:
                    building = entity.AddComponent<TownCenter>();
                    break;
                case BuildingType.House:
                    building = entity.AddComponent<House>();
                    break;
                case BuildingType.Farm:
                    building = entity.AddComponent<Farm>();
                    break;
                case BuildingType.LumberMill:
                    building = entity.AddComponent<LumberMill>();
                    break;
                case BuildingType.Mill:
                    building = entity.AddComponent<Mill>();
                    break;
                case BuildingType.Market:
                    building = entity.AddComponent<Market>();
                    break;
                case BuildingType.Wonder:
                    building = entity.AddComponent<Wonder>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }

            return building;
        }
    }
}