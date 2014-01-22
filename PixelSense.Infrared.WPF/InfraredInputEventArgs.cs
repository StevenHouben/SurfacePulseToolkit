using System;


namespace PixelSense.Infrared.WPF
{
	public class InfraredInputEventArgs : EventArgs
	{
		public int Id { get; private set; }


		public InfraredInputEventArgs( int id )
		{
			Id = id;
		}
	}
}
