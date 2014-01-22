using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pixelsense.Infrared
{
    public class InfraredInputEventArgs:EventArgs
    {
        public InfraredInput InfraredInput { get; set; }
        public InfraredInputEventArgs(InfraredInput infraInput)
        {
            InfraredInput = infraInput;
        }
    }
}
