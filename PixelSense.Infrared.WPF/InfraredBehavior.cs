using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Surface.Presentation.Controls;
using Whathecode.System.Extensions;
using Whathecode.System.Windows.DependencyPropertyFactory;
using Whathecode.System.Windows.DependencyPropertyFactory.Attributes;
using Pixelsense.Infrared;
using System.Windows.Interop;
using System.Reactive;


namespace PixelSense.Infrared.WPF
{
	/// <summary>
	///   Properties which can be attached to a <see cref = "FrameworkElement" /> to detect infrared touch behavior.
	/// </summary>
	public static class LightBehavior
	{
		class TrackTag
		{
            static int _recognizedTags = 0;
			const double Margin = 100;
			readonly TimeSpan _intervalMargin = TimeSpan.FromMilliseconds( 100 );

			FrameworkElement _element;
			Point _position;
			readonly Subject<Point> _touchDowns = new Subject<Point>();

            public int Id { get; private set; }
			public TimeSpan Interval { get; private set; }
			public bool IsRemoved { get; private set; }


			public TrackTag( FrameworkElement element, Point position )
			{
                Id = _recognizedTags++;
				_element = element;
				_position = position;

				var tagAdded = _touchDowns
					.TimeInterval()
					.Skip( 1 )	// The first interval isn't one in between 2 events.
					.Do( d => Interval = d.Interval )
					.TakeWhile( d => Interval < TimeSpan.FromSeconds(1) && WithinInterval( d.Interval ) )
					.Take( 1 );	// Once two subsequent consistent intervals have been detected, the tag has been added.
				tagAdded.Subscribe( a =>
				{
					GetFrequencyTagAddedCommand( element ).SafeExecute( new InfraredTagArgs( Id, Interval, position ) );

                    var tagRemoved = _touchDowns
                        .Timeout(Interval + _intervalMargin)
                        .TimeInterval()
                        .SkipWhile( d =>
                        {
                            bool withinInterval = WithinInterval(d.Interval);
                            if (withinInterval)
                            {
                                var movedTo = new Point( d.Value.X, d.Value.Y );
                                GetFrequencyTagMovedCommand( element ).SafeExecute( new InfraredTagArgs( Id, Interval, movedTo ) );
                            }
                            return withinInterval;
                        } )
                        .Catch(Observable.Return<TimeInterval<Point>>(new TimeInterval<Point>()))
                        .Take(1);
					tagRemoved.Subscribe( r =>
					{
                        var removedAt = new Point( r.Value.X, r.Value.Y );
                        element.Dispatcher.BeginInvoke( (Action)(() => GetFrequencyTagRemovedCommand(element).SafeExecute( new InfraredTagArgs( Id, Interval, removedAt ) ) ) );
						IsRemoved = true;
					} );
				} );
			}

			bool WithinInterval( TimeSpan newInterval )
			{
				return newInterval > Interval - _intervalMargin && newInterval < Interval + _intervalMargin;
			}

			public void TouchDown( Point position )
			{
				_position = position;
				_touchDowns.OnNext( position );
			}

			public bool InRange( Point point )
			{
				return
					point.X >= _position.X - Margin && point.X <= _position.X + Margin &&
					point.Y >= _position.Y - Margin && point.Y <= _position.Y + Margin;
			}
		}

        public class InfraredTagArgs
        {
            public int Id { get; private set; }
            public TimeSpan Frequency { get; private set; }
            public Point Position { get; private set; }

            public InfraredTagArgs( int id, TimeSpan frequency, Point position )
            {
                Id = id;
                Frequency = frequency;
                Position = position;
            }
        }


		static void SafeExecute( this ICommand command, object parameter )
		{
			if ( command != null && command.CanExecute( null ) )
			{
				command.Execute( parameter );
			}
		}



		[Flags]
		public enum Properties
		{
			IsEnabled,
			FrequencyTagAdded,
			FrequencyTagMoved,
			FrequencyTagRemoved
		}


		static readonly DependencyPropertyFactory<Properties> DependencyProperties = new DependencyPropertyFactory<Properties>();

		public static DependencyProperty IsEnabledProperty = DependencyProperties[ Properties.IsEnabled ];
		public static DependencyProperty FrequencyTagAddedCommandProperty = DependencyProperties[ Properties.FrequencyTagAdded ];
		public static DependencyProperty FrequencyTagMovedCommandProperty = DependencyProperties[ Properties.FrequencyTagMoved ];
		public static DependencyProperty FrequencyTagRemovedCommandProperty = DependencyProperties[ Properties.FrequencyTagRemoved ];

		static InfraredDevice _input;


		#region IsEnabled Property

		[DependencyProperty( Properties.IsEnabled )]
		public static bool GetIsEnabled( SurfaceWindow target )
		{
			return (bool)DependencyProperties.GetValue( target, Properties.IsEnabled );
		}

		[DependencyProperty( Properties.IsEnabled )]
		public static void SetIsEnabled( SurfaceWindow target, bool value )
		{
			DependencyProperties.SetValue( target, Properties.IsEnabled, value );
		}

		[DependencyPropertyChanged( Properties.IsEnabled )]
		static void OnIsEnabledChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			bool isEnabled = (bool)e.NewValue;

			if ( isEnabled && _input == null )
			{
                Window w = d is Window ? (Window)d : Window.GetWindow(d);
                _input = new InfraredDevice( new WindowInteropHelper( w ).Handle );
				_input.InfraredInputAdded += ( s, a ) => d.Dispatcher.Invoke(
					() =>
					{
						PresentationSource activeSource = PresentationSource.FromVisual( (Visual)d );
						InfraredTouchDevice.TouchDown( a, activeSource );
					} );
				_input.InfraredInputLocationChanged += ( s, a ) => d.Dispatcher.Invoke(
					() =>
					{
						PresentationSource activeSource = PresentationSource.FromVisual( (Visual)d );
						InfraredTouchDevice.TouchMove( a, activeSource );
					} );
				_input.InfraredInputRemoved += ( s, a ) => d.Dispatcher.Invoke(
					() =>
					{
						PresentationSource activeSource = PresentationSource.FromVisual( (Visual)d );
						InfraredTouchDevice.TouchUp( a, activeSource );
					} );
			}
			else
			{
				// TODO: Cleanup. What if it's enabled on several windows?
			}
		}

		#endregion // IsEnabled Property


		#region FrequencyTagAdded Command

		[DependencyProperty( Properties.FrequencyTagAdded )]
		public static ICommand GetFrequencyTagAddedCommand( FrameworkElement target )
		{
			return (ICommand)DependencyProperties.GetValue( target, Properties.FrequencyTagAdded );
		}

		[DependencyProperty( Properties.FrequencyTagAdded )]
		public static void SetFrequencyTagAddedCommand( FrameworkElement target, ICommand value )
		{
			DependencyProperties.SetValue( target, Properties.FrequencyTagAdded, value );
		}

		[DependencyPropertyChanged( Properties.FrequencyTagAdded )]
		static void OnFrequencyTagAddedCommandChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			Initialize( d );
		}

		#endregion // FrequencyTagAdded Command


		#region FrequencyTagMoved Command

		[DependencyProperty( Properties.FrequencyTagMoved )]
		public static ICommand GetFrequencyTagMovedCommand( FrameworkElement target )
		{
			return (ICommand)DependencyProperties.GetValue( target, Properties.FrequencyTagMoved );
		}

		[DependencyProperty( Properties.FrequencyTagMoved )]
		public static void SetFrequencyTagMovedCommand( FrameworkElement target, ICommand value )
		{
			DependencyProperties.SetValue( target, Properties.FrequencyTagMoved, value );
		}

		[DependencyPropertyChanged( Properties.FrequencyTagMoved )]
		static void OnFrequencyTagMovedCommandChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			Initialize( d );
		}

		#endregion // FrequencyTagMoved Command


		#region FrequencyTagRemoved Command

		[DependencyProperty( Properties.FrequencyTagRemoved)]
		public static ICommand GetFrequencyTagRemovedCommand( FrameworkElement target )
		{
			return (ICommand)DependencyProperties.GetValue( target, Properties.FrequencyTagRemoved );
		}

		[DependencyProperty( Properties.FrequencyTagRemoved )]
		public static void SetFrequencyTagRemovedCommand( FrameworkElement target, ICommand value )
		{
			DependencyProperties.SetValue( target, Properties.FrequencyTagRemoved, value );
		}

		[DependencyPropertyChanged( Properties.FrequencyTagRemoved )]
		static void OnFrequencyTagRemovedCommandChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			Initialize( d );
		}

		#endregion // FrequencyTagRemoved Command


		static void Initialize( DependencyObject o )
		{
			var element = o as UIElement;

			if ( element != null )
			{
				// Make sure to only hook event once.
				element.TouchDown -= OnTouchDown;
				element.TouchDown += OnTouchDown;
			}

			// Ensure that infrared input is enabled for this window.
			Window w = Window.GetWindow( o );
			if ( w != null )
			{
				w.SetValue( IsEnabledProperty, true );
			}
		}

		static readonly Dictionary<object, List<TrackTag>> TrackedTags = new Dictionary<object, List<TrackTag>>();
		static void OnTouchDown( object sender, TouchEventArgs e )
		{
			var device = e.Device as InfraredTouchDevice;
			if ( device == null )
			{
				return;
			}

			var element = (FrameworkElement)sender;
			Point point = e.GetTouchPoint( element ).Position;

			// Add or update existing trackers.
			TrackTag existingTag = TrackedTags
				.FirstOrDefault( p => p.Key == sender )
				.Value.IfNotNull( tags => tags.FirstOrDefault( t => t.InRange( point ) ) );
			if ( existingTag == null )
			{
				// Create new tag tracker.
				var tag = new TrackTag( element, point );
				if ( !TrackedTags.ContainsKey( sender ) )
				{
					TrackedTags[ sender ] = new List<TrackTag>();
				}
				TrackedTags[ sender ].Add( tag );
			}
			else
			{
				// Update position for existing tracker.
				existingTag.TouchDown( point );
			}

			// Remove existing trackers.
			var toRemove = TrackedTags.Select( p => Tuple.Create( p.Key, p.Value.Where( tag => tag.IsRemoved ).ToList() ) ).ToList();
			foreach ( var list in toRemove )
			{
				foreach ( var r in list.Item2 )
				{
					TrackedTags[ list.Item1 ].Remove( r );
				}
			}

			e.Handled = true;
		}
	}
}
