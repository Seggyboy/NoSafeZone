using Sandbox;

public sealed class TacticalUI : Component
{
	protected override void OnUpdate()
	{
		if ( Scene.Camera is null )
			return;

		var hud = Scene.Camera.Hud;

		

		

		hud.DrawText( new TextRendering.Scope( "Hello!", Color.Red, 32 ), Screen.Width * 0.5f );
		
	}
}
