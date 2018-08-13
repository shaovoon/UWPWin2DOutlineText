using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.Foundation;
using System.Collections.Generic;
using System.Numerics;

namespace OutlineTextComponent
{
    public sealed class Canvas
    {
        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static ITextStrategy TextGlow(
            Color clrText,
            Color clrOutline,
            int nThickness)
        {
            TextGlowStrategy strat = new TextGlowStrategy();
            strat.Init(clrText, clrOutline, nThickness);

            return strat;
        }

        public static ITextStrategy TextGlow(
            ICanvasBrush brushText,
            Color clrOutline,
            int nThickness)
        {
            TextGlowStrategy strat = new TextGlowStrategy();
            strat.Init(brushText, clrOutline, nThickness);

            return strat;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static ITextStrategy TextOutline(
            Color clrText,
            Color clrOutline,
            int nThickness)
        {
            TextOutlineStrategy strat = new TextOutlineStrategy();
            strat.Init(clrText, clrOutline, nThickness);

            return strat;
        }

        public static ITextStrategy TextOutline(
            ICanvasBrush brushText,
            Color clrOutline,
            int nThickness)
        {
            TextOutlineStrategy strat = new TextOutlineStrategy();
            strat.Init(brushText, clrOutline, nThickness);

            return strat;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static ITextStrategy TextGradOutline(
            Color clrText,
            Color clrOutline1,
            Color clrOutline2,
            int nThickness,
            GradientType gradType)
        {
            TextGradOutlineStrategy strat = new TextGradOutlineStrategy();
            strat.Init(clrText, clrOutline1, clrOutline2, nThickness, gradType);

            return strat;
        }

        public static ITextStrategy TextGradOutline(
            ICanvasBrush brushText,
            Color clrOutline1,
            Color clrOutline2,
            int nThickness,
            GradientType gradType)
        {
            TextGradOutlineStrategy strat = new TextGradOutlineStrategy();
            strat.Init(brushText, clrOutline1, clrOutline2, nThickness, gradType);

            return strat;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static ITextStrategy TextGradOutlineLast(
            Color clrText,
            Color clrOutline1,
            Color clrOutline2,
            int nThickness,
            GradientType gradType)
        {
            TextGradOutlineLastStrategy strat = new TextGradOutlineLastStrategy();
            strat.Init(clrText, clrOutline1, clrOutline2, nThickness, gradType);

            return strat;
        }

        public static ITextStrategy TextGradOutlineLast(
            ICanvasBrush brushText,
            Color clrOutline1,
            Color clrOutline2,
            int nThickness,
            GradientType gradType)
        {
            TextGradOutlineLastStrategy strat = new TextGradOutlineLastStrategy();
            strat.Init(brushText, clrOutline1, clrOutline2, nThickness, gradType);

            return strat;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static ITextStrategy TextNoOutline(
            Color clrText)
        {
            TextNoOutlineStrategy strat = new TextNoOutlineStrategy();
            strat.Init(clrText);

            return strat;
        }

        public static ITextStrategy TextNoOutline(
            ICanvasBrush brushText)
        {
            TextNoOutlineStrategy strat = new TextNoOutlineStrategy();
            strat.Init(brushText);

            return strat;
        }

        public static ITextStrategy TextOnlyOutline(
            Color clrOutline,
            int nThickness)
        {
            TextOnlyOutlineStrategy strat = new TextOnlyOutlineStrategy();
            strat.Init(clrOutline, nThickness);

            return strat;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static CanvasRenderTarget GenImage(int width, int height)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            return new CanvasRenderTarget(device, width, height, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Premultiplied);
        }

        public static CanvasRenderTarget GenImage(int width, int height, IList<Color> colors, bool horizontal)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            CanvasGradientStop[] grad_stops = new CanvasGradientStop[colors.Count];

            for (int i=0; i<colors.Count; ++i)
            {
                grad_stops[i] = new CanvasGradientStop();
                grad_stops[i].Color = colors[i];
                grad_stops[i].Position = i*(float)(1.0/(colors.Count-1));
            }

            CanvasRenderTarget offscreen = new CanvasRenderTarget(device, width, height, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);

            CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(device, grad_stops);
            if(horizontal)
            {
                brush.StartPoint = new Vector2(0, 0);
                brush.EndPoint = new Vector2(offscreen.SizeInPixels.Width, 0);
            }
            else
            {
                brush.StartPoint = new Vector2(0, 0);
                brush.EndPoint = new Vector2(0, offscreen.SizeInPixels.Height);
            }

            using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
            {
                ds.FillRectangle(new Rect(0.0, 0.0, offscreen.SizeInPixels.Width, offscreen.SizeInPixels.Height), brush);
            }

            return offscreen;
        }

        public static CanvasRenderTarget GenImage(int width, int height, Color clr)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            CanvasRenderTarget offscreen = new CanvasRenderTarget(device, width, height, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);

            byte[] pixels = offscreen.GetPixelBytes();

            if (pixels == null)
                return null;

            uint col = 0;
            uint stride = offscreen.SizeInPixels.Width;
            for (uint row = 0; row < offscreen.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                for (col = 0; col < offscreen.SizeInPixels.Width; ++col)
                {
                    uint index = (total_row_len + col) * 4;

                    pixels[index + 3] = clr.A;
                    pixels[index + 2] = clr.R;
                    pixels[index + 1] = clr.G;
                    pixels[index] = clr.B;
                }
            }
            offscreen.SetPixelBytes(pixels);

            return offscreen;
        }

        [Windows.Foundation.Metadata.DefaultOverloadAttribute]
        public static CanvasRenderTarget GenImage(int width, int height, Color clr, byte alpha)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            CanvasRenderTarget offscreen = new CanvasRenderTarget(device, width, height, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);
            byte[] pixels = offscreen.GetPixelBytes();

            if (pixels == null)
                return null;

            uint col = 0;
            uint stride = offscreen.SizeInPixels.Width;
            for (uint row = 0; row < offscreen.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                for (col = 0; col < offscreen.SizeInPixels.Width; ++col)
                {
                    uint index = (total_row_len + col)*4;

                    pixels[index + 3] = alpha;
                    pixels[index + 2] = clr.R;
                    pixels[index + 1] = clr.G;
                    pixels[index] = clr.B;
                }
            }
            offscreen.SetPixelBytes(pixels);

            return offscreen;
        }
        public static CanvasRenderTarget GenImage(int width, int height, CanvasLinearGradientBrush brush, byte alpha)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            CanvasRenderTarget offscreen = new CanvasRenderTarget(device, width, height, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);

            using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
            {
                ds.FillRectangle(new Rect(0.0, 0.0, offscreen.SizeInPixels.Width, offscreen.SizeInPixels.Height), brush);
            }

            byte[] pixels = offscreen.GetPixelBytes();

            if (pixels == null)
                return null;

            uint col = 0;
            uint stride = offscreen.SizeInPixels.Width;
            for (uint row = 0; row < offscreen.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                for (col = 0; col < offscreen.SizeInPixels.Width; ++col)
                {
                    uint index = (total_row_len + col)*4;

                    pixels[index + 3] = alpha;
                }
            }
            offscreen.SetPixelBytes(pixels);

            return offscreen;
        }

        public static CanvasRenderTarget GenMask(
            ITextStrategy strategy,
            int width,
            int height,
            Point offset,
            CanvasTextLayout text_layout)
        {
            if (strategy == null || text_layout == null)
                return null;

            CanvasDevice device = CanvasDevice.GetSharedDevice();

            CanvasRenderTarget offscreen = new CanvasRenderTarget(device, width, height, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);

            using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
            {
                strategy.DrawString(ds, text_layout, (float)offset.X, (float)offset.Y);
            }
            
            return offscreen;
        }

        public static bool MeasureMaskLength(
            CanvasRenderTarget mask,
            Color maskColor,
            out uint top,
            out uint left,
            out uint bottom,
            out uint right)
        {
            top = 30000;
            left = 30000;
            bottom = 0;
            right = 0;

            if (mask == null)
                return false;

            byte[] pixelsMask = mask.GetPixelBytes();

            if (pixelsMask == null)
                    return false;

            uint col = 0;
            uint stride = mask.SizeInPixels.Width;
            for (uint row = 0; row < mask.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                for (col = 0; col < mask.SizeInPixels.Width; ++col)
                {
                    uint index = total_row_len + col;
                    byte nAlpha = 0;

                    if (MaskColor.IsEqual(maskColor, MaskColor.Red))
                        nAlpha = pixelsMask[index + 2];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Green))
                        nAlpha = pixelsMask[index + 1];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Blue))
                        nAlpha = pixelsMask[index];

                    if (nAlpha > 0)
                    {
                        if (col < left)
                            left = col;
                        if (row < top)
                            top = row;
                        if (col > right)
                            right = col;
                        if (row > bottom)
                            bottom = row;

                    }
                }
            }

            return true;
        }

        public static bool ApplyImageToMask(
            CanvasRenderTarget image,
            CanvasRenderTarget mask,
            CanvasRenderTarget canvas,
            Color maskColor,
            bool NoAlphaAtBoundary)
        {
            if (image == null || mask == null || canvas == null)
                return false;

            byte[] pixelsImage = image.GetPixelBytes();
            byte[] pixelsMask = mask.GetPixelBytes();
            byte[] pixelsCanvas = canvas.GetPixelBytes();
            if (pixelsImage == null || pixelsMask == null || pixelsCanvas == null)
                return false;

            uint col = 0;
            uint image_stride = image.SizeInPixels.Width;
            uint stride = canvas.SizeInPixels.Width;
            uint mask_stride = mask.SizeInPixels.Width;
            for (uint row = 0; row < canvas.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                uint total_row_mask_len = (uint)(row * (mask_stride >> 2));
                uint total_row_image_len = (uint)(row * (image_stride >> 2));
                for (col = 0; col < canvas.SizeInPixels.Width; ++col)
                {
                    if (row >= image.SizeInPixels.Height || col >= image.SizeInPixels.Width)
                        continue;
                    if (row >= mask.SizeInPixels.Height || col >= mask.SizeInPixels.Width)
                        continue;

                    uint index = (total_row_len + col) * 4;
                    uint indexMask = (total_row_mask_len + col) * 4;
                    uint indexImage = (total_row_image_len + col) * 4;

                    byte maskByte = 0;

                    if (MaskColor.IsEqual(maskColor, MaskColor.Red))
                        maskByte = pixelsMask[indexMask + 2];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Green))
                        maskByte = pixelsMask[indexMask + 1];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Blue))
                        maskByte = pixelsMask[indexMask];

                    if (maskByte > 0)
                    {
                        uint pixels_canvas = (uint)((pixelsCanvas[index + 3] << 24) | (pixelsCanvas[index + 2] << 16) | (pixelsCanvas[index + 1] << 8) | pixelsCanvas[index]);
                        uint pixels_image = (uint)((pixelsImage[index + 3] << 24) | (pixelsImage[index + 2] << 16) | (pixelsImage[index + 1] << 8) | pixelsImage[index]);
                        if (NoAlphaAtBoundary)
                        {
                            uint result = AlphablendNoAlphaAtBoundary(pixels_canvas, pixels_image, pixelsMask[indexMask + 3], pixelsMask[indexMask + 3]);
                            pixelsCanvas[index + 3] = (byte)((result & 0xff000000) >> 24);
                            pixelsCanvas[index + 2] = (byte)((result & 0xff0000) >> 16);
                            pixelsCanvas[index + 1] = (byte)((result & 0xff00) >> 8);
                            pixelsCanvas[index] = (byte)(result & 0xff);
                        }
                        else
                        {
                            uint result = Alphablend(pixels_canvas, pixels_image, pixelsMask[indexMask + 3], pixelsMask[indexMask + 3]);
                            pixelsCanvas[index + 3] = (byte)((result & 0xff000000) >> 24);
                            pixelsCanvas[index + 2] = (byte)((result & 0xff0000) >> 16);
                            pixelsCanvas[index + 1] = (byte)((result & 0xff00) >> 8);
                            pixelsCanvas[index] = (byte)(result & 0xff);
                        }
                    }
                }
            }
            canvas.SetPixelBytes(pixelsCanvas);

            return true;
        }

        public static bool ApplyColorToMask(
            Color clr,
            CanvasRenderTarget mask,
            CanvasRenderTarget canvas,
            Color maskColor)
        {
            if (mask == null || canvas == null)
                return false;

            byte[] pixelsMask = mask.GetPixelBytes();
            byte[] pixelsCanvas = canvas.GetPixelBytes();

            if (pixelsMask == null || pixelsCanvas == null)
                return false;

            uint col = 0;
            uint stride = canvas.SizeInPixels.Width;
            uint mask_stride = mask.SizeInPixels.Width;
            for (uint row = 0; row < canvas.SizeInPixels.Height; ++row)
            {
                uint total_row_len = row * stride;
                uint total_row_mask_len = (uint)(row * (mask_stride >> 2));
                for (col = 0; col < canvas.SizeInPixels.Width; ++col)
                {
                    if (row >= mask.SizeInPixels.Height || col >= mask.SizeInPixels.Width)
                        continue;

                    uint index = (total_row_len + col) * 4;
                    uint indexMask = (total_row_mask_len + col) * 4;

                    byte maskByte = 0;

                    if (MaskColor.IsEqual(maskColor, MaskColor.Red))
                        maskByte = pixelsMask[indexMask + 2];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Green))
                        maskByte = pixelsMask[indexMask + 1];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Blue))
                        maskByte = pixelsMask[indexMask];

                    uint color = (uint)(0xff << 24 | clr.R << 16 | clr.G << 8 | clr.B);

                    if (maskByte > 0)
                    {
                        uint pixels_canvas = (uint)((pixelsCanvas[index + 3] << 24) | (pixelsCanvas[index + 2] << 16) | (pixelsCanvas[index + 1] << 8) | pixelsCanvas[index]);

                        uint result = Alphablend(pixels_canvas, color, pixelsMask[indexMask + 3], pixelsMask[indexMask + 3]);
                        pixelsCanvas[index + 3] = (byte)((result & 0xff000000) >> 24);
                        pixelsCanvas[index + 2] = (byte)((result & 0xff0000) >> 16);
                        pixelsCanvas[index + 1] = (byte)((result & 0xff00) >> 8);
                        pixelsCanvas[index] = (byte)(result & 0xff);
                    }
                }
            }
            canvas.SetPixelBytes(pixelsCanvas);

            return true;
        }

        public static bool ApplyColorToMask(
            Color clr,
            CanvasRenderTarget mask,
            CanvasRenderTarget canvas,
            Color maskColor,
            Point offset)
        {
            if (mask == null || canvas == null)
                return false;

            byte[] pixelsMask = mask.GetPixelBytes();
            byte[] pixelsCanvas = canvas.GetPixelBytes();

            if (pixelsMask == null || pixelsCanvas == null)
                return false;

            uint col = 0;
            uint stride = canvas.SizeInPixels.Width;
            uint mask_stride = mask.SizeInPixels.Width;
            for (uint row = 0; row < canvas.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                uint total_row_mask_len = (uint)((row - offset.Y) * (mask_stride >> 2));
                for (col = 0; col < canvas.SizeInPixels.Width; ++col)
                {
                    if ((row - offset.Y) >= mask.SizeInPixels.Height || (col - offset.X) >= mask.SizeInPixels.Width ||
                        (row - offset.Y) < 0 || (col - offset.X) < 0)
                        continue;

                    uint index = (total_row_len + col) * 4;
                    uint indexMask = (uint)((total_row_mask_len + (col - offset.X)) * 4);

                    byte maskByte = 0;

                    if (MaskColor.IsEqual(maskColor, MaskColor.Red))
                        maskByte = pixelsMask[indexMask + 2];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Green))
                        maskByte = pixelsMask[indexMask + 1];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Blue))
                        maskByte = pixelsMask[indexMask];

                    uint color = (uint)(0xff << 24 | clr.R << 16 | clr.G << 8 | clr.B);

                    if (maskByte > 0)
                    {
                        uint pixels_canvas = (uint)((pixelsCanvas[index + 3] << 24) | (pixelsCanvas[index + 2] << 16) | (pixelsCanvas[index + 1] << 8) | pixelsCanvas[index]);

                        uint result = Alphablend(pixels_canvas, color, pixelsMask[indexMask + 3], pixelsMask[indexMask + 3]);
                        pixelsCanvas[index + 3] = (byte)((result & 0xff000000) >> 24);
                        pixelsCanvas[index + 2] = (byte)((result & 0xff0000) >> 16);
                        pixelsCanvas[index + 1] = (byte)((result & 0xff00) >> 8);
                        pixelsCanvas[index] = (byte)(result & 0xff);
                    }
                }
            }
            canvas.SetPixelBytes(pixelsCanvas);

            return true;
        }

        public static bool ApplyShadowToMask(
            Color clrShadow,
            CanvasRenderTarget mask,
            CanvasRenderTarget canvas,
            Color maskColor)
        {
            if (mask == null || canvas == null)
                return false;

            byte[] pixelsMask = mask.GetPixelBytes();
            byte[] pixelsCanvas = canvas.GetPixelBytes();

            if (pixelsMask == null || pixelsCanvas == null)
                    return false;

            uint col = 0;
            uint stride = canvas.SizeInPixels.Width;
            uint mask_stride = mask.SizeInPixels.Width;
            for (uint row = 0; row < canvas.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                uint total_row_mask_len = (uint)(row * (mask_stride >> 2));
                for (col = 0; col < canvas.SizeInPixels.Width; ++col)
                {
                    if (row >= mask.SizeInPixels.Height || col >= mask.SizeInPixels.Width)
                        continue;

                    uint index = (total_row_len + col) * 4;
                    uint indexMask = (total_row_mask_len + col) * 4;

                    byte maskByte = 0;

                    if (MaskColor.IsEqual(maskColor, MaskColor.Red))
                        maskByte = pixelsMask[indexMask + 2];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Green))
                        maskByte = pixelsMask[indexMask + 1];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Blue))
                        maskByte = pixelsMask[indexMask];

                    uint color = (uint)(0xff << 24 | clrShadow.R << 16 | clrShadow.G << 8 | clrShadow.B);

                    if (maskByte > 0)
                    {
                        uint pixels_canvas = (uint)((pixelsCanvas[index + 3] << 24) | (pixelsCanvas[index + 2] << 16) | (pixelsCanvas[index + 1] << 8) | pixelsCanvas[index]);

                        uint result = Alphablend(pixels_canvas, color, pixelsMask[indexMask + 3], clrShadow.A);
                        pixelsCanvas[index + 3] = (byte)((result & 0xff000000) >> 24);
                        pixelsCanvas[index + 2] = (byte)((result & 0xff0000) >> 16);
                        pixelsCanvas[index + 1] = (byte)((result & 0xff00) >> 8);
                        pixelsCanvas[index] = (byte)(result & 0xff);

                    }
                }
            }
            canvas.SetPixelBytes(pixelsCanvas);

            return true;
        }

        public static bool ApplyShadowToMask(
            Color clrShadow,
            CanvasRenderTarget mask,
            CanvasRenderTarget canvas,
            Color maskColor,
            Point offset)
        {
            if (mask == null || canvas == null)
                return false;

            byte[] pixelsMask = mask.GetPixelBytes();
            byte[] pixelsCanvas = canvas.GetPixelBytes();

            if (pixelsMask == null || pixelsCanvas == null)
                return false;

            uint col = 0;
            uint stride = canvas.SizeInPixels.Width;
            uint mask_stride = mask.SizeInPixels.Width;
            for (uint row = 0; row < canvas.SizeInPixels.Height; ++row)
            {
                uint total_row_len = (uint)(row * stride);
                uint total_row_mask_len = (uint)((row - offset.Y) * (mask_stride >> 2));
                for (col = 0; col < canvas.SizeInPixels.Width; ++col)
                {
                    if ((row - offset.Y) >= mask.SizeInPixels.Height || (col - offset.X) >= mask.SizeInPixels.Width ||
                        (row - offset.Y) < 0 || (col - offset.X) < 0)
                        continue;

                    uint index = (total_row_len + col) * 4;
                    uint indexMask = (uint)((total_row_mask_len + (col - offset.X)) * 4);

                    byte maskByte = 0;

                    if (MaskColor.IsEqual(maskColor, MaskColor.Red))
                        maskByte = pixelsMask[indexMask + 2];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Green))
                        maskByte = pixelsMask[indexMask + 1];
                    else if (MaskColor.IsEqual(maskColor, MaskColor.Blue))
                        maskByte = pixelsMask[indexMask];

                    uint color = (uint)(0xff << 24 | clrShadow.R << 16 | clrShadow.G << 8 | clrShadow.B);

                    if (maskByte > 0)
                    {
                        uint pixels_canvas = (uint)((pixelsCanvas[index + 3] << 24) | (pixelsCanvas[index + 2] << 16) | (pixelsCanvas[index + 1] << 8) | pixelsCanvas[index]);

                        uint result = Alphablend(pixels_canvas, color, pixelsMask[indexMask + 3], clrShadow.A);
                        pixelsCanvas[index + 3] = (byte)((result & 0xff000000) >> 24);
                        pixelsCanvas[index + 2] = (byte)((result & 0xff0000) >> 16);
                        pixelsCanvas[index + 1] = (byte)((result & 0xff00) >> 8);
                        pixelsCanvas[index] = (byte)(result & 0xff);

                    }
                }
            }
            canvas.SetPixelBytes(pixelsCanvas);

            return true;
        }

        public static bool DrawTextImage(
            ITextStrategy strategy,
            CanvasRenderTarget image,
            Point offset,
            CanvasTextLayout text_layout)
        {
            if (strategy == null || image == null || text_layout == null)
                return false;

            bool bRet = false;
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            using (CanvasDrawingSession ds = image.CreateDrawingSession())
            {
                strategy.DrawString(ds, text_layout, (float)offset.X, (float)offset.Y);
            }

            return bRet;

        }

        private static UInt32 AddAlpha(UInt32 dest, UInt32 source, Byte nAlpha)
        {
            if (0 == nAlpha)
                return dest;

            if (255 == nAlpha)
                return source;

            Byte nSrcRed = (Byte)((source & 0xff0000) >> 16);
            Byte nSrcGreen = (Byte)((source & 0xff00) >> 8);
            Byte nSrcBlue = (Byte)((source & 0xff));

            return (UInt32)(nAlpha << 24 | nSrcRed << 16 | nSrcGreen << 8 | nSrcBlue);
        }

        private static UInt32 AlphablendNoAlphaAtBoundary(UInt32 dest, UInt32 source, Byte nAlpha, Byte nAlphaFinal)
        {
            Byte nInvAlpha = (Byte)(~nAlpha);

            Byte nSrcRed = (Byte)((source & 0xff0000) >> 16);
            Byte nSrcGreen = (Byte)((source & 0xff00) >> 8);
            Byte nSrcBlue = (Byte)((source & 0xff));

            Byte nDestRed = (Byte)((dest & 0xff0000) >> 16);
            Byte nDestGreen = (Byte)((dest & 0xff00) >> 8);
            Byte nDestBlue = (Byte)(dest & 0xff);

            Byte nRed = (Byte)((nSrcRed * nAlpha + nDestRed * nInvAlpha) >> 8);
            Byte nGreen = (Byte)((nSrcGreen * nAlpha + nDestGreen * nInvAlpha) >> 8);
            Byte nBlue = (Byte)((nSrcBlue * nAlpha + nDestBlue * nInvAlpha) >> 8);

            return (UInt32)(0xff << 24 | nRed << 16 | nGreen << 8 | nBlue);
        }

        private static UInt32 Alphablend(UInt32 dest, UInt32 source, Byte nAlpha, Byte nAlphaFinal)
        {
            Byte nInvAlpha = (Byte)(~nAlpha);

            Byte nSrcRed = (Byte)((source & 0xff0000) >> 16);
            Byte nSrcGreen = (Byte)((source & 0xff00) >> 8);
            Byte nSrcBlue = (Byte)((source & 0xff));

            Byte nDestRed = (Byte)((dest & 0xff0000) >> 16);
            Byte nDestGreen = (Byte)((dest & 0xff00) >> 8);
            Byte nDestBlue = (Byte)(dest & 0xff);

            Byte nRed = (Byte)((nSrcRed * nAlpha + nDestRed * nInvAlpha) >> 8);
            Byte nGreen = (Byte)((nSrcGreen * nAlpha + nDestGreen * nInvAlpha) >> 8);
            Byte nBlue = (Byte)((nSrcBlue * nAlpha + nDestBlue * nInvAlpha) >> 8);

            return (UInt32)(nAlphaFinal << 24 | nRed << 16 | nGreen << 8 | nBlue);
        }
        private static UInt32 PreMultipliedAlphablend(UInt32 dest, UInt32 source)
        {
            Byte Alpha = (Byte)((source & 0xff000000) >> 24);
            Byte nInvAlpha = (Byte)(255 - Alpha);

            Byte nSrcRed = (Byte)((source & 0xff0000) >> 16);
            Byte nSrcGreen = (Byte)((source & 0xff00) >> 8);
            Byte nSrcBlue = (Byte)((source & 0xff));

            Byte nDestRed = (Byte)((dest & 0xff0000) >> 16);
            Byte nDestGreen = (Byte)((dest & 0xff00) >> 8);
            Byte nDestBlue = (Byte)(dest & 0xff);

            Byte nRed = (Byte)(nSrcRed + ((nDestRed * nInvAlpha) >> 8));
            Byte nGreen = (Byte)(nSrcGreen + ((nDestGreen * nInvAlpha) >> 8));
            Byte nBlue = (Byte)(nSrcBlue + ((nDestBlue * nInvAlpha) >> 8));

            return (UInt32)(0xff << 24 | nRed << 16 | nGreen << 8 | nBlue);
        }

    }
}
