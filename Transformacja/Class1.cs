using PluginInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Transformacja
{
    public class TransTool : IPlugin
    {
        private int _thickness; // = 3; // default value
        private Color _color; // = Colors.Black; //default color
        private Canvas _canvas;

        public MenuItem GetMenuItem()
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Transformation";
            return menuItem;
        }

        public void Initialize(Canvas canvas, Color color, int thickness)
        {
            _canvas = canvas;
            _color = color;
            _thickness = thickness;
            if (_canvas != null)
            {
                _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
                _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
               // _canvas.MouseMove += Canvas_MouseMove;
            }
        }

  public void SetColor(Color color)
        {
            if (color != null)
            {
                _color = color;
            }
        }

        public void SetThickness(int thickness)
        {
            if (thickness >= 0)
            {
                _thickness = thickness;
            }
        }
      
        public void Dispose()
        {
            if (_canvas != null)
            {
                _canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
                _canvas.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;
                //_canvas.MouseMove -= Canvas_MouseMove;
            }
        }

        private async void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TransfoermAsync();
          //  var flag = await TransformAsync();

            /*var canvas = (Canvas)sender;

            

            if (canvas.CaptureMouse())
            {
                var startPoint = e.GetPosition(canvas);
                var line = new Line
                {
                    Stroke = new SolidColorBrush(_color),
                    StrokeThickness = _thickness,
                    X1 = startPoint.X,
                    Y1 = startPoint.Y,
                    X2 = startPoint.X,
                    Y2 = startPoint.Y,
                };

                canvas.Children.Add(line);
            } */
        }

      // private async Task TransformAsync()
      private void TransfoermAsync()
        {
          //  return await Task.Run(() =>
           // {
                RotateTransform rotate = new RotateTransform(130);
                _canvas.LayoutTransform = rotate;
            //});
        }

        /* private void Canvas_MouseMove(object sender, MouseEventArgs e)
         {
             var canvas = (Canvas)sender;

             if (canvas.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
             {
                 var line = canvas.Children.OfType<Line>().LastOrDefault();

                 if (line != null)
                 {
                     var endPoint = e.GetPosition(canvas);
                     line.X2 = endPoint.X;
                     line.Y2 = endPoint.Y;
                 }
             }
         } 
         */
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
         {
             ((Canvas)sender).ReleaseMouseCapture();
         }
    }
}
