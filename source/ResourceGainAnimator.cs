using System;
using System.Collections;
using Steel;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom
{
    public class ResourceGainAnimator : ScriptComponent
    {
        public void Animate(Vector3 position, ResourceType resourceType)
        {
            Entity entity = new Entity("Resource", Entity);
            entity.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.GetImage(ResourceIcon(resourceType));
            entity.Transformation.Position = position + new Vector3(0, 0, 2);

            StartCoroutine(AnimateCoroutine(entity));
        }

        private string ResourceIcon(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Wood:
                    return "wood_icon.png";
                case ResourceType.Food:
                    return "food_icon.png";
                case ResourceType.Gold:
                    return "gold_icon.png";
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }

        private IEnumerator AnimateCoroutine(Entity entity)
        {
            float timer = 0.0f;
            while (timer < 0.5f)
            {
                entity.Transformation.Position += new Vector3(0, Time.DeltaTime * 3.0f, 0);
                
                timer += Time.DeltaTime;
                yield return null;
            }
            
            entity.Destroy();
        }
    }
}