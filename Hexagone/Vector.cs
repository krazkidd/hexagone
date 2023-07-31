using Godot;
using System;

namespace Hexagone;

public static class Vector
{
    public static readonly float X = 0.5f * Mathf.Cos(Mathf.DegToRad(30)) / Mathf.Tan(Mathf.DegToRad(30)); // Y / Mathf.Tan(30)
    public static readonly float Y = 0.5f * Mathf.Cos(Mathf.DegToRad(30));

    public static readonly Vector2 UpLeft = new Vector2(X, Y).Normalized();
    public static readonly Vector2 Up = Vector2.Up;
    public static readonly Vector2 UpRight = new Vector2(X, -Y).Normalized();

    public static readonly Vector2 DownLeft = new Vector2(-X, Y).Normalized();
    public static readonly Vector2 Down = Vector2.Down;
    public static readonly Vector2 DownRight = new Vector2(X, Y).Normalized();

    public static Vector2 GetDirVector(Dir dir) =>
        dir switch
        {
            Dir.UpLeft => UpLeft,
            Dir.Up => Up,
            Dir.UpRight => UpRight,
            Dir.DownLeft => DownLeft,
            Dir.DownRight => DownRight,
            Dir.Down => Down,
            _ => throw new ArgumentException("Invalid Dir", "dir"),
            //TODO better error message
        };
}