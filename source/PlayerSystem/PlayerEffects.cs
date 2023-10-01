namespace SteelCustom.PlayerSystem
{
    public class PlayerEffects
    {
        public int WorkerCapacityBonus { get; set; } = 0;
        public float WorkerSpeedBonus { get; set; } = 1;
        public float WorkerWoodGatherSpeedBonus { get; set; } = 1;
        public float WorkerFoodGatherSpeedBonus { get; set; } = 1;
        
        public float ConstructionSpeedBonus { get; set; } = 1;
        public int FarmResourcesBonus { get; set; } = 0;
        
        public bool AdvanceGameActionReserved { get; set; }
        public bool BowSawGameActionReserved { get; set; }
        public bool HeavyPlowGameActionReserved { get; set; }
        public bool TreadmillCraneGameActionReserved { get; set; }
        public bool WheelbarrowGameActionReserved { get; set; }
    }
}