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
            CreateTips();
            
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
            CreateWinInfo();
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
            
            Time.TimeScale = GameController.Instance.GameSpeed;
            
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

        private void CreateWinInfo()
        {
            UIImage image = UI.CreateUIImage(ResourcesManager.GetImage("ui_frame.png"), "Background", menu);
            image.RectTransform.AnchorMin = new Vector2(0.5f, 1);
            image.RectTransform.AnchorMax = new Vector2(0.5f, 1);
            image.RectTransform.Pivot = new Vector2(0.5f, 1);
            image.RectTransform.Size = new Vector2(160 * K, 40 * K);
            image.RectTransform.AnchoredPosition = new Vector2(0 * K, -2 * K);
            
            UIText header = UI.CreateUIText($"Game completed!", "Text", menu);
            header.Color = UIController.DarkColor;
            header.TextSize = 80;
            header.TextAlignment = AlignmentType.TopLeft;
            header.RectTransform.AnchorMin = new Vector2(0.5f, 1);
            header.RectTransform.AnchorMax = new Vector2(0.5f, 1);
            header.RectTransform.Pivot = new Vector2(0.5f, 1);
            header.RectTransform.Size = new Vector2(160 * K, (20 - 4) * K);
            header.RectTransform.AnchoredPosition = new Vector2(4 * K, -4 * K);
            
            UIText text = UI.CreateUIText($"You built the Wonder in just {(int)GameController.Instance.Player.GameTime} seconds!\n\nFaster then 2000 is a good result)", "Text", menu);
            text.Color = UIController.DarkColor;
            text.TextSize = 32;
            text.TextAlignment = AlignmentType.TopLeft;
            text.RectTransform.AnchorMin = new Vector2(0.5f, 1);
            text.RectTransform.AnchorMax = new Vector2(0.5f, 1);
            text.RectTransform.Pivot = new Vector2(0.5f, 1);
            text.RectTransform.Size = new Vector2(160 * K, (40 - 4) * K);
            text.RectTransform.AnchoredPosition = new Vector2(4 * K, (-4 - 20) * K);
        }

        private void CreateTips()
        {
            UIImage image = UI.CreateUIImage(ResourcesManager.GetImage("ui_frame.png"), "Background", menu);
            image.RectTransform.AnchorMin = new Vector2(1, 0.0f);
            image.RectTransform.AnchorMax = new Vector2(1, 0.0f);
            image.RectTransform.Pivot = new Vector2(1.0f, 0.0f);
            image.RectTransform.Size = new Vector2(160 * K, 54 * K);
            image.RectTransform.AnchoredPosition = new Vector2(-2 * K, 100 * K);
            
            UIText text = UI.CreateUIText("Tips:\nChop wood, to collect resources and free up space.\nBuild more workers, create farms, research technologies.\nAdvance to the next era and build the Wonder to win.\n\n" +
                                          "Left click to select worker or building, right click to give orders.\nUse WASD or mouse to move the camera, press F to recenter.", "Tips", menu);
            text.Color = UIController.DarkColor;
            text.TextSize = 32;
            text.TextAlignment = AlignmentType.TopLeft;
            text.TextOverflowMode = OverflowMode.WrapByWords;
            text.RectTransform.AnchorMin = new Vector2(1, 0.0f);
            text.RectTransform.AnchorMax = new Vector2(1, 0.0f);
            text.RectTransform.Pivot = new Vector2(1.0f, 0.0f);
            text.RectTransform.Size = new Vector2(160 * K, (54 - 4) * K);
            text.RectTransform.AnchoredPosition = new Vector2(0 * K, 102 * K);
        }
    }
}