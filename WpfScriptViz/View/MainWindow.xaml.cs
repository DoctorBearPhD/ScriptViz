using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScriptViz.ViewModel;

namespace ScriptViz.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Vars
        MainWindowViewModel _vm;

        bool _flagDragging = false;

        Point originalMousePos,
              newMousePos,
              originalCanvasPos;
        #endregion // Vars

        public MainWindow()
        {
            InitializeComponent();

            // Set ViewModel reference
            _vm = (MainWindowViewModel)this.Resources["viewModel"];

            if (_vm.CloseAction == null) _vm.CloseAction = new Action(Close); // Sets the Window.Close function to the ViewModel command
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

        public void Window_Deactivated(object sender, EventArgs e)
        {
            UndoMoveBoxes();
        }
        #endregion
        
        #region Slider Events
        
        private void sliderCurrentFrame_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _vm.FrameChanged();
        }

        #endregion // Slider Events

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
            if (useOriginalPosition)
                _vm.CanvasPosition = new Point(originalCanvasPos.X + position.X, originalCanvasPos.Y + position.Y);
            else
                _vm.CanvasPosition = new Point(position.X, position.Y);
        }
        
    }

    //public struct Position
    //{
    //    public int TickStart, TickEnd;
    //    public float Movement;
    //    public int Flag;
    //}

}

