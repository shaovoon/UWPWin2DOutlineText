using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;

namespace OutlineTextComponent
{
    public interface ITextStrategy : IDisposable
    {
       	ITextStrategy Clone();

        bool DrawString(
            CanvasDrawingSession graphics,
            CanvasTextLayout textLayout,
            float x, float y);
    }
}
