using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using OutlineTextComponent;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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
        }
        public async Task<Size> DrawOutlineText(int dim, string font, string text, string saved_file)
        {
            Size size = new Size();
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (CanvasRenderTarget offscreen = new CanvasRenderTarget(device, dim, dim, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied))
            {
                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {
                    ds.Clear(ColorHelper.FromArgb(255, 255, 255, 255));

                    Color text_color = Colors.White;

                    CanvasSolidColorBrush brush = new CanvasSolidColorBrush(device, text_color);
                    CanvasTextFormat format = new CanvasTextFormat();
                    format.FontFamily = font;
                    format.FontStyle = Windows.UI.Text.FontStyle.Normal;
                    format.FontSize = 60;
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
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.TemporaryFolder;
                string saved_file2 = "\\";
                saved_file2 += saved_file;
                await offscreen.SaveAsync(storageFolder.Path + saved_file2);
                imgOutlineText.Source = new BitmapImage(new Uri(storageFolder.Path + saved_file2));

                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {
                    ds.Clear(Colors.White);
                }

                return size;
            }
        }

        private async void btnDrawOutlineText_Click(object sender, RoutedEventArgs e)
        {
            Size size = await DrawOutlineText(512, "Arial", "Good Morning!", "text.png");
        }

        public async Task<Size> DrawOutlineTextWithLibrary(int dim, string font, string text, string saved_file)
        {
            Size size = new Size();
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (CanvasRenderTarget offscreen = new CanvasRenderTarget(device, dim, dim, 96, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied))
            {
                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {
                    ds.Clear(Colors.White);
                }
                Color text_color = Colors.White;

                CanvasSolidColorBrush brush = new CanvasSolidColorBrush(device, text_color);
                CanvasTextFormat format = new CanvasTextFormat();
                format.FontFamily = font;
                format.FontStyle = Windows.UI.Text.FontStyle.Normal;
                format.FontSize = 60;
                format.FontWeight = Windows.UI.Text.FontWeights.Bold;

                float layoutWidth = dim;
                float layoutHeight = dim;
                CanvasTextLayout textLayout = new CanvasTextLayout(device, text, format, layoutWidth, layoutHeight);

                ITextStrategy strat = CanvasHelper.TextOutline(Colors.Blue, Colors.Black, 10);
                CanvasHelper.DrawTextImage(strat, offscreen, new Point(10.0, 10.0), textLayout);

                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.TemporaryFolder;
                string saved_file2 = "\\";
                saved_file2 += saved_file;
                await offscreen.SaveAsync(storageFolder.Path + saved_file2);
                imgOutlineText.Source = new BitmapImage(new Uri(storageFolder.Path + saved_file2));

                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {
                    ds.Clear(Colors.White);
                }

                return size;
            }
        }

        private async void btnDrawOutlineTextWithLibrary_Click(object sender, RoutedEventArgs e)
        {
            Size size = await DrawOutlineTextWithLibrary(512, "Arial", "Hello World!", "text2.png");
        }

        public async Task<Size> DrawVacationText(int dim, string font, string text, string saved_file)
        {
            Size size = new Size();
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (CanvasRenderTarget offscreen = CanvasHelper.GenImage(dim, dim, Colors.White))
            {
                CanvasTextFormat format = new CanvasTextFormat();
                format.FontFamily = font;
                format.FontStyle = Windows.UI.Text.FontStyle.Normal;
                format.FontSize = 52;
                format.FontWeight = Windows.UI.Text.FontWeights.Black;

                float layoutWidth = dim;
                float layoutHeight = dim;
                CanvasTextLayout textLayout = new CanvasTextLayout(device, text, format, layoutWidth, layoutHeight);

                Color light_purple = Color.FromArgb(255, 102, 159, 206);
                Color dark_purple = Color.FromArgb(255, 35, 68, 95);
                Point pt = new Point(10.0, 10.0);
                using (var strategyOutline3 = CanvasHelper.TextGradOutline(light_purple, dark_purple, light_purple, 9, GradientType.Linear))
                {
                    CanvasHelper.DrawTextImage(strategyOutline3, offscreen, pt, textLayout);
                }

                CanvasRenderTarget maskOutline2;
                using (var strategyOutline2 = CanvasHelper.TextNoOutline(MaskColor.Blue))
                {
                    maskOutline2 = CanvasHelper.GenMask(strategyOutline2, dim, dim, pt, textLayout);
                }
                Color light_yellow = Color.FromArgb(255, 255, 227, 85);
                Color dark_yellow = Color.FromArgb(255, 243, 163, 73);
                using (CanvasRenderTarget text_image = CanvasHelper.GenImage(dim, dim, dark_yellow))
                {
                    using (var strategyText2 = CanvasHelper.TextGradOutlineLast(light_yellow, dark_yellow, light_yellow, 9, GradientType.Sinusoid))
                    {
                        CanvasHelper.DrawTextImage(strategyText2, text_image, pt, textLayout);
                        CanvasHelper.ApplyImageToMask(text_image, maskOutline2, offscreen, MaskColor.Blue, true);
                    }
                }

                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.TemporaryFolder;
                string saved_file2 = "\\";
                saved_file2 += saved_file;
                await offscreen.SaveAsync(storageFolder.Path + saved_file2);
                imgOutlineText.Source = new BitmapImage(new Uri(storageFolder.Path + saved_file2));

                return size;
            }
        }

        private async void btnDrawVacationText_Click(object sender, RoutedEventArgs e)
        {
            Size size = await DrawVacationText(512, "Arial", "VACATION", "vacation.png");
        }
    }
}
