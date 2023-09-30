using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Units
{
    public abstract class Unit : SelectableObject
    {
        public Tile OnTile => _onTile;
        
        protected abstract string SpritePath { get; }
        protected abstract float Speed { get; }

        private List<Tile> _currentPath = new List<Tile>();
        private int _currentPathTileIndex;
        private Tile _reservedTile;
        protected Tile _onTile;
        protected ResourceObject _targetResourceObject;
        protected ResourceType? _targetResourceObjectType;
        protected Vector2? _targetResourceObjectPosition;
        protected Building _targetBuilding;

        private List<Entity> _drawnPath = new List<Entity>();
        private Entity _selection;

        public override void OnUpdate()
        {
            if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft) && GameController.Instance.UnitsController.HoveredUnit == this)
                GameController.Instance.Player.Select(this);
            
            UpdateMovement();
            UpdateUnit();
        }

        public virtual void Init(Tile tile)
        {
            Sprite sprite = ResourcesManager.GetImage(SpritePath);
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;
            Entity.AddComponent<BoxCollider>().Size = Vector2.One;
            Entity.AddComponent<RigidBody>().RigidBodyType = RigidBodyType.Static;
            
            _selection = new Entity("Selection", Entity);
            _selection.Transformation.LocalPosition = new Vector3(0, 0, 0.6f);
            _selection.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage("unit_selection.png");
            _selection.IsActiveSelf = false;

            _reservedTile = tile;
            _onTile = tile;
            tile.SetOnReservingUnit(this);
        }
        
        public virtual void Dispose()
        {
            GameController.Instance.UnitsController.RemoveUnit(this);
            ClearDrawnPath();
        }

        public override void OnMouseEnter()
        {
            GameController.Instance.UnitsController.HoveredUnit = this;
        }

        public override void OnMouseExit()
        {
            GameController.Instance.UnitsController.HoveredUnit = null;
        }

        public void MoveToTarget(Tile tile, ResourceObject resourceObject, Building targetBuilding)
        {
            if (MoveTo(tile))
            {
                _targetResourceObject = resourceObject;
                _targetResourceObjectType = resourceObject?.ResourceType;
                _targetResourceObjectPosition = resourceObject?.Entity.Transformation.Position;
                _targetBuilding = targetBuilding;
            }
        }

        protected bool MoveTo(Tile tile)
        {
            _reservedTile.ClearOnReservingUnit();
            List<Tile> newPath = GameController.Instance.Map.GetPath(_onTile, tile);
            _reservedTile.SetOnReservingUnit(this);
            
            if (!newPath.Any())
                return false;

            _currentPathTileIndex = _currentPath.Any() && newPath.Count > 1 ? 1 : 0;
            _currentPath = newPath;

            _reservedTile.ClearOnReservingUnit();
            _reservedTile = newPath.Last();
            _reservedTile.SetOnReservingUnit(this);

            DrawPath();

            Log.LogInfo($"Unit move path len {_currentPath.Count}");

            OnMovementStarted();

            return true;
        }

        protected override void OnSelectedChanged()
        {
            _selection.IsActiveSelf = IsSelected;
        }

        protected virtual void UpdateUnit() { }
        protected virtual void OnMovementStarted() { }
        protected virtual void OnMovementCompleted() { }

        private void UpdateMovement()
        {
            if (!_currentPath.Any())
                return;

            if (_currentPath[_currentPathTileIndex].IsOccupiedAndBlocked)
            {
                if (!MoveTo(_currentPath.Last()))
                {
                    MoveTo(_onTile);
                }
                
                return;
            }

            var map = GameController.Instance.Map;
            var targetPosition = (Vector3)map.CoordsToPosition(_currentPath[_currentPathTileIndex].X, _currentPath[_currentPathTileIndex].Y) + new Vector3(0, 0, 0.3f);
            Transformation.Position = MoveTowards(Transformation.Position, targetPosition, Speed * Time.DeltaTime);
            Rotate(targetPosition);

            if (Vector3.Distance(Transformation.Position, targetPosition) < 0.05f)
            {
                Transformation.Position = targetPosition;
                _onTile = _currentPath[_currentPathTileIndex];
                _currentPathTileIndex++;
                UpdateDrawnPath();

                if (_currentPathTileIndex >= _currentPath.Count)
                {
                    _currentPath.Clear();
                    _currentPathTileIndex = 0;
                    ClearDrawnPath();
                    
                    OnMovementCompleted();
                }
            }
        }
        
        protected void Rotate(Vector3 target)
        {
            Vector2 vector = target - Transformation.Position;
            float angle = -Math.Atan2(vector.X, vector.Y);
            angle = Math.Round(angle / (Math.Pi / 4)) * (Math.Pi / 4); // rounding to 45 degrees

            Transformation.Rotation = new Vector3(0.0f, 0.0f, angle);
        }
        
        private static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 a = target - current;
            float magnitude = a.Magnitude();
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                return target;
            }
            return current + a / magnitude * maxDistanceDelta;
        }

        private void DrawPath()
        {
            ClearDrawnPath();
            
            if (!_currentPath.Any())
                return;

            var map = GameController.Instance.Map;
            foreach (Tile tile in _currentPath)
            {
                Entity entity = new Entity("Path", map.Entity);
                entity.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage("path_point.png");
                entity.Transformation.Position = (Vector3)map.CoordsToPosition(tile.X, tile.Y) + new Vector3(0, 0, 0.15f);
                
                _drawnPath.Add(entity);
            }
        }

        private void UpdateDrawnPath()
        {
            for (int i = 0; i < _currentPathTileIndex; i++)
            {
                _drawnPath[i].IsActiveSelf = false;
            }
        }

        private void ClearDrawnPath()
        {
            foreach (Entity entity in _drawnPath)
                entity.Destroy();
            _drawnPath.Clear();
        }
    }
}