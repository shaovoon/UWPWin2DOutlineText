using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;

namespace OutlineTextComponent
{
    public sealed class TextOutlineStrategy : ITextStrategy
    {
		public TextOutlineStrategy()
		{
			m_nThickness=2;
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

		~TextOutlineStrategy()
		{
			Dispose(false);
		}
        public ITextStrategy Clone()
		{
			TextOutlineStrategy p = new TextOutlineStrategy();
			if (m_bClrText)
				p.Init(m_clrText, m_clrOutline, m_nThickness);
			else
				p.Init(m_brushText, m_clrOutline, m_nThickness);

			return p;
		}

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public void Init(
			Color clrText, 
			Color clrOutline, 
			int nThickness )
		{
			m_clrText = clrText;
			m_bClrText = true;
			m_clrOutline = clrOutline;
			m_nThickness = nThickness; 
		}

		public void Init(
            ICanvasBrush brushText,
			Color clrOutline,
			int nThickness)
		{
			m_brushText = brushText;
			m_bClrText = false;
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
        private Color m_clrOutline;
        private ICanvasBrush m_brushText;
        private bool m_bClrText;
        private bool disposed;
	}
}
