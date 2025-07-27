using System;
using Sandbox;
using Sandbox.UI;

public sealed class UIinputHandeler : Component
{
	[Property] public float Zoom = 1f;
	[Property] public MapUI mapUI { get; set; }
	public float ZoomSpeed = 4f;
	public float PanSpeed = 200f;
	public float MinZoom = 1f;
	public float MaxZoom = 4f;


	[Property] public Vector2 PanOffset;
	private bool dragging = false;
	private Vector2 previousMouse;
	protected override void OnUpdate()
	{
		HandleZoom();
		HandlePanning();
		HandleButtonPress();
	}

	private void HandleZoom()
	{
		float scroll = -Input.MouseWheel.y;
		if ( scroll == 0 ) return;

		Vector2 mouseScreen = Mouse.Position;
		Vector2 panelPos = mouseScreen / mapUI.Panel.ScaleFromScreen;

		// Convert screen space to world space before zoom
		Vector2 worldBefore = (panelPos - PanOffset) / Zoom;

		// Apply zoom change
		float oldZoom = Zoom;
		Zoom = Math.Clamp( Zoom - scroll * ZoomSpeed * Time.Delta, MinZoom, MaxZoom );

		// Convert screen space to world space after zoom
		Vector2 worldAfter = (panelPos - PanOffset) / Zoom;

		// Shift the pan offset so the world point under the cursor stays in the same spot
		PanOffset += (worldAfter - worldBefore) * Zoom;

		ClampPanOffset();

	}

	public void HandleButtonPress()
	{
		var shouldDraw = mapUI.ShouldDrawMap;
		if ( Input.Pressed( "Map" ) && !shouldDraw)
		{
			mapUI.ShouldDrawMap = true;
		}
		else if ( Input.Pressed( "Map" ) && shouldDraw )
		{
			mapUI.ShouldDrawMap = false;
		}
	}

	public void HandlePanning()
	{
		// Mouse drag to pan
		if ( Input.Down( "Attack1" ) )
		{
			if ( !dragging )
			{
				dragging = true;
				previousMouse = Mouse.Position;

			}
			else
			{
				
				Vector2 delta = Mouse.Position - previousMouse;
				Log.Info( "Delta: " + delta );	
				PanOffset += (delta) / 2; // adjust for screen scaling and zoom
				previousMouse = Mouse.Position;

				ClampPanOffset();
			}
		}
		else
		{
			dragging = false;
		}

		// Optional: arrow key panning
		float speed = 800f * Time.Delta / Zoom;
		if ( Input.Down( "Left" ) ) PanOffset.x += speed;
		if ( Input.Down( "Right" ) ) PanOffset.x -= speed;
		if ( Input.Down( "Forward" ) ) PanOffset.y += speed;
		if ( Input.Down( "Backward" ) ) PanOffset.y -= speed;
		ClampPanOffset();
		
	}

	private void ClampPanOffset()
	{


		// Base size of the image (non-zoomed)
		Vector2 viewport = mapUI.Panel.Box.Rect.Size;
		float baseMapWidth = viewport.x;
		float baseMapHeight = viewport.y;

		// Scaled map size
		float mapWidth = baseMapWidth * Zoom;
		float mapHeight = baseMapHeight * Zoom;

		// Viewport size (the visible screen/UI panel)
		

		// If map is smaller than screen, center it and disable panning
		if ( mapWidth <= viewport.x && mapHeight <= viewport.y )
		{
			PanOffset = new Vector2(
				(viewport.x - mapWidth) / 2f,
				(viewport.y - mapHeight) / 2f
			);
			return;
		}

		// Clamp if the map is larger than screen
		float minX = Math.Min( viewport.x - mapWidth, 0f );
		float minY = Math.Min( viewport.y - mapHeight, 0f );
		float maxX = 0f;
		float maxY = 0f;

		Vector2 pan = PanOffset;
		pan.x = Math.Clamp( pan.x, minX, maxX );
		pan.y = Math.Clamp( pan.y, minY, maxY );

		PanOffset = pan;
	}






}
