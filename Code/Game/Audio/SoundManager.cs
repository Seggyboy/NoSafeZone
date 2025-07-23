using Sandbox;

public sealed class SoundManager : Component
{
	[Property] SoundHandle soundHandle;
	[Property] SoundHandle soundHandle_two;
	protected override void OnEnabled()
	{
		
	}

	public void PlaySounds()
	{
		PlayMusic();
		PlayBackground();
	}

	public void PlayMusic()
	{
	
		soundHandle = Sound.Play( "sound/music/combatfinal.sound" );
	}

	public void PlayBackground()
	{
		
		soundHandle_two = Sound.Play( "sound/ambient/city/ambientbackground.sound" );
	}

	public void PlayLossSound()
	{
		soundHandle.Stop();
		soundHandle = Sound.Play( "sound/music/whatwelostgameover.sound" );
	}

	public void PlayWinSound()
	{
		soundHandle.Stop();
		soundHandle = Sound.Play( "sound/music/cominghome.sound" );
		
	}

	
	public void StopSound()
	{
		Sound.StopAll( 0.5f );
	}

	protected override void OnUpdate()
	{
		if ( soundHandle == null || soundHandle_two == null )
		{
	
			return;
		}
		if ( !soundHandle.IsPlaying )
		{
			Log.Info( "Playing new soudn." );
			PlayMusic();
		}

		if (!soundHandle_two.IsPlaying )
		{
			Log.Info( "Playing new soudn." );
			PlayBackground();
			
		}	


	}

}
