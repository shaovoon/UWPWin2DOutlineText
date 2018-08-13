using System;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;

namespace OutlineTextComponent
{
    public sealed class TextGradOutlineStrategy : ITextStrategy
    {
		public TextGradOutlineStrategy()
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
		~TextGradOutlineStrategy()
		{
			Dispose(false);
		}
        public ITextStrategy Clone()
		{
			TextGradOutlineStrategy p = new TextGradOutlineStrategy();
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
                if (m_GradientType == GradientType.Sinusoid)
                    TextGradOutlineLastStrategy.CalculateCurvedGradient(m_clrOutline1, m_clrOutline2, m_nThickness, list);
                else
                    TextGradOutlineLastStrategy.CalculateGradient(m_clrOutline1, m_clrOutline2, m_nThickness, list);

                for (int i = m_nThickness; i >= 1; --i)
                {
                    graphics.DrawGeometry(geometry, x, y, list[i - 1], i, stroke);
                }

                if (m_bClrText)
                {
                    graphics.FillGeometry(geometry, x, y, m_clrText);
                }
                else
                    graphics.FillGeometry(geometry, x, y, m_brushText);
            }
            return true;
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
