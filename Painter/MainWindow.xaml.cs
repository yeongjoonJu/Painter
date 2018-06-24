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
        PaintTool paintTool;
        Dictionary<String, SolidColorBrush> colorDict;
        Point priorPoint, startPoint;
        Thickness margeStartMove;  // 도형 시작 Margin 값

        // 두께 설정 상수
        const int THIN = 2;
        const int THICK = 6;
        const int MORE_THICK = 10;
        int thickness = THIN;

        // 이벤트 플래그
        bool mouseDownFlag = false;
        bool mouseUpFlag = false;
        // bool mouseMoveFlag = false;
        bool dragMove = false;
        bool painting = false;
        bool spoiding = false;
        bool erasing = false;

        // 선택 박스, 점
        Shape choiceBox = null;
        Rectangle[] vertexes = null;

        // 그리고 있거나 선택 중인 객체
        object DrawingObject;

        MouseEventHandler mouseMoveEvent;

        public MainWindow()
        {
            InitializeComponent();
            paintedShape = new List<object>();
            foreGroundColor = Brushes.Black;
            backGroundColor = Brushes.White;
            paintCanvas.MouseDown += new MouseButtonEventHandler(StartDraw);
            paintCanvas.MouseUp += new MouseButtonEventHandler(FinishDraw);
            //paintCanvas.MouseMove += new Mouse EventHandler(MoveMouse);
            mouseMoveEvent = new MouseEventHandler(MoveMouse);
            paintTool = new PaintTool(paintCanvas);

            colorDict = paintTool.InitColor();
        }

        // 선 그리기 버튼 클릭
        public void DrawLine(object sender, RoutedEventArgs e)
        {
            Line line = new Line();
            line.MouseDown += PressLine;
            ReadyToDraw(line);
            line.Stroke = foreGroundColor;
            line.StrokeThickness = thickness;
        }

        // 브러쉬 버튼 클릭
        public void DrawBrush(object sender, RoutedEventArgs e)
        {
            Path brush = new Path();
            brush.MouseDown += PressLine;
            ReadyToDraw(brush);
            brush.Stroke = foreGroundColor;
            brush.StrokeThickness = thickness;
        }

        // 사각형 그리기 버튼 클릭
        public void DrawSquare(object sender, RoutedEventArgs e)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.MouseDown += PressShape;
            rectangle.MouseMove += MoveShape;
            // rectangle.MouseMove += MoveShape;
            ReadyToDraw(rectangle);
            rectangle.Fill = Brushes.Transparent;
            rectangle.Stroke = foreGroundColor;
            rectangle.StrokeThickness = thickness;
        }

        // 원 그리기 버튼 클릭
        public void DrawCircle(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.MouseDown += PressShape;
            ellipse.MouseMove += MoveShape;
            ;
            ReadyToDraw(ellipse);
            ellipse.Fill = Brushes.Transparent;
            ellipse.Stroke = foreGroundColor;
            ellipse.StrokeThickness = thickness;
        }

        // 도형을 그릴 준비를 한다.
        public void ReadyToDraw(object shape)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            if (shape.GetType().ToString().Equals("System.Windows.Shapes.Path"))
                Mouse.OverrideCursor = Cursors.Pen;
            DrawingObject = shape;
            mouseDownFlag = true;
        }

        // 그리기 버튼 클릭 후 캔버스 클릭 시 이벤트
        public void StartDraw(object sender, MouseButtonEventArgs e)
        {
            if (mouseDownFlag)
            {
                // 객체가 선택되면
                if (paintCanvas.Children.Contains((UIElement)DrawingObject))
                {
                    mouseUpFlag = true;
                    FinishDraw(sender, e);
                    return;
                }
                    
                Point point = paintTool.GetCurrentPoint();
                priorPoint = point;
                // 선 그리기
                if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Line"))
                {
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
                mouseUpFlag = true;
                //mouseMoveFlag = true;
                paintCanvas.MouseMove += mouseMoveEvent;
            }
            //paintCanvas.MouseUp += mouseUpEvent;
            //paintCanvas.MouseMove += mouseMoveEvent;
        }

        // 그리기가 끝났을 때 발생하는 이벤트
        public void FinishDraw(object sender, MouseButtonEventArgs e)
        {
            if (mouseUpFlag)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                //paintCanvas.MouseUp -= mouseUpEvent;
                //paintCanvas.MouseDown -= startDrawEvent;
                paintCanvas.MouseMove -= mouseMoveEvent;
                mouseUpFlag = false;
                mouseDownFlag = false;
                //paintedShape.Add(DrawingObject);
                DrawingObject = null;

                this.WindowState = WindowState.Minimized;
                this.WindowState = WindowState.Maximized;
            }
        }

        public void MoveMouse(object sender, MouseEventArgs e)
        {
            Point point = paintTool.GetCurrentPoint();
            // 캔버스 벗어나는 것을 막는다
            if (point.X >= paintCanvas.ActualWidth || point.Y >= paintCanvas.ActualHeight || point.X < 0 || point.Y < 0)
                return;

            /* 원래의 타입으로 변환*/

            // 선 그리기
            if (DrawingObject.GetType().ToString().Equals("System.Windows.Shapes.Line"))
            {
                Line line = (Line)DrawingObject;
                line.X2 = point.X;
                line.Y2 = point.Y;
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
            }

            // 정사각형, 원 그리기
            else
            {
                Shape shape = (Shape)DrawingObject;

                double width = point.X - startPoint.X;
                double height = point.Y - startPoint.Y;

                if (width < 0)
                {
                    width = Math.Abs(width);
                    Canvas.SetLeft(shape, point.X);
                }
                if (height < 0)
                {
                    height = Math.Abs(height);
                    Canvas.SetTop(shape, point.Y);
                }

                shape.Width = width;
                shape.Height = height;
            }
            priorPoint = point;

            this.Title = "그림판  X : " + priorPoint.X.ToString() + " Y : " + priorPoint.Y.ToString();
        }

        // Drag정도에 따라 캔버스 크기 조정
        private void Canvas_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = paintCanvas.Height + e.VerticalChange / 10;
            double xadjust = paintCanvas.Width + e.HorizontalChange / 10;

            if (((xadjust >= 100) && (yadjust >= 100)) && (xadjust <= 1800) && (yadjust <= 900))
            {
                paintCanvas.Width = xadjust;
                paintCanvas.Height = yadjust;
                Canvas.SetLeft(CanvasThumb, Canvas.GetLeft(CanvasThumb) + e.HorizontalChange);
                Canvas.SetTop(CanvasThumb, Canvas.GetTop(CanvasThumb) + e.VerticalChange);
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

        // 색상 버튼을 누르면 해당 버튼의 색으로 변경
        public void ChangeColor(object sender, RoutedEventArgs e)
        {
            Button colorButton = (Button)sender;
            String color = colorButton.Name.Substring(4);
            foreGroundColor = colorDict[color];
            foreColor.Background = foreGroundColor;
        }

        // 색상 추출
        private void Btn_Pipette_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            spoiding = true;
        }

        private void menu_exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Btn_Paint_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            painting = true;
        }

        // 그려진 도형을 선택 시 이벤트 발생
        private void PressShape(object sender, MouseEventArgs e)
        {
            Shape shape = (Shape)sender;

            if (painting)
            {
                shape.Fill = foreGroundColor;
                Mouse.OverrideCursor = Cursors.Arrow;
                painting = false;
            }
            else if (spoiding)
            {
                foreColor.Background = shape.Fill;
                foreGroundColor = (SolidColorBrush)foreColor.Background;
                spoiding = false;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (erasing)
            {
                paintCanvas.Children.Remove(shape);
                erasing = false;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else
            {
                // 어떤 도형이 선택되어 있으면
                if (choiceBox != null)
                {
                    // 현재 선택한 도형과 이미 선택된 도형과 같은지 확인
                    if (shape == DrawingObject)
                    {
                        return;
                    }

                    // 다른 도형 선택 시 현재 선택 박스 제거
                    paintCanvas.Children.Remove(choiceBox);
                    for (int i = 0; i < 4; i++)
                        paintCanvas.Children.Remove(vertexes[i]);
                }

                DrawingObject = shape;

                // 선택 영역 박스 감싸기
                choiceBox = paintTool.MakeChoiceBox(shape);
                vertexes = paintTool.MakeVertexes(choiceBox);

                vertexes[0].MouseMove += LeftTopVertex_Click;
                vertexes[1].MouseMove += RightTopVertex_Click;
                vertexes[2].MouseMove += LeftBottomVertex_Click;
                vertexes[3].MouseMove += RightBottomVertex_Click;
                shape.MouseMove += MoveShape;
            }
        }
              
        // 브러쉬, 선을 클릭시 발생
        private void PressLine(object sender, RoutedEventArgs e)
        {
            if (painting)
            {
                Shape shape = (Shape)sender;
                shape.Stroke = foreGroundColor;
                Mouse.OverrideCursor = Cursors.Arrow;
                painting = false;
            }
        }

        // 커서 버튼 클릭 이벤트
        private void Btn_Cursor_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            mouseDownFlag = false;
            mouseUpFlag = false;
            paintCanvas.MouseMove -= mouseMoveEvent;
            painting = false;
            choiceBox = null;
            DrawingObject = null;
            spoiding = false;
        }

        // 새로 만들기 메뉴 버튼 클릭 이벤트
        private void menu_newfile_Click(object sender, RoutedEventArgs e)
        {
            paintCanvas.Children.Clear();
        }

        // 지우개 버튼 클릭 시 발생
        private void Btn_Eraser_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.No;
            erasing = true;
        }

        // 두께 버튼 클릭 이벤트
        private void Btn_Thickness_Click(object sender, RoutedEventArgs e)
        {
            thickness += 4;
            if (thickness > MORE_THICK)
                thickness = THIN;

            switch (thickness)
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

        // 도형을 이동
        public void MoveShape(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(paintCanvas);
                Shape shape = (Shape)DrawingObject;
                Point pointInShape = e.GetPosition(shape);

                double deltaX = priorPoint.X - point.X;
                double deltaY = priorPoint.Y - point.Y;

                // 도형 위치 변경
                Canvas.SetLeft(shape, Canvas.GetLeft(shape) - deltaX);
                Canvas.SetTop(shape, Canvas.GetTop(shape) - deltaY);

                // 선택 영역 변경
                Canvas.SetLeft(choiceBox, Canvas.GetLeft(shape) - deltaX - 1);
                Canvas.SetTop(choiceBox, Canvas.GetTop(shape) - deltaY - 1);

                // 꼭지점 위치 설정
                Canvas.SetLeft(vertexes[0], Canvas.GetLeft(shape) - deltaX - 4);
                Canvas.SetTop(vertexes[0], Canvas.GetTop(shape) - deltaY - 4);
                Canvas.SetLeft(vertexes[1], Canvas.GetLeft(shape) + shape.Width - deltaX - 4);
                Canvas.SetTop(vertexes[1], Canvas.GetTop(shape) - deltaY - 4);
                Canvas.SetLeft(vertexes[2], Canvas.GetLeft(shape) - deltaX - 4);
                Canvas.SetTop(vertexes[2], Canvas.GetTop(shape) + shape.Height - deltaY - 4);
                Canvas.SetLeft(vertexes[3], Canvas.GetLeft(shape) + shape.Width - deltaX - 4);
                Canvas.SetTop(vertexes[3], Canvas.GetTop(shape) + shape.Height - deltaY - 4);
            }

            priorPoint = e.GetPosition(paintCanvas);
        }

        private void LeftTopVertex_Click(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(paintCanvas);
                Shape shape = (Shape)DrawingObject;

                double deltaX = Canvas.GetLeft(shape) - point.X;
                double deltaY = Canvas.GetTop(shape) - point.Y;

                // 도형 크기 변경
                shape.Width += deltaX;
                shape.Height += deltaY;
                Canvas.SetLeft(shape, point.X);
                Canvas.SetTop(shape, point.Y);

                // 선택 영역 변경
                choiceBox.Width += deltaX;
                choiceBox.Height += deltaY;
                Canvas.SetLeft(choiceBox, point.X - 1);
                Canvas.SetTop(choiceBox, point.Y - 1);

                // 꼭지점 위치 설정
                Canvas.SetLeft(vertexes[0], point.X - 4);
                Canvas.SetTop(vertexes[0], point.Y - 4);
                Canvas.SetTop(vertexes[1], point.Y - 4);
                Canvas.SetLeft(vertexes[2], point.X - 4);
            }
        }

        private void RightTopVertex_Click(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(paintCanvas);
                Shape shape = (Shape)DrawingObject;

                double deltaX = Canvas.GetLeft(shape) + shape.Width - point.X;
                double deltaY = Canvas.GetTop(shape) - point.Y;

                // 도형 크기 변경
                shape.Width -= deltaX;
                shape.Height += deltaY;
                Canvas.SetTop(shape, point.Y);

                // 선택 영역 변경
                choiceBox.Width -= deltaX;
                choiceBox.Height += deltaY;
                Canvas.SetTop(choiceBox, point.Y - 1);

                // 꼭지점 위치 설정
                Canvas.SetTop(vertexes[0], point.Y - 4);
                Canvas.SetLeft(vertexes[1], point.X - 4);
                Canvas.SetTop(vertexes[1], point.Y - 4);
                Canvas.SetLeft(vertexes[3], point.X - 4);
            }
        }

        private void LeftBottomVertex_Click(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(paintCanvas);
                Shape shape = (Shape)DrawingObject;

                double deltaX = Canvas.GetLeft(shape) - point.X;
                double deltaY = Canvas.GetTop(shape) + shape.Height - point.Y;

                // 도형 크기 변경
                shape.Width += deltaX;
                shape.Height -= deltaY;
                Canvas.SetLeft(shape, point.X);

                // 선택 영역 변경
                choiceBox.Width += deltaX;
                choiceBox.Height -= deltaY;
                Canvas.SetLeft(choiceBox, point.X - 1);

                // 꼭지점 위치 설정
                Canvas.SetLeft(vertexes[0], point.X - 4);
                Canvas.SetLeft(vertexes[2], point.X - 4);
                Canvas.SetTop(vertexes[2], point.Y - 4);
                Canvas.SetTop(vertexes[3], point.Y - 4);
            }
        }

        private void RightBottomVertex_Click(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(paintCanvas);
                Shape shape = (Shape)DrawingObject;

                double deltaX = Canvas.GetLeft(shape) + shape.Width - point.X;
                double deltaY = Canvas.GetTop(shape) + shape.Height - point.Y;

                // 도형 크기 변경
                shape.Width -= deltaX;
                shape.Height -= deltaY;

                // 선택 영역 변경
                choiceBox.Width -= deltaX;
                choiceBox.Height -= deltaY;

                // 꼭지점 위치 설정
                Canvas.SetLeft(vertexes[1], point.X - 4);
                Canvas.SetTop(vertexes[2], point.Y - 4);
                Canvas.SetLeft(vertexes[3], point.X - 4);
                Canvas.SetTop(vertexes[3], point.Y - 4);
            }
        }
    }
}
