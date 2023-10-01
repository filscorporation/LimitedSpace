using Steel;

namespace SteelCustom.UIElements
{
    public class UIGameHUDController : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        public UIInfo UIInfo { get; private set; }
        public UIResourcesController UIResourcesController { get; private set; }
        public UISelectedPanel UISelectedPanel { get; private set; }
        public UIBuilderController UIBuilderController { get; private set; }
        public UIPopulation UIPopulation { get; private set; }
        public UIGameTime UIGameTime { get; private set; }
        
        private Entity _root;
        private UIImage _speedButtonImage;
        
        public void Init()
        {
            GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            _root = UI.CreateUIElement("HUD", Entity);
            RectTransformation rt = _root.GetComponent<RectTransformation>();
            rt.AnchorMin = Vector2.Zero;
            rt.AnchorMax = Vector2.One;

            CreatePanels();

            CreateSpeedButton();
        }

        private void CreatePanels()
        {
            UIInfo = UI.CreateUIImage(ResourcesManager.GetImage("ui_info.png"), "Info", _root).Entity.AddComponent<UIInfo>();
            UIInfo.Init();
            
            UIResourcesController = UI.CreateUIImage(ResourcesManager.GetImage("ui_resources.png"), "Resources", _root).Entity.AddComponent<UIResourcesController>();
            UIResourcesController.Init();
            
            UISelectedPanel = UI.CreateUIImage(ResourcesManager.GetImage("ui_selected_panel.png"), "Selected panel", _root).Entity.AddComponent<UISelectedPanel>();
            UISelectedPanel.Init();
            
            UIBuilderController = UI.CreateUIImage(ResourcesManager.GetImage("ui_builder_controller.png"), "Builder controller", _root).Entity.AddComponent<UIBuilderController>();
            UIBuilderController.Init();
            
            UIPopulation = UI.CreateUIImage(ResourcesManager.GetImage("ui_population.png"), "Population", _root).Entity.AddComponent<UIPopulation>();
            UIPopulation.Init();
            
            UIGameTime = UI.CreateUIImage(ResourcesManager.GetImage("ui_game_time.png"), "Game time", _root).Entity.AddComponent<UIGameTime>();
            UIGameTime.Init();
        }

        private void CreateSpeedButton()
        {
            UIButton speedButton = UI.CreateUIButton(ResourcesManager.GetImage("ui_speed_button.png"), "Speed", _root);
            speedButton.RectTransform.AnchorMin = new Vector2(1, 1);
            speedButton.RectTransform.AnchorMax = new Vector2(1, 1);
            speedButton.RectTransform.Pivot = new Vector2(1, 1);
            speedButton.RectTransform.Size = new Vector2(20 * K, 16 * K);
            speedButton.RectTransform.AnchoredPosition = new Vector2(0 * K, -18 * K);

            speedButton.OnClick.AddCallback(OnSpeedButtonClicked);

            _speedButtonImage = speedButton.TargetImage;
        }

        private void OnSpeedButtonClicked()
        {
            if (Math.Approximately(GameController.Instance.GameSpeed, 1))
            {
                GameController.Instance.GameSpeed = 4;
                _speedButtonImage.Sprite = ResourcesManager.GetImage("ui_speed_normal_button.png");
            }
            else
            {
                GameController.Instance.GameSpeed = 1;
                _speedButtonImage.Sprite = ResourcesManager.GetImage("ui_speed_button.png");
            }
        }
    }
}