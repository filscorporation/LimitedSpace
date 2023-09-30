namespace SteelCustom.MapSystem
{
    public interface IMapElement
    {
        int X { get; }
        int Y { get; }

        int Row { get; set; }
        bool Passable { get; }
        int List { get; set; }
        int FValue { get; set; }
        int GValue { get; set; }
        int HValue { get; set; }
        int ParentX { get; set; }
        int ParentZ { get; set; }
    }
}