using System;
using System.Collections;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.MapSystem;
using SteelCustom.PlayerSystem;
using SteelCustom.UIElements;
using SteelCustom.Units;

namespace SteelCustom
{
    public class GameController : ScriptComponent
    {
        public static GameController Instance;
        
        public const float DEFAULT_VOLUME = 0.25f;
        public static bool SoundOn { get; set; } = true;
        
        public Player Player { get; private set; }
        public Map Map { get; private set; }
        public CameraController CameraController { get; private set; }
        public UIController UIController { get; private set; }
        public BuilderController BuilderController { get; private set; }
        public UnitsController UnitsController { get; private set; }

        public GameState GameState { get; private set; }
        
        private bool _changeState;
        private bool _startGame;
        private bool _winGame;
        
        public override void OnCreate()
        {
            Instance = this;

            Screen.Color = new Color(204, 146, 94);
            Screen.Width = 1600;
            Screen.Height = 900;
            Camera.Main.ResizingMode = CameraResizingMode.KeepWidth;
            Camera.Main.Width = 64;

            StartCoroutine(IntroCoroutine());
        }

        public override void OnUpdate()
        {
            if (_changeState)
            {
                _changeState = false;
                
                switch (GameState)
                {
                    case GameState.Intro:
                        StartCoroutine(IntroCoroutine());
                        break;
                    case GameState.Tutorial:
                        StartCoroutine(TutorialCoroutine());
                        break;
                    case GameState.Game:
                        if (_winGame)
                            StartCoroutine(WinGameCoroutine());
                        break;
                    case GameState.Win:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (GameState == GameState.Game)
            {
                
            }
        }

        public void StartGame()
        {
            _startGame = true;
        }

        public void RestartGame()
        {
            SceneManager.SetActiveScene(SceneManager.GetActiveScene());
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void WinGame()
        {
            _winGame = true;
            _changeState = true;
        }

        private IEnumerator IntroCoroutine()
        {
            GameState = GameState.Intro;
            Log.LogInfo("Start Intro state");

            Player = new Entity("Player").AddComponent<Player>();
            Map = new Entity("Map").AddComponent<Map>();
            CameraController = Camera.Main.Entity.AddComponent<CameraController>();
            BuilderController = new Entity("BuilderController").AddComponent<BuilderController>();
            UnitsController = new Entity("UnitsController").AddComponent<UnitsController>();
            
            Player.Init();
            Map.Init();
            CameraController.Init();
            BuilderController.Init();
            UnitsController.Init();
            
            Player.InitBuildingsAndResources();

            //yield return new WaitWhile(() => !_startGame);

            //yield return new WaitForSeconds(1.0f);

            GameState = GameState.Game;
            _changeState = true;
            
            yield break;
        }

        private IEnumerator TutorialCoroutine()
        {
            GameState = GameState.Tutorial;
            Log.LogInfo("Start Tutorial state");

            yield return new WaitForSeconds(1.0f);

            Log.LogInfo("End Tutorial state");
            _changeState = true;
        }

        private IEnumerator StartGameCoroutine()
        {
            GameState = GameState.Game;
            Log.LogInfo("Start Game state");

            yield return new WaitForSeconds(1.0f);
            
            Log.LogInfo("End Game state");
        }

        private IEnumerator WinGameCoroutine()
        {
            GameState = GameState.Win;
            Log.LogInfo("Start Win state");

            //DialogController.ShowWinDialog();
            
            yield return new WaitForSeconds(0.5f);
        }
    }
}