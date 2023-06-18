extends Node2D

class_name Tile, "res://Scenes/Hexagon.tscn"

var x : int: get = get_x, set = set_x
var y : int: get = get_y, set = set_y

var type

var color : Color: set = set_color
var isCursor : bool: get = get_is_cursor, set = set_is_cursor

@onready var _face : Polygon2D = $Face
@onready var _back : Polygon2D = $Back


func get_x() -> int:
    return x


func set_x(value : int):
    set_xy(value, y)


func get_y() -> int:
    return y


func set_y(value : int):
    set_xy(x, value)


func set_xy(xval : int, yval : int):
    # bypass set_x()/set_y() so we only update position once
    x = xval
    y = yval

    position = Vector2(x * 1.5, y * 2 - 1.0 * (x % 2))


func set_color(value : Color):
    color = value
    _face.color = value


func set_is_cursor(value : bool):
    _back.visible = value


func get_is_cursor() -> bool:
    return _back.visible


func _init():
    init()


func _ready():
    ready()


func init():
    type = Global.TileType.Normal
    color = Global.get_color(randi() % 6)


func ready():
    _face.color = color


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
# A pistil is the structure of the gynoecium, or the female
# reproductive organ.
#
func is_pistile() -> bool:
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


#
# Checks if this tile is the outer ring of a flower, where a neighboring
# tile returns is_pistile() == true.
#
# A petal is a part of the corolla--a reproductive whorl.
#
func is_petal() -> bool:
    var neighborTopLeft : Tile = get_neighbor(Global.Dir.UpLeft)
    var neighborTop : Tile = get_neighbor(Global.Dir.Up)
    var neighborTopRight : Tile = get_neighbor(Global.Dir.UpRight)
    var neighborBottomRight : Tile = get_neighbor(Global.Dir.DownRight)
    var neighborBottom : Tile = get_neighbor(Global.Dir.Down)
    var neighborBottomLeft : Tile = get_neighbor(Global.Dir.DownLeft)

    # we compare the colors as a shortcut--so we don't have to call .is_pistile()
    return ((neighborTopLeft != null and color != neighborTopLeft.color and neighborTopLeft.is_pistile())
        or (neighborTop != null and color != neighborTop.color and neighborTop.is_pistile())
        or (neighborTopRight != null and color != neighborTopRight.color and neighborTopRight.is_pistile())
        or (neighborBottomRight != null and color != neighborBottomRight.color and neighborBottomRight.is_pistile())
        or (neighborBottom != null and color != neighborBottom.color and neighborBottom.is_pistile())
        or (neighborBottomLeft != null and color != neighborBottomLeft.color and neighborBottomLeft.is_pistile()))


func _on_MouseArea_mouse_entered():
    Global.Board.cursorTile = self

    set_is_cursor(true)

    set_cursor_neighbors()


func set_cursor_neighbors():
    var pos : Vector2 = get_viewport().get_mouse_position()

    var nearest1 : Tile = get_first_neighbor()
    var nearest2 : Tile = null

    var tile : Tile

    tile = get_neighbor(Global.Dir.Up)
    if tile != null:
        if (tile.global_position - pos).length() < (nearest1.global_position - pos).length():
            nearest1 = tile

    tile = get_neighbor(Global.Dir.UpRight)
    if tile != null:
        if (tile.global_position - pos).length() < (nearest1.global_position - pos).length():
            nearest1 = tile

    tile = get_neighbor(Global.Dir.DownRight)
    if tile != null:
        if (tile.global_position - pos).length() < (nearest1.global_position - pos).length():
            nearest1 = tile

    tile = get_neighbor(Global.Dir.Down)
    if tile != null:
        if (tile.global_position - pos).length() < (nearest1.global_position - pos).length():
            nearest1 = tile

    tile = get_neighbor(Global.Dir.DownLeft)
    if tile != null:
        if (tile.global_position - pos).length() < (nearest1.global_position - pos).length():
            nearest1 = tile

    tile = get_neighbor(Global.Dir.UpLeft)
    if tile != null:
        if (tile.global_position - pos).length() < (nearest1.global_position - pos).length():
            nearest1 = tile

    #######################################################

    nearest2 = get_mutual_neighbor(nearest1)

    tile = get_neighbor(Global.Dir.Up)
    if tile != null and tile != nearest1 and nearest1.is_neighbor(tile):
        if (tile.global_position - pos).length() < (nearest2.global_position - pos).length():
            nearest2 = tile

    tile = get_neighbor(Global.Dir.UpRight)
    if tile != null and tile != nearest1 and nearest1.is_neighbor(tile):
        if (tile.global_position - pos).length() < (nearest2.global_position - pos).length():
            nearest2 = tile

    tile = get_neighbor(Global.Dir.DownRight)
    if tile != null and tile != nearest1 and nearest1.is_neighbor(tile):
        if (tile.global_position - pos).length() < (nearest2.global_position - pos).length():
            nearest2 = tile

    tile = get_neighbor(Global.Dir.Down)
    if tile != null and tile != nearest1 and nearest1.is_neighbor(tile):
        if (tile.global_position - pos).length() < (nearest2.global_position - pos).length():
            nearest2 = tile

    tile = get_neighbor(Global.Dir.DownLeft)
    if tile != null and tile != nearest1 and nearest1.is_neighbor(tile):
        if (tile.global_position - pos).length() < (nearest2.global_position - pos).length():
            nearest2 = tile

    tile = get_neighbor(Global.Dir.UpLeft)
    if tile != null and tile != nearest1 and nearest1.is_neighbor(tile):
        if (tile.global_position - pos).length() < (nearest2.global_position - pos).length():
            nearest2 = tile

    nearest1.isCursor = true
    nearest2.isCursor = true


func clear_cursor():
    set_is_cursor(false)

    var tile : Tile

    tile = get_neighbor(Global.Dir.Up)
    if tile != null and tile.isCursor:
        tile.isCursor = false

    tile = get_neighbor(Global.Dir.UpRight)
    if tile != null and tile.isCursor:
        tile.isCursor = false

    tile = get_neighbor(Global.Dir.DownRight)
    if tile != null and tile.isCursor:
        tile.isCursor = false

    tile = get_neighbor(Global.Dir.Down)
    if tile != null and tile.isCursor:
        tile.isCursor = false

    tile = get_neighbor(Global.Dir.DownLeft)
    if tile != null and tile.isCursor:
        tile.isCursor = false

    tile = get_neighbor(Global.Dir.UpLeft)
    if tile != null and tile.isCursor:
        tile.isCursor = false


func spin(spinDir):
    var neighborTopLeft : Tile = get_neighbor(Global.Dir.UpLeft)
    var neighborTop : Tile = get_neighbor(Global.Dir.Up)
    var neighborTopRight : Tile = get_neighbor(Global.Dir.UpRight)
    var neighborBottomRight : Tile = get_neighbor(Global.Dir.DownRight)
    var neighborBottom : Tile = get_neighbor(Global.Dir.Down)
    var neighborBottomLeft : Tile = get_neighbor(Global.Dir.DownLeft)

    if (neighborTopLeft != null and neighborTopLeft.isCursor
        and neighborTop != null and neighborTop.isCursor):
        _spin(self, neighborTopLeft, neighborTop, spinDir)
    elif (neighborTop != null and neighborTop.isCursor
        and neighborTopRight != null and neighborTopRight.isCursor):
        _spin(self, neighborTop, neighborTopRight, spinDir)
    elif (neighborTopRight != null and neighborTopRight.isCursor
        and neighborBottomRight != null and neighborBottomRight.isCursor):
        _spin(self, neighborTopRight, neighborBottomRight, spinDir)
    elif (neighborBottomRight != null and neighborBottomRight.isCursor
        and neighborBottom != null and neighborBottom.isCursor):
        _spin(self, neighborBottomRight, neighborBottom, spinDir)
    elif (neighborBottom != null and neighborBottom.isCursor
        and neighborBottomLeft != null and neighborBottomLeft.isCursor):
        _spin(self, neighborBottom, neighborBottomLeft, spinDir)
    elif (neighborBottomLeft != null and neighborBottomLeft.isCursor
        and neighborTopLeft != null and neighborTopLeft.isCursor):
        _spin(self, neighborBottomLeft, neighborTopLeft, spinDir)


func _spin(tile1 : Tile, tile2 : Tile, tile3 : Tile, spinDir):
    var tile1x : int = tile1.x
    var tile1y : int = tile1.y

    if spinDir == Global.SpinDir.Clockwise:
        tile1.set_xy(tile2.x, tile2.y)
        tile2.set_xy(tile3.x, tile3.y)
        tile3.set_xy(tile1x, tile1y)
    else:
        tile1.set_xy(tile3.x, tile3.y)
        tile3.set_xy(tile2.x, tile2.y)
        tile2.set_xy(tile1x, tile1y)

    Global.Board.Board[tile1.x][tile1.y] = tile1
    Global.Board.Board[tile2.x][tile2.y] = tile2
    Global.Board.Board[tile3.x][tile3.y] = tile3


func safe_free():
    if not is_queued_for_deletion():
        queue_free()