using Steel;

namespace SteelCustom.MapSystem
{
    public class Tree : ResourceObject
    {
        public void Init()
        {
            Sprite sprite = ResourcesManager.GetImage("tree_1.png");
            Entity.AddComponent<SpriteRenderer>().Sprite = sprite;
        }
    }
}