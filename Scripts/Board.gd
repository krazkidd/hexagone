extends Node2D

class_name Board

var cursorTile : Tile: set = set_cursor_tile

var Board : Array


func set_cursor_tile(value : Tile):
    if is_instance_valid(cursorTile):
        cursorTile.clear_cursor()

    cursorTile = value


func _init():
    Global.Board = self

    Board = []
    Board.resize(10) #10

    for x in range(Board.size()):
        Board[x] = []
        Board[x].resize(9) #9

        for y in range(Board[x].size()):
            Board[x][y] = null

    for x in range(Board.size()):
        for y in range(Board[x].size()):
            if not is_empty_spot(x, y):
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


#
# Whether or not the given tile coordinates on the board is
# *supposed* to be empty, because the board columns are staggered.
#
func is_empty_spot(x : int, y : int) -> bool:
    return (x % 2 == 0) and (y == 9 - 1)


func create_tile(tiletype, x : int, y : int):
    var tile : Tile = null

    match tiletype:
        Global.TileType.Normal:
            tile = Global.Resources.Tile.instantiate()
        Global.TileType.Flower:
            tile = Global.Resources.Flower.instantiate()
        Global.TileType.Pearl:
            #TODO
            pass

    tile.x = x
    tile.y = y

    return tile


func check_board() -> bool:
    #TODO for clusters, just need to test the ones that moved;
    #     for pistils, just need to test their neighbors

    var clusters : Array = []
    var pistils : Array = []

    for x in range(Board.size()):
        for y in range(Board[x].size()):
            if not is_empty_spot(x, y):
                var tile : Tile = Board[x][y]

                if tile.is_cluster():
                    clusters.append(tile)
                elif tile.is_pistile():
                    pistils.append(tile)

    for tile in clusters:
        tile.safe_free()

        Board[tile.x][tile.y] = null

    for tile in pistils:
        tile.safe_free()

        Board[tile.x][tile.y] = null

        # save type
        var type = tile.get_neighbor(Global.Dir.UpLeft).type

        var neighborTopLeft : Tile = tile.get_neighbor(Global.Dir.UpLeft)
        var neighborTop : Tile = tile.get_neighbor(Global.Dir.Up)
        var neighborTopRight : Tile = tile.get_neighbor(Global.Dir.UpRight)
        var neighborBottomRight : Tile = tile.get_neighbor(Global.Dir.DownRight)
        var neighborBottom : Tile = tile.get_neighbor(Global.Dir.Down)
        var neighborBottomLeft : Tile = tile.get_neighbor(Global.Dir.DownLeft)

        neighborTopLeft.safe_free()
        neighborTop.safe_free()
        neighborTopRight.safe_free()
        neighborBottomRight.safe_free()
        neighborBottom.safe_free()
        neighborBottomLeft.safe_free()

        Board[neighborTopLeft.x][neighborTopLeft.y] = null
        Board[neighborTop.x][neighborTop.y] = null
        Board[neighborTopRight.x][neighborTopRight.y] = null
        Board[neighborBottomRight.x][neighborBottomRight.y] = null
        Board[neighborBottom.x][neighborBottom.y] = null
        Board[neighborBottomLeft.x][neighborBottomLeft.y] = null

        match type:
            Global.TileType.Pearl:
                #TODO game over
                pass

                break;
            Global.TileType.Flower:
                var newTile : Tile = create_tile(Global.TileType.Pearl, tile.x, tile.y)

                Board[tile.x][tile.y] = newTile

                # add to scene tree
                add_child(newTile)

                break;
            Global.TileType.Normal:
                var newTile : Tile = create_tile(Global.TileType.Flower, tile.x, tile.y)

                Board[tile.x][tile.y] = newTile

                # add to scene tree
                add_child(newTile)

                break;

    if clusters.size() > 0 or pistils.size() > 0:
        #TODO review this logic when not tired

        for x in range(Board.size()):
            var ptrY : int = Board[x].size() - 1

            while is_empty_spot(x, ptrY):
                ptrY -= 1

            for y in range(ptrY, -1, -1):
                while Board[x][ptrY] == null and ptrY >= 0:
                    ptrY -= 1

                if y == ptrY:
                    ptrY -= 1
                elif ptrY >= 0:
                    Board[x][y] = Board[x][ptrY]

                    Board[x][y].x = x
                    Board[x][y].y = y
                    Board[x][y].position = Vector2(x * 1.5, y * 2 - 1.0 * (x % 2))

                    Board[x][ptrY] = null

                    ptrY -= 1
                else:
                    var tile : Tile = create_tile(Global.TileType.Normal, x, y)

                    Board[x][y] = tile

                    # add to scene tree
                    add_child(tile)

        return true

    return false


func _input(event):
    if cursorTile != null and event is InputEventMouseButton and event.pressed:
        if event.button_index == MOUSE_BUTTON_LEFT:
            cursorTile.spin(Global.SpinDir.AntiClockwise)
        elif event.button_index == MOUSE_BUTTON_RIGHT:
            cursorTile.spin(Global.SpinDir.Clockwise)

        while check_board():
            # just call check_board() again
            pass
