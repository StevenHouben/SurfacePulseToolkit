using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pixelsense.Infrared
{
    public class InfraredInput
    {
        public int Frequency { get; set; }
        public Point Center { get; set; }
        public Size Size { get; set; }
        public Guid Id { get; set; }

        public InfraredInput(Point center)
        {
            Center = center;
            Size = new Size(200, 200);
            Id = Guid.NewGuid();
        }

        internal bool InRange(Point p)
        {
            double halfX = this.Size.Width/2;
            double halfY = this.Size.Width/2;
            var r= 
                p.X > Center.X - halfX && 
                p.X < Center.X + halfX &&
                p.Y > Center.Y - halfY && 
                p.Y < Center.Y + halfY;
            return r;

        }
    }
}
