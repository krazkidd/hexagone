using Godot;
using System;

namespace Hexagone;

public static class Color
{

    public static readonly Godot.Color Red = new("#dc322f");
    public static readonly Godot.Color Magenta = new("#d33682");
    public static readonly Godot.Color Yellow = new("#b58900");
    public static readonly Godot.Color Green = new("#859900");
    public static readonly Godot.Color Blue = new("#6c71c4");
    public static readonly Godot.Color Purple = new("#6c71c4");

    public static Godot.Color GetColor(TileColor tileColor)
    {
        switch (tileColor)
        {
            case TileColor.Red:
                return Red;
            case TileColor.Magenta:
                return Magenta;
            case TileColor.Yellow:
                return Yellow;
            case TileColor.Green:
                return Green;
            case TileColor.Blue:
                return Blue;
            case TileColor.Purple:
                return Purple;

            default:
                //TODO better error message
                throw new ArgumentException("Invalid TileColor", "tileColor");
        }
    }

}