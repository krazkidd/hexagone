using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Hexagone;

public partial class Board : Node2D
{

	private Tile _cursorTile;

	public Tile CursorTile
	{
		get
		{
			return _cursorTile;
		}
		set
		{
			if (IsInstanceValid(_cursorTile))
			{
				_cursorTile.ClearCursor();
			}

			_cursorTile = value;
		}
	}

	public Tile[][] Board_;

	private readonly PackedScene tileScene = GD.Load<PackedScene>("res://scenes/Tile.tscn");
	private readonly PackedScene flowerTileScene = GD.Load<PackedScene>("res://scenes/FlowerTile.tscn");

	public Board()
	{
		Board_ = new Tile[10][];

		foreach (int x in Enumerable.Range(0, Board_.Length))
		{
			Board_[x] = new Tile[9];

			foreach (int y in Enumerable.Range(0, Board_[x].Length))
			{
				Board_[x][y] = null;
			}
		}

		foreach (int x in Enumerable.Range(0, Board_.Length))
		{
			foreach (int y in Enumerable.Range(0, Board_[x].Length))
			{
				if (!IsEmptySpot(x, y))
				{
					Tile tile = CreateTile(TileType.Normal, x, y);

					// since we're always on an edge as we build the board,
					// new tiles cannot be the center of a flower
					while (tile.IsCluster() || tile.IsPetal())
					{
						tile.Free();

						tile = CreateTile(TileType.Normal, x, y);
					}

					Board_[x][y] = tile;

					AddChild(tile);
				}
				else
				{
					Board_[x][y] = null;
				}
			}
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Whether or not the given tile coordinates on the board is
	// *supposed* to be empty, because the board columns are staggered.
	public bool IsEmptySpot(int x, int y)
	{
		return (x % 2 == 0) && (y == 9 - 1);
	}

	public Tile CreateTile(TileType tiletype, int x, int y)
	{
		Tile tile = null;

		switch (tiletype)
		{
			case TileType.Normal:
				tile = tileScene.Instantiate<Tile>();

				break;
			case TileType.Flower:
				tile = flowerTileScene.Instantiate<Tile>();

				break;
			case TileType.Pearl:
				// TODO

				break;
		}

		tile.X = x;
		tile.Y = y;

		Area2D mouseArea = tile.GetNode<Area2D>("MouseArea");
		mouseArea.MouseEntered += () =>
		{
			CursorTile = tile;

			tile.IsCursor = true;

			tile.SetCursorNeighbors();
		};

		return tile;
	}

	public bool CheckBoard()
	{
		//TODO for clusters, just need to test the ones that moved;
		//     for pistils, just need to test their neighbors

		IList<Tile> clusters = new List<Tile>();
		IList<Tile> pistils = new List<Tile>();

		foreach (int x in Enumerable.Range(0, Board_.Length))
		{
			foreach (int y in Enumerable.Range(0, Board_[x].Length))
			{
				if (!IsEmptySpot(x, y))
				{
					Tile tile = Board_[x][y];

					if (tile.IsCluster())
					{
						clusters.Add(tile);
					}
					else if (tile.IsPistile())
					{
						pistils.Add(tile);
					}
				}
			}
		}

		foreach (Tile tile in clusters)
		{
			tile.SafeFree();

			Board_[tile.X][tile.Y] = null;
		}

		foreach (Tile tile in pistils)
		{
			tile.SafeFree();

			Board_[tile.X][tile.Y] = null;

			// save type
			TileType type = tile.GetNeighbor(Dir.UpLeft).Type;

			Tile neighborTopLeft = tile.GetNeighbor(Dir.UpLeft);
			Tile neighborTop = tile.GetNeighbor(Dir.Up);
			Tile neighborTopRight = tile.GetNeighbor(Dir.UpRight);
			Tile neighborBottomRight = tile.GetNeighbor(Dir.DownRight);
			Tile neighborBottom = tile.GetNeighbor(Dir.Down);
			Tile neighborBottomLeft = tile.GetNeighbor(Dir.DownLeft);

			neighborTopLeft.SafeFree();
			neighborTop.SafeFree();
			neighborTopRight.SafeFree();
			neighborBottomRight.SafeFree();
			neighborBottom.SafeFree();
			neighborBottomLeft.SafeFree();

			Board_[neighborTopLeft.X][neighborTopLeft.Y] = null;
			Board_[neighborTop.X][neighborTop.Y] = null;
			Board_[neighborTopRight.X][neighborTopRight.Y] = null;
			Board_[neighborBottomRight.X][neighborBottomRight.Y] = null;
			Board_[neighborBottom.X][neighborBottom.Y] = null;
			Board_[neighborBottomLeft.X][neighborBottomLeft.Y] = null;

			switch (type)
			{
				case TileType.Pearl:
					//TODO game over

					break;
				case TileType.Flower:
					Tile newTile = CreateTile(TileType.Pearl, tile.X, tile.Y);

					Board_[tile.X][tile.Y] = newTile;

					// add to scene tree
					AddChild(newTile);

					break;
				case TileType.Normal:
					Tile newTile2 = CreateTile(TileType.Flower, tile.X, tile.Y);

					Board_[tile.X][tile.Y] = newTile2;

					AddChild(newTile2);

					break;
			}
		}

		if (clusters.Any() || pistils.Any())
		{
			//TODO review this logic when not tired

			foreach (int x in Enumerable.Range(0, Board_.Length))
			{
				int ptrY = Board_[x].Length - 1;

				while (IsEmptySpot(x, ptrY))
				{
					ptrY -= 1;
				}

				for (int y = ptrY; y >= 0; y--)
				{
					while (Board_[x][ptrY] == null && ptrY >= 0)
					{
						ptrY -= 1;
					}

					if (y == ptrY)
					{
						ptrY -= 1;
					}
					else if (ptrY >= 0)
					{
						Board_[x][y] = Board_[x][ptrY];

						Board_[x][y].X = x;
						Board_[x][y].Y = y;
						Board_[x][y].Position = new Vector2(x * 1.5f, y * 2.0f - 1.0f * (x % 2));

						Board_[x][ptrY] = null;

						ptrY -= 1;
					}
					else
					{
						Tile tile = CreateTile(TileType.Normal, x, y);

						Board_[x][y] = tile;

						AddChild(tile);
					}
				}
			}

			return true;
		}

		return false;
	}

	//TODO is this actually being used?
	// public void _input(InputEvent event)
	// {
	// 	if (cursorTile != null && event.GetType() is InputEventMouseButton and event.pressed)
	// 	{
	// 		if (event.button_index == MOUSE_BUTTON_LEFT)
	// 		{
	// 			cursorTile.spin(Global.SpinDir.AntiClockwise);
	// 		}
	// 		else if (event.button_index == MOUSE_BUTTON_RIGHT)
	// 		{
	// 			cursorTile.spin(Global.SpinDir.Clockwise);
	// 		}

	// 		while (CheckBoard())
	// 		{
	// 			// just call check_board() again
	// 		}
	// 	}
	// }

}
