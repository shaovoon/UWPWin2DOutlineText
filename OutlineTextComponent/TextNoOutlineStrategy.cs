using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;

namespace OutlineTextComponent
{
    public sealed class TextNoOutlineStrategy : ITextStrategy
    {
		public TextNoOutlineStrategy()
		{
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
		~TextNoOutlineStrategy()
		{
			Dispose(false);
		}
        public ITextStrategy Clone()
		{
			TextNoOutlineStrategy p = new TextNoOutlineStrategy();
			if (m_bClrText)
				p.Init(m_clrText);
			else
				p.Init(m_brushText);

			return p;
		}

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public void Init(
			Color clrText)
		{
			m_clrText = clrText;
			m_bClrText = true;
		}

		public void Init(
			ICanvasBrush brushText)
		{
			m_brushText = brushText;
			m_bClrText = false;
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
        private ICanvasBrush m_brushText;
        private bool m_bClrText;
        private bool disposed;
	}
}
