using System;
using Sandbox;
using System.Collections.Generic;
using System.Collections;

public sealed class BackgroundScrollerParallax : Component
{
	// Initialize three cells of parallax background with different heights and scroll factors

	[Property] public GameObject Camera;
	[Property] public Texture BackgroundTexture1;
	[Property] public Texture BackgroundTexture2;
	[Property] public Texture BackgroundTexture3;
	[Property] public bool Moving = false; // Whether the background should scroll with the camera movement
	[Property] public int XOffset = 50; // offset in the x direction for each background layer
	[Property] public int CameraDistance = 207;
	[Property] public Dictionary<int, List<GameObject>> BackgroundLayers = new(); // index correlates with background texture num.

	List<float> tileOrigins = new()
	{
		-30, // 
		-10.8f,  // 
		29f  // Origin (bottom)
	};

	List<float> tileHeights = new()     {
		35f, // Height of tile 0
		10f, // Height of tile 1
		81f  // Height of tile 2
	};

	[Property] private List<float> layerScrollFactors = new() { 1.0f, 0.7f, -2f }; // 0 road, 1 dirt, 2 mountains


	// We'll scroll 0 and 1 back against the camera to slow down how fast it appears to be moving, keep 0 static. We'll run the snapping function seperately on each layer.

	[Property] public Vector3 TileSize = new( 250, 71, 0 );
	[Property] public float GridOffsetX = 15f;

	private Vector3 StartPos;
	private const float TileSizeScalar = 1.615f;



	

	protected override void OnStart()
	{
		
		TravelerEvents.ConvoyMovingEvent += StartMoving;

		TravelerEvents.ConvoyStopEvent += StopMoving;
		

		if ( Camera == null )
			return;
		StartPos = new Vector3( 100, -200, 0); // World-space position, not tied to camera


		for (int i = 0; i < 3; i++ )
		{
			
			var tileList = new List<GameObject>();
			for (int x = 0; x<3;  x++ )
			{
				var go = new GameObject( $"background_layer_{i}" );
				var rend = go.Components.Create<SpriteRenderer>();
				go.Parent = this.GameObject;
				rend.Texture = i switch
				{
					0 => BackgroundTexture1,
					1 => BackgroundTexture2,
					2 => BackgroundTexture3,
					_ => null
				};
				rend.Size = new Vector2( TileSize.x, tileHeights[i] );
				rend.Lighting = true;


				Vector3 basePos = StartPos + new Vector3(XOffset * i, x* 200, tileOrigins[i] );

				go.WorldPosition = basePos;
				tileList.Add( go );

			}
			
			BackgroundLayers.Add( i, tileList );
			

		}
		GameObject.WorldPosition = new Vector3( CameraDistance, 0, 0 ); // Set the world position of this component
	}

	public void StopMoving()
	{
		Moving = false;
	}	
	public void StartMoving()
	{
		Moving = true;
	}

	protected override void OnDisabled()
	{
		TravelerEvents.ConvoyStopEvent -= StopMoving;
		TravelerEvents.ConvoyMovingEvent -= StartMoving;
	}

	protected override void OnUpdate()
	{
		if ( Camera == null )
			return;
		if ( !Moving )
			return;
		foreach ( var kvp in BackgroundLayers )
		{
			int layerIndex = kvp.Key;
			var tileList = kvp.Value;
			float scrollFactor = layerScrollFactors[layerIndex] ; // 1.0 = foreground, 0.4 = background

			float tileWidth = TileSize.x * TileSizeScalar;
			float layerSpeed = scrollFactor * GameConstants.Travel.MAXCAMERASPEED;

			for ( int i = 0; i < tileList.Count; i++ )
			{
				var tile = tileList[i];
				if ( tile == null ) continue;

				// Move opposite the camera (left-to-right world motion)
				Vector3 position = tile.WorldPosition;
				position.y += layerSpeed * Time.Delta;
				tile.WorldPosition = position;
				bool isOffScreen = tile.WorldPosition.y < Camera.WorldPosition.y - (200 * TileSizeScalar);
				
				// Wrap tile if it goes too far right
				if ( isOffScreen )
				{
					tileList.Sort( ( a, b ) => b.WorldPosition.y.CompareTo( a.WorldPosition.y ) );
					var topTile = tileList[0];
					Log.Info( tile.Name + " Offscreen? " + isOffScreen );
					tile.WorldPosition = topTile.WorldPosition + new Vector3( 0, 195, 0 );
				}
			}
		}
	}


}

