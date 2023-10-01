using System;
using SteelCustom.PlayerSystem.Resources;

namespace SteelCustom.Buildings
{
    public static class BuildingsInfo
    {
        public static ResourceCost GetCost(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.TownCenter:
                    return new ResourceCost(300, 0, 0);
                case BuildingType.House:
                    return new ResourceCost(30, 0, 0);
                case BuildingType.Farm:
                    return new ResourceCost(60, 0, 0);
                case BuildingType.LumberMill:
                    return new ResourceCost(100, 0, 0);
                case BuildingType.Mill:
                    return new ResourceCost(100, 0, 0);
                case BuildingType.Market:
                    return new ResourceCost(200, 0, 0);
                case BuildingType.Wonder:
                    return new ResourceCost(5000, 0, 2000);
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }
        
        public static string GetName(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.TownCenter:
                    return "Town center";
                case BuildingType.House:
                    return "House";
                case BuildingType.Farm:
                    return "Farm";
                case BuildingType.LumberMill:
                    return "Lumber mill";
                case BuildingType.Mill:
                    return "Mill";
                case BuildingType.Market:
                    return "Market";
                case BuildingType.Wonder:
                    return "Wonder";
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }
        
        public static string GetDescription(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.TownCenter:
                    return "Can produce workers, advance to the new age and research economy technologies.\nHotkey 3";
                case BuildingType.House:
                    return "Gives you 5 population space, it is required to produce workers.\nHotkey 1";
                case BuildingType.Farm:
                    return "Can be used by one worker to gather food like resource, expires after food is gone.\nHotkey 2";
                case BuildingType.LumberMill:
                    return "Can be used to drop wood and research wood chopping technologies.\nHotkey 4";
                case BuildingType.Mill:
                    return "Can be used to drop food and research farming technologies.\nHotkey 5";
                case BuildingType.Market:
                    return "Allows you to sell wood for gold.\nHotkey 6";
                case BuildingType.Wonder:
                    return "Symbolizes the power of your kingdom. Wins the game.\nRequires advanced era.\nHotkey 9";
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }
        
        public static string GetIcon(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.TownCenter:
                    return "ui_town_center.png";
                case BuildingType.House:
                    return "ui_house.png";
                case BuildingType.Farm:
                    return "ui_farm.png";
                case BuildingType.LumberMill:
                    return "ui_lumber_mill.png";
                case BuildingType.Mill:
                    return "ui_mill.png";
                case BuildingType.Market:
                    return "ui_market.png";
                case BuildingType.Wonder:
                    return "ui_wonder.png";
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }
    }
}