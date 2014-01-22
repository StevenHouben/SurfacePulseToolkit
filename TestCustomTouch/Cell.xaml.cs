using System;
using System.Windows;
using Microsoft.Surface;
using Whathecode.System.Windows.DependencyPropertyFactory;
using Whathecode.System.Windows.DependencyPropertyFactory.Attributes;
using Whathecode.System.Windows.Input;
using PixelSense.Infrared.WPF;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Surface.Presentation.Input;

namespace TestCustomTouch
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>
    public partial class Cell : UserControl
    {
		[Flags]
		public enum Property
		{
			TagAdded,
            TagMoved,
			TagRemoved
		}

		static readonly DependencyPropertyFactory<Property> PropertyFactory = new DependencyPropertyFactory<Property>();
		public static readonly DependencyProperty TagAddedProperty = PropertyFactory[ Property.TagAdded ];
        public static readonly DependencyProperty TagMovedProperty = PropertyFactory[ Property.TagMoved ];
		public static readonly DependencyProperty TagRemovedProperty = PropertyFactory[ Property.TagRemoved ];

		[DependencyProperty( Property.TagAdded )]
		public DelegateCommand<LightBehavior.InfraredTagArgs> TagAdded
		{
			get { return (DelegateCommand<LightBehavior.InfraredTagArgs>)PropertyFactory.GetValue( this, Property.TagAdded ); }
			private set { PropertyFactory.SetValue( this, Property.TagAdded, value ); }
		}

        [DependencyProperty(Property.TagMoved)]
        public DelegateCommand<LightBehavior.InfraredTagArgs> TagMoved
        {
            get { return (DelegateCommand<LightBehavior.InfraredTagArgs>)PropertyFactory.GetValue(this, Property.TagMoved); }
            private set { PropertyFactory.SetValue(this, Property.TagMoved, value); }
        }

		[DependencyProperty( Property.TagRemoved )]
		public DelegateCommand<LightBehavior.InfraredTagArgs> TagRemoved
		{
			get { return (DelegateCommand<LightBehavior.InfraredTagArgs>)PropertyFactory.GetValue( this, Property.TagRemoved ); }
			private set { PropertyFactory.SetValue( this, Property.TagRemoved, value ); }
		}


		/// <summary>
		/// Default constructor.
		/// </summary>
		public Cell()
		{
			InitializeComponent();

			TagAdded = new DelegateCommand<LightBehavior.InfraredTagArgs>( OnTagAdded );
            //TagMoved = new DelegateCommand<InfraredBehavior.InfraredTagArgs>( OnTagMoved );
            //TagRemoved = new DelegateCommand<InfraredBehavior.InfraredTagArgs>( OnTagRemoved );
		}

        Dictionary<int, FrameworkElement> _tags = new Dictionary<int, FrameworkElement>();
		void OnTagAdded( LightBehavior.InfraredTagArgs added )
		{
			Console.WriteLine( "Frequency tag added!" );

            
		}

        Color[] _colors = new Color[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Cyan, Colors.Purple, Colors.White };
        private SolidColorBrush GetBrush(int val)
        {
            const int start = 50;
            const int interval = 100;    // Minimum possible interval seems to be 50, but differentiating between < 30ms is difficult.

            int step = (val - start) / interval;

            return step >= 0 && step < _colors.Length
                ? new SolidColorBrush(_colors[step])
                : new SolidColorBrush(Colors.Lime);
        }

        private void OnFingerTouch(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (e.Device.GetIsFingerRecognized())
            {
                
            }
        }
    }
}
