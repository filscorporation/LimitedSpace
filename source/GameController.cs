using System;
using System.Collections;
using Steel;

namespace SteelCustom
{
    public class GameController : ScriptComponent
    {
        public static GameController Instance;
        
        public const float DEFAULT_VOLUME = 0.25f;
        public static bool SoundOn { get; set; } = true;

        public GameState GameState { get; private set; }
        private bool _changeState = false;
        private bool _startGame = false;
        private bool _winGame = false;
        private bool _loseGame = false;
        
        public override void OnCreate()
        {
            Instance = this;
            
            Screen.Color = new Color(243, 223, 193);
            Screen.Width = 1600;
            Screen.Height = 900;
            Camera.Main.ResizingMode = CameraResizingMode.KeepWidth;
            Camera.Main.Width = 10;

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
                        if (_loseGame)
                            StartCoroutine(LoseGameCoroutine());
                        else if (_winGame)
                            StartCoroutine(WinGameCoroutine());
                        break;
                    case GameState.Win:
                        break;
                    case GameState.Lose:
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

        public void LoseGame()
        {
            if (_winGame)
            {
                ExitGame();
                return;
            }
            
            _loseGame = true;
            _changeState = true;
        }

        private IEnumerator IntroCoroutine()
        {
            GameState = GameState.Intro;
            Log.LogInfo("Start Intro state");

            yield return new WaitWhile(() => !_startGame);

            yield return new WaitForSeconds(1.0f);
            
            _changeState = true;
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

        private IEnumerator LoseGameCoroutine()
        {
            GameState = GameState.Lose;
            Log.LogInfo("Start Lose state");

            yield return new WaitForSeconds(0.5f);
            
            //UIController.Menu.OpenOnLoseScreen();
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