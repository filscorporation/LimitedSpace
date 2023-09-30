using Steel;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.MapSystem
{
    public class Tree : ResourceObject
    {
        public override float GatherDuration => 1;
        public override ResourceType ResourceType => ResourceType.Wood;

        private const int MAX_WOOD_AMOUNT = 50;
        
        public void Init()
        {
            Sprite sprite = ResourcesManager.GetImage("tree_1.png");
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;

            ResourceAmount = MAX_WOOD_AMOUNT;
        }
    }
}