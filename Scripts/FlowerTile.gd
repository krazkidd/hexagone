extends "res://Scripts/Tile.gd"

class_name FlowerTile


func init():
    type = Global.TileType.Flower


func ready():
    pass


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
    var neighborTopLeft : Tile = get_neighbor(Global.Dir.UpLeft)
    var neighborTop : Tile = get_neighbor(Global.Dir.Up)
    var neighborTopRight : Tile = get_neighbor(Global.Dir.UpRight)
    var neighborBottomRight : Tile = get_neighbor(Global.Dir.DownRight)
    var neighborBottom : Tile = get_neighbor(Global.Dir.Down)
    var neighborBottomLeft : Tile = get_neighbor(Global.Dir.DownLeft)

    var neighborTopLeftColor : Color = neighborTopLeft.color

    if spinDir == Global.SpinDir.Clockwise:
        neighborTopLeft.set_color(neighborTop.color)
        neighborTop.set_color(neighborTopRight.color)
        neighborTopRight.set_color(neighborBottomRight.color)
        neighborBottomRight.set_color(neighborBottom.color)
        neighborBottom.set_color(neighborBottomLeft.color)
        neighborBottomLeft.set_color(neighborTopLeftColor)
    else:
        neighborTopLeft.set_color(neighborBottomLeft.color)
        neighborBottomLeft.set_color(neighborBottom.color)
        neighborBottom.set_color(neighborBottomRight.color)
        neighborBottomRight.set_color(neighborTopRight.color)
        neighborTopRight.set_color(neighborTop.color)
        neighborTop.set_color(neighborTopLeftColor)