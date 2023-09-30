using Steel;
using SteelCustom.Buildings;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.Units;

namespace SteelCustom.PlayerSystem
{
    public class Player : ScriptComponent
    {
        public PlayerResources Resources { get; } = new PlayerResources();
        
        public void Init()
        {
            
        }

        public void InitBuildingsAndResources()
        {
            Resources.Wood = 200;
            Resources.Food = 300;
            Resources.Gold = 100;

            Map map = GameController.Instance.Map;
            var tc = (TownCenter)GameController.Instance.BuilderController.PlaceBuildingInstant(BuildingType.TownCenter, new Vector2(map.Size * 0.5f + 0.5f, map.Size * 0.5f + 0.5f));

            tc.SpawnUnit<Worker>();
            tc.SpawnUnit<Worker>();
            tc.SpawnUnit<Worker>();
        }
    }
}