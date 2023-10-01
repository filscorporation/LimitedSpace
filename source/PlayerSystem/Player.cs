using System;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.Units;

namespace SteelCustom.PlayerSystem
{
    public class Player : ScriptComponent
    {
        public event Action OnChanged;
        
        public PlayerResources Resources { get; } = new PlayerResources();
        public PlayerEffects Effects { get; } = new PlayerEffects();
        public bool IsInAdvancedEra { get; private set; }

        public int Population
        {
            get => _population;
            set
            {
                _population = value;
                OnChanged?.Invoke();
            }
        }
        
        public int PopulationSpace
        {
            get => _populationSpace;
            set
            {
                _populationSpace = value;
                OnChanged?.Invoke();
            }
        }
        
        public bool HasPopulationSpace => Population < PopulationSpace;
        
        public double GameTime { get; private set; }

        private TownCenter _townCenter;
        private int _population;
        private int _populationSpace;

        public override void OnUpdate()
        {
            UpdateTime();
        }

        public void Init()
        {
            
        }

        public void InitBuildingsAndResources()
        {
            Resources.Wood = 100;
            Resources.Food = 200;
            Resources.Gold = 0;

            Map map = GameController.Instance.Map;
            _townCenter = (TownCenter)GameController.Instance.BuilderController.PlaceBuildingInstant(BuildingType.TownCenter, new Vector2(map.Size * 0.5f - 0.5f, map.Size * 0.5f - 0.5f));
        }

        public void InitWorkers()
        {
            _townCenter.SpawnUnit<Worker>(true);
            _townCenter.SpawnUnit<Worker>(true);
            _townCenter.SpawnUnit<Worker>(true);
        }

        public void Advance()
        {
            IsInAdvancedEra = true;
            OnChanged?.Invoke();
        }

        private void UpdateTime()
        {
            if (GameController.Instance.GameState == GameState.Game)
                GameTime += Time.DeltaTime;
        }
    }
}