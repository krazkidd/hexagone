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

    public static Godot.Color GetColor(TileColor tileColor) =>
        tileColor switch
        {
            TileColor.Red => Red,
            TileColor.Magenta => Magenta,
            TileColor.Yellow => Yellow,
            TileColor.Green => Green,
            TileColor.Blue => Blue,
            TileColor.Purple => Purple,
            _ => throw new ArgumentException("Invalid TileColor", "tileColor"),
            //TODO better error message
        };
}