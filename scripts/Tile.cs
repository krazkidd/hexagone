using Godot;
using System;

using Hexagone;

public partial class Tile : Area2D
{

	private int _x;
	private int _y;
	private Godot.Color _color;

	public int X
	{
		get
		{
			return _x;
		}
		set
		{
			_x = value;

			SetXY(_x, _y);
		}
	}

	public int Y
	{
		get
		{
			return _y;
		}
		set
		{
			_y = value;

			SetXY(_x, _y);
		}
	}

	public virtual TileType Type { get; } = TileType.Normal;

	public Godot.Color Color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;

			if (face != null)
			{
				face.Color = value;
			}
		}
	}

	public bool IsCursor
	{
		get
		{
			return back.Visible;
		}
		set
		{
			back.Visible = value;
		}
	}

	private Polygon2D face;
	private Polygon2D back;

    public void SetXY(int xval, int yval) => 
		Position = new Vector2(xval * 75.0f, yval * 100.0f - 50.0f * (xval % 2));

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		face = GetNode<Polygon2D>("Face");
		back = GetNode<Polygon2D>("Back");

		face.Color = Color;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public Tile GetNeighbor(Board board, Dir dir)
	{
		int otherX = X;
		int otherY = Y;

		if (dir is Dir.UpLeft or Dir.DownLeft)
		{
			otherX -= 1;
		}
		else if (dir is Dir.UpRight or Dir.DownRight)
		{
			otherX += 1;
		}

		if (X % 2 == 0)
		{
			if (dir == Dir.Up)
			{
				otherY -= 1;
			}
			else if (dir is Dir.DownLeft or Dir.Down or Dir.DownRight)
			{
				otherY += 1;
			}
		}
		else
		{
			if (dir is Dir.UpLeft or Dir.Up or Dir.UpRight)
			{
				otherY -= 1;
			}
			else if (dir == Dir.Down)
			{
				otherY += 1;
			}
		}

		if (otherX < 0 || otherY < 0)
		{
			return null;
		}
		else if (otherX > board.Board_.Length - 1)
		{
			return null;
		}
		else if (otherY > board.Board_[otherX].Length - 1)
		{
			return null;
		}

		// might return null if x % 2 == 0
		return board.Board_[otherX][otherY];
	}

	private Dir FindNeighbor(Board board, Tile tile)
	{
		if (GetNeighbor(board, Dir.UpLeft) == tile)
		{
			return Dir.UpLeft;
		}
		else if (GetNeighbor(board, Dir.Up) == tile)
		{
			return Dir.Up;
		}
		else if (GetNeighbor(board, Dir.UpRight) == tile)
		{
			return Dir.UpRight;
		}
		else if (GetNeighbor(board, Dir.DownRight) == tile)
		{
			return Dir.DownRight;
		}
		else if (GetNeighbor(board, Dir.Down) == tile)
		{
			return Dir.Down;
		}
		else if (GetNeighbor(board, Dir.DownLeft) == tile)
		{
			return Dir.DownLeft;
		}

		return Dir.Unknown;
	}

    public bool IsNeighbor(Board board, Tile tile) => 
		FindNeighbor(board, tile) != Dir.Unknown;

    //TODO can pass in startDir param and then have two loops or osmething
    public Tile GetFirstNeighbor(Board board)
	{
		Tile tile = GetNeighbor(board, Dir.Up);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.UpRight);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.DownRight);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.Down);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.DownLeft);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.UpLeft);
		if (tile != null)
		{
			return tile;
		}

		return null;
	}

	public Tile GetMutualNeighbor(Board board, Tile neighbor)
	{
		Tile tile = GetNeighbor(board, Dir.Up);
		if (tile != null && neighbor.IsNeighbor(board, tile))
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.UpRight);
		if (tile != null && neighbor.IsNeighbor(board, tile))
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.DownRight);
		if (tile != null && neighbor.IsNeighbor(board, tile))
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.Down);
		if (tile != null && neighbor.IsNeighbor(board, tile))
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.DownLeft);
		if (tile != null && neighbor.IsNeighbor(board, tile))
		{
			return tile;
		}

		tile = GetNeighbor(board, Dir.UpLeft);
		if (tile != null && neighbor.IsNeighbor(board, tile))
		{
			return tile;
		}

		return null;
	}

	// Checks if this tile is a member of a cluster, where at least
	// two adjacent neighbors are of the same color as this tile.
	public bool IsCluster(Board board)
	{
		Tile neighborTopLeft = GetNeighbor(board, Dir.UpLeft);
		Tile neighborTop = GetNeighbor(board, Dir.Up);
		Tile neighborTopRight = GetNeighbor(board, Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(board, Dir.DownRight);
		Tile neighborBottom = GetNeighbor(board, Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(board, Dir.DownLeft);

		return (neighborTopLeft != null && neighborTop != null && Color == neighborTopLeft.Color && Color == neighborTop.Color
			|| neighborTop != null && neighborTopRight != null && Color == neighborTop.Color && Color == neighborTopRight.Color
			|| neighborTopRight != null && neighborBottomRight != null && Color == neighborTopRight.Color && Color == neighborBottomRight.Color
			|| neighborBottomRight != null && neighborBottom != null && Color == neighborBottomRight.Color && Color == neighborBottom.Color
			|| neighborBottom != null && neighborBottomLeft != null && Color == neighborBottom.Color && Color == neighborBottomLeft.Color
			|| neighborBottomLeft != null && neighborTopLeft != null && Color == neighborBottomLeft.Color && Color == neighborTopLeft.Color);
	}

	// Checks if this tile is the center of a flower, where every neighbor
	// is of the same color but different from the color of this tile.
	//
	// A pistil is the structure of the gynoecium, or the female
	// reproductive organ.
	public bool IsPistile(Board board)
	{
		Tile neighborTopLeft = GetNeighbor(board, Dir.UpLeft);
		Tile neighborTop = GetNeighbor(board, Dir.Up);
		Tile neighborTopRight = GetNeighbor(board, Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(board, Dir.DownRight);
		Tile neighborBottom = GetNeighbor(board, Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(board, Dir.DownLeft);

		return (neighborTopLeft != null && neighborTop != null && neighborTopRight != null
			&& neighborBottomLeft != null && neighborBottom != null && neighborBottomRight != null
			&& neighborTopLeft.Color != Color
			&& neighborTopLeft.Color == neighborTop.Color
			&& neighborTopLeft.Color == neighborTopRight.Color
			&& neighborTopLeft.Color == neighborBottomRight.Color
			&& neighborTopLeft.Color == neighborBottom.Color
			&& neighborTopLeft.Color == neighborBottomLeft.Color);
	}

	// Checks if this tile is the outer ring of a flower, where a neighboring
	// tile returns is_pistile() == true.
	//
	// A petal is a part of the corolla--a reproductive whorl.
	public bool IsPetal(Board board)
	{
		Tile neighborTopLeft = GetNeighbor(board, Dir.UpLeft);
		Tile neighborTop = GetNeighbor(board, Dir.Up);
		Tile neighborTopRight = GetNeighbor(board,  Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(board, Dir.DownRight);
		Tile neighborBottom = GetNeighbor(board, Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(board, Dir.DownLeft);

		// we compare the colors as a shortcut--so we don't have to call .is_pistile()
		return ((neighborTopLeft != null && Color != neighborTopLeft.Color && neighborTopLeft.IsPistile(board))
			|| (neighborTop != null && Color != neighborTop.Color && neighborTop.IsPistile(board))
			|| (neighborTopRight != null && Color != neighborTopRight.Color && neighborTopRight.IsPistile(board))
			|| (neighborBottomRight != null && Color != neighborBottomRight.Color && neighborBottomRight.IsPistile(board))
			|| (neighborBottom != null && Color != neighborBottom.Color && neighborBottom.IsPistile(board))
			|| (neighborBottomLeft != null && Color != neighborBottomLeft.Color && neighborBottomLeft.IsPistile(board)));
	}

	public virtual void SetCursorNeighbors(Board board)
	{
		Vector2 pos = GetViewport().GetMousePosition();

		Tile nearest1 = GetFirstNeighbor(board);
		Tile nearest2 = null;

		Tile tile = GetNeighbor(board, Dir.Up);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.UpRight);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.DownRight);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.Down);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.DownLeft);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.UpLeft);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		///////////////////////////////////////////////////////////////

		nearest2 = GetMutualNeighbor(board, nearest1);

		tile = GetNeighbor(board, Dir.Up);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(board, tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.UpRight);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(board, tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.DownRight);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(board, tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.Down);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(board, tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.DownLeft);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(board, tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(board, Dir.UpLeft);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(board, tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		nearest1.IsCursor = true;
		nearest2.IsCursor = true;
	}

	public void ClearCursor(Board board)
	{
		IsCursor = false;

		Tile tile = GetNeighbor(board, Dir.Up);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(board, Dir.UpRight);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(board, Dir.DownRight);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(board, Dir.Down);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(board, Dir.DownLeft);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(board, Dir.UpLeft);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}
	}
	public virtual void Spin(Board board, SpinDir spinDir)
	{
		Tile neighborTopLeft = GetNeighbor(board, Dir.UpLeft);
		Tile neighborTop = GetNeighbor(board, Dir.Up);
		Tile neighborTopRight = GetNeighbor(board, Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(board, Dir.DownRight);
		Tile neighborBottom = GetNeighbor(board, Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(board, Dir.DownLeft);

		if ((neighborTopLeft != null && neighborTopLeft.IsCursor)
			&& neighborTop != null && neighborTop.IsCursor)
		{
			_spin(board, this, neighborTopLeft, neighborTop, spinDir);
		}
		else if ((neighborTop != null && neighborTop.IsCursor)
			&& neighborTopRight != null && neighborTopRight.IsCursor)
		{
			_spin(board, this, neighborTop, neighborTopRight, spinDir);
		}
		else if ((neighborTopRight != null && neighborTopRight.IsCursor)
			&& neighborBottomRight != null && neighborBottomRight.IsCursor)
		{
			_spin(board, this, neighborTopRight, neighborBottomRight, spinDir);
		}
		else if ((neighborBottomRight != null && neighborBottomRight.IsCursor)
			&& neighborBottom != null && neighborBottom.IsCursor)
		{
			_spin(board, this, neighborBottomRight, neighborBottom, spinDir);
		}
		else if ((neighborBottom != null && neighborBottom.IsCursor)
			&& neighborBottomLeft != null && neighborBottomLeft.IsCursor)
		{
			_spin(board, this, neighborBottom, neighborBottomLeft, spinDir);
		}
		else if ((neighborBottomLeft != null && neighborBottomLeft.IsCursor)
			&& neighborTopLeft != null && neighborTopLeft.IsCursor)
		{
			_spin(board, this, neighborBottomLeft, neighborTopLeft, spinDir);
		}
	}

	private void _spin(Board board, Tile tile1, Tile tile2, Tile tile3, SpinDir spinDir)
	{
		int tile1x = tile1.X;
		int tile1y = tile1.Y;

		if (spinDir == SpinDir.Clockwise)
		{
			tile1.SetXY(tile2.X, tile2.Y);
			tile2.SetXY(tile3.X, tile3.Y);
			tile3.SetXY(tile1x, tile1y);
		}
		else
		{
			tile1.SetXY(tile3.X, tile3.Y);
			tile3.SetXY(tile2.X, tile2.Y);
			tile2.SetXY(tile1x, tile1y);
		}

		board.Board_[tile1.X][tile1.Y] = tile1;
		board.Board_[tile2.X][tile2.Y] = tile2;
		board.Board_[tile3.X][tile3.Y] = tile3;
	}

	public void SafeFree()
	{
		if (!IsQueuedForDeletion())
		{
			QueueFree();
		}
	}

}
