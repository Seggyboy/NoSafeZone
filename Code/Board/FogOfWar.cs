using System;
using System.Collections.Generic;
using Sandbox;

public sealed class FogOfWar : Component
{
	[Property] private Board board;
	[Property] private BattleMaster battleMaster;

	[Property] private HashSet<Tile> previouslyVisible = new();

	protected override void OnEnabled()
	{
		
		board = Scene.GetAllComponents<Board>().FirstOrDefault();
		battleMaster.LevelLoadedEvent += InitializeVisibility;
		battleMaster.StartPlayerTurnEvent += SetVisibility;
		

	}


	public void InitializeVisibility()
	{
		foreach ( var tile in board.World[0] )
		{
			tile.SetFlag( TileFlags.Visible, false );
			tile.SetFlag( TileFlags.Explored, false );

			var tileRenderer = tile.GetComponent<ModelRenderer>();
			var unitRenderer = tile.unit?.GetComponent<ModelRenderer>();
			var tileDetail = tile.tileDetail?.GetComponent<ModelRenderer>();

			if ( tileRenderer != null )
				tileRenderer.Tint = Color.Black;

			if ( unitRenderer != null )
				unitRenderer.Tint = Color.Transparent;

			if ( tileDetail != null )
				tileDetail.Tint = Color.Transparent;
		}
		previouslyVisible.Clear();
	}
	public void ClearVisibility()
	{
		foreach ( var tile in previouslyVisible )
		{
			tile.SetFlag( TileFlags.Visible, false );

			var tileRenderer = tile.GetComponent<ModelRenderer>();
			var unitRenderer = tile.unit != null ? tile.unit.GetComponent<ModelRenderer>() : null;
			var tileDetailRenderer = tile.tileDetail != null ? tile.tileDetail.GetComponent<ModelRenderer>() : null;

			if ( tileRenderer != null )
				tileRenderer.Tint = tile.Explored ? Color.Gray : Color.Black;

			if ( unitRenderer != null )
				unitRenderer.Tint = Color.Transparent;

			if ( tileDetailRenderer != null )
				tileDetailRenderer.Tint = tile.Explored ? Color.White : Color.Transparent;
		}
		previouslyVisible.Clear();
	}

	public void SetVisibility()
	{
		//Log.Info( "SetVisibility called" );
		ClearVisibility();

		var playerUnits = BattleMaster.Instance?.PlayerUnitsList;
		if ( playerUnits != null )
		{
			foreach ( var go in playerUnits )
			{
				var unit = go.GetComponent<Unit>();
				if ( unit?.CurrentTile == null ) continue;
				ComputeFOV( unit, unit.VisionRadius, unit.FOV );
			}
		}
	}

	private void ComputeFOV( Unit unit, int visionRadius, float fovDegrees )
	{
		Vector2 unitPos = new Vector2( unit.CurrentTile.X, unit.CurrentTile.Y );
		Vector2 forward = new Vector2( unit.Forward.x, unit.Forward.y ).Normal;
		float halfFOV = fovDegrees / 2f;
		int rayCount = 220; // Reduced from 220 for performance

		for ( int i = 0; i < rayCount; i++ )
		{
			float angle = -halfFOV + (fovDegrees * i / (rayCount - 1));
			Vector2 dir = Rotate( forward, angle ).Normal;
			Vector2 end = unitPos + dir * visionRadius;

			int endX = Math.Clamp( (int)MathF.Round( end.x ), 0, board.Width - 1 );
			int endY = Math.Clamp( (int)MathF.Round( end.y ), 0, board.Length - 1 );

			CastRay( (int)unitPos.x, (int)unitPos.y, endX, endY, unit );
		}
	}

	private void CastRay( int x0, int y0, int x1, int y1, Unit unit )
	{
		foreach ( var tile in BresenhamLine( x0, y0, x1, y1 ) )
		{
			previouslyVisible.Add( tile );
			if ( !tile.HasFlag( TileFlags.Visible ) )
			{
				tile.SetFlag( TileFlags.Visible, true );
				tile.SetFlag( TileFlags.Explored, true );
				//Log.Info( $"Tile ({tile.X},{tile.Y}) marked visible" );


				var tileRenderer = tile.GetComponent<ModelRenderer>();
				var unitRenderer = tile.unit != null ? tile.unit.GetComponent<ModelRenderer>() : null;
				var tileDetailRenderer = tile.tileDetail != null ? tile.tileDetail.GetComponent<ModelRenderer>() : null;

				if ( tileRenderer != null )
					tileRenderer.Tint = Color.White;

				if ( tile.unit != null && unitRenderer != null )
					unitRenderer.Tint = tile.unit.Team == UnitTeam.Player ? Color.White : Color.Green;

				if ( tileDetailRenderer != null )
					tileDetailRenderer.Tint = Color.White;





			}

			if ( tile.BlocksVision )
				break;
		}
	}

	private IEnumerable<Tile> BresenhamLine( int x0, int y0, int x1, int y1 )
	{
		int dx = Math.Abs( x1 - x0 );
		int dy = Math.Abs( y1 - y0 );
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx - dy;

		while ( true )
		{
			var tile = board.GetTile( x0, y0 );
			if ( tile is null ) yield break;

			yield return tile;

			if ( x0 == x1 && y0 == y1 )
				break;

			int e2 = 2 * err;
			if ( e2 > -dy ) { err -= dy; x0 += sx; }
			if ( e2 < dx ) { err += dx; y0 += sy; }
		}
	}

	public static Vector2 Rotate( Vector2 v, float degrees )
	{
		float radians = MathF.PI / 180f * degrees;
		float cos = MathF.Cos( radians );
		float sin = MathF.Sin( radians );
		return new Vector2(
			v.x * cos - v.y * sin,
			v.x * sin + v.y * cos
		);
	}
}
