using Godot;
using System;

using Hexagone;

public partial class Tile : Node2D
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
			SetXY(value, _y);
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
			SetXY(_y, value);
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
			face.Color = value;
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

	public void SetXY(int xval, int yval)
	{
		Position = new Vector2(xval * 1.5f, yval * 2.0f - 1.0f * (xval % 2));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		face = GetNode<Polygon2D>("Face");
		back = GetNode<Polygon2D>("Back");

		Color = Hexagone.Color.GetColor((TileColor)(new Random().Next() % 6));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Tile GetNeighbor(Dir dir)
	{
		int otherX = X;
		int otherY = Y;

		if (dir == Dir.UpLeft || dir == Dir.DownLeft)
		{
			otherX -= 1;
		}
		else if (dir == Dir.UpRight || dir == Dir.DownRight)
		{
			otherX += 1;
		}

		if (X % 2 == 0)
		{
			if (dir == Dir.Up)
			{
				otherY -= 1;
			}
			else if (dir == Dir.DownLeft || dir == Dir.Down || dir == Dir.DownRight)
			{
				otherY += 1;
			}
		}
		else
		{
			if (dir == Dir.UpLeft || dir == Dir.Up || dir == Dir.UpRight)
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
		else if (otherX > Global.Board.Board_.Length - 1)
		{
			return null;
		}
		else if (otherY > Global.Board.Board_[otherX].Length - 1)
		{
			return null;
		}

		// might return null if x % 2 == 0
		return Global.Board.Board_[otherX][otherY];
	}

	private Dir FindNeighbor(Tile tile)
	{
		if (GetNeighbor(Dir.UpLeft) == tile)
		{
			return Dir.UpLeft;
		}
		else if (GetNeighbor(Dir.Up) == tile)
		{
			return Dir.Up;
		}
		else if (GetNeighbor(Dir.UpRight) == tile)
		{
			return Dir.UpRight;
		}
		else if (GetNeighbor(Dir.DownRight) == tile)
		{
			return Dir.DownRight;
		}
		else if (GetNeighbor(Dir.Down) == tile)
		{
			return Dir.Down;
		}
		else if (GetNeighbor(Dir.DownLeft) == tile)
		{
			return Dir.DownLeft;
		}

		return Dir.Unknown;
	}

	public bool IsNeighbor(Tile tile)
	{
		return FindNeighbor(tile) != Dir.Unknown;
	}

	//TODO can pass in startDir param and then have two loops or osmething
	public Tile GetFirstNeighbor()
	{
		Tile tile = GetNeighbor(Dir.Up);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(Dir.UpRight);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(Dir.DownRight);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(Dir.Down);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(Dir.DownLeft);
		if (tile != null)
		{
			return tile;
		}

		tile = GetNeighbor(Dir.UpLeft);
		if (tile != null)
		{
			return tile;
		}

		return null;
	}

	public Tile GetMutualNeighbor(Tile neighbor)
	{
		Tile tile = GetNeighbor(Dir.Up);
		if (tile != null && neighbor.IsNeighbor(tile))
		{
			return tile;
		}

		tile = GetNeighbor(Dir.UpRight);
		if (tile != null && neighbor.IsNeighbor(tile))
		{
			return tile;
		}

		tile = GetNeighbor(Dir.DownRight);
		if (tile != null && neighbor.IsNeighbor(tile))
		{
			return tile;
		}

		tile = GetNeighbor(Dir.Down);
		if (tile != null && neighbor.IsNeighbor(tile))
		{
			return tile;
		}

		tile = GetNeighbor(Dir.DownLeft);
		if (tile != null && neighbor.IsNeighbor(tile))
		{
			return tile;
		}

		tile = GetNeighbor(Dir.UpLeft);
		if (tile != null && neighbor.IsNeighbor(tile))
		{
			return tile;
		}

		return null;
	}

	// Checks if this tile is a member of a cluster, where at least
	// two adjacent neighbors are of the same color as this tile.
	public bool IsCluster()
	{
		Tile neighborTopLeft = GetNeighbor(Dir.UpLeft);
		Tile neighborTop = GetNeighbor(Dir.Up);
		Tile neighborTopRight = GetNeighbor(Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(Dir.DownRight);
		Tile neighborBottom = GetNeighbor(Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(Dir.DownLeft);

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
	public bool IsPistile()
	{
		Tile neighborTopLeft = GetNeighbor(Dir.UpLeft);
		Tile neighborTop = GetNeighbor(Dir.Up);
		Tile neighborTopRight = GetNeighbor(Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(Dir.DownRight);
		Tile neighborBottom = GetNeighbor(Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(Dir.DownLeft);

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
	public bool IsPetal()
	{
		Tile neighborTopLeft = GetNeighbor(Dir.UpLeft);
		Tile neighborTop = GetNeighbor(Dir.Up);
		Tile neighborTopRight = GetNeighbor(Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(Dir.DownRight);
		Tile neighborBottom = GetNeighbor(Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(Dir.DownLeft);

		// we compare the colors as a shortcut--so we don't have to call .is_pistile()
		return ((neighborTopLeft != null && Color != neighborTopLeft.Color && neighborTopLeft.IsPistile())
			|| (neighborTop != null && Color != neighborTop.Color && neighborTop.IsPistile())
			|| (neighborTopRight != null && Color != neighborTopRight.Color && neighborTopRight.IsPistile())
			|| (neighborBottomRight != null && Color != neighborBottomRight.Color && neighborBottomRight.IsPistile())
			|| (neighborBottom != null && Color != neighborBottom.Color && neighborBottom.IsPistile())
			|| (neighborBottomLeft != null && Color != neighborBottomLeft.Color && neighborBottomLeft.IsPistile()));
	}

	public virtual void SetCursorNeighbors()
	{
		Vector2 pos = GetViewport().GetMousePosition();

		Tile nearest1 = GetFirstNeighbor();
		Tile nearest2 = null;

		Tile tile = GetNeighbor(Dir.Up);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(Dir.UpRight);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(Dir.DownRight);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(Dir.Down);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(Dir.DownLeft);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		tile = GetNeighbor(Dir.UpLeft);
		if (tile != null)
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest1.GlobalPosition - pos).Length())
			{
				nearest1 = tile;
			}
		}

		///////////////////////////////////////////////////////////////

		nearest2 = GetMutualNeighbor(nearest1);

		tile = GetNeighbor(Dir.Up);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(Dir.UpRight);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(Dir.DownRight);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(Dir.Down);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(Dir.DownLeft);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		tile = GetNeighbor(Dir.UpLeft);
		if (tile != null && tile != nearest1 && nearest1.IsNeighbor(tile))
		{
			if ((tile.GlobalPosition - pos).Length() < (nearest2.GlobalPosition - pos).Length())
			{
				nearest2 = tile;
			}
		}

		nearest1.IsCursor = true;
		nearest2.IsCursor = true;
	}

	public void ClearCursor()
	{
		IsCursor = false;

		Tile tile = GetNeighbor(Dir.Up);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(Dir.UpRight);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(Dir.DownRight);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(Dir.Down);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(Dir.DownLeft);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}

		tile = GetNeighbor(Dir.UpLeft);
		if (tile != null && tile.IsCursor)
		{
			tile.IsCursor = false;
		}
	}

	public virtual void Spin(SpinDir spinDir)
	{
		Tile neighborTopLeft = GetNeighbor(Dir.UpLeft);
		Tile neighborTop = GetNeighbor(Dir.Up);
		Tile neighborTopRight = GetNeighbor(Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(Dir.DownRight);
		Tile neighborBottom = GetNeighbor(Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(Dir.DownLeft);

		if ((neighborTopLeft != null && neighborTopLeft.IsCursor)
			&& neighborTop != null && neighborTop.IsCursor)
		{
			_spin(this, neighborTopLeft, neighborTop, spinDir);
		}
		else if ((neighborTop != null && neighborTop.IsCursor)
			&& neighborTopRight != null && neighborTopRight.IsCursor)
		{
			_spin(this, neighborTop, neighborTopRight, spinDir);
		}
		else if ((neighborTopRight != null && neighborTopRight.IsCursor)
			&& neighborBottomRight != null && neighborBottomRight.IsCursor)
		{
			_spin(this, neighborTopRight, neighborBottomRight, spinDir);
		}
		else if ((neighborBottomRight != null && neighborBottomRight.IsCursor)
			&& neighborBottom != null && neighborBottom.IsCursor)
		{
			_spin(this, neighborBottomRight, neighborBottom, spinDir);
		}
		else if ((neighborBottom != null && neighborBottom.IsCursor)
			&& neighborBottomLeft != null && neighborBottomLeft.IsCursor)
		{
			_spin(this, neighborBottom, neighborBottomLeft, spinDir);
		}
		else if ((neighborBottomLeft != null && neighborBottomLeft.IsCursor)
			&& neighborTopLeft != null && neighborTopLeft.IsCursor)
		{
			_spin(this, neighborBottomLeft, neighborTopLeft, spinDir);
		}
	}

	private void _spin(Tile tile1, Tile tile2, Tile tile3, SpinDir spinDir)
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

		Global.Board.Board_[tile1.X][tile1.Y] = tile1;
		Global.Board.Board_[tile2.X][tile2.Y] = tile2;
		Global.Board.Board_[tile3.X][tile3.Y] = tile3;
	}

	public void SafeFree()
	{
		if (!IsQueuedForDeletion())
		{
			QueueFree();
		}
	}

}
