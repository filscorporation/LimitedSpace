using Steel;
using SteelCustom.PlayerSystem;

namespace SteelCustom.UIElements
{
    public class UIGameTime : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        private Player _player;
        private UIText _text;

        public override void OnUpdate()
        {
            _text.Text = $"{(int)_player.GameTime}";
        }

        public void Init()
        {
            _player = GameController.Instance.Player;
            
            RectTransformation rt = Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = new Vector2(1, 1);
            rt.AnchorMax = new Vector2(1, 1);
            rt.Size = new Vector2(50 * K, 16 * K);
            rt.Pivot = new Vector2(1, 1);
            
            _text = UI.CreateUIText("0", "Text", Entity);
            _text.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            _text.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            _text.RectTransform.Pivot = new Vector2(0, 0);
            _text.RectTransform.Size = new Vector2(40 * K, 20 * K);
            _text.RectTransform.AnchoredPosition = new Vector2((3 + 11) * K, 0 * K);

            _text.TextAlignment = AlignmentType.BottomLeft;
            _text.TextSize = 80;
            _text.Color = UIController.DarkColor;
        }
    }
}