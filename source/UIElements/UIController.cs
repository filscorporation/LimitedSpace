using Steel;

namespace SteelCustom.UIElements
{
    public class UIController : ScriptComponent
    {
        public float K { get; private set; }
        public static Color DarkColor => new Color(53, 43, 64);
        public static Color RedColor => new Color(147, 63, 69);
        
        public UIMenu Menu { get; private set; }
        public UIGameHUDController UIGameHUDController { get; private set; }
        public Entity UIRoot { get; private set; }

        public override void OnUpdate()
        {
            if (GameController.Instance.GameState == GameState.Game
             || GameController.Instance.GameState == GameState.Win)
                UpdateInGameUI();
        }

        public void CreateUI()
        {
            K = Screen.Width / 320f;
            
            UIRoot = UI.CreateUIElement();
            UIRoot.GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            UIRoot.GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            Menu = UI.CreateUIElement("Menu").AddComponent<UIMenu>();
            Menu.Init();
            
            UIGameHUDController = UI.CreateUIElement("HUD", UIRoot).AddComponent<UIGameHUDController>();
            UIGameHUDController.Init();
        }

        private void UpdateInGameUI()
        {
            
        }
    }
}