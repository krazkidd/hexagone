extends "res://Scripts/Tile.gd"

class_name FlowerTile


func _ready():
    type = Global.TileType.Flower


func set_cursor_neighbors():
    #TODO how to handle edges?

    var tile : Tile

    tile = get_neighbor(Global.Dir.UpLeft)
    if tile != null:
        tile.isCursor = true

    tile = get_neighbor(Global.Dir.Up)
    if tile != null:
        tile.isCursor = true

    tile = get_neighbor(Global.Dir.UpRight)
    if tile != null:
        tile.isCursor = true

    tile = get_neighbor(Global.Dir.DownRight)
    if tile != null:
        tile.isCursor = true

    tile = get_neighbor(Global.Dir.Down)
    if tile != null:
        tile.isCursor = true

    tile = get_neighbor(Global.Dir.DownLeft)
    if tile != null:
        tile.isCursor = true


func spin(spinDir):
    pass