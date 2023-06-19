namespace Hexagone
{
    public enum TileType
    {
        Normal,
        Flower,
        Pearl
    }

    public enum TileColor
    {
        Red,
        Magenta,
        Yellow,
        Green,
        Blue,
        Purple,

        Flower,
        Pearl
    }

    public enum Dir
    {
        Unknown = -1,
        UpLeft = 0,
        Up,
        UpRight,
        DownRight,
        Down,
        DownLeft
    }

    public enum SpinDir
    {
        Clockwise,
        AntiClockwise
    }
}