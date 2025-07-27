using System;
using Sandbox;
using System.Collections.Generic;

public sealed class BackgroundScroller : Component
{
	[Property] public GameObject Camera;
	[Property] public Texture BackgroundTexture;

	[Property] public Vector2 TileSize = new( 115, 71 );
	[Property] public float GridOffsetX = 0.5f;
	private Vector3 StartPos;
	private const float TileSizeScalar = 1.615f;
	private int numTilesRendered = 0;

	private List<GameObject> tiles = new();

	protected override void OnStart()
	{

	
		if ( Camera == null )
			return;
		 StartPos = Camera.WorldPosition + new Vector3( GridOffsetX, 0, 0 );

		for ( int y = 0; y < 3; y++ )
		{
			var tile = new GameObject( "background_tile" );
			tile.Parent = GameObject;
			var renderer = tile.Components.Create<SpriteRenderer>();
			renderer.Texture = BackgroundTexture;
			renderer.Size = TileSize;
			renderer.Lighting = true;

			Vector3 tileOffset = new Vector3( 0, y * TileSize.y * TileSizeScalar, 0 );
			tile.WorldPosition = StartPos + tileOffset;
			numTilesRendered++;



			tiles.Add( tile );
		}
			

		
	}

	protected override void OnUpdate()
	{
		if ( Camera == null )
			return;
		foreach( var tile in tiles )
		{
			if ( tile == null )
				continue;
			bool isOffScreen = tile.WorldPosition.y < Camera.WorldPosition.y - (TileSize.y * TileSizeScalar);
			if(isOffScreen)
			{
				Vector3 tileOffset = new Vector3( 0, numTilesRendered * TileSize.y * TileSizeScalar, 0 );
				numTilesRendered++;
				tile.WorldPosition = StartPos + tileOffset;
			}
			
		}
	}
}
