using Godot;
using System;

using Hexagone;

public partial class FlowerTile : Tile
{

	public override TileType Type { get; } = TileType.Flower;

	public override void SetCursorNeighbors()
	{
		//TODO how to handle edges?

		Tile tile = GetNeighbor(Dir.UpLeft);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(Dir.Up);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(Dir.UpRight);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(Dir.DownRight);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(Dir.Down);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(Dir.DownLeft);
		if (tile != null)
		{
			tile.IsCursor = true;
		}
	}

	public override void Spin(SpinDir spinDir)
	{
		Tile neighborTopLeft = GetNeighbor(Dir.UpLeft);
		Tile neighborTop = GetNeighbor(Dir.Up);
		Tile neighborTopRight = GetNeighbor(Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(Dir.DownRight);
		Tile neighborBottom = GetNeighbor(Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(Dir.DownLeft);

		int neighborTopLeftx = neighborTopLeft.X;
		int neighborTopLefty = neighborTopLeft.Y;

		if (spinDir == SpinDir.Clockwise)
		{
			neighborTopLeft.SetXY(neighborTop.X, neighborTop.Y);
			neighborTop.SetXY(neighborTopRight.X, neighborTopRight.Y);
			neighborTopRight.SetXY(neighborBottomRight.X, neighborBottomRight.Y);
			neighborBottomRight.SetXY(neighborBottom.X, neighborBottom.Y);
			neighborBottom.SetXY(neighborBottomLeft.X, neighborBottomLeft.Y);
			neighborBottomLeft.SetXY(neighborTopLeftx, neighborTopLefty);
		}
		else
		{
			neighborTopLeft.SetXY(neighborBottomLeft.X, neighborBottomLeft.Y);
			neighborBottomLeft.SetXY(neighborBottom.X, neighborBottom.Y);
			neighborBottom.SetXY(neighborBottomRight.X, neighborBottomRight.Y);
			neighborBottomRight.SetXY(neighborTopRight.X, neighborTopRight.Y);
			neighborTopRight.SetXY(neighborTop.X, neighborTop.Y);
			neighborTop.SetXY(neighborTopLeftx, neighborTopLefty);
		}

		Global.Board.Board_[neighborTopLeft.X][neighborTopLeft.Y] = neighborTopLeft;
		Global.Board.Board_[neighborTop.X][neighborTop.Y] = neighborTop;
		Global.Board.Board_[neighborTopRight.X][neighborTopRight.Y] = neighborTopRight;
		Global.Board.Board_[neighborBottomRight.X][neighborBottomRight.Y] = neighborBottomRight;
		Global.Board.Board_[neighborBottom.X][neighborBottom.Y] = neighborBottom;
		Global.Board.Board_[neighborBottomLeft.X][neighborBottomLeft.Y] = neighborBottomLeft;
	}

}
