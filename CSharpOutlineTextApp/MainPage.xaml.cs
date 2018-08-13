﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI;
using Windows.UI.Text;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;

using Microsoft.Graphics.Canvas.Geometry;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CSharpOutlineTextApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            uint width = 0;
            uint height = 0;
            DrawOutlineText(1024, "Arial", "Hello Mandy2!", "text.png", false, width, height);
        }
        public async void DrawOutlineText(int dim, string font, string text, string saved_file, bool isShadow, uint width, uint height)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (CanvasRenderTarget offscreen = new CanvasRenderTarget(device, dim, dim, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                Microsoft.Graphics.Canvas.CanvasAlphaMode.Premultiplied))
            {
                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {

                    if (isShadow)
                        ds.Clear(Windows.UI.ColorHelper.FromArgb(0, 0, 0, 0));
                    else
                        ds.Clear(Windows.UI.ColorHelper.FromArgb(0, 255, 255, 255));

                    Windows.UI.Color shadow_color = Windows.UI.ColorHelper.FromArgb(100, 0, 0, 0);
                    Windows.UI.Color text_color = isShadow ? shadow_color : Colors.White;

                    CanvasSolidColorBrush brush = new CanvasSolidColorBrush(device, text_color);
                    CanvasTextFormat format = new CanvasTextFormat();
                    format.FontFamily = font;
                    format.FontStyle = Windows.UI.Text.FontStyle.Normal;
                    format.FontSize = 120;
                    format.FontWeight = Windows.UI.Text.FontWeights.Bold;

                    float layoutWidth = dim;
                    float layoutHeight = dim;
                    CanvasTextLayout textLayout = new CanvasTextLayout(device, text, format, layoutWidth, layoutHeight);
                    CanvasGeometry geometry = CanvasGeometry.CreateText(textLayout);

                    CanvasStrokeStyle stroke = new CanvasStrokeStyle();
                    stroke.DashStyle = CanvasDashStyle.Solid;
                    stroke.DashCap = CanvasCapStyle.Round;
                    stroke.StartCap = CanvasCapStyle.Round;
                    stroke.EndCap = CanvasCapStyle.Round;
                    stroke.LineJoin = CanvasLineJoin.Round;

                    ds.DrawGeometry(geometry, 10.0f, 10.0f, Colors.Black, 10.0f, stroke);

                    ds.FillGeometry(geometry, 10.0f, 10.0f, brush);
                }
                if (isShadow == false)
                {
                    uint stride = (uint)dim;
                    width = 0;
                    height = 0;
                    byte[] pixels = offscreen.GetPixelBytes();
                    for (uint row = 0; row < dim; ++row)
                    {
                        uint row_stride = row * stride;

                        for (uint col = 0; col < dim; ++col)
                        {
                            uint index = (row_stride + col) * 4;
                            if (pixels[index + 3] > 0)
                            {
                                if (col > width)
                                    width = col;
                                if (row > height)
                                    height = row;
                            }
                        }
                    }
                }
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.TemporaryFolder;
                txtImagePath.Text = storageFolder.Path;
                string saved_file2 = "\\";
                saved_file2 += saved_file;
                await offscreen.SaveAsync(storageFolder.Path + saved_file2);
            }
        }
    }
}
