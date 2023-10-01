using System.Collections.Generic;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.Units;

namespace SteelCustom.Buildings
{
    public class Farm : Building, IResource
    {
        public override (int X, int Y) Size => (6, 6);
        protected override string SpritePath => "farm.png";
        public override BuildingType Type => BuildingType.Farm;
        public override float BuildingDuration => 5f;
        public override bool IsBlocking => false;
        public Vector2 Position => Transformation.Position;
        public bool IsDestroyed => Entity.IsDestroyed();

        public override string Name => "Farm";
        public override List<GameAction> GameActions => new List<GameAction>();

        public ResourceType ResourceType => ResourceType.Food;
        public int ResourceAmount { get; private set; }
        public float GatherDuration => 1;
        public bool CanHandleMultipleWorkers => false;

        private Worker _currentGatheredBy;
        private bool _resetCurrentGatheredBy;
        
        private const int MAX_WOOD_AMOUNT = 100;

        public bool TryGather(Worker worker)
        {
            if (!CanBeGathered(worker))
                return false;
            
            _currentGatheredBy = worker;
            
            if (ResourceAmount <= 0)
                return false;

            ResourceAmount--;
            OnGathered();

            if (ResourceAmount <= 0)
                OnResourceDepleted();
            
            return true;
        }

        public bool CanBeGathered(Worker worker)
        {
            return IsBuilt && (_currentGatheredBy == null || _currentGatheredBy == worker);
        }

        public MapObject ToMapObject()
        {
            return this;
        }

        private void OnGathered()
        {
            // fx?
            OverrideColor(new Color(240, 233, 201), 0.1f);
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            if (_resetCurrentGatheredBy)
            {
                _currentGatheredBy = null;
                _resetCurrentGatheredBy = false;
            }
            if (_currentGatheredBy != null)
            {
                _resetCurrentGatheredBy = true;
            }
        }

        protected override void OnPlaced()
        {
            base.OnPlaced();

            ResourceAmount = MAX_WOOD_AMOUNT + GameController.Instance.Player.Effects.FarmResourcesBonus;
        }

        private void OnResourceDepleted()
        {
            Destroy();
        }

        public bool CanGatherFrom(Tile onTile)
        {
            return _onTiles.Contains(onTile);
        }
    }
}