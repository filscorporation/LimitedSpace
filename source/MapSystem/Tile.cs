using Steel;
using SteelCustom.Units;

namespace SteelCustom.MapSystem
{
    public class Tile : IMapElement
    {
        public int X { get; }
        public int Y { get; }

        public MapObject OnObject { get; private set; }
        public Unit OnReservingUnit { get; private set; }

        public bool IsOccupied => OnObject != null && !OnObject.Entity.IsDestroyed();
        public bool IsReserved => OnReservingUnit != null && !OnReservingUnit.Entity.IsDestroyed();
        public bool Passable => (!IsOccupied || !OnObject.IsBlocking) && !IsReserved;

        #region Path finder
        
        public int Row { get; set; }
        public int List { get; set; }
        public int FValue { get; set; }
        public int GValue { get; set; }
        public int HValue { get; set; }
        public int ParentX { get; set; }
        public int ParentZ { get; set; }

        #endregion

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetOnObject(MapObject onObject)
        {
            if (IsOccupied)
                Log.LogError($"Tile ({X}, {Y}) is already occupied with {OnObject}");

            OnObject = onObject;
        }

        public void ClearOnObject()
        {
            OnObject = null;
        }

        public void SetOnReservingUnit(Unit unit)
        {
            if (IsReserved)
                Log.LogError($"Tile ({X}, {Y}) is already reserved by {OnReservingUnit}");

            OnReservingUnit = unit;
        }

        public void ClearOnReservingUnit()
        {
            OnReservingUnit = null;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}