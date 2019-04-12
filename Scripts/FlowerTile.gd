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

    var neighborTopLeftx : int = neighborTopLeft.x
    var neighborTopLefty : int = neighborTopLeft.y

    if spinDir == Global.SpinDir.Clockwise:
        neighborTopLeft.set_xy(neighborTop.x, neighborTop.y)
        neighborTop.set_xy(neighborTopRight.x, neighborTopRight.y)
        neighborTopRight.set_xy(neighborBottomRight.x, neighborBottomRight.y)
        neighborBottomRight.set_xy(neighborBottom.x, neighborBottom.y)
        neighborBottom.set_xy(neighborBottomLeft.x, neighborBottomLeft.y)
        neighborBottomLeft.set_xy(neighborTopLeftx, neighborTopLefty)
    else:
        neighborTopLeft.set_xy(neighborBottomLeft.x, neighborBottomLeft.y)
        neighborBottomLeft.set_xy(neighborBottom.x, neighborBottom.y)
        neighborBottom.set_xy(neighborBottomRight.x, neighborBottomRight.y)
        neighborBottomRight.set_xy(neighborTopRight.x, neighborTopRight.y)
        neighborTopRight.set_xy(neighborTop.x, neighborTop.y)
        neighborTop.set_xy(neighborTopLeftx, neighborTopLefty)

    Global.Board.Board[neighborTopLeft.x][neighborTopLeft.y] = neighborTopLeft
    Global.Board.Board[neighborTop.x][neighborTop.y] = neighborTop
    Global.Board.Board[neighborTopRight.x][neighborTopRight.y] = neighborTopRight
    Global.Board.Board[neighborBottomRight.x][neighborBottomRight.y] = neighborBottomRight
    Global.Board.Board[neighborBottom.x][neighborBottom.y] = neighborBottom
    Global.Board.Board[neighborBottomLeft.x][neighborBottomLeft.y] = neighborBottomLeft