using Sandbox;


public sealed class Mapdisplay : Component
{
	[Property] ModelRenderer renderer;
	

	public void DrawNoiseMap( float[,] noiseMap )
	{
		int width = noiseMap.GetLength( 0 );
		int height = noiseMap.GetLength( 1 );

		var texture = Texture.Create(  width, height, ImageFormat.RGBA8888 );

		Color[] colors = new Color[width * height];
		byte[] pixelData = new byte[width * height * 4];

		for (int y = 0; y < height; y++) {
			for ( int x = 0; x < width; x++ )
			{
				float value = noiseMap[x, y];
				if (y == 0 && x < 10 )
				{
					Log.Info(noiseMap[x, y] );
				}
				value = MathX.Clamp( value, 0f, 1f );
				colors[y * width + x] = Color.Lerp( Color.Black, Color.White, value );
				int i = (y * width + x) * 4;
				pixelData[i + 0] = (byte)(colors[y * width + x].r * 255f); // Red
				pixelData[i + 1] = (byte)(colors[y * width + x].g * 255f); // Green
				pixelData[i + 2] = (byte)(colors[y * width + x].b * 255f); // Blue
				pixelData[i + 3] = (byte)(colors[y * width + x].a * 255f); // Alpha
				
				//Log.Info(pixelData[i + 0] + ' ' + pixelData[i + 1] + ' ' + pixelData[i + 2] + ' ' + pixelData[i + 3] );
				
			}
		}
	
		texture.WithData( pixelData );
		var tex = texture.Finish();

		using var stream = FileSystem.Data.OpenWrite( "terrain2.png", System.IO.FileMode.OpenOrCreate );
		stream.Write( tex.GetBitmap( 0 ).ToPng() );
		if ( FileSystem.Data.FileExists( "terrain2.png" ) )
		{
			Log.Info( "File created successfully!" );
		}
		else
		{
			Log.Error( "Failed to create file." );
		}
		stream.Close();

	}

	protected override void OnEnabled()
	{
		DrawNoiseMap( NoiseArray.GenNoiseArray( 1024,1024, 12345, 50, 5, 0.6f, 2, new Vector2(0,0) ) );
	}





}
