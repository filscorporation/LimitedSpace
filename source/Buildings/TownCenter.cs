using System.Collections.Generic;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Buildings
{
    public class TownCenter : Building
    {
        public override (int X, int Y) Size => (10, 10);
        protected override string SpritePath => GameController.Instance.Player.IsInAdvancedEra ? "advanced_town_center.png" : "town_center.png";
        public override BuildingType Type => BuildingType.TownCenter;
        public override float BuildingDuration => 30f;

        public override string Name => "Town center";
        public override List<GameAction> GameActions => new List<GameAction> { new CreateWorkerGameAction(), new WheelbarrowGameAction(), new AdvanceGameAction(), new TreadmillCraneGameAction() };

        private bool _isAdvanced;

        public override bool IsStorage(ResourceType resourceType)
        {
            return true;
        }

        protected override void OnConstructed()
        {
            base.OnConstructed();

            _isAdvanced = GameController.Instance.Player.IsInAdvancedEra;

            GameController.Instance.Player.PopulationSpace += 5;
            GameController.Instance.Player.OnChanged += OnPlayerChanged;

            UpdateImage();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (IsBuilt)
            {
                GameController.Instance.Player.PopulationSpace -= 5;
                GameController.Instance.Player.OnChanged -= OnPlayerChanged;
            }
        }

        private void UpdateImage()
        {
            if (GameController.Instance.Player.IsInAdvancedEra != _isAdvanced)
            {
                _isAdvanced = GameController.Instance.Player.IsInAdvancedEra;

                GetComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage(SpritePath);
            }
        }

        private void OnPlayerChanged()
        {
            UpdateImage();
        }
    }
}