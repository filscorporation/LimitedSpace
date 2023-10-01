using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.GameActions;

namespace SteelCustom.UIElements
{
    public class UISelectedPanel : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        private SelectionController _selectionController;
        private UIText _nameText;
        private SelectableObject _selectedObject;
        private readonly List<(UIButton Button, GameAction GameAction)> _gameActionsButtons = new List<(UIButton Button, GameAction GameAction)>();
        private UIText _queueTimerText;
        private UIImage _queueItemIcon;

        public override void OnUpdate()
        {
            
        }

        public void Init()
        {
            GameController.Instance.Player.OnChanged += OnPlayerChanged;
            GameController.Instance.Player.Resources.OnChanged += OnPlayerResourcesChanged;
            
            _selectionController = GameController.Instance.SelectionController;
            _selectionController.OnSelectedChanged += OnSelectedChanged;
            
            RectTransformation rt = Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = new Vector2(0, 0);
            rt.AnchorMax = new Vector2(0, 0);
            rt.Size = new Vector2(166 * K, 31 * K);
            rt.Pivot = new Vector2(0, 0);

            _nameText = UI.CreateUIText("Selected", "Name", Entity);
            _nameText.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            _nameText.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            _nameText.RectTransform.Pivot = new Vector2(0, 0);
            _nameText.RectTransform.Size = new Vector2(85 * K, 20 * K);
            _nameText.RectTransform.AnchoredPosition = new Vector2(2 * K, 16 * K);

            _nameText.TextAlignment = AlignmentType.BottomLeft;
            _nameText.TextSize = 80;
            _nameText.Color = UIController.DarkColor;

            SetPanel(null);
        }

        private void SetPanel(SelectableObject selectedObject)
        {
            if (_selectedObject != null)
                _selectedObject.OnChanged -= OnSelectedObjectChanged;
            
            _selectedObject = selectedObject;

            foreach ((UIButton Button, GameAction GameAction) pair in _gameActionsButtons)
            {
                pair.Button.Entity.Destroy();
            }
            _gameActionsButtons.Clear();
            
            if (selectedObject == null)
            {
                Entity.IsActiveSelf = false;
                return;
            }

            Entity.IsActiveSelf = true;

            _nameText.Text = selectedObject.Name;
            int x = 2;
            foreach (GameAction gameAction in selectedObject.GameActions)
            {
                if (!gameAction.IsVisible())
                    continue;
                
                _gameActionsButtons.Add((CreateAction(x, gameAction), gameAction));
                x += (14 + 2);
            }

            UpdatePanel();
        }

        private void UpdatePanel()
        {
            if (_selectedObject == null)
                return;

            foreach ((UIButton Button, GameAction GameAction) pair in _gameActionsButtons)
            {
                pair.Button.Entity.IsActiveSelf = pair.GameAction.IsVisible();
                pair.Button.Interactable = pair.GameAction.IsAvailable() == NotAvailableReason.None;
            }
        }

        private void RecreatePanel()
        {
            SetPanel(_selectedObject);
        }

        private UIButton CreateAction(int x, GameAction gameAction)
        {
            UIButton button = UI.CreateUIButton(ResourcesManager.GetImage("ui_game_action_button.png"), "Action", Entity);
            button.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            button.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            button.RectTransform.Pivot = new Vector2(0, 0);
            button.RectTransform.Size = new Vector2(14 * K, 14 * K);
            button.RectTransform.AnchoredPosition = new Vector2(x * K, 2 * K);

            button.OnClick.AddCallback(() => OnActionClicked(gameAction));
            
            UIImage icon = UI.CreateUIImage(ResourcesManager.GetImage(gameAction.Icon), "Icon", button.Entity);
            icon.RectTransform.AnchorMin = Vector2.Zero;
            icon.RectTransform.AnchorMax = Vector2.One;
            icon.RectTransform.OffsetMin = new Vector2(1 * K, 1 * K);
            icon.RectTransform.OffsetMax = new Vector2(1 * K, 1 * K);
            icon.ConsumeEvents = false;

            return button;
        }

        private void OnActionClicked(GameAction gameAction)
        {
            if (!gameAction.IsVisible())
            {
                UpdatePanel();
                return;
            }
            
            NotAvailableReason reason = gameAction.IsAvailable();
            if (reason != NotAvailableReason.None)
            {
                Log.LogInfo($"Can't {gameAction.GetType()}, reason {reason}");
                return;
            }

            if (_selectedObject is Building building)
            {
                building.AddGameActionToQueue(gameAction);
                RecreatePanel();
            }
            else
            {
                // ??
            }
        }

        private void OnSelectedChanged()
        {
            var selected = _selectionController.SelectedObjects;
            if (selected.Count != 1)
            {
                SetPanel(null);
                return;
            }

            SetPanel(selected.First());
        }

        private void OnSelectedObjectChanged()
        {
            UpdatePanel();
        }

        private void OnPlayerChanged()
        {
            UpdatePanel();
        }

        private void OnPlayerResourcesChanged()
        {
            UpdatePanel();
        }
    }
}