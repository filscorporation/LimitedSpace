using System.Collections.Generic;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.GameActions;

namespace SteelCustom.UIElements
{
    public class UIBuilderController : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;

        private readonly List<BuildingType> _buildingTypes = new List<BuildingType> { BuildingType.House, BuildingType.Farm, BuildingType.TownCenter, BuildingType.LumberMill, BuildingType.Mill, BuildingType.Market, BuildingType.Wonder };
        private SelectionController _selectionController;
        private readonly List<UIButton> _buildingsButtons = new List<UIButton>();

        public void Init()
        {
            GameController.Instance.Player.OnChanged += OnPlayerChanged;
            GameController.Instance.Player.Resources.OnChanged += OnPlayerResourcesChanged;
            
            _selectionController = GameController.Instance.SelectionController;
            _selectionController.OnSelectedChanged += OnSelectedChanged;
            
            RectTransformation rt = Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = new Vector2(0, 0);
            rt.AnchorMax = new Vector2(0, 0);
            rt.Size = new Vector2(114 * K, 31 * K);
            rt.Pivot = new Vector2(0, 0);

            UIText nameText = UI.CreateUIText("Build", "Name", Entity);
            nameText.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            nameText.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            nameText.RectTransform.Pivot = new Vector2(0, 0);
            nameText.RectTransform.Size = new Vector2(85 * K, 20 * K);
            nameText.RectTransform.AnchoredPosition = new Vector2(2 * K, 16 * K);

            nameText.TextAlignment = AlignmentType.BottomLeft;
            nameText.TextSize = 80;
            nameText.Color = UIController.DarkColor;

            int x = 2;
            foreach (BuildingType buildingType in _buildingTypes)
            {
                _buildingsButtons.Add(CreateButton(x, buildingType));
                x += (14 + 2);
            }

            UpdatePanel();
        }

        private void ShowPanel()
        {
            Entity.IsActiveSelf = true;
        }

        private void HidePanel()
        {
            Entity.IsActiveSelf = false;
        }

        private void UpdatePanel()
        {
            for (int i = 0; i < _buildingTypes.Count; i++)
            {
                _buildingsButtons[i].Interactable = GameController.Instance.BuilderController.CanStartPlacing(_buildingTypes[i]) == NotAvailableReason.None;
            }
        }

        private UIButton CreateButton(int x, BuildingType buildingType)
        {
            UIButton button = UI.CreateUIButton(ResourcesManager.GetImage("ui_game_action_button.png"), "Build", Entity);
            button.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            button.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            button.RectTransform.Pivot = new Vector2(0, 0);
            button.RectTransform.Size = new Vector2(14 * K, 14 * K);
            button.RectTransform.AnchoredPosition = new Vector2(x * K, 2 * K);

            button.OnClick.AddCallback(() => OnBuildingClicked(buildingType));
            
            UIImage icon = UI.CreateUIImage(ResourcesManager.GetImage(BuildingsInfo.GetIcon(buildingType)), "Icon", button.Entity);
            icon.RectTransform.AnchorMin = Vector2.Zero;
            icon.RectTransform.AnchorMax = Vector2.One;
            icon.RectTransform.OffsetMin = new Vector2(1 * K, 1 * K);
            icon.RectTransform.OffsetMax = new Vector2(1 * K, 1 * K);
            icon.ConsumeEvents = false;
            
            button.Entity.AddComponent<UIInfoProvider>().Init(GameController.Instance.UIController.UIGameHUDController.UIInfo, buildingType);

            return button;
        }

        private void OnBuildingClicked(BuildingType buildingType)
        {
            GameController.Instance.BuilderController.TryStartPlacing(buildingType);
        }

        private void OnSelectedChanged()
        {
            var selected = _selectionController.SelectedObjects;
            if (selected.Count != 1)
            {
                ShowPanel();
                return;
            }

            HidePanel();
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