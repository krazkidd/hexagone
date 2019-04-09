extends Node2D

class_name Board

var cursorTile : Tile setget set_cursor_tile

var Board : Array = []


func set_cursor_tile(value : Tile):
    if cursorTile != null:
        cursorTile.clear_cursor()

    cursorTile = value


func _init():
    Global.Board = self

    for x in range(10):
        Board.append([])
        Board[x].resize(9)

        for y in range(9):
            if not (x % 2 == 0 and y == 9 - 1):
                var tile : Tile = create_tile(Global.TileType.Normal, x, y)

                # since we're always on an edge as we build the board,
                # new tiles cannot be the center of a flower
                while tile.is_cluster() or tile.is_petal():
                    tile.free()

                    tile = create_tile(Global.TileType.Normal, x, y)

                Board[x][y] = tile

                # add to scene tree
                add_child(tile)
            else:
                Board[x][y] = null


func create_tile(tiletype, x : int, y : int):
    var tile : Tile = null

    match tiletype:
        Global.TileType.Normal:
            tile = Global.Resources.Tile.instance()
        Global.TileType.Flower:
            tile = Global.Resources.Flower.instance()
        Global.TileType.Pearl:
            #TODO
            pass

    tile.x = x
    tile.y = y

    # position is relative to other tiles on the board
    # and will be transformed according to the board's transform
    tile.position = Vector2(x * 1.5, y * 2 - 1.0 * (x % 2))

    return tile


func check_board():
    #TODO for clusters, just need to test the ones that moved;
    #     for pistils, just need to test their neighbors

    var clusters : Array = []
    var pistils : Array = []

    for x in Board.size():
        for y in Board[x].size():
            var tile : Tile = Board[x][y]

            if tile != null:
                if tile.is_cluster():
                    clusters.append(tile)
                elif tile.is_pistile():
                    pistils.append(tile)

    for tile in clusters:
        tile.safe_free()

    for tile in pistils:
        tile.safe_free()

        var neighborTop : Tile = tile.get_neighbor(Global.Dir.UpLeft)
        neighborTop.safe_free()
        tile.get_neighbor(Global.Dir.Up).safe_free()
        tile.get_neighbor(Global.Dir.UpRight).safe_free()
        tile.get_neighbor(Global.Dir.DownLeft).safe_free()
        tile.get_neighbor(Global.Dir.Down).safe_free()
        tile.get_neighbor(Global.Dir.DownRight).safe_free()

        var newTile : Tile = null

        if neighborTop.type == Global.TileType.Pearl:
            #TODO game over
            pass
        elif neighborTop.type == Global.TileType.Flower:
            newTile = create_tile(Global.TileType.Pearl, tile.x, tile.y)
        elif neighborTop.type == Global.TileType.Normal:
            newTile = create_tile(Global.TileType.Flower, tile.x, tile.y)

        Board[tile.x][tile.y] = newTile

        # add to scene tree
        add_child(newTile)


func _input(event):
    if cursorTile != null and event is InputEventMouseButton and event.pressed:
        if event.button_index == BUTTON_LEFT:
            cursorTile.spin(Global.SpinDir.AntiClockwise)
        elif event.button_index == BUTTON_RIGHT:
            cursorTile.spin(Global.SpinDir.Clockwise)

        #TODO we will need to check more than once when we implement "dropping" tiles
        check_board()
