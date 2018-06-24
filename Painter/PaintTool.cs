using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Painter
{
    class PaintTool
    {
        public Canvas paintCanvas;

        public PaintTool(Canvas paintCanvas)
        {
            this.paintCanvas = paintCanvas;
        }

        public Point GetCurrentPoint()
        {
            Mouse.Capture(paintCanvas);
            return Mouse.GetPosition(paintCanvas);
        }

        public Dictionary<String, SolidColorBrush> InitColor()
        {
            Dictionary<string, SolidColorBrush> colorDict = new Dictionary<string, SolidColorBrush>();
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

            return colorDict;
        }

        public Rectangle[] MakeVertexes(Shape choiceBox)
        {
            // 기준점
            Point standard = new Point();
            // 꼭지점 네 개 생성
            Rectangle[] vertexes = new Rectangle[4];
            for (int i = 0; i < 4; i++)
            {
                vertexes[i] = new Rectangle();
                vertexes[i].Width = 8;
                vertexes[i].Height = 8;
                vertexes[i].Stroke = Brushes.Black;
                vertexes[i].Fill = Brushes.White;
                paintCanvas.Children.Add(vertexes[i]);
            }

            // 기준점 설정
            standard.X = Canvas.GetLeft(choiceBox);
            standard.Y = Canvas.GetTop(choiceBox);

            // 꼭지점 위치 설정
            Canvas.SetLeft(vertexes[0], standard.X - 4);
            Canvas.SetTop(vertexes[0], standard.Y - 4);
            Canvas.SetLeft(vertexes[1], standard.X + choiceBox.Width - 4);
            Canvas.SetTop(vertexes[1], standard.Y - 4);
            Canvas.SetLeft(vertexes[2], standard.X - 4);
            Canvas.SetTop(vertexes[2], standard.Y + choiceBox.Height - 4);
            Canvas.SetLeft(vertexes[3], standard.X + choiceBox.Width - 4);
            Canvas.SetTop(vertexes[3], standard.Y + choiceBox.Height - 4);

            return vertexes;
        }

        // 선택 영역 박스 만들기
        public Rectangle MakeChoiceBox(Shape shape)
        {
            Point standard = new Point();
            Rectangle choiceBox = new Rectangle();

            choiceBox.Width = shape.Width + 2;
            choiceBox.Height = shape.Height + 2;
            choiceBox.Stroke = Brushes.Blue;
            choiceBox.StrokeDashArray = new DoubleCollection() { 3 };

            standard.X = Canvas.GetLeft(shape) - 1;
            standard.Y = Canvas.GetTop(shape) - 1;

            Canvas.SetLeft(choiceBox, standard.X);
            Canvas.SetTop(choiceBox, standard.Y);

            paintCanvas.Children.Add(choiceBox);

            return choiceBox;
        }
    }
}
