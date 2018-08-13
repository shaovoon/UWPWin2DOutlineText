using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;

namespace OutlineTextComponent
{
    public sealed class TextDblGlowStrategy : ITextStrategy
    {
	    public TextDblGlowStrategy()
        {
            m_nThickness1=2;
            m_nThickness2=2;
            m_brushText = null;
            m_bClrText = true;
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
        ~TextDblGlowStrategy()
        {
            Dispose(false);
        }
        public ITextStrategy Clone()
        {
            TextDblGlowStrategy p = new TextDblGlowStrategy();
            if (m_bClrText)
                p.Init(m_clrText, m_clrOutline1, m_clrOutline2, m_nThickness1, m_nThickness2);
            else
                p.Init(m_brushText, m_clrOutline1, m_clrOutline2, m_nThickness1, m_nThickness2);

            return p;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public void Init(
		    Color clrText, 
		    Color clrOutline1, 
		    Color clrOutline2, 
		    int nThickness1,
		    int nThickness2 )
        {
            m_clrText = clrText;
            m_bClrText = true;
            m_clrOutline1 = clrOutline1;
            m_clrOutline2 = clrOutline2;
            m_nThickness1 = nThickness1; 
            m_nThickness2 = nThickness2; 
        }

        public void Init(
            ICanvasBrush brushText,
            Color clrOutline1,
            Color clrOutline2,
            int nThickness1,
            int nThickness2)
        {
            m_brushText = brushText;
            m_bClrText = false;
            m_clrOutline1 = clrOutline1;
            m_clrOutline2 = clrOutline2;
            m_nThickness1 = nThickness1;
            m_nThickness2 = nThickness2;
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

                for (int i = m_nThickness1; i <= m_nThickness1 + m_nThickness2; ++i)
                {
                    graphics.DrawGeometry(geometry, x, y, m_clrOutline2, i, stroke);
                }
                graphics.DrawGeometry(geometry, x, y, m_clrOutline1, m_nThickness1, stroke);
                if (m_bClrText)
                {
                    graphics.FillGeometry(geometry, x, y, m_clrText);
                }
                else
                    graphics.FillGeometry(geometry, x, y, m_brushText);
            }
            return true;
        }

	    private Color m_clrText;
        private Color m_clrOutline1;
        private Color m_clrOutline2;
        private int m_nThickness1;
        private int m_nThickness2;
        private ICanvasBrush m_brushText;
        private bool m_bClrText;
        private bool disposed;
    }
}
