using System.Linq;
using Steel;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Units
{
    public class Worker : Unit
    {
        public int ResourceAmount { get; private set; }
        public ResourceType ResourceType { get; private set; }
        public bool IsFull => ResourceAmount >= Capacity;
        
        protected override string SpritePath => _isMale ? "worker_m.png" : "worker_f.png";
        protected override float Speed => 3f;
        protected float GatherSpeed => 1;
        protected int Capacity => 10;

        private bool _isMale;
        private bool _isWorking;
        
        private float _gatherTimer;

        public override void Init(Tile tile)
        {
            _isMale = Random.NextFloat(0, 1) < 0.5f;
            
            base.Init(tile);
        }

        protected override void UpdateUnit()
        {
            base.UpdateUnit();

            if (_isWorking)
            {
                Gather(_targetResourceObject);
            }
        }

        protected override void OnMovementStarted()
        {
            _isWorking = false;
            _gatherTimer = 0;
        }

        protected override void OnMovementCompleted()
        {
            if ((_targetResourceObject == null || _targetResourceObject.Entity.IsDestroyed()) && _targetResourceObjectType != null)
            {
                if (!TryFindNewResourceObject())
                {
                    _targetResourceObjectType = null;
                    _targetBuilding = null;
                }
            }
            if (_targetResourceObject != null && !_targetResourceObject.Entity.IsDestroyed() && GameController.Instance.Map.IsNextTo(_onTile, _targetResourceObject))
            {
                _isWorking = true;
            }
            if (_targetBuilding != null && !_targetBuilding.Entity.IsDestroyed() && GameController.Instance.Map.IsNextTo(_onTile, _targetBuilding))
            {
                DropResource();
                
                if (_targetResourceObject != null && !_targetResourceObject.Entity.IsDestroyed())
                {
                    if (GameController.Instance.Map.IsNextTo(_onTile, _targetResourceObject))
                        _isWorking = true;
                    else
                    {
                        var tile = GameController.Instance.Map.GetClosestPassableTileAround(_onTile, _targetResourceObject, false);
                        if (tile != null)
                            MoveTo(tile);
                    }
                }
            }
        }

        private bool TryFindNewResourceObject()
        {
            if (!_targetResourceObjectPosition.HasValue || !_targetResourceObjectType.HasValue)
                return false;
            
            var resourceObject = GameController.Instance.Map.GetClosestResourceObject(_targetResourceObjectPosition.Value, _targetResourceObjectType.Value);
            if (resourceObject != null)
            {
                _targetResourceObject = resourceObject;
                return true;
            }

            return false;
        }

        private void Gather(ResourceObject resourceObject)
        {
            if (IsFull || resourceObject == null || resourceObject.Entity.IsDestroyed() || resourceObject.ResourceAmount <= 0)
            {
                GoToStorage();
                return;
            }
            
            _gatherTimer += GatherSpeed * Time.DeltaTime;
            if (_gatherTimer >= resourceObject.GatherDuration)
            {
                _gatherTimer = 0;
                if (resourceObject.TryGather())
                {
                    AddResource(1, resourceObject.ResourceType);
                }
            }
            
            Rotate(resourceObject.Transformation.Position);
        }

        private void GoToStorage()
        {
            Log.LogInfo($"Worker go to storage: {ResourceAmount} {ResourceType}");
            var closestStorage = GameController.Instance.Map.GetClosestStorage(_onTile, ResourceType);
            if (closestStorage != null)
            {
                var closestStorageTile = GameController.Instance.Map.GetClosestPassableTileAround(_onTile, closestStorage, false);
                if (closestStorageTile != null)
                {
                    _targetBuilding = closestStorage;
                    MoveTo(closestStorageTile);
                }
            }
            else
            {
                // wait
            }
        }

        private void AddResource(int amount, ResourceType resourceType)
        {
            if (ResourceType != resourceType)
                ResourceAmount = 0;

            ResourceType = resourceType;
            ResourceAmount += amount;
        }

        private void DropResource()
        {
            if (ResourceAmount <= 0)
                return;
            
            Log.LogInfo($"Drop resource {ResourceType}: {GameController.Instance.Player.Resources.GetAmount(ResourceType)} +{ResourceAmount}");
            
            GameController.Instance.Player.Resources.AddAmount(ResourceType, ResourceAmount);
            ResourceAmount = 0;
        }
    }
}