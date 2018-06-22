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
        SolidColorBrush foreGroundColor;
        SolidColorBrush backGroundColor;
        MouseButtonEventHandler mouseUpEvent;
        MouseEventHandler mouseMoveEvent;
        MouseButtonEventHandler startDrawEvent;
        Dictionary<String, SolidColorBrush> colorDict;

        // 두께 설정 상수
        const int THIN = 2;
        const int THICK = 6;
        const int MORE_THICK = 10;
        int thickness = THIN;

        object DrawingObject;

        public MainWindow()
        {
            InitializeComponent();
            paintedShape = new List<object>();
            foreGroundColor = Brushes.Black;
            backGroundColor = Brushes.White;
            mouseUpEvent = new MouseButtonEventHandler(FinishDraw);
            mouseMoveEvent = new MouseEventHandler(MoveMouse);
            startDrawEvent = new MouseButtonEventHandler(StartDraw);
            InitColor();
        }

        public void InitColor()
        {
            colorDict = new Dictionary<string, SolidColorBrush>();
            colorDict["Black"] = Brushes.Black;
            colorDict["White"] = Brushes.White;
            colorDict["Red"] = Brushes.Red;
            colorDict["Orange"] = Brushes.Orange;
            colorDict["Yellow"] = Brushes.Yellow;
            colorDict["Gray"] = Brushes.Gray;
            colorDict["DimGray"] = Brushes.DimGray;
            colorDict["Pink"] = Brushes.Pink;
            colorDict["Beige"] = Brushes.Beige;
            colorDict["Green"] = Brushes.Green;
            colorDict["GreenYellow"] = Brushes.GreenYellow;
            colorDict["SkyBlue"] = Brushes.SkyBlue;
            colorDict["LightCoral"] = Brushes.LightCoral;
            colorDict["HotPink"] = Brushes.HotPink;
            colorDict["Brown"] = Brushes.Brown;
        }

        public Point GetCurrentPoint()
        {
            Mouse.Capture(Canvas);
            return Mouse.GetPosition(Canvas);
        }

        public void ReadyToDraw(object shape)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            DrawingObject = shape;
            Canvas.MouseDown += startDrawEvent;
        }

        public void StartDraw(object sender, MouseButtonEventArgs e)
        {
            Point point = GetCurrentPoint();
            // 선 그리기
            if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Line")) {
                Line line = (Line)DrawingObject;
                line.X1 = point.X;
                line.Y1 = point.Y;
                line.X2 = point.X;
                line.Y2 = point.Y;
                Canvas.Children.Add(line);
            }
            Canvas.MouseUp += mouseUpEvent;
            Canvas.MouseMove += mouseMoveEvent;
        }

        // 그리기가 끝났을 때 발생하는 이벤트
        public void FinishDraw(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Canvas.MouseUp -= mouseUpEvent;
            Canvas.MouseDown -= startDrawEvent;
            Canvas.MouseMove -= mouseMoveEvent;
            paintedShape.Add(DrawingObject);
            DrawingObject = null;
        }

        public void MoveMouse(object sender, MouseEventArgs e)
        {
            Point point = GetCurrentPoint();
            
            /* 원래의 타입으로 변환*/

            // 선 그리기
           if(DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Line")){
                Line line = (Line)DrawingObject;
                line.X2 = point.X;
                line.Y2 = point.Y;
                Canvas.Children.Remove(line);
                Canvas.Children.Add(line);
            }
        }
        
        public void DrawLine(object sender, RoutedEventArgs e)
        {
            Line line = new Line();
            ReadyToDraw(line);
            line.Stroke = foreGroundColor;
            line.StrokeThickness = thickness;
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
        
        public void ChangeColor(object sender, RoutedEventArgs e)
        {
            Button colorButton = (Button)sender;
            String color = colorButton.Name.Substring(4);
            foreGroundColor = colorDict[color];
            foreColor.Background = foreGroundColor;
        }

        private void Btn_Thickness_Click(object sender, RoutedEventArgs e)
        {
            thickness += 4;
            if (thickness > MORE_THICK)
                thickness = THIN;

            switch (thickness )
            {
                case THIN:
                    thickIcon.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,/images/thin.jpg"));
                    break;
                case THICK:
                    thickIcon.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,/images/thick.jpg"));
                    break;
                case MORE_THICK:
                    thickIcon.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,/images/morethick.jpg"));
                    break;
            }
        }
    }
}
