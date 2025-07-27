using System.Threading.Tasks;
using System;
using Sandbox;

public sealed class MusicManager : Component
{


	[Property] public float GlobalVolume { get; set; } = 0.7f;
	[Property] public string currentSoundEvent;
	private SoundHandle currentHandle;
	private Queue<string> queue = new();


	public void PlayTrack( string path, bool fade = true )
	{
		StopCurrent( fade );

		currentSoundEvent = path;
		currentHandle = Sound.Play( path );
		currentHandle.Volume = GlobalVolume;
	}

	public void QueueTrack( string path )
	{
		queue.Enqueue( path );
	}

	public void StopCurrent( bool fade = true )
	{
		if ( currentHandle != null)
		{
			if ( fade )
			{
				FadeOutAndStop( currentHandle, 1f );
			}
			else
			{
				currentHandle.Stop();
			}
		}

		currentHandle = default;

	}



	public void SetVolume( float newVolume )
	{
		GlobalVolume = newVolume;
		if ( currentHandle.IsValid )
			currentHandle.Volume = GlobalVolume;
	}

	private void FadeOutAndStop( SoundHandle handle, float duration )
	{
		
			handle.Stop(duration);
	}

	protected override void OnUpdate()
	{
		if ( currentHandle == null )
			return;
		if ( !currentHandle.IsValid && queue.Count > 0 )
		{
			PlayTrack( queue.Dequeue(), true );
		}
		else if (!currentHandle.IsValid)
		{
			PlayTrack( currentSoundEvent );
		}
	}
}





