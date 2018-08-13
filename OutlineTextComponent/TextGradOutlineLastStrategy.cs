using System;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using System.Numerics;

namespace OutlineTextComponent
{
    public enum GradientType
    {
        Linear,
        Sinusoid
    }
    public sealed class TextGradOutlineLastStrategy : ITextStrategy
    {
		public TextGradOutlineLastStrategy()
		{
			m_nThickness=2;
			m_brushText = null;
			m_bClrText = true;
            m_GradientType = GradientType.Linear;
            disposed = false;
		}
        public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
        public void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
                    if (m_brushText != null)
                    {
                        m_brushText.Dispose();
                    }
				}

				disposed = true;
			}
		}
		~TextGradOutlineLastStrategy()
		{
			Dispose(false);
		}
        public ITextStrategy Clone()
		{
			TextGradOutlineLastStrategy p = new TextGradOutlineLastStrategy();
			if (m_bClrText)
				p.Init(m_clrText, m_clrOutline1, m_clrOutline2, m_nThickness, m_GradientType);
			else
				p.Init(m_brushText, m_clrOutline1, m_clrOutline2, m_nThickness, m_GradientType);

			return p;
		}

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public void Init(
			Color clrText, 
			Color clrOutline1, 
			Color clrOutline2, 
			int nThickness,
            GradientType useCurveGradient)
		{
			m_clrText = clrText;
			m_bClrText = true;
			m_clrOutline1 = clrOutline1;
			m_clrOutline2 = clrOutline2;
			m_nThickness = nThickness;
            m_GradientType = useCurveGradient;
        }

		public void Init(
			ICanvasBrush brushText,
			Color clrOutline1,
			Color clrOutline2,
			int nThickness,
            GradientType useCurveGradient)
		{
			m_brushText = brushText;
			m_bClrText = false;
			m_clrOutline1 = clrOutline1;
			m_clrOutline2 = clrOutline2;
			m_nThickness = nThickness;
            m_GradientType = useCurveGradient;
        }

        public bool DrawString(
            CanvasDrawingSession graphics,
            CanvasTextLayout textLayout,
            float x, float y)
		{
            using (CanvasGeometry geometry = CanvasGeometry.CreateText(textLayout))
            {
                CanvasStrokeStyle stroke = new CanvasStrokeStyle();
                stroke.DashStyle = CanvasDashStyle.Solid;
                stroke.DashCap = CanvasCapStyle.Round;
                stroke.StartCap = CanvasCapStyle.Round;
                stroke.EndCap = CanvasCapStyle.Round;
                stroke.LineJoin = CanvasLineJoin.Round;

                List<Color> list = new List<Color>();
                if(m_GradientType == GradientType.Sinusoid)
				    CalculateCurvedGradient(m_clrOutline1, m_clrOutline2, m_nThickness, list);
                else
                    CalculateGradient(m_clrOutline1, m_clrOutline2, m_nThickness, list);

                if (m_bClrText)
				{
                    graphics.FillGeometry(geometry, x, y, m_clrText);
                }
				else
                    graphics.FillGeometry(geometry, x, y, m_brushText);

                for (int i = m_nThickness; i >= 1; --i)
				{
                    graphics.DrawGeometry(geometry, x, y, list[i - 1], i, stroke);
                }

			}
			return true;
		}

        public static void CalculateGradient(
            Color clr1,
            Color clr2,
            int nThickness,
            IList<Color> list)
        {
            list.Clear();
            int nWidth = nThickness;
            CanvasGradientStop[] grad_stops = new CanvasGradientStop[2];
            grad_stops[0] = new CanvasGradientStop();
            grad_stops[0].Color = clr1;
            grad_stops[0].Position = 0.0f;
            grad_stops[1] = new CanvasGradientStop();
            grad_stops[1].Color = clr2;
            grad_stops[1].Position = 1.0f;
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(device, grad_stops);
            brush.StartPoint = new Vector2(0, 0);
            brush.EndPoint = new Vector2(nWidth, 0);

            using (CanvasRenderTarget offscreen = new CanvasRenderTarget(device, nWidth, 1, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    CanvasAlphaMode.Premultiplied))
            {
                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {
                    ds.DrawRectangle(new Rect(0, 0, nWidth, 1), brush);
                }

                uint stride = 4;
                byte[] pixels = offscreen.GetPixelBytes();

                for (uint row = 0; row < 1; ++row)
                {
                    for (uint col = 0; col < nWidth; ++col)
                    {
                        uint index = (uint)(row * stride + col) * 4;
                        uint color = pixels[index];
                        Color gdiColor = Color.FromArgb(pixels[index+3], pixels[index+2], pixels[index+1], pixels[index]);
                        list.Add(gdiColor);
                    }
                }
            }
		}

        public static void CalculateCurvedGradient(
            Color clr1,
            Color clr2,
            int nThickness,
            IList<Color> list)
        {
            list.Clear();
            if (nThickness == 0)
                return;
            for (int i = 0; i < nThickness; ++i)
            {
                double degree = i / (double)(nThickness) * 90.0;
                double percent = 1.0 - Math.Sin(GetRadians(degree));
                double inv_percent = 1.0 - percent;
                int r = (int)((clr1.R * percent) + (clr2.R * inv_percent));
                byte rb = Clamp(r);
                int g = (int)((clr1.G * percent) + (clr2.G * inv_percent));
                byte gb = Clamp(g);
                int b = (int)((clr1.B * percent) + (clr2.B * inv_percent));
                byte bb = Clamp(b);
                list.Add(Color.FromArgb(0xff, rb,gb,bb));
            }
        }

        public static byte Clamp(int comp)
        {
            byte val = 0;
            if (comp < 0)
                val = 0;
            else if (comp > 255)
                val = 255;
            else
                val = (byte)comp;

            return val;
        }

        public static double GetRadians(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }
        private int m_nThickness;
        private Color m_clrText;
        private Color m_clrOutline1;
        private Color m_clrOutline2;
        private ICanvasBrush m_brushText;
        private bool m_bClrText;
        private GradientType m_GradientType;
        private bool disposed;
	}
}
