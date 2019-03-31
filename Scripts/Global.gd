extends Node

var Font : Font = load("res://Fonts/LibreBaskerville_DynamicFont.tres")

enum TileColor {
    Red = 0,
    Yellow,
    Green,
    Blue,
    Purple
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
    Clockwise = 0,
    AntiClockwise
}

const Red : Color = Color("#dc322f")
const Yellow : Color = Color("#b58900")
const Green : Color = Color("#859900")
const Blue : Color = Color("#6c71c4")
const Purple : Color = Color("#6c71c4")

const _x : float = 0.5 * cos(deg2rad(30)) / tan(deg2rad(30)) # _y / tan(30)
const _y : float = 0.5 * cos(deg2rad(30))

var UpLeft : Vector2 = Vector2(-_x, -_y).normalized()
var Up : Vector2 = Vector2.UP
var UpRight : Vector2 = Vector2(_x, -_y).normalized()

var DownLeft : Vector2 = Vector2(-_x, _y).normalized()
var Down : Vector2 = Vector2.DOWN
var DownRight : Vector2 = Vector2(_x, _y).normalized()

var Board : Board


func _ready():
    randomize()


func get_color(tileColor):
    match tileColor:
        TileColor.Red:
            return Red
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