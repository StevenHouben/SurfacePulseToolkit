using System;
using System.Timers;


namespace PixelSense.Infrared.WPF
{
	/// <summary>
	///   Identifies infrared touchpoints for Pixelsense.
	/// </summary>
	public class InfraredInput
	{
		readonly Timer _update = new Timer( 50 );

		public event EventHandler<InfraredInputEventArgs> TouchDown = delegate { };
		public event EventHandler<InfraredInputEventArgs> TouchMove = delegate { };
		public event EventHandler<InfraredInputEventArgs> TouchUp = delegate { };

		int count = 0;

		public InfraredInput()
		{
			_update.Elapsed += ( s, a ) =>
			{
				lock ( this )
				{
					TouchDown( this, new InfraredInputEventArgs( 0 ) );
					TouchUp( this, new InfraredInputEventArgs( 0 ) );

					if ( count++ > 3 )
					{
						_update.Stop();
					}
				}
			};
			_update.Start();
		}
	}
}
