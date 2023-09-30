using System;
using System.Linq;
using Steel;

namespace SteelCustom.Buildings
{
    public class BuilderController : ScriptComponent
    {
        public void Init()
        {
            
        }

        public Building PlaceBuildingInstant(BuildingType buildingType, Vector2 position)
        {
            var building = CreateBuilding(buildingType);
            
            var map = GameController.Instance.Map;
            if (!map.IsValid(position, building.Size.X, building.Size.Y))
                Log.LogError("PlaceBuildingInstant position is not valid");
            
            building.Transformation.Position = (Vector3)map.GetCenterPosition(position, building.Size.X, building.Size.Y) + new Vector3(0, 0, 0.5f);
            Log.LogInfo($"TC position {building.Transformation.Position}");
            building.Init();
            var bottomLeftTile = map.GetBottomLeftTile(position, building.Size.X, building.Size.Y);
            var allTiles = map.GetTiles(position, building.Size.X, building.Size.Y).ToList();
            building.Place(bottomLeftTile, allTiles);

            return building;
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
    }
}