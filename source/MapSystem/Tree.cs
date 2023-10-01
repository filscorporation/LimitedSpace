using System.Collections.Generic;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.MapSystem
{
    public class Tree : ResourceObject
    {
        public override float GatherDuration => 1;
        public override ResourceType ResourceType => ResourceType.Wood;

        private const int MAX_WOOD_AMOUNT = 50;

        public override string Name => "Tree";
        public override List<GameAction> GameActions => new List<GameAction>();
        
        public override void Init()
        {
            Sprite sprite = ResourcesManager.GetImage(GetSpriteName());
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;

            ResourceAmount = MAX_WOOD_AMOUNT;
            
            base.Init();
        }

        protected override void OnGathered()
        {
            base.OnGathered();

            if (MAX_WOOD_AMOUNT - ResourceAmount >= 10)
                SetChopped();
        }

        private string GetSpriteName()
        {
            const int spritesCount = 2;
            int index = Random.NextInt(1, spritesCount);
            return $"tree_{index}.png";
        }

        private void SetChopped()
        {
            GetComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage("tree_chopped.png");
        }
    }
}