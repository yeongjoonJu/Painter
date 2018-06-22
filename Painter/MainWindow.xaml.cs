using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
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
        Point priorPoint, startPoint;

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
            Mouse.Capture(paintCanvas);
            return Mouse.GetPosition(paintCanvas);
        }

        public void ReadyToDraw(object shape)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            if (shape.GetType().ToString().Equals("System.Windows.Shapes.Path"))
                Mouse.OverrideCursor = Cursors.Pen;
            DrawingObject = shape;
            paintCanvas.MouseDown += startDrawEvent;
        }

        public void StartDraw(object sender, MouseButtonEventArgs e)
        {
            Point point = GetCurrentPoint();
            priorPoint = point;
            // 선 그리기
            if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Line")) {
                Line line = (Line)DrawingObject;
                line.X1 = point.X;
                line.Y1 = point.Y;
                line.X2 = point.X;
                line.Y2 = point.Y;
                paintCanvas.Children.Add(line);
            }
            // 브러쉬로 그리기
            else if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Path"))
            {
                Path brush = (Path)DrawingObject;
                PathGeometry geometry = new PathGeometry();
                PathFigureCollection figures = new PathFigureCollection();
                PathFigure figure = new PathFigure();
                figure.StartPoint = point;
                figure.Segments = new PathSegmentCollection();
                figures.Add(figure);
                geometry.Figures = figures;
                brush.Data = geometry;
                paintCanvas.Children.Add(brush);
            }
            else if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Rectangle"))
            {
                Rectangle rectangle = (Rectangle)DrawingObject;
                Canvas.SetLeft(rectangle, point.X);
                Canvas.SetTop(rectangle, point.Y);
                startPoint = point;
                paintCanvas.Children.Add(rectangle);
            }
            else if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Ellipse"))
            {
                Ellipse ellipse = (Ellipse)DrawingObject;
                Canvas.SetLeft(ellipse, point.X);
                Canvas.SetTop(ellipse, point.Y);
                startPoint = point;
                paintCanvas.Children.Add(ellipse);
            }
            paintCanvas.MouseUp += mouseUpEvent;
            paintCanvas.MouseMove += mouseMoveEvent;
        }

        // 그리기가 끝났을 때 발생하는 이벤트
        public void FinishDraw(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            paintCanvas.MouseUp -= mouseUpEvent;
            paintCanvas.MouseDown -= startDrawEvent;
            paintCanvas.MouseMove -= mouseMoveEvent;
            paintedShape.Add(DrawingObject);
            DrawingObject = null;
        }

        public void MoveMouse(object sender, MouseEventArgs e)
        {
            Point point = GetCurrentPoint();

            /* 원래의 타입으로 변환*/

            // 선 그리기
            if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Line"))
            {
                Line line = (Line)DrawingObject;
                line.X2 = point.X;
                line.Y2 = point.Y;
                paintCanvas.Children.Remove(line);
                paintCanvas.Children.Add(line);
            }
            // 브러쉬로 그리기
            else if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Path"))
            {
                Path brush = (Path)DrawingObject;
                PathGeometry geometry = (PathGeometry)brush.Data;
                PathFigureCollection figures = geometry.Figures;
                PathFigure figure = figures[0];
                PathSegmentCollection pathSegments = figure.Segments;
                QuadraticBezierSegment quadratic = new QuadraticBezierSegment();
                quadratic.Point1 = priorPoint;
                quadratic.Point2 = point;
                pathSegments.Add(quadratic);
                paintCanvas.Children.Remove(brush);
                paintCanvas.Children.Add(brush);
            }
            else if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Rectangle"))
            {
                Rectangle rectangle = (Rectangle)DrawingObject;
             
                double width = point.X - startPoint.X;
                double height = point.Y - startPoint.Y;

                if(width < 0) {
                    width = Math.Abs(width);
                    Canvas.SetLeft(rectangle, point.X);
                }
                if(height < 0) {
                    height = Math.Abs(height);
                    Canvas.SetTop(rectangle, point.Y);
                }

                rectangle.Width = width;
                rectangle.Height = height;

                paintCanvas.Children.Remove(rectangle);
                paintCanvas.Children.Add(rectangle);
            }
            else if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Ellipse"))
            {
                Ellipse ellipse = (Ellipse)DrawingObject;

                double width = point.X - startPoint.X;
                double height = point.Y - startPoint.Y;

                if (width < 0)
                {
                    width = Math.Abs(width);
                    Canvas.SetLeft(ellipse, point.X);
                }
                if (height < 0)
                {
                    height = Math.Abs(height);
                    Canvas.SetTop(ellipse, point.Y);
                }

                ellipse.Width = width;
                ellipse.Height = height;

                paintCanvas.Children.Remove(ellipse);
                paintCanvas.Children.Add(ellipse);
            }
            priorPoint = point;
        }
        
        public void DrawLine(object sender, RoutedEventArgs e)
        {
            Line line = new Line();
            ReadyToDraw(line);
            line.Stroke = foreGroundColor;
            line.StrokeThickness = thickness;
        }

        public void DrawBrush(object sender, RoutedEventArgs e)
        {
            Path brush = new Path();
            ReadyToDraw(brush);
            brush.Stroke = foreGroundColor;
            brush.StrokeThickness = thickness;
        }

        public void DrawSquare(object sender, RoutedEventArgs e)
        {
            Rectangle rectangle = new Rectangle();
            ReadyToDraw(rectangle);
            rectangle.Stroke = foreGroundColor;
            rectangle.StrokeThickness = thickness;
        }

        public void DrawCircle(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ReadyToDraw(ellipse);
            ellipse.Stroke = foreGroundColor;
            ellipse.StrokeThickness = thickness;
        }

        // Drag정도에 따라 캔버스 크기 조정
        private void Canvas_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = paintCanvas.Height + e.VerticalChange / 10;
            double xadjust = paintCanvas.Width + e.HorizontalChange / 10;

            if ((xadjust >= 100) && (yadjust >= 100))
            {
                paintCanvas.Width = xadjust;
                paintCanvas.Height = yadjust;
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
