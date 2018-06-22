using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Painter
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<object> paintedShape;
        

        public MainWindow()
        {
            InitializeComponent();
            paintedShape = new List<object>();
        }

        public void DrawLine(object sender, RoutedEventArgs e)
        {
            Line line = new Line();
        }

        // Drag정도에 따라 캔버스 크기 조정
        private void Canvas_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = Canvas.Height + e.VerticalChange / 10;
            double xadjust = Canvas.Width + e.HorizontalChange / 10;

            if ((xadjust >= 100) && (yadjust >= 100))
            {
                Canvas.Width = xadjust;
                Canvas.Height = yadjust;
                Canvas.SetLeft(CanvasThumb, Canvas.GetLeft(CanvasThumb) +
                                        e.HorizontalChange);
                Canvas.SetTop(CanvasThumb, Canvas.GetTop(CanvasThumb) +
                                        e.VerticalChange);
            }
        }

        private void Canvas_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            CanvasThumb.Background = Brushes.Orange;
        }

        private void Canvas_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            CanvasThumb.Background = Brushes.LightGray;
        }

    }
}
