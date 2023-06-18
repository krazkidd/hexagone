extends Node

var Resources : Dictionary = {
    "Font": load("res://Fonts/LibreBaskerville_DynamicFont.tres"),

    "Tile": load("res://Scenes/Tile.tscn"),
    "Flower": load("res://Scenes/FlowerTile.tscn")
}

enum TileType {
    Normal,
    Flower,
    Pearl
}

enum TileColor {
    Red,
    Magenta,
    Yellow,
    Green,
    Blue,
    Purple,

    Flower,
    Pearl
}

enum Dir {
    Unknown = -1,
    UpLeft = 0,
    Up,
    UpRight,
    DownRight,
    Down,
    DownLeft
}

enum SpinDir {
    Clockwise,
    AntiClockwise
}

const Red : Color = Color("#dc322f")
const Magenta : Color = Color("#d33682")
const Yellow : Color = Color("#b58900")
const Green : Color = Color("#859900")
const Blue : Color = Color("#6c71c4")
const Purple : Color = Color("#6c71c4")

const _x : float = 0.5 * cos(deg_to_rad(30)) / tan(deg_to_rad(30)) # _y / tan(30)
const _y : float = 0.5 * cos(deg_to_rad(30))

var UpLeft : Vector2 = Vector2(-_x, -_y).normalized()
var Up : Vector2 = Vector2.UP
var UpRight : Vector2 = Vector2(_x, -_y).normalized()

var DownLeft : Vector2 = Vector2(-_x, _y).normalized()
var Down : Vector2 = Vector2.DOWN
var DownRight : Vector2 = Vector2(_x, _y).normalized()

var Board : Board


func _init():
    #TODO randomize()
    pass


func get_color(tileColor):
    match tileColor:
        TileColor.Red:
            return Red
        TileColor.Magenta:
            return Magenta
        TileColor.Yellow:
            return Yellow
        TileColor.Green:
            return Green
        TileColor.Blue:
            return Blue
        TileColor.Purple:
            return Purple

func get_dir_vector(dir):
    match dir:
        Dir.UpLeft:
            return UpLeft
        Dir.Up:
            return Up
        Dir.UpRight:
            return UpRight
        Dir.DownRight:
            return DownRight
        Dir.Down:
            return Down
        Dir.DownLeft:
            return DownLeft