using Steel;

namespace SteelCustom.UIElements
{
    public class UIMenu : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        private bool menuOpened;

        private Entity menu;
        private UIButton playButton;
        private UIButton continueButton;
        private UIButton soundButton;
        private bool winMenuOpened = false;

        public override void OnUpdate()
        {
            if (Input.IsKeyJustPressed(KeyCode.Escape))
            {
                if (menuOpened)
                    CloseMenu();
                else
                    OpenMenu();
            }
        }

        public void Init()
        {
            GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            menu = UI.CreateUIImage(ResourcesManager.GetImage("ui_dim.png"), "Menu", Entity).Entity;
            RectTransformation menuRT = menu.GetComponent<RectTransformation>();
            menuRT.AnchorMin = Vector2.Zero;
            menuRT.AnchorMax = Vector2.One;

            playButton = CreateMenuButton("Play", 24 * K);
            playButton.OnClick.AddCallback(Play);
            continueButton = CreateMenuButton("Continue", 24 * K);
            continueButton.OnClick.AddCallback(CloseMenu);
            continueButton.Entity.IsActiveSelf = false;
            CreateMenuButton("Exit", 2 * K).OnClick.AddCallback(Exit);
            soundButton = CreateSoundButton();
            CreateAbout();
            
            Time.TimeScale = 0.0f;
        }

        public void OpenOnWinScreen()
        {
            winMenuOpened = true;
            
            menu?.Destroy();
            
            menu = UI.CreateUIImage(ResourcesManager.GetImage("ui_dim.png"), "Menu", Entity).Entity;
            RectTransformation menuRT = menu.GetComponent<RectTransformation>();
            menuRT.AnchorMin = Vector2.Zero;
            menuRT.AnchorMax = Vector2.One;

            CreateMenuButton("Restart", 24 * K).OnClick.AddCallback(Restart);
            CreateMenuButton("Exit", 2 * K).OnClick.AddCallback(Exit);
            CreateAbout();
        }

        private void OpenMenu()
        {
            if (winMenuOpened)
                return;
            
            Time.TimeScale = 0.0f;
            
            menuOpened = true;
            menu.IsActiveSelf = true;
        }

        private void CloseMenu()
        {
            if (winMenuOpened)
                return;
            
            Time.TimeScale = 1.0f;
            
            menuOpened = false;
            menu.IsActiveSelf = false;
        }

        private void Play()
        {
            continueButton.Entity.IsActiveSelf = true;
            playButton.Entity.IsActiveSelf = true;

            CloseMenu();
            
            GameController.Instance.StartGame();
        }

        private void Restart()
        {
            GameController.Instance.RestartGame();
        }

        private void Exit()
        {
            GameController.Instance.ExitGame();
        }

        private void ChangeSound()
        {
            if (GameController.SoundOn)
            {
                GameController.SoundOn = false;
                Camera.Main.Entity.AddComponent<AudioListener>().Volume = 0.0f;
                soundButton.TargetImage.Sprite = ResourcesManager.GetImage("ui_sound_off.png");
            }
            else
            {
                GameController.SoundOn = true;
                Camera.Main.Entity.AddComponent<AudioListener>().Volume = GameController.DEFAULT_VOLUME;
                soundButton.TargetImage.Sprite = ResourcesManager.GetImage("ui_sound_on.png");
            }
        }

        private UIButton CreateMenuButton(string text, float y)
        {
            y += 80 * K;
            
            Sprite sprite = ResourcesManager.GetImage("ui_frame.png");
            UIButton button = UI.CreateUIButton(sprite, "Menu button", menu);
            button.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            button.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            button.RectTransform.Pivot = new Vector2(0, 0);
            button.RectTransform.Size = new Vector2(80 * K, 20 * K);
            button.RectTransform.AnchoredPosition = new Vector2((20 + 2) * K, y);

            UIText uiText = UI.CreateUIText(text, "Label", button.Entity);
            uiText.Color = Color.Black;
            uiText.TextSize = 64;
            uiText.TextAlignment = AlignmentType.CenterMiddle;
            uiText.RectTransform.AnchorMin = Vector2.Zero;
            uiText.RectTransform.AnchorMax = Vector2.One;

            return button;
        }

        private UIButton CreateSoundButton()
        {
            Sprite sprite = ResourcesManager.GetImage("ui_sound_on.png");
            UIButton button = UI.CreateUIButton(sprite, "Sound button", menu);
            button.RectTransform.AnchorMin = new Vector2(1.0f, 0.0f);
            button.RectTransform.AnchorMax = new Vector2(1.0f, 0.0f);
            button.RectTransform.Pivot = new Vector2(1, 0);
            button.RectTransform.Size = new Vector2(15 * K, 15 * K);
            button.RectTransform.AnchoredPosition = new Vector2(-2 * K, 2 * K);
            
            button.OnClick.AddCallback(ChangeSound);

            return button;
        }

        private void CreateAbout()
        {
            UIText text = UI.CreateUIText("Created in 48 hours for LD54 using Steel Engine", "About", menu);
            text.Color = UIController.DarkColor;
            text.TextSize = 32;
            text.RectTransform.AnchorMin = new Vector2(0.5f, 0.0f);
            text.RectTransform.AnchorMax = new Vector2(0.5f, 0.0f);
            text.RectTransform.Pivot = new Vector2(0.0f, 0.0f);
            text.RectTransform.Size = new Vector2(160 * K, 10 * K);
        }
    }
}