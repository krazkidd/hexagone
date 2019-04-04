extends Node2D

class_name Board

var cursorTile : Tile setget set_cursor_tile

var Board : Array = []


func set_cursor_tile(value : Tile):
    if cursorTile != null:
        cursorTile.clear_cursor()

    cursorTile = value


func _ready():
    Global.Board = self

    var id : int = 0
    var r : Resource = load("res://Scenes/Tile.tscn")
    #var r : Resource = load("res://Scenes/FlowerTile.tscn")

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

                # position is relative to other tiles on the board
                # and will be transformed according to the board's transform
                tile.position = Vector2(x * 1.5, y * 2 - 1.0 * (x % 2))

                #print(String(tile.id) + ": " + String(tile.position))

                # add to scene tree
                add_child(tile)

            Board[x][y] = tile


func _input(event):
    if event is InputEventMouseButton:
        if event.pressed and event.button_index == BUTTON_LEFT:
            cursorTile.spin(Global.SpinDir.AntiClockwise)
        elif event.pressed and event.button_index == BUTTON_RIGHT:
            cursorTile.spin(Global.SpinDir.Clockwise)

        #TODO we will need to check more than once when we implement "dropping" tiles
        _check_board()


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
        #TODO convert tile to flower
        tile.get_neighbor(Global.Dir.UpLeft).safe_free()
        tile.get_neighbor(Global.Dir.Up).safe_free()
        tile.get_neighbor(Global.Dir.UpRight).safe_free()
        tile.get_neighbor(Global.Dir.DownLeft).safe_free()
        tile.get_neighbor(Global.Dir.Down).safe_free()
        tile.get_neighbor(Global.Dir.DownRight).safe_free()
