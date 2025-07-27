public static class TileUtils
{
	public static bool IsBlockedByWall( Tile from, Tile to )
	{
		int dx = to.X - from.X;
		int dy = to.Y - from.Y;

		// Moving right
		if ( dx == 1 )
		{
			bool wall = from.HasFlag( TileFlags.EastWall ) || to.HasFlag( TileFlags.WestWall );
			bool door = from.HasFlag( TileFlags.Door ) || to.HasFlag(TileFlags.Door);
			if ( wall && !door ) return true;
		}
		// Moving left
		else if ( dx == -1 )
		{
			bool wall = from.HasFlag( TileFlags.WestWall ) || to.HasFlag( TileFlags.EastWall );
			bool door = from.HasFlag( TileFlags.Door ) || to.HasFlag( TileFlags.Door );
			if ( wall && !door ) return true;
		}
		// Moving down
		else if ( dy == 1 )
		{
			bool wall = from.HasFlag( TileFlags.SouthWall ) || to.HasFlag( TileFlags.NorthWall );
			bool door = from.HasFlag( TileFlags.Door ) || to.HasFlag( TileFlags.Door );
			if ( wall && !door ) return true;
		}
		// Moving up
		else if ( dy == -1 )
		{
			bool wall = from.HasFlag( TileFlags.NorthWall ) || to.HasFlag( TileFlags.SouthWall );
			bool door = from.HasFlag( TileFlags.Door ) || to.HasFlag( TileFlags.Door );
			if ( wall && !door ) return true;
		}

		return false;
	}
}
