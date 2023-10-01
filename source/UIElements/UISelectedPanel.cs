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
        private UIButton _deleteButton;
        private SelectableObject _selectedObject;
        private readonly List<(UIButton Button, GameAction GameAction)> _gameActionsButtons = new List<(UIButton Button, GameAction GameAction)>();

        private bool _isDrawingQueue = true;
        private Entity _queueRoot;
        private UIText _queueTimerText;
        private readonly List<UIImage> _queueItemIcons = new List<UIImage>();

        public override void OnUpdate()
        {
            UpdateQueue();
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

            _queueTimerText = UI.CreateUIText("0", "Timer", Entity);
            _queueTimerText.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            _queueTimerText.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            _queueTimerText.RectTransform.Pivot = new Vector2(0, 0);
            _queueTimerText.RectTransform.Size = new Vector2(85 * K, 20 * K);
            _queueTimerText.RectTransform.AnchoredPosition = new Vector2(145 * K, 0 * K);

            _queueTimerText.TextAlignment = AlignmentType.BottomLeft;
            _queueTimerText.TextSize = 80;
            _queueTimerText.Color = UIController.DarkColor;

            int x = 93;
            for (int i = 0; i < 3; i++)
            {
                _queueItemIcons.Add(CreateQueueItem(x));
                x += (14 + 2);
            }
            _queueItemIcons.Reverse();

            CreateDeleteButton();

            SetPanel(null);
        }

        private void SetPanel(SelectableObject selectedObject)
        {
            if (_selectedObject != null)
                _selectedObject.OnChanged -= OnSelectedObjectChanged;
            
            _selectedObject = selectedObject;
            
            if (_selectedObject != null)
                _selectedObject.OnChanged += OnSelectedObjectChanged;

            foreach ((UIButton Button, GameAction GameAction) pair in _gameActionsButtons)
            {
                pair.Button.Entity.Destroy();
            }
            _gameActionsButtons.Clear();
            
            if (_selectedObject == null)
            {
                Entity.IsActiveSelf = false;
                return;
            }

            Entity.IsActiveSelf = true;

            _deleteButton.Entity.IsActiveSelf = _selectedObject is Building;
            _nameText.Text = _selectedObject.Name;
            int x = 2;
            foreach (GameAction gameAction in _selectedObject.GameActions)
            {
                if (!gameAction.IsVisible())
                    continue;
                
                _gameActionsButtons.Add((CreateAction(x, gameAction), gameAction));
                x += (14 + 2);
            }

            UpdatePanel();
            UpdateQueueItems();
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

        private void UpdateQueue()
        {
            if (_selectedObject is Building building && building.HasQueue)
            {
                if (!_isDrawingQueue)
                    DrawQueue();
                
                _queueTimerText.Text = $"{(int)building.QueueTimeLeft}";
                _queueTimerText.Color = building.IsQueueBlocked ? UIController.RedColor : UIController.DarkColor;
            }
            else
            {
                if (_isDrawingQueue)
                    HideQueue();
            }
        }

        private void UpdateQueueItems()
        {
            if (_selectedObject is Building building && building.HasQueue)
            {
                var queue = building.TopQueue;
                int i = 0;
                for (; i < queue.Count; i++)
                {
                    _queueItemIcons[i].Sprite = ResourcesManager.GetImage(queue[i].Icon);
                }
                for (; i < _queueItemIcons.Count; i++)
                {
                    _queueItemIcons[i].Sprite = null;
                }
            }
        }

        private void DrawQueue()
        {
            _queueTimerText.Entity.IsActiveSelf = true;
            foreach (UIImage image in _queueItemIcons)
                image.Entity.Parent.IsActiveSelf = true;
            
            _isDrawingQueue = true;
        }

        private void HideQueue()
        {
            _queueTimerText.Entity.IsActiveSelf = false;
            foreach (UIImage image in _queueItemIcons)
                image.Entity.Parent.IsActiveSelf = false;
            
            _isDrawingQueue = false;
        }

        private UIImage CreateQueueItem(int x)
        {
            UIImage button = UI.CreateUIImage(ResourcesManager.GetImage("ui_game_action_button.png"), "Queue item", Entity);
            button.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            button.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            button.RectTransform.Pivot = new Vector2(0, 0);
            button.RectTransform.Size = new Vector2(14 * K, 14 * K);
            button.RectTransform.AnchoredPosition = new Vector2(x * K, 2 * K);
            
            UIImage icon = UI.CreateUIImage(null, "Icon", button.Entity);
            icon.RectTransform.AnchorMin = Vector2.Zero;
            icon.RectTransform.AnchorMax = Vector2.One;
            icon.RectTransform.OffsetMin = new Vector2(1 * K, 1 * K);
            icon.RectTransform.OffsetMax = new Vector2(1 * K, 1 * K);
            icon.ConsumeEvents = false;

            return icon;
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
            
            button.Entity.AddComponent<UIInfoProvider>().Init(GameController.Instance.UIController.UIGameHUDController.UIInfo, gameAction);

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
                GameController.Instance.UIController.UIGameHUDController.UIInfo.Hide();
            }
            else
            {
                // ??
            }
        }

        private void CreateDeleteButton()
        {
            _deleteButton = UI.CreateUIButton(ResourcesManager.GetImage("ui_delete_button.png"), "Delete", Entity);
            _deleteButton.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            _deleteButton.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            _deleteButton.RectTransform.Pivot = new Vector2(0, 0);
            _deleteButton.RectTransform.Size = new Vector2(10 * K, 10 * K);
            _deleteButton.RectTransform.AnchoredPosition = new Vector2(168 * K, 0 * K);

            _deleteButton.OnClick.AddCallback(OnDeleteButtonClicked);
        }

        private void OnDeleteButtonClicked()
        {
            if (_selectedObject is Building building)
            {
                building.Destroy();
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
            UpdateQueueItems();
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