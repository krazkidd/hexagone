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
				_cursorTile.ClearCursor(this);
			}

			value.IsCursor = true;
			value.SetCursorNeighbors(this);

			_cursorTile = value;
		}
	}

	public Tile[][] Board_;

	private readonly PackedScene tileScene = GD.Load<PackedScene>("res://scenes/tile.tscn");
	private readonly PackedScene flowerTileScene = GD.Load<PackedScene>("res://scenes/flower_tile.tscn");

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
					while (tile.IsCluster(this) || tile.IsPetal(this))
					{
						GD.PushWarning("New tile must be recreated.");

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
				tile = flowerTileScene.Instantiate<FlowerTile>();

				break;
			case TileType.Pearl:
				// TODO

				break;
		}

		tile.X = x;
		tile.Y = y;

		tile.MouseEntered += () => CursorTile = tile;

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

					if (tile.IsCluster(this))
					{
						clusters.Add(tile);
					}
					else if (tile.IsPistile(this))
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
			TileType type = tile.GetNeighbor(this, Dir.UpLeft).Type;

			Tile neighborTopLeft = tile.GetNeighbor(this, Dir.UpLeft);
			Tile neighborTop = tile.GetNeighbor(this, Dir.Up);
			Tile neighborTopRight = tile.GetNeighbor(this, Dir.UpRight);
			Tile neighborBottomRight = tile.GetNeighbor(this, Dir.DownRight);
			Tile neighborBottom = tile.GetNeighbor(this, Dir.Down);
			Tile neighborBottomLeft = tile.GetNeighbor(this, Dir.DownLeft);

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

	public override void _Input(InputEvent @event)
	{
		if (CursorTile != null)
		{
			if (@event.IsActionPressed("spin_left"))
			{
				CursorTile.Spin(this, SpinDir.AntiClockwise);
			}
			else if (@event.IsActionPressed("spin_right"))
			{
				CursorTile.Spin(this, SpinDir.Clockwise);
			}

			while (CheckBoard())
			{
				// just call check_board() again
			}
		}
	}

}
