using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;

namespace OutlineTextComponent
{
    public sealed class TextOnlyOutlineStrategy : ITextStrategy
    {
	    public TextOnlyOutlineStrategy()
        {
            m_nThickness=2;
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
                }

                disposed = true;
            }
        }

	    ~TextOnlyOutlineStrategy()
        {
            Dispose(false);
        }
        public ITextStrategy Clone()
        {
            TextOnlyOutlineStrategy p = new TextOnlyOutlineStrategy();
            p.Init(m_clrOutline, m_nThickness);

            return p;
        }

	    public void Init(
		    Color clrOutline, 
		    int nThickness)
        {
            m_clrOutline = clrOutline;
            m_nThickness = nThickness; 
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

                graphics.DrawGeometry(geometry, x, y, m_clrOutline, m_nThickness, stroke);
            }
            return true;
        }

        private int m_nThickness;
        private Color m_clrOutline;
        private bool disposed;
    }
}
