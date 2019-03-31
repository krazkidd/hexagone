extends Node2D

class_name Tile, "res://Scenes/Hexagon.tscn"

var id : int

var x : int
var y : int

var color : Color setget set_color


func set_color(value : Color):
    color = value
    ($Polygon2D as Polygon2D).color = value


func get_neighbor(dir) -> Tile:
    var otherX : int = x
    var otherY : int = y

    if dir == Global.Dir.UpLeft or dir == Global.Dir.DownLeft:
        otherX -= 1
    elif dir == Global.Dir.UpRight or dir == Global.Dir.DownRight:
        otherX += 1

    if x % 2 == 0:
        if dir == Global.Dir.Up:
            otherY -= 1
        elif dir == Global.Dir.DownLeft or dir == Global.Dir.Down or dir == Global.Dir.DownRight:
            otherY += 1
    else:
        if dir == Global.Dir.UpLeft or dir == Global.Dir.Up or dir == Global.Dir.UpRight:
            otherY -= 1
        elif dir == Global.Dir.Down:
            otherY += 1

    if otherX < 0 or otherY < 0:
        return null
    elif otherX > Global.Board.Board.size() - 1:
        return null
    elif otherY > Global.Board.Board[otherX].size() - 1:
        return null

    # might return null if x % 2 == 0
    return Global.Board.Board[otherX][otherY]


func find_neighbor(tile : Tile):
    if get_neighbor(Global.Dir.UpLeft) == tile:
        return Global.Dir.UpLeft
    elif get_neighbor(Global.Dir.Up) == tile:
        return Global.Dir.Up
    elif get_neighbor(Global.Dir.UpRight) == tile:
        return Global.Dir.UpRight
    elif get_neighbor(Global.Dir.DownRight) == tile:
        return Global.Dir.DownRight
    elif get_neighbor(Global.Dir.Down) == tile:
        return Global.Dir.Down
    elif get_neighbor(Global.Dir.DownLeft) == tile:
        return Global.Dir.DownLeft

    return Global.Dir.Unknown


func is_neighbor(tile : Tile) -> bool:
    return find_neighbor(tile) != Global.Dir.Unknown


#TODO can pass in startDir param and then have two loops or osmething
func get_first_neighbor() -> Tile:
    var tile : Tile

    tile = get_neighbor(Global.Dir.Up)
    if tile != null:
        return tile

    tile = get_neighbor(Global.Dir.UpRight)
    if tile != null:
        return tile

    tile = get_neighbor(Global.Dir.DownRight)
    if tile != null:
        return tile

    tile = get_neighbor(Global.Dir.Down)
    if tile != null:
        return tile

    tile = get_neighbor(Global.Dir.DownLeft)
    if tile != null:
        return tile

    tile = get_neighbor(Global.Dir.UpLeft)
    if tile != null:
        return tile

    return null


func get_mutual_neighbor(neighbor : Tile) -> Tile:
    var tile : Tile

    tile = get_neighbor(Global.Dir.Up)
    if tile != null and neighbor.is_neighbor(tile):
        return tile

    tile = get_neighbor(Global.Dir.UpRight)
    if tile != null and neighbor.is_neighbor(tile):
        return tile

    tile = get_neighbor(Global.Dir.DownRight)
    if tile != null and neighbor.is_neighbor(tile):
        return tile

    tile = get_neighbor(Global.Dir.Down)
    if tile != null and neighbor.is_neighbor(tile):
        return tile

    tile = get_neighbor(Global.Dir.DownLeft)
    if tile != null and neighbor.is_neighbor(tile):
        return tile

    tile = get_neighbor(Global.Dir.UpLeft)
    if tile != null and neighbor.is_neighbor(tile):
        return tile

    return null


#
# Checks if this tile is a member of a cluster, where at least
# two adjacent neighbors are of the same color as this tile.
#
func is_cluster() -> bool:
    var neighborTopLeft : Tile = get_neighbor(Global.Dir.UpLeft)
    var neighborTop : Tile = get_neighbor(Global.Dir.Up)
    var neighborTopRight : Tile = get_neighbor(Global.Dir.UpRight)
    var neighborBottomRight : Tile = get_neighbor(Global.Dir.DownRight)
    var neighborBottom : Tile = get_neighbor(Global.Dir.Down)
    var neighborBottomLeft : Tile = get_neighbor(Global.Dir.DownLeft)

    return (neighborTopLeft != null and neighborTop != null and color == neighborTopLeft.color and color == neighborTop.color
        or neighborTop != null and neighborTopRight != null and color == neighborTop.color and color == neighborTopRight.color
        or neighborTopRight != null and neighborBottomRight != null and color == neighborTopRight.color and color == neighborBottomRight.color
        or neighborBottomRight != null and neighborBottom != null and color == neighborBottomRight.color and color == neighborBottom.color
        or neighborBottom != null and neighborBottomLeft != null and color == neighborBottom.color and color == neighborBottomLeft.color
        or neighborBottomLeft != null and neighborTopLeft != null and color == neighborBottomLeft.color and color == neighborTopLeft.color)


#
# Checks if this tile is the center of a flower, where every neighbor
# is of the same color but different from the color of this tile.
#
func is_flower() -> bool:
    var neighborTopLeft : Tile = get_neighbor(Global.Dir.UpLeft)
    var neighborTop : Tile = get_neighbor(Global.Dir.Up)
    var neighborTopRight : Tile = get_neighbor(Global.Dir.UpRight)
    var neighborBottomRight : Tile = get_neighbor(Global.Dir.DownRight)
    var neighborBottom : Tile = get_neighbor(Global.Dir.Down)
    var neighborBottomLeft : Tile = get_neighbor(Global.Dir.DownLeft)

    return (neighborTopLeft != null and neighborTop != null and neighborTopRight != null
        and neighborBottomLeft != null and neighborBottom != null and neighborBottomRight != null
        and neighborTopLeft.color != color
        and neighborTopLeft.color == neighborTop.color
        and neighborTopLeft.color == neighborTopRight.color
        and neighborTopLeft.color == neighborBottomRight.color
        and neighborTopLeft.color == neighborBottom.color
        and neighborTopLeft.color == neighborBottomLeft.color)


func _on_MouseArea_mouse_entered():
    Global.Board.cursor_update(self)


func safe_free():
    if not is_queued_for_deletion():
        Global.Board.Board[x][y] = null
        queue_free()