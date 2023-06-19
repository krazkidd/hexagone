using Godot;
using System;

using Hexagone;

public partial class FlowerTile : Tile
{

	public override TileType Type { get; } = TileType.Flower;

	public override void SetCursorNeighbors(Board board)
	{
		//TODO how to handle edges?

		Tile tile = GetNeighbor(board, Dir.UpLeft);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(board, Dir.Up);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(board, Dir.UpRight);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(board, Dir.DownRight);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(board, Dir.Down);
		if (tile != null)
		{
			tile.IsCursor = true;
		}

		tile = GetNeighbor(board, Dir.DownLeft);
		if (tile != null)
		{
			tile.IsCursor = true;
		}
	}

	public override void Spin(Board board, SpinDir spinDir)
	{
		Tile neighborTopLeft = GetNeighbor(board, Dir.UpLeft);
		Tile neighborTop = GetNeighbor(board, Dir.Up);
		Tile neighborTopRight = GetNeighbor(board, Dir.UpRight);
		Tile neighborBottomRight = GetNeighbor(board, Dir.DownRight);
		Tile neighborBottom = GetNeighbor(board, Dir.Down);
		Tile neighborBottomLeft = GetNeighbor(board, Dir.DownLeft);

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

		board.Board_[neighborTopLeft.X][neighborTopLeft.Y] = neighborTopLeft;
		board.Board_[neighborTop.X][neighborTop.Y] = neighborTop;
		board.Board_[neighborTopRight.X][neighborTopRight.Y] = neighborTopRight;
		board.Board_[neighborBottomRight.X][neighborBottomRight.Y] = neighborBottomRight;
		board.Board_[neighborBottom.X][neighborBottom.Y] = neighborBottom;
		board.Board_[neighborBottomLeft.X][neighborBottomLeft.Y] = neighborBottomLeft;
	}

}
