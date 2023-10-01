using Steel;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.UIElements
{
    public class UIInfo : ScriptComponent
    {
        private float K => GameController.Instance.UIController.K;
        
        private UIText _nameText;
        private UIText _descriptionText;

        public void Init()
        {
            RectTransformation rt = Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = new Vector2(0, 0);
            rt.AnchorMax = new Vector2(0, 0);
            rt.Size = new Vector2(86 * K, 31 * K);
            rt.Pivot = new Vector2(0, 0);
            rt.AnchoredPosition = new Vector2(0, 33 * K);

            _nameText = UI.CreateUIText("Header", "Header", Entity);
            _nameText.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            _nameText.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            _nameText.RectTransform.Pivot = new Vector2(0, 0);
            _nameText.RectTransform.Size = new Vector2(80 * K, 10 * K);
            _nameText.RectTransform.AnchoredPosition = new Vector2(2 * K, 16 * K);

            _nameText.TextAlignment = AlignmentType.TopLeft;
            _nameText.TextSize = 32;
            _nameText.Color = UIController.DarkColor;

            _descriptionText = UI.CreateUIText("Description", "Description", Entity);
            _descriptionText.RectTransform.AnchorMin = new Vector2(0.0f, 0.0f);
            _descriptionText.RectTransform.AnchorMax = new Vector2(0.0f, 0.0f);
            _descriptionText.RectTransform.Pivot = new Vector2(0, 0);
            _descriptionText.RectTransform.Size = new Vector2(80 * K, 18 * K);
            _descriptionText.RectTransform.AnchoredPosition = new Vector2(2 * K, 0 * K);

            _descriptionText.TextAlignment = AlignmentType.TopLeft;
            _descriptionText.TextSize = 16;
            _descriptionText.TextOverflowMode = OverflowMode.WrapByWords;
            _descriptionText.Color = UIController.DarkColor;

            Hide();
        }

        public void Set(string header, string body, ResourceCost? cost)
        {
            Entity.IsActiveSelf = true;

            if (cost.HasValue && !(cost.Value.Wood == 0 && cost.Value.Food == 0 && cost.Value.Gold == 0))
            {
                body += $"\nCost: {(cost.Value.Wood > 0 ? $"{cost.Value.Wood} Wood, " : "")}{(cost.Value.Food > 0 ? $"{cost.Value.Food} Food, " : "")}{(cost.Value.Gold > 0 ? $"{cost.Value.Gold} Gold, " : "")}";
            }

            _nameText.Text = header;
            _descriptionText.Text = body;
        }

        public void Hide()
        {
            Entity.IsActiveSelf = false;
        }
    }
}