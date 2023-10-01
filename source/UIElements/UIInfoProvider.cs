using Steel;
using SteelCustom.Buildings;
using SteelCustom.GameActions;

namespace SteelCustom.UIElements
{
    public class UIInfoProvider : ScriptComponent
    {
        private UIInfo _info;
        private GameAction _gameAction;
        private BuildingType? _buildingType;
        
        public override void OnMouseEnterUI()
        {
            if (_gameAction != null)
            {
                _info.Set(_gameAction.Name, _gameAction.Description, _gameAction.Cost);
            }

            if (_buildingType.HasValue)
            {
                _info.Set(BuildingsInfo.GetName(_buildingType.Value), BuildingsInfo.GetDescription(_buildingType.Value), BuildingsInfo.GetCost(_buildingType.Value));
            }
        }
        
        public override void OnMouseExitUI()
        {
            _info.Hide();
        }

        public void Init(UIInfo info, GameAction gameAction)
        {
            _info = info;
            _gameAction = gameAction;
        }

        public void Init(UIInfo info, BuildingType buildingType)
        {
            _info = info;
            _buildingType = buildingType;
        }
    }
}