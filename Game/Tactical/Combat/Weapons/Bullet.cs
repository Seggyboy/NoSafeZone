using System;
using Sandbox;

public sealed class Bullet : Component
{
	public Vector3 StartPos;
	public Vector3 EndPos;
	public int Damage;
	public float Speed = 1000f;
	public float DistanceTraveled = 0f;
	ModelRenderer Renderer;
	[Property] public Board board;
	[Property] public Tile StartTile;
	[Property] public Tile CurrentTile;
	

	private float totalDistance;

	
	protected override void OnStart()
	{
		board = GameObject.Scene.GetAllComponents<Board>().FirstOrDefault();
		StartTile = board.GetWorldTile( StartPos );
		GameObject.WorldPosition = StartPos;
		totalDistance = Vector3.DistanceBetween( StartPos, EndPos );
		var Direction = (EndPos - StartPos).Normal;
		GameObject.WorldRotation = Rotation.LookAt( Direction, Vector3.Up );

		Renderer = GameObject.AddComponent<ModelRenderer>();
		Renderer.Model = Model.Load( "models/weapons/bullet.vmdl" );
		
	}

	protected override void OnUpdate()
	{
		//Log.Info( "Bullet Fired from " + StartPos + " to " + EndPos + "Total Distance: " + totalDistance);
		if ( totalDistance <= 0f )
		{
			Log.Warning( "Bullet didn't fire" );
				return;
		}
		
		float step = Speed * Time.Delta;
		DistanceTraveled += step;

		float t = Math.Clamp( DistanceTraveled / totalDistance, 0f, 1f );
		GameObject.WorldPosition = Vector3.Lerp( StartPos, EndPos, t );
		CurrentTile = board.GetWorldTile( GameObject.WorldPosition );



		bool hasHitTile = CurrentTile != StartTile && CurrentTile.IsOccupied;

		if ( t >= 1f || hasHitTile)
		{
			//Log.Info("Stopped on tile: " + CurrentTile.X + ", " + CurrentTile.Y + " - Occupied: " + CurrentTile.IsOccupied + ", Full Cover: " + CurrentTile.FullCover );
			// Impact reached
			OnHit();
			GameObject.Destroy();
		}

	}

	private void OnHit()
	{
		if ( CurrentTile.IsOccupied && CurrentTile.unit != null)
		{
			//Log.Info( "Hitting unit: " + CurrentTile.unit.GameObject.Name + " with damage: " + Damage );
			CurrentTile.unit.Hurt( Damage );
		}
	}
}
