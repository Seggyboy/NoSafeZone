using System.Security.Cryptography.X509Certificates;
using Sandbox;
using SpriteTools;

public sealed class VehicleRenderer : Component
{
	[Property]GameObject HumveePrefab { get; set; }
	[Property] GameObject LMTVPrefab { get; set; }

	[Property] public List<GameObject> Vehicles = new();
	[Property] CameraComponent Cam;
	[Property] Vector2 VehicleSize = new( 8, 8 ); // Size of the vehicle sprites
	[Property] int Spacing;
	[Property] int XOffset = 30; // Offset for the X position of the vehicles
	[Property] int ZOffset = -10;
	[Property] int YOffset = -12; // Offset for the Y position of the vehicles	
	protected override void OnEnabled()
	{
		CreateVehicle(VehicleType.LMTV, "LMTV" );	
		CreateVehicle( VehicleType.Humvee, "Humvee" );
		CreateVehicle( VehicleType.Humvee, "Humvee2" );
		CreateVehicle( VehicleType.LMTV, "LMTV2" );

		for ( int y = 0; y < Vehicles.Count; y++ )
		{
			var vehicle = Vehicles[y];
			if ( vehicle == null )
				continue;

			Vector3 offset = new Vector3( XOffset, y * (VehicleSize.y + Spacing) + YOffset, ZOffset );
			vehicle.WorldScale = new Vector3( 0.075f, 0.075f, 0.075f ); // Set the scale of the vehicle
			var spriteRenderer = vehicle.Components.Get<SpriteComponent>();
			spriteRenderer.SpriteFlags = SpriteFlags.HorizontalFlip | SpriteFlags.DrawBackface;
			spriteRenderer.UpDirection = SpriteComponent.Axis.ZPositive;
			vehicle.WorldPosition = Cam.WorldPosition + offset;
			vehicle.SetParent( Cam.GameObject, true ); // Set parent to camera for movement tracking
		}

	}

	public void CreateVehicle( VehicleType type, string name )
	{

		switch(type)
		{
			case VehicleType.LMTV:
				var lmtv = LMTVPrefab?.Clone();
				lmtv.Parent = Cam.GameObject;
				Vehicles.Add( lmtv );
				return;
			case VehicleType.Humvee:
				var humvee = HumveePrefab?.Clone();
				humvee.Parent = Cam.GameObject;
				Vehicles.Add(humvee );
				return;
			default:
				return;
		}


	
	}

	

	protected override void OnUpdate()
	{
		

		
	}
}




public enum VehicleType
{
	LMTV,
	Humvee,
	FuelTruck,
}
