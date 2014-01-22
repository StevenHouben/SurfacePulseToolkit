using Pixelsense.Infrared;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace PixelSense.Infrared.WPF
{
	/// <summary>
	///   A touch device which can recognize infrared pens or markers.
	/// </summary>
	public class InfraredTouchDevice : TouchDevice
	{
		static readonly Dictionary<Guid, InfraredTouchDevice> Devices = new Dictionary<Guid, InfraredTouchDevice>();

		static InfraredTouchDevice GetDevice( Guid id )
		{
			InfraredTouchDevice device;
			if ( !Devices.TryGetValue( id, out device ) )
			{
				device = new InfraredTouchDevice( id );
				Devices.Add( id, device );
			}

			return device;
		}

		public static void TouchDown( InfraredInputEventArgs args, PresentationSource activeSource )
		{
			var device = GetDevice( args.InfraredInput.Id );
			device._action = TouchAction.Down;

			device.SetActiveSource( activeSource );
            device._position = new Point(args.InfraredInput.Center.X, args.InfraredInput.Center.Y);
			device.Activate();
			device.ReportDown();
		}

		public static void TouchMove( InfraredInputEventArgs args, PresentationSource activeSource )
		{
			var device = GetDevice( args.InfraredInput.Id );
			device._action = TouchAction.Move;

			if ( device.IsActive )
			{
                device._position = new Point(args.InfraredInput.Center.X,args.InfraredInput.Center.Y);
				device.ReportMove();
			}
		}

		public static void TouchUp( InfraredInputEventArgs args, PresentationSource activeSource )
		{
			var device = GetDevice( args.InfraredInput.Id );
			device._action = TouchAction.Up;

			if ( device.IsActive )
			{
                device._position = new Point(args.InfraredInput.Center.X,args.InfraredInput.Center.Y);
				device.ReportUp();
				device.Deactivate();
			}
		}


		Point _position = new Point( 300, 300 );
		TouchAction _action;


		InfraredTouchDevice( Guid deviceId )
			: base( deviceId.GetHashCode() )
		{
		}


		public override TouchPoint GetTouchPoint( IInputElement relativeTo )
		{
			Point p = _position;
			if ( relativeTo != null && ActiveSource != null )
			{
				p = ActiveSource.RootVisual.TransformToDescendant( (Visual)relativeTo ).Transform( p );
			}

			var rect = new Rect( p, new Size( 1.0, 1.0 ) );

			return new TouchPoint( this, p, rect, _action );
		}

		public override TouchPointCollection GetIntermediateTouchPoints( IInputElement relativeTo )
		{
			// This can be used to aggregate a sequence of high frequency touch positions and not overwhelm the system with individual Touch events.
			// For now, it’s sufficient to return an empty collection and call ReportMove() on every detected touch movements.
			return new TouchPointCollection();
		}
	}
}