using System;
using System.Collections.Generic;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem;
using SteelCustom.PlayerSystem.Resources;
using Random = Steel.Random;

namespace SteelCustom.Units
{
    public class Worker : Unit
    {
        public int ResourceAmount { get; private set; }
        public ResourceType ResourceType { get; private set; }
        public bool IsFull => ResourceAmount >= Capacity;

        public override string Name => "Worker";
        public override List<GameAction> GameActions => new List<GameAction>();
        
        protected override string SpritePath => _isMale ? "worker_m.png" : "worker_f.png";
        protected override float Speed => 3f * _playerEffects.WorkerSpeedBonus;
        protected int Capacity => 10 + _playerEffects.WorkerCapacityBonus;

        private bool _isMale;
        private bool _isWorking;

        private bool _isIdleEffectActive;
        private Entity _idleEffect;
        private PlayerEffects _playerEffects;
        private float _gatherTimer;
        private AudioSource _audioSource;
        private AudioTrack _hitAudio;

        private const float GATHER_SPEED = 1f;

        public override void Init(Tile tile)
        {
            _isMale = Random.NextFloat(0, 1) < 0.5f;
            _playerEffects = GameController.Instance.Player.Effects;

            _audioSource = Entity.AddComponent<AudioSource>();
            _hitAudio = ResourcesManager.GetAudioTrack("hit.wav");

            _idleEffect = new Entity("Idle", Entity);
            _idleEffect.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage("idle_icon.png");
            _idleEffect.Transformation.LocalPosition = new Vector3(0, 1, 1);
            _idleEffect.IsActiveSelf = false;
            _isIdleEffectActive = false;
            
            UpdateIdle(true);
            
            base.Init(tile);
        }

        protected override void UpdateUnit()
        {
            base.UpdateUnit();

            if (_isWorking)
            {
                Gather(_targetResource);
            }
        }

        protected override void OnMovementStarted()
        {
            _isWorking = false;
            _gatherTimer = 0;
            
            UpdateIdle(false);
        }

        protected override void OnMovementCompleted()
        {
            UpdateIdle(true);
            
            if ((_targetResource == null || _targetResource.IsDestroyed || !_targetResource.CanBeGathered(this)) && _targetResourceObjectType != null)
            {
                if (!TryFindNewResourceObject())
                {
                    _targetResourceObjectType = null;
                    _targetBuilding = null;
                }
            }
            if (_targetResource != null && !_targetResource.IsDestroyed && _targetResource.CanBeGathered(this) && _targetResource.CanGatherFrom(_onTile))
            {
                _isWorking = true;
                
                UpdateIdle(false);
            }
            if (_targetBuilding != null && !_targetBuilding.Entity.IsDestroyed() && GameController.Instance.Map.IsNextTo(_onTile, _targetBuilding))
            {
                DropResource();
                
                if (_targetResource != null && !_targetResource.IsDestroyed && _targetResource.CanBeGathered(this))
                {
                    if (GameController.Instance.Map.IsNextTo(_onTile, _targetResource.ToMapObject()))
                        _isWorking = true;
                    else
                    {
                        var tile = GameController.Instance.Map.GetClosestPassableTile(_onTile, _targetResource.ToMapObject(), false);
                        if (tile != null)
                        {
                            MoveTo(tile);
                        }
                    }
                
                    UpdateIdle(false);
                }
            }
        }

        protected override void Rotate(Vector3 target)
        {
            base.Rotate(target);
            
            _idleEffect.Transformation.Rotation = Vector3.Zero;
        }

        private bool TryFindNewResourceObject()
        {
            if (!_targetResourceObjectPosition.HasValue || !_targetResourceObjectType.HasValue)
                return false;
            
            var resourceObject = GameController.Instance.Map.GetClosestResource(this, _targetResourceObjectPosition.Value, _targetResourceObjectType.Value);
            if (resourceObject != null)
            {
                _targetResource = resourceObject;
                return true;
            }

            return false;
        }

        private void Gather(IResource resource)
        {
            if (IsFull || resource == null || resource.IsDestroyed || !resource.CanBeGathered(this) || resource.ResourceAmount <= 0)
            {
                GoToStorage();
                return;
            }
            
            _gatherTimer += GetGatherSpeed(resource.ResourceType) * Time.DeltaTime;
            if (_gatherTimer >= resource.GatherDuration)
            {
                _gatherTimer = 0;
                if (resource.TryGather(this))
                {
                    AddResource(1, resource.ResourceType);
                    
                    if (GameController.Instance.Player.Population <= 5 || Random.NextFloat(0, 1) < 0.33f)
                        _audioSource.Play(_hitAudio);
                }
            }
            
            Rotate(resource.Position);
        }

        private float GetGatherSpeed(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Wood:
                    return GATHER_SPEED * _playerEffects.WorkerWoodGatherSpeedBonus;
                case ResourceType.Food:
                    return GATHER_SPEED * _playerEffects.WorkerFoodGatherSpeedBonus;
                case ResourceType.Gold:
                    return GATHER_SPEED;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private void GoToStorage()
        {
            UpdateIdle(true);
            
            Log.LogInfo($"Worker go to storage: {ResourceAmount} {ResourceType}");
            var closestStorage = GameController.Instance.Map.GetClosestStorage(_onTile, ResourceType);
            if (closestStorage != null)
            {
                var closestStorageTile = GameController.Instance.Map.GetClosestPassableTile(_onTile, closestStorage, false);
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
            
            GameController.Instance.ResourceGainAnimator.Animate(Transformation.Position, ResourceType);
            
            Log.LogInfo($"Drop resource {ResourceType}: {GameController.Instance.Player.Resources.GetAmount(ResourceType)} +{ResourceAmount}");
            
            GameController.Instance.Player.Resources.AddAmount(ResourceType, ResourceAmount);
            ResourceAmount = 0;
        }

        private void UpdateIdle(bool isIdle)
        {
            if (_isIdleEffectActive == isIdle)
                return;

            _isIdleEffectActive = isIdle;
            _idleEffect.IsActiveSelf = _isIdleEffectActive;
        }
    }
}