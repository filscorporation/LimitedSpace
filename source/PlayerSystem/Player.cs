using System.Collections.Generic;
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
        public TownCenter TownCenter { get; private set; }

        public List<SelectableObject> SelectedObjects => new List<SelectableObject>(_selectedObjects);
        
        private readonly List<SelectableObject> _selectedObjects = new List<SelectableObject>();

        public void Init()
        {
            
        }

        public void InitBuildingsAndResources()
        {
            Resources.Wood = 200;
            Resources.Food = 300;
            Resources.Gold = 100;

            Map map = GameController.Instance.Map;
            TownCenter = (TownCenter)GameController.Instance.BuilderController.PlaceBuildingInstant(BuildingType.TownCenter, new Vector2(map.Size * 0.5f - 0.5f, map.Size * 0.5f - 0.5f));

            TownCenter.SpawnUnit<Worker>();
            TownCenter.SpawnUnit<Worker>();
            TownCenter.SpawnUnit<Worker>();
        }

        public void Select(SelectableObject selectableObject)
        {
            foreach (SelectableObject selectedObject in _selectedObjects)
                selectedObject.Deselect();
            _selectedObjects.Clear();

            selectableObject.Select();
            _selectedObjects.Add(selectableObject);
            
            Log.LogInfo($"Selected {selectableObject}");
        }

        public void DeselectAll()
        {
            foreach (SelectableObject selectedObject in _selectedObjects)
                selectedObject.Deselect();
            _selectedObjects.Clear();
        }
    }
}