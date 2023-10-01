using Steel;

namespace SteelCustom.UIElements
{
    public class UIGameHUDController : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        public UIResourcesController UIResourcesController { get; private set; }
        public UISelectedPanel UISelectedPanel { get; private set; }
        public UIPopulation UIPopulation { get; private set; }
        public UIGameTime UIGameTime { get; private set; }
        
        private Entity _root;
        
        public void Init()
        {
            GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            _root = UI.CreateUIElement("HUD", Entity);
            RectTransformation rt = _root.GetComponent<RectTransformation>();
            rt.AnchorMin = Vector2.Zero;
            rt.AnchorMax = Vector2.One;

            CreatePanels();
        }

        private void CreatePanels()
        {
            UIResourcesController = UI.CreateUIImage(ResourcesManager.GetImage("ui_resources.png"), "Resources", _root).Entity.AddComponent<UIResourcesController>();
            UIResourcesController.Init();
            
            UISelectedPanel = UI.CreateUIImage(ResourcesManager.GetImage("ui_selected_panel.png"), "Selected panel", _root).Entity.AddComponent<UISelectedPanel>();
            UISelectedPanel.Init();
            
            UIPopulation = UI.CreateUIImage(ResourcesManager.GetImage("ui_population.png"), "Population", _root).Entity.AddComponent<UIPopulation>();
            UIPopulation.Init();
            
            UIGameTime = UI.CreateUIImage(ResourcesManager.GetImage("ui_game_time.png"), "GameTime", _root).Entity.AddComponent<UIGameTime>();
            UIGameTime.Init();
        }
    }
}