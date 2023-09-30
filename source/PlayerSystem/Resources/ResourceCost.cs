using System;

namespace SteelCustom.PlayerSystem.Resources
{
    public struct ResourceCost
    {
        private int _wood;
        private int _food;
        private int _gold;

        public int Wood => _wood;
        public int Food => _food;
        public int Gold => _gold;

        public ResourceCost(int wood, int food, int gold)
        {
            _wood = wood;
            _food = food;
            _gold = gold;
        }

        public ResourceCost Discount(int wood, int food, int gold)
        {
            return new ResourceCost(Math.Max(0, Wood - wood), Math.Max(0, Food - food), Math.Max(0, Gold - gold));
        }
    }
}