extends Node2D

class_name Board

# selected/cursor tiles
var Tile1 : Tile
var Tile2 : Tile
var Tile3 : Tile

var Board : Array = []

func _ready():
    Global.Board = self

    var id : int = 0
    var r : Resource = load("res://Scenes/Tile.tscn")

    for x in range(10):
        Board.append([])

        for y in range(9):
            Board[x].append([])

            var tile : Tile = null

            if not (x % 2 == 0 and y == 9 - 1):
                id += 1

                tile = r.instance()

                tile.id = id

                tile.x = x
                tile.y = y

                tile.set_color(Global.get_color(randi() % 5))

                # position is relative to other tiles on the board
                # and will be transformed according to the board's transform
                tile.position = Vector2(x * 1.5, y * 2 - 1.0 * (x % 2))

                print(String(tile.id) + ": " + String(tile.position))

                # add to scene tree
                $Tiles.add_child(tile)

            Board[x][y] = tile

    Tile1 = Board[0][0]
    Tile2 = Board[1][0]
    Tile3 = Board[1][1]


func _input(event):
    if event is InputEventMouseButton:
        if event.pressed and event.button_index == BUTTON_LEFT:
            spin(Global.SpinDir.AntiClockwise)
        elif event.pressed and event.button_index == BUTTON_RIGHT:
            spin(Global.SpinDir.Clockwise)


func spin(spinDir):
    var dir = Tile1.find_neighbor(Tile2)

    match dir:
        Global.Dir.UpLeft:
            if Tile1.get_neighbor(Global.Dir.DownLeft) == Tile3:
                _spin(Tile1, Tile2, Tile3, spinDir)
            else:
                _spin(Tile1, Tile3, Tile2, spinDir)
        Global.Dir.Up:
            if Tile1.get_neighbor(Global.Dir.UpLeft) == Tile3:
                _spin(Tile1, Tile2, Tile3, spinDir)
            else:
                _spin(Tile1, Tile3, Tile2, spinDir)
        Global.Dir.UpRight:
            if Tile1.get_neighbor(Global.Dir.Up) == Tile3:
                _spin(Tile1, Tile2, Tile3, spinDir)
            else:
                _spin(Tile1, Tile3, Tile2, spinDir)
        Global.Dir.DownRight:
            if Tile1.get_neighbor(Global.Dir.UpRight) == Tile3:
                _spin(Tile1, Tile2, Tile3, spinDir)
            else:
                _spin(Tile1, Tile3, Tile2, spinDir)
        Global.Dir.Down:
            if Tile1.get_neighbor(Global.Dir.DownRight) == Tile3:
                _spin(Tile1, Tile2, Tile3, spinDir)
            else:
                _spin(Tile1, Tile3, Tile2, spinDir)
        Global.Dir.DownLeft:
            if Tile1.get_neighbor(Global.Dir.Down) == Tile3:
                _spin(Tile1, Tile2, Tile3, spinDir)
            else:
                _spin(Tile1, Tile3, Tile2, spinDir)

    #TODO we will need to check more than once when we implement "dropping" tiles
    _check_board()


func _spin(tile1 : Tile, tile2 : Tile, tile3 : Tile, spinDir):
    var tile1Color : Color = tile1.color

    if spinDir == Global.SpinDir.Clockwise:
        tile1.set_color(tile2.color)
        tile2.set_color(tile3.color)
        tile3.set_color(tile1Color)
    else:
        tile1.set_color(tile3.color)
        tile3.set_color(tile2.color)
        tile2.set_color(tile1Color)

func cursor_update(tile : Tile):
    # don't bother unless it's a new tile
    if tile != Tile1 and tile != Tile2 and tile != Tile3:
        if not tile.is_neighbor(Tile1):
            Tile1 = tile
        elif not tile.is_neighbor(Tile2):
            Tile2 = Tile1
            Tile1 = tile
        else:
            Tile3 = Tile1
            Tile1 = tile

        print(String(tile.id) + ": " + String(tile.x) + ", " + String(tile.y))

        _snug_up()

        $Cursor/Hexagon1.global_position = Tile1.global_position
        $Cursor/Hexagon2.global_position = Tile2.global_position
        $Cursor/Hexagon3.global_position = Tile3.global_position


#
# Tighten up the cursor tiles so they are all neighbors of each other.
#
func _snug_up():
    var isNeighbor2 : bool = Tile1.is_neighbor(Tile2)
    var isNeighbor3 : bool = Tile1.is_neighbor(Tile3)

    if not isNeighbor2 and not isNeighbor3:
        Tile2 = Tile1.get_first_neighbor()
        Tile3 = Tile1.get_mutual_neighbor(Tile2)
    elif not isNeighbor2:
        Tile2 = Tile1.get_mutual_neighbor(Tile3)
    elif not isNeighbor3:
        Tile3 = Tile1.get_mutual_neighbor(Tile2)


func _check_board():
    #TODO for clusters, just need to test the ones that moved;
    #     for flowers, just need to test their neighbors

    var clusters : Array = []
    var flowers : Array = []

    for x in Board.size():
        for y in Board[x].size():
            var tile : Tile = Board[x][y]

            if tile != null and tile.is_cluster():
                clusters.append(tile)
            elif tile != null and tile.is_flower():
                flowers.append(tile)

    for tile in clusters:
        tile.safe_free()

    for tile in flowers:
        tile.get_neighbor(Global.Dir.UpLeft).safe_free()
        tile.get_neighbor(Global.Dir.Up).safe_free()
        tile.get_neighbor(Global.Dir.UpRight).safe_free()
        tile.get_neighbor(Global.Dir.DownLeft).safe_free()
        tile.get_neighbor(Global.Dir.Down).safe_free()
        tile.get_neighbor(Global.Dir.DownRight).safe_free()