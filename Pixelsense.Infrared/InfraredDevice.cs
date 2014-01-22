using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Surface.Core;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace Pixelsense.Infrared
{
    public delegate void InfraredInputEventHandler(object sender, InfraredInputEventArgs e); 
    public class InfraredDevice
    {
        #region Events
        public event InfraredInputEventHandler InfraredInputLocationChanged;
        public event InfraredInputEventHandler InfraredInputAdded;
        public event InfraredInputEventHandler InfraredInputRemoved;
        #endregion

        #region Private Members
        private List<InfraredInput> Inputs = new List<InfraredInput>();
        private TouchTarget contactTarget;
        private byte[] raw;
        private ImageMetrics imageMetrics;
        private bool imageAvailable;
        private CircleF[] contourCircles;
        #endregion

        #region Constructor
        public InfraredDevice(IntPtr handle)
        {
            contactTarget = new TouchTarget(handle);
            contactTarget.EnableInput();
            contactTarget.EnableImage(ImageType.Normalized);
            contactTarget.FrameReceived += new EventHandler<FrameReceivedEventArgs>(contactTarget_FrameReceived);

        }
        #endregion

        #region Event Handlers
        private void contactTarget_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            int paddingLeft, paddingRight;
            if (raw == null)
            {
                imageAvailable = e.TryGetRawImage(ImageType.Normalized,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Left,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Top,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Width,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Height,
                    out raw, out imageMetrics, out paddingLeft, out paddingRight);
            }
            else
            {
                imageAvailable = e.UpdateRawImage(ImageType.Normalized, raw,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Left,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Top,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Width,
                    Microsoft.Surface.Core.InteractiveSurface.PrimarySurfaceDevice.Height);
            }
            if (!imageAvailable)
                return;


            var r = CreateEmguCvImage(raw, imageMetrics);

            ProcessFrame(r);
            DetectInputs();
        }
        #endregion

        #region Processing
        internal void DetectInputs()
        {
            if (contourCircles == null)
                return;

            var toRemove = new List<InfraredInput>();
            var contours = contourCircles;
            for(var i=0;i<Inputs.Count;i++)
            {
                var led = Inputs[i];
                bool found = false;

                for (var j = 0; j < contours.Length; j++)
                {
                    var circle = contourCircles[j];
                    if (led.InRange(new Point((int)circle.Center.X * 2, (int)circle.Center.Y * 2)))
                    {
                        found = true;
                    }
                }
                if (!found)
                    toRemove.Add(led);
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                Inputs.Remove(toRemove[i]);
                if (InfraredInputRemoved != null)
                {
                    InfraredInputRemoved(this, new InfraredInputEventArgs(toRemove[i]));
                }
            }
            for (var i = 0; i < contours.Length; i++)
            {
                var circle = contours[i];
                bool ledFound = false;

                for (int j = 0; j < Inputs.Count; j++)
                {
                    var led = Inputs[j];
                    if (led.InRange(new Point((int)circle.Center.X * 2, (int)circle.Center.Y * 2)))
                    {

                        Inputs[j].Center = new Point((int)circle.Center.X * 2, (int)circle.Center.Y * 2);
                        ledFound = true;

                        if (InfraredInputLocationChanged != null)
                            InfraredInputLocationChanged(this, new InfraredInputEventArgs(led));
                    }
                }
                if (!ledFound)
                {
                    var led = new InfraredInput(new Point((int)circle.Center.X * 2, (int)circle.Center.Y * 2));
                    Inputs.Add(led);
                    if (InfraredInputAdded != null)
                    {
                        Console.WriteLine("Added at:"+led.Center.ToString());
                        InfraredInputAdded(this, new InfraredInputEventArgs(led));
                    }
                }
            }
        }
        internal Image<Gray, byte> CreateEmguCvImage(byte[] image, ImageMetrics metrics)
        {
            return new Image<Gray, byte>(metrics.Width, metrics.Height) { Bytes = image };
        }
        internal Image<Gray, byte> ProcessFrame(Image<Gray, byte> image)
        {
            image = image.ThresholdBinary(
            new Gray(254), new Gray(255));

            Contour<System.Drawing.Point> contours =
            image.FindContours(
            Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
            Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
            contourCircles = FindPossibleCircles(contours);
            return image;
        }
        internal CircleF[] FindPossibleCircles(Contour<System.Drawing.Point> contours)
        {
            //ResetContoursNavigation(ref contours);
            IList<CircleF> circles = new List<CircleF>();
            if (contours != null)
            {
                for (; contours.HNext != null; contours = contours.HNext)
                {
                    if (contours.Area >= 10 && contours.Area <= 50)
                    {
                        circles.Add(new CircleF(new PointF(
                        contours.BoundingRectangle.Left +
                        (contours.BoundingRectangle.Width / 2),
                        contours.BoundingRectangle.Top +
                        (contours.BoundingRectangle.Height / 2)),
                        contours.BoundingRectangle.Width / 2));
                    }
                }
                if (contours.Area >= 10 && contours.Area <= 50)
                {
                    circles.Add(new CircleF(new PointF(
                    contours.BoundingRectangle.Left +
                    contours.BoundingRectangle.Width / 2,
                    contours.BoundingRectangle.Top +
                    contours.BoundingRectangle.Height / 2),
                    contours.BoundingRectangle.Width / 2));
                }
            }
            return circles.ToArray();
        }
        internal void ResetContoursNavigation(ref Contour<System.Drawing.Point> contours)
        {
            if (contours == null)
                return;
            while (contours.HPrev != null)
                contours = contours.HPrev;
        }
        #endregion
    }
}
