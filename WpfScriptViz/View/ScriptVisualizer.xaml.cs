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

namespace ScriptViz.View
{
    /// <summary>
    /// Interaction logic for ScriptVisualizer.xaml
    /// </summary>
    public partial class ScriptVisualizer : UserControl
    {
        #region Vars

        bool _flagDragging = false;

        Point originalMousePos,
              newMousePos,
              originalCanvasPos;

        #endregion // Vars

        public ScriptVisualizer()
        {
            InitializeComponent();
        }

        #region Event Responses

        #region Canvas Drag Events
        // start drag  - User presses left mouse button on Canvas
        //private void canvasContainer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    originalMousePos = e.GetPosition(null);
        //    Console.WriteLine(originalMousePos.ToString());
        //    originalCanvasPos = new Point(Canvas.GetLeft(containerCanvasRectangles), Canvas.GetBottom(containerCanvasRectangles));
        //}

        public void canvasContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvasContainer.CaptureMouse();

            originalMousePos = e.GetPosition(null);

            // store mouse position and original canvas position
            originalCanvasPos = new Point(Canvas.GetLeft(containerCanvasRectangles), Canvas.GetBottom(containerCanvasRectangles));

            _flagDragging = true;
            e.Handled = true; // Sets the mouse-down event as having been handled.
        }

        // drag - User moves mouse (anywhere) while holding left mouse button
        public void canvasContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_flagDragging)
            {
                // Move boxes
                newMousePos = e.GetPosition(null); // Update mouse position

                Point positionDelta = new Point(newMousePos.X - originalMousePos.X, -(newMousePos.Y - originalMousePos.Y));

                SetCanvasPosition(positionDelta);
            }
        }

        // end drag    - User releases left mouse button (anywhere)
        public void canvasContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_flagDragging)
            {
                _flagDragging = false;
                canvasContainer.ReleaseMouseCapture();
            }
        }

        // cancel drag - User presses right mouse button while holding left mouse button
        public void canvasContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            UndoMoveBoxes();
        }

        private void ScriptVisualizer_LostFocus(object sender, RoutedEventArgs e)
        {
            UndoMoveBoxes();
        }

        #endregion

        #endregion // Event Handling


        private void UndoMoveBoxes()
        {
            if (_flagDragging)
            {
                _flagDragging = false;

                SetCanvasPosition(new Point());
                canvasContainer.ReleaseMouseCapture();
            }
        }

        private void SetCanvasPosition(Point position, bool useOriginalPosition = true)
        {
            var _vm = this.DataContext as ScriptViz.ViewModel.ScriptVisualizerViewModel;

            if (useOriginalPosition)
                _vm.CanvasPosition = new Point(originalCanvasPos.X + position.X, originalCanvasPos.Y + position.Y);
            else
                _vm.CanvasPosition = new Point(position.X, position.Y);
        }
    }
}
