using Steel;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.UIElements
{
    public class UIResourcesController : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        private PlayerResources _resources;
        private UIText _woodText;
        private UIText _foodText;
        private UIText _goldText;
        
        public override void OnUpdate()
        {
            UpdateResourcesAmount();
        }

        public void Init()
        {
            _resources = GameController.Instance.Player.Resources;
            
            RectTransformation rt = Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = new Vector2(0, 1);
            rt.AnchorMax = new Vector2(0, 1);
            rt.Size = new Vector2(119 * K, 16 * K);
            rt.Pivot = new Vector2(0, 1);

            _woodText = CreateResource(3, "wood_big_icon.png");
            _foodText = CreateResource(42, "food_big_icon.png");
            _goldText = CreateResource(81, "gold_big_icon.png");
        }

        private UIText CreateResource(int x, string icon)
        {
            Sprite sprite = ResourcesManager.GetImage(icon);
            UIImage image = UI.CreateUIImage(sprite, "Icon", Entity);
            image.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            image.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            image.RectTransform.Pivot = new Vector2(0, 0);
            image.RectTransform.Size = new Vector2(10 * K, 10 * K);
            image.RectTransform.AnchoredPosition = new Vector2(x * K, 3 * K);
            
            UIText text = UI.CreateUIText("0", "Text", Entity);
            text.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            text.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            text.RectTransform.Pivot = new Vector2(0, 0);
            text.RectTransform.Size = new Vector2(28 * K, 20 * K);
            text.RectTransform.AnchoredPosition = new Vector2((x + 11) * K, 0 * K);

            text.TextAlignment = AlignmentType.BottomLeft;
            text.TextSize = 80;
            text.Color = UIController.DarkColor;

            return text;
        }

        private void UpdateResourcesAmount()
        {
            _woodText.Text = ResourceToString(_resources.Wood);
            _foodText.Text = ResourceToString(_resources.Food);
            _goldText.Text = ResourceToString(_resources.Gold);
        }

        private string ResourceToString(int amount)
        {
            return amount >= 2000 ? $"{amount / 1000}k" : $"{amount}";
        }
    }
}