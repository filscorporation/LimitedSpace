using System;
using System.Collections.Generic;
using Steel;

namespace SteelCustom.PlayerSystem.Resources
{
    public class PlayerResources
    {
        public event Action OnChanged;
        
        public int Wood
        {
            get => GetAmount(ResourceType.Wood);
            set => SetAmount(ResourceType.Wood, value);
        }
        
        public int Food
        {
            get => GetAmount(ResourceType.Food);
            set => SetAmount(ResourceType.Food, value);
        }
        
        public int Gold
        {
            get => GetAmount(ResourceType.Gold);
            set => SetAmount(ResourceType.Gold, value);
        }
        
        private readonly int[] _data = new int[3];

        public List<(ResourceType ResourceType, int Amount)> GetAll() => new List<(ResourceType ResourceType, int Amount)> { (ResourceType.Wood, Wood), (ResourceType.Food, Food), (ResourceType.Gold, Gold) };
        
        public int GetAmount(ResourceType resourceType) => _data[(int)resourceType];
        public void SetAmount(ResourceType resourceType, int value)
        {
            _data[(int)resourceType] = value;
            OnChanged?.Invoke();
        }
        public void AddAmount(ResourceType resourceType, int value)
        {
            _data[(int)resourceType] += value;
            OnChanged?.Invoke();
        }

        public bool HasAmount(ResourceType resourceType, int amount)
        {
            return GetAmount(resourceType) >= amount;
        }

        public bool HasAmount(ResourceCost resourceCost)
        {
            return Wood >= resourceCost.Wood && Food >= resourceCost.Food && Gold >= resourceCost.Gold;
        }

        public IEnumerable<ResourceType> GetHasAmountReason(ResourceCost resourceCost)
        {
            if (HasAmount(ResourceType.Wood, resourceCost.Wood))
                yield return ResourceType.Wood;
            
            if (HasAmount(ResourceType.Food, resourceCost.Food))
                yield return ResourceType.Food;
            
            if (HasAmount(ResourceType.Gold, resourceCost.Gold))
                yield return ResourceType.Gold;
        }

        public void SpendAmount(ResourceType resourceType, int amount)
        {
            SetAmount(resourceType, GetAmount(resourceType) - amount);
            
            if (GetAmount(resourceType) < 0)
                Log.LogError($"{resourceType} amount is negative: {GetAmount(resourceType)}");
        }

        public void SpendAmount(ResourceCost resourceCost)
        {
            SpendAmount(ResourceType.Wood, resourceCost.Wood);
            SpendAmount(ResourceType.Food, resourceCost.Food);
            SpendAmount(ResourceType.Gold, resourceCost.Gold);
        }
    }
}