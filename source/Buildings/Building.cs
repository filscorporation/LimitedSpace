using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.UIElements;
using SteelCustom.Units;

namespace SteelCustom.Buildings
{
    public abstract class Building : MapObject
    {
        protected abstract string SpritePath { get; }
        public abstract BuildingType Type { get; }
        public abstract float BuildingDuration { get; }

        public bool IsBuilt { get; private set; }
        public bool IsQueueBlocked { get; private set; }
        
        private Entity _selectionEffect;
        private bool _isDraft;
        private float _buildingTimer;

        private readonly Queue<GameAction> _queue = new Queue<GameAction>();
        private float _queueTimer;
        private Entity _constructionEffect;

        public override void OnUpdate()
        {
            if (!IsBuilt && !_isDraft)
                UpdateConstruction();
            
            if (IsBuilt)
                OnBuildingUpdated();
        }

        public override void OnMouseEnter()
        {
            if (UI.IsPointerOverUI() || !IsBuilt)
                return;
            
            GameController.Instance.SelectionController.Hovered = this;
            
            //OverrideColor(new Color(240, 233, 201));
        }

        public override void OnMouseExit()
        {
            if (GameController.Instance.SelectionController.Hovered == this)
                GameController.Instance.SelectionController.Hovered = null;
            
            ResetOverride();
        }

        public virtual void Init()
        {
            Sprite sprite = ResourcesManager.GetImage(SpritePath);
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;
            Entity.AddComponent<BoxCollider>().Size = new Vector2(Size.X, Size.Y);
            Entity.AddComponent<RigidBody>().RigidBodyType = RigidBodyType.Static;

            _selectionEffect = new Entity("Selection", Entity);
            _selectionEffect.Transformation.LocalPosition = new Vector3(0, 0, 1);
            _selectionEffect.AddComponent<SpriteRenderer>().Sprite = Size.X > 6 ? ResourcesManager.GetImage("building_big_selection.png") : ResourcesManager.GetImage("building_selection.png");
            _selectionEffect.IsActiveSelf = false;

            ReplaceShader();
        }
        
        public virtual void Dispose()
        {
            foreach (var tile in _onTiles)
            {
                tile.ClearOnObject();
            }
        }

        public void Place(Tile bottomLeftTile, List<Tile> allTiles, bool instant = false)
        {
            OnBottomLeftTile = bottomLeftTile;
            _onTiles = allTiles;
            
            foreach (var tile in _onTiles)
            {
                tile.SetOnObject(this);
            }
            
            OnPlaced();

            if (!instant)
            {
                _buildingTimer = BuildingDuration;
                OverrideAlpha(0.8f);

                _constructionEffect = ResourcesManager.GetAsepriteData("construction_effect.aseprite", true).CreateEntityFromAsepriteData();
                _constructionEffect.Transformation.Position = Transformation.Position + new Vector3(0, 0, 1);
                _constructionEffect.GetComponent<Animator>().Play("Effect");
            }
            else
            {
                IsBuilt = true;
                OnConstructed();
            }
        }

        public void Destroy()
        {
            Entity.Destroy();
            Dispose();
        }

        public void AddGameActionToQueue(GameAction gameAction)
        {
            if (!GameController.Instance.Player.Resources.HasAmount(gameAction.Cost))
                return;

            GameController.Instance.Player.Resources.SpendAmount(gameAction.Cost);
            gameAction.ReserveAction();
            
            _queue.Enqueue(gameAction);

            CallOnChanged();
        }

        public T SpawnUnit<T>() where T : Unit, new()
        {
            var tile = GameController.Instance.Map.GetPassableTilesAround(this).FirstOrDefault();
            if (tile == null)
                return null;

            return GameController.Instance.UnitsController.SpawnUnit<T>(tile);
        }

        public virtual bool IsStorage(ResourceType resourceType)
        {
            return false;
        }

        public void SetIsDraft(bool isDraft)
        {
            _isDraft = isDraft;
            
            if (_isDraft)
                OverrideAlpha(0.4f);
            else
                ResetOverride();
        }

        public void SetCanPlace(bool canPlace)
        {
            if (!canPlace)
                OverrideColor(UIController.RedColor);
            else
                ResetOverrideColor();
        }

        public float GetZ()
        {
            return (IsBlocking ? 0.5f : 0.1f) + (_isDraft ? 0.3f : 0.0f);
        }

        protected virtual void OnPlaced() { }

        protected virtual void OnConstructed()
        {
            Log.LogInfo($"{Type} is constructed");

            if (_constructionEffect != null)
            {
                _constructionEffect.Destroy();
                _constructionEffect = null;
            }
            
            ResetOverride();
        }

        protected virtual void OnBuildingUpdated()
        {
            UpdateQueue();
        }

        private void UpdateQueue()
        {
            if (!_queue.Any())
                return;

            if (!_queue.Peek().CanDequeue())
            {
                if (!IsQueueBlocked)
                {
                    IsQueueBlocked = true;
                    
                    CallOnChanged();
                }
                return;
            }

            IsQueueBlocked = false;
            
            _queueTimer += Time.DeltaTime;
            if (_queueTimer >= _queue.Peek().Duration)
            {
                GameAction gameAction = _queue.Dequeue();

                CompleteGameAction(gameAction);

                CallOnChanged();
            }
        }

        private void CompleteGameAction(GameAction gameAction)
        {
            gameAction.DoAction(this);
        }

        protected override void OnSelectedChanged()
        {
            base.OnSelectedChanged();
            
            _selectionEffect.IsActiveSelf = IsSelected;
        }

        private void UpdateConstruction()
        {
            _buildingTimer -= Time.DeltaTime * GetConstructionSpeed();
            if (_buildingTimer <= 0)
            {
                IsBuilt = true;
                OnConstructed();
            }
        }

        private float GetConstructionSpeed()
        {
            return 1 * GameController.Instance.Player.Effects.ConstructionSpeedBonus;
        }
    }
}