﻿using System;
using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Buildings
{
    public class BuilderController : ScriptComponent
    {
        private Building _draftBuilding;
        
        public override void OnUpdate()
        {
            UpdateBuildingHotkeys();
            UpdateDraft();
        }

        public void Init()
        {
            
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
            building.Place(bottomLeftTile, allTiles);

            return building;
        }

        private void UpdateBuildingHotkeys()
        {
            if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft))
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
        }

        private void TryStartPlacing(BuildingType buildingType)
        {
            if (!GameController.Instance.Player.Resources.HasAmount(GetBuildingCost(buildingType)))
                return;
            
            CreateDraft(buildingType);
            UpdateDraft();
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
        }

        private void PlaceDraft()
        {
            if (_draftBuilding == null)
                return;
            
            var map = GameController.Instance.Map;
            
            Vector2 position = _draftBuilding.Transformation.Position;
            
            var allTiles = map.GetTiles(position, _draftBuilding.Size.X, _draftBuilding.Size.Y).ToList();
            if (allTiles.Any(t => t.IsOccupied))
                return;
            
            var allTilesHashSet = new HashSet<Tile>(allTiles);
            if (GameController.Instance.UnitsController.Units.Any(u => allTilesHashSet.Contains(u.OnTile)))
                return;

            var cost = GetBuildingCost(_draftBuilding.Type);
            if (!GameController.Instance.Player.Resources.HasAmount(cost))
                return;
            
            GameController.Instance.Player.Resources.SpendAmount(cost);

            _draftBuilding.SetIsDraft(false);
            
            _draftBuilding.Transformation.Position = (Vector3)map.GetCenterPosition(position, _draftBuilding.Size.X, _draftBuilding.Size.Y) + new Vector3(0, 0, _draftBuilding.GetZ());
            var bottomLeftTile = map.GetBottomLeftTile(position, _draftBuilding.Size.X, _draftBuilding.Size.Y);
            _draftBuilding.Place(bottomLeftTile, allTiles);

            _draftBuilding = null;
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
                case BuildingType.University:
                    building = entity.AddComponent<University>();
                    break;
                case BuildingType.Wonder:
                    building = entity.AddComponent<Wonder>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }

            return building;
        }

        private ResourceCost GetBuildingCost(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.TownCenter:
                    return new ResourceCost(300, 0, 0);
                case BuildingType.House:
                    return new ResourceCost(30, 0, 0);
                case BuildingType.Farm:
                    return new ResourceCost(60, 0, 0);
                case BuildingType.University:
                    return new ResourceCost(200, 0, 0);
                case BuildingType.Wonder:
                    return new ResourceCost(5000, 0, 2000);
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }
    }
}