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


namespace TestCustomTouch
{
	/// <summary>
	/// Interaction logic for SurfaceWindow1.xaml
	/// </summary>
	public partial class SurfaceWindow1
	{


        public SurfaceWindow1()
        {
            InitializeComponent();

            const int rows = 8;
            const int columns = 15;
            for (int r = 0; r < rows; ++r)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for ( int c = 0; c < columns; ++c )
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() );
            }


            for ( int r = 0; r < rows; ++r )
            {
                for (int c = 0; c < columns; ++c)
                {
                    var cell = new Cell();
                    cell.SetValue(Grid.RowProperty, r);
                    cell.SetValue(Grid.ColumnProperty, c);
                    grid.Children.Add(cell);
                }
            }
        }

        //[Flags]
        //public enum Property
        //{
        //    TagAdded,
        //    TagMoved,
        //    TagRemoved
        //}

        //static readonly DependencyPropertyFactory<Property> PropertyFactory = new DependencyPropertyFactory<Property>();
        //public static readonly DependencyProperty TagAddedProperty = PropertyFactory[ Property.TagAdded ];
        //public static readonly DependencyProperty TagMovedProperty = PropertyFactory[ Property.TagMoved ];
        //public static readonly DependencyProperty TagRemovedProperty = PropertyFactory[ Property.TagRemoved ];

        //[DependencyProperty( Property.TagAdded )]
        //public DelegateCommand<InfraredBehavior.InfraredTagArgs> TagAdded
        //{
        //    get { return (DelegateCommand<InfraredBehavior.InfraredTagArgs>)PropertyFactory.GetValue( this, Property.TagAdded ); }
        //    private set { PropertyFactory.SetValue( this, Property.TagAdded, value ); }
        //}

        //[DependencyProperty(Property.TagMoved)]
        //public DelegateCommand<InfraredBehavior.InfraredTagArgs> TagMoved
        //{
        //    get { return (DelegateCommand<InfraredBehavior.InfraredTagArgs>)PropertyFactory.GetValue(this, Property.TagMoved); }
        //    private set { PropertyFactory.SetValue(this, Property.TagMoved, value); }
        //}

        //[DependencyProperty( Property.TagRemoved )]
        //public DelegateCommand<InfraredBehavior.InfraredTagArgs> TagRemoved
        //{
        //    get { return (DelegateCommand<InfraredBehavior.InfraredTagArgs>)PropertyFactory.GetValue( this, Property.TagRemoved ); }
        //    private set { PropertyFactory.SetValue( this, Property.TagRemoved, value ); }
        //}


        ///// <summary>
        ///// Default constructor.
        ///// </summary>
        //public SurfaceWindow1()
        //{
        //    InitializeComponent();

        //    TagAdded = new DelegateCommand<InfraredBehavior.InfraredTagArgs>( OnTagAdded );
        //    TagMoved = new DelegateCommand<InfraredBehavior.InfraredTagArgs>( OnTagMoved );
        //    TagRemoved = new DelegateCommand<InfraredBehavior.InfraredTagArgs>( OnTagRemoved );

        //    this.WindowStyle = System.Windows.WindowStyle.None;
        //}

        //Dictionary<int, FrameworkElement> _tags = new Dictionary<int, FrameworkElement>();
        //void OnTagAdded( InfraredBehavior.InfraredTagArgs added )
        //{
        //    Console.WriteLine( "Frequency tag added!" );

        //    Rectangle rect = new Rectangle()
        //    {
        //        Fill = GetBrush( (int)added.Frequency.TotalMilliseconds / 2 ),
        //        Width = 100,
        //        Height= 100
        //    };
        //    PositionVisual(rect, added.Position);
        //    _tags[added.Id] = rect;
        //    Container.Children.Add(rect);
        //}

        //void OnTagMoved(InfraredBehavior.InfraredTagArgs moved)
        //{
        //    FrameworkElement rect = _tags[moved.Id];
        //    PositionVisual(rect, moved.Position);
        //}

        //void OnTagRemoved( InfraredBehavior.InfraredTagArgs removed)
        //{
        //    Console.WriteLine( "Frequency tag removed!" );

        //    FrameworkElement rect = _tags[ removed.Id ];
        //    Container.Children.Remove(rect);
        //    _tags.Remove(removed.Id);
        //}

        //void PositionVisual(FrameworkElement visual, Point position )
        //{
        //    double width = visual.Width == double.NaN ? visual.ActualWidth : visual.Width;
        //    double height = visual.Height == double.NaN ? visual.ActualHeight : visual.Height;
        //    visual.SetValue(Canvas.LeftProperty, position.X - width/2);
        //    visual.SetValue(Canvas.TopProperty, position.Y - height/2);
        //}

        //Color[] _colors = new Color[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Cyan, Colors.Purple, Colors.White };
        //private SolidColorBrush GetBrush(int val)
        //{
        //    const int start = 50;
        //    const int interval = 100;    // Minimum possible interval seems to be 50, but differentiating between < 30ms is difficult.

        //    int step = (val - start) / interval;

        //    return step >= 0 && step < _colors.Length
        //        ? new SolidColorBrush(_colors[step])
        //        : new SolidColorBrush(Colors.Lime);
        //}
	}
}