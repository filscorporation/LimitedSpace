using System.Collections.Generic;
using Steel;
using SteelCustom.GameActions;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Buildings
{
    public class Mill : Building
    {
        public override (int X, int Y) Size => (4, 4);
        protected override string SpritePath => "mill.aseprite";
        public override BuildingType Type => BuildingType.Mill;
        public override float BuildingDuration => 10;

        public override string Name => "Mill";
        public override List<GameAction> GameActions => new List<GameAction> { new HeavyPlowGameAction() };

        public override void Init()
        {
            base.Init();
            
            var animator = Entity.AddComponent<Animator>();
            animator.AddAnimations(ResourcesManager.GetAsepriteData(SpritePath, true).Animations);
            animator.Play("Idle");
        }

        public override bool IsStorage(ResourceType resourceType)
        {
            return resourceType == ResourceType.Food;
        }
    }
}