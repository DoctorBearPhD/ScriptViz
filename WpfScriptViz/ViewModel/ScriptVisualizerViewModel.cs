using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ScriptLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ScriptViz.ViewModel
{
    public class ScriptVisualizerViewModel : VMBase
    {
        #region Vars

        #region Constants

        public const int LARGE_FRAME_CHANGE = 10;

        public const float CANVAS_PADDING = 15;
        public const float BOX_SCALAR = 100;

        const int POSITION_X_FLAG = 32768,
                  POSITION_Y_FLAG = 65536;

        #endregion // Constants

        #region Colors

        Color _hurtboxFillColor, _hurtboxStrokeColor,
              _hitboxFillColor, _hitboxStrokeColor,
              _physboxFillColor, _physboxStrokeColor,
              _proxboxFillColor, _proxboxStrokeColor;
        
        #endregion // Colors

        #region Lists

        public ObservableCollection<Box> CurrFrameBoxes;

        List<Position> _positions;
        List<Position> _currFramePositions;

        ObservableCollection<Box> _boxes;
        public ObservableCollection<Box> Boxes
        {
            get => _boxes;
            set { _boxes = value; RaisePropertyChanged("Boxes"); }
        }

        ObservableCollection<Rect> _rectangles;
        public ObservableCollection<Rect> Rectangles
        {
            get => _rectangles;
            set { _rectangles = value; RaisePropertyChanged("Rectangles"); }
        }

        #endregion // Lists

        #region Control Properties

        Point _canvasPosition;
        public Point CanvasPosition
        {
            get { return _canvasPosition; }
            set { _canvasPosition = value; RaisePropertyChanged(nameof(CanvasPosition)); }
        }

        #endregion // Control Properties

        #region Frame

        double _currentFrame;
        public double CurrentFrame
        {
            get => _currentFrame;
            set
            {
                _currentFrame = value;
                RaisePropertyChanged(nameof(CurrentFrame));
                FrameChanged();
            }
        }

        double _maxFrame;
        public double MaxFrame
        {
            get => _maxFrame;
            set
            {
                _maxFrame = value;
                RaisePropertyChanged(nameof(MaxFrame));
            }
        }

        #endregion // Frame

        #region Commands

        public ICommand PreviousFrameCommand => new RelayCommand(GoToPreviousFrame);
        public ICommand NextFrameCommand => new RelayCommand(GoToNextFrame);
        public ICommand PreviousFrameManyCommand => new RelayCommand<object>(amount => GoToPreviousFrame((int)amount));
        public ICommand NextFrameManyCommand => new RelayCommand<object>(amount => GoToNextFrame((int)amount));

        #endregion // Commands

        Move SelectedMove;

        #endregion // Vars

        #region Constructor

        public ScriptVisualizerViewModel()
        {
            #region Setup Colors

            _hurtboxFillColor = Colors.Green;
            _hurtboxFillColor.A = 124;

            _hurtboxStrokeColor = Colors.GreenYellow;
            _hurtboxStrokeColor.A = 180;

            _hitboxFillColor = Colors.Red;
            _hitboxFillColor.A = 124;

            _hitboxStrokeColor = Colors.Salmon;
            _hitboxStrokeColor.A = 124;

            _physboxFillColor = Colors.Cyan;
            _physboxFillColor.A = 124;

            _physboxStrokeColor = Colors.LightCyan;
            _physboxStrokeColor.A = 180;

            _proxboxFillColor = Colors.LightPink;
            _proxboxFillColor.A = 80;

            _proxboxStrokeColor = Colors.HotPink;
            _proxboxStrokeColor.A = 80;

            #endregion // Setup Colors

            Messenger.Default.Register<Move>(this, move => SelectedMoveChangedHandler(move));
        }

        #endregion // Constructor

        #region Methods

        public void ResetCanvasPosition()
        {
            CanvasPosition = new Point(0, CANVAS_PADDING);
        }

        public void ResetDisplay()
        {
            ResetCanvasPosition();

            Boxes = new ObservableCollection<Box>();
            _positions = new List<Position>();
        }

        public void DrawBoxes()
        {
            // Clear the canvas
            Rectangles = new ObservableCollection<Rect>();

            // Update the list of boxes that are active in the current frame.
            UpdateCurrFrameBoxes();
            UpdateCurrFramePositionModifiers();

            #region Draw Rectangles
            // For each Box, draw a Rectangle
            foreach (Box box in CurrFrameBoxes)
            {
                SolidColorBrush fill = new SolidColorBrush();
                SolidColorBrush stroke = new SolidColorBrush();

                #region Determine Fill/Stroke Color
                switch (box.GetType().Name)
                {
                    case "Hurtbox":
                        fill.Color = _hurtboxFillColor;
                        stroke.Color = _hurtboxStrokeColor;
                        break;

                    case "Hitbox":
                        if ((box as Hitbox).HitboxEffectIndex == -1 ||
                            (box as Hitbox).HitType == 4)
                        {
                            fill.Color = _proxboxFillColor;
                            stroke.Color = _proxboxStrokeColor;
                            break;
                        }
                        else
                        {
                            fill.Color = _hitboxFillColor;
                            stroke.Color = _hitboxStrokeColor;
                            break;
                        }

                    case "PhysicsBox":
                        fill.Color = _physboxFillColor;
                        stroke.Color = _physboxStrokeColor;
                        break;

                    default:
                        fill.Color = Colors.Gray;
                        stroke.Color = Colors.LightGray;
                        break;
                }
                #endregion

                // TODO: If enabledBoxTypes contains box.BoxType, draw box. For enabling/disabling visibility of Boxes.
                var rectangle = new Rect()
                {
                    Width = box.Width * BOX_SCALAR,
                    Height = box.Height * BOX_SCALAR,
                    Fill = fill,
                    Stroke = stroke
                };

                Rectangles.Add(rectangle);
            }

            #endregion // Draw Rectangles

            UpdateRectangleLocations();
        }

        /// <summary>
        /// Updates the list of all boxes that are active on the current frame.
        /// </summary>
        private void UpdateCurrFrameBoxes()
        {
            CurrFrameBoxes = new ObservableCollection<Box>();

            foreach (Box box in Boxes)
            {
                if (_currentFrame.IsBetween(box.TickStart, box.TickEnd - 1))
                    CurrFrameBoxes.Add(box);
            }
        }

        private void UpdateCurrFramePositionModifiers()
        {
            _currFramePositions = new List<Position>();

            foreach (Position pos in _positions)
            {
                if (_currentFrame.IsBetween(pos.TickStart, pos.TickEnd - 1))
                    _currFramePositions.Add(pos);
            }
        }

        public void UpdateRectangleLocations()
        {
            if (Rectangles != null)
            {
                for (int i = 0; i < Rectangles.Count; i++)
                {
                    Rect rectangle = Rectangles[i];
                    Box box = CurrFrameBoxes[i];

                    // Modify position of box, based on Positions
                    Point boxScaledLocation = new Point(box.X * BOX_SCALAR,
                                                        box.Y * BOX_SCALAR);
                    Point boxOrigin = new Point(boxScaledLocation.X,
                                                boxScaledLocation.Y);

                    Point location = boxOrigin;

                    foreach (var pos in _currFramePositions)
                    {
                        if (pos.Flag == POSITION_X_FLAG)
                            location.X += pos.Movement * BOX_SCALAR;
                        else if (pos.Flag == POSITION_Y_FLAG)
                            location.Y += pos.Movement * BOX_SCALAR;
                    }

                    rectangle.X = (float)location.X;
                    rectangle.Y = (float)location.Y;
                }
            }
        }

        #region Event Handling

        //

        #region Frame Change

        public void GoToPreviousFrame()
        {
            if (CurrentFrame > 0) CurrentFrame--;
        }

        public void GoToPreviousFrame(int amount)
        {
            if (CurrentFrame > 0)
            {
                if (CurrentFrame - amount < 0)
                    CurrentFrame = 0;
                else
                    CurrentFrame -= amount;
            }
        }

        public void GoToNextFrame()
        {
            if (CurrentFrame < _maxFrame) CurrentFrame++;
        }

        public void GoToNextFrame(int amount)
        {
            if (CurrentFrame < _maxFrame)
            {
                if (CurrentFrame + amount > _maxFrame)
                    CurrentFrame = _maxFrame;
                else
                    CurrentFrame += amount;
            }
        }

        public void FrameChanged() { if (Boxes != null) DrawBoxes(); }

        #endregion // Frame Change

        public void SelectedMoveChangedHandler(Move move)
        {
            SelectedMove = move;

            ResetDisplay();

            LoadMove();
        }

        #endregion // Event Handling

        void LoadMove()
        {
            if (SelectedMove == null) return;

            CurrentFrame = 0;
            MaxFrame = SelectedMove.TotalTicks - 1; // Gets length of move (amount of time)

            #region Check for Boxes

            if (SelectedMove.Hurtboxes != null && SelectedMove.Hurtboxes.Length > 0)
                foreach (var hurtbox in SelectedMove.Hurtboxes)
                    Boxes.Add(hurtbox);
            
            if (SelectedMove.Hitboxes != null && SelectedMove.Hitboxes.Length > 0)
                foreach (var hitbox in SelectedMove.Hitboxes)
                    Boxes.Add(hitbox);
            
            if (SelectedMove.PhysicsBoxes != null && SelectedMove.PhysicsBoxes.Length > 0)
                foreach (var physbox in SelectedMove.PhysicsBoxes)
                    Boxes.Add(physbox);
            
            #endregion

            #region Check for Positions
            if (SelectedMove.Positions != null && SelectedMove.Positions.Length > 0)
                foreach (var position in SelectedMove.Positions)
                    _positions.Add(position);
            #endregion

            #region No Candidates Found
            if (Boxes.Count == 0)
            {
                Console.WriteLine("Candidates not found.");
                return;
            }
            #endregion

            DrawBoxes();
        }

        #endregion // Methods
    }
}
