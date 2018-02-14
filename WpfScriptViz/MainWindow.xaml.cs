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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace WpfScriptViz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Box> _boxes;
        List<Box> _currFrameBoxes;
        List<Rectangle> _rectangles;
        List<Position> _positions;
        List<Position> _currFramePositions;

        private double _currentFrame;
        private double _maxFrame;

        public const string LABEL_TEXT_COLOR = "#FFababad";
        public const float BOX_SCALAR = 100;
        public const float CANVAS_PADDING = 15;

        const int POSITION_X_FLAG = 32768, 
                  POSITION_Y_FLAG = 65536;

        const bool DEBUG = true;

        bool _flagDragging = false;
        Point originalMousePos,
              newMousePos,
              originalCanvasPos;

        private Color _hurtboxFillColor, _hurtboxStrokeColor, 
                      _hitboxFillColor,  _hitboxStrokeColor,
                      _physboxFillColor, _physboxStrokeColor,
                      _proxboxFillColor, _proxboxStrokeColor;

        public MainWindow()
        {
            InitializeComponent();

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
            #endregion

            if (DEBUG)
                LoadDefaultScript();
            
        }

        #region Event Responses

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateVisualizer();
        }

        #region Canvas Drag Events
        // start drag  - User presses left mouse button on Canvas
        //private void containerCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    originalMousePos = e.GetPosition(null);
        //    Console.WriteLine(originalMousePos.ToString());
        //    originalCanvasPos = new Point(Canvas.GetLeft(canvasScriptViz), Canvas.GetBottom(canvasScriptViz));
        //}

        private void containerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            containerCanvas.CaptureMouse();

            originalMousePos = e.GetPosition(null);

            // store mouse position and original canvas position
            originalCanvasPos = new Point(Canvas.GetLeft(canvasScriptViz), Canvas.GetBottom(canvasScriptViz));

            _flagDragging = true;
            e.Handled = true; // Sets the mouse-down event as having been handled.
        }
        
        // drag - User moves mouse (anywhere) while holding left mouse button
        private void containerCanvas_MouseMove(object sender, MouseEventArgs e)
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
        private void containerCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_flagDragging)
            {
                _flagDragging = false;
                containerCanvas.ReleaseMouseCapture();
            }
        }

        // cancel drag - User presses right mouse button while holding left mouse button
        private void containerCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            UndoMoveBoxes();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            UndoMoveBoxes();
        }
        #endregion

        private void containerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // update inner canvas size
            canvasScriptViz.Width = e.NewSize.Width;
            canvasScriptViz.Height = e.NewSize.Height;

            // update boxes
            if (_boxes != null)
                UpdateBoxPositions();
        }
        
        #region Menu Item Events
        private void menuitemExit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void menuitemRemoveBVI_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Create yes/no dialogue

            // if yes, replace
            String text = tbScriptBox.Text;
            Regex regex = new Regex(@"""BACVERint\d"": (?=0)\d,\s+");

            tbScriptBox.Text = regex.Replace(text, "");
        }
        #endregion

        #region Slider Events
        private void btnPrevFrame_Click(object sender, RoutedEventArgs e)
        {
            sliderCurrentFrame.Value--;
        }

        private void btnNextFrame_Click(object sender, RoutedEventArgs e)
        {
            sliderCurrentFrame.Value++;
        }

        private void sliderCurrentFrame_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            #region Update Frame Counter
            // update currentframe
            _currentFrame = e.NewValue;
            // update label
            UpdateSliderLabel(); // TODO: Make it a TextBox
            #endregion

            #region Update Displayed Hitboxes
            if (_boxes != null)
                DrawBoxes();
            #endregion
        }
        #endregion

        #endregion

        private void UpdateVisualizer()
        {
            #region Reset Display
            spDisplayContainer.Children.Clear();

            SetCanvasPosition(new Point(), false);

            _boxes = new List<Box>();
            _positions = new List<Position>();
            _currentFrame = sliderCurrentFrame.Minimum;
            _maxFrame = sliderCurrentFrame.Minimum;
            sliderCurrentFrame.Value = sliderCurrentFrame.Minimum;
            #endregion

            #region Get ScriptBox Text
            string scriptboxText = tbScriptBox.Text;
            // If the text is just the default text or is blank, ignore it.
            if (scriptboxText.Equals("") ||
                scriptboxText.Equals(FindResource("ScriptBoxDefaultText")))
                return;
            #endregion

            #region Parse JSON
            // Try to parse JSON out of the text
            // Try to parse as a JObject
            if (scriptboxText.TrimStart().StartsWith("{"))
            {
                #region Try to Clean JSON
                Regex regex = new Regex(@",\s*}");
                regex.Replace(scriptboxText, "}");
                regex = new Regex(@",\s*]");
                regex.Replace(scriptboxText, "]");
                #endregion


                // Convert the JSON String to a C# object.
                var jobj = JsonConvert.DeserializeObject<dynamic>(scriptboxText,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                _maxFrame = jobj.TotalTicks - 1;

                #region Check for Boxes
                if ((jobj as JObject)["Hurtboxes"] != null && jobj.Hurtboxes.HasValues)
                {
                    foreach (JToken hurtbox in (jobj.Hurtboxes as JArray))
                    {
                        //Console.WriteLine(hurtbox);
                        Box box = hurtbox.ToObject<Box>();
                        box.BoxType = "Hurtbox";

                        _boxes.Add(box);
                    }
                }
                if ((jobj as JObject)["Hitboxes"] != null && jobj.Hitboxes.HasValues)
                {
                    Console.WriteLine((jobj.Hitboxes));
                    foreach (JToken hitbox in (jobj.Hitboxes as JArray))
                    {
                        //Console.WriteLine(hurtbox);
                        Box box = hitbox.ToObject<Box>();
                        box.BoxType = (box.HitboxEffectIndex == -1 || box.HitType == 4) ? "ProxBox" : "Hitbox";

                        _boxes.Add(box);
                    }
                }
                if ((jobj as JObject)["PhysicsBoxes"] != null && jobj.PhysicsBoxes.HasValues)
                {
                    foreach (JToken physbox in (jobj.PhysicsBoxes as JArray))
                    {
                        //Console.WriteLine(hurtbox);
                        Box box = physbox.ToObject<Box>();
                        box.BoxType = "PhysicsBox";

                        _boxes.Add(box);
                    }
                }
                #endregion

                #region Check for Positions
                if ((jobj as JObject)["Positions"] != null && jobj.Positions.HasValues)
                {
                    foreach (JToken position in (jobj.Positions as JArray))
                    {
                        Position pos = position.ToObject<Position>();

                        _positions.Add(pos);
                    }
                }
                #endregion

                #region No Candidates Found
                if (!_boxes.Any())
                {
                    Console.WriteLine("Candidates not found.");
                    return;
                }
                #endregion

                #region Populate TreeView
                foreach (Box box in _boxes)
                {
                    Expander expanderBoxData = new Expander
                    {
                        Header = String.Format("{0} ({1}, {2})", box.BoxType, box.TickStart, box.TickEnd),
                        Content = GenerateBoxData(box),
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LABEL_TEXT_COLOR))
                    };

                    spDisplayContainer.Children.Add(expanderBoxData);
                }
                #endregion

                DrawBoxes();

                // Update Slider Max
                sliderCurrentFrame.Maximum = _maxFrame;
                UpdateSliderLabel();

            }
            else
            {

            }
            #endregion
        }

        private void DrawBoxes()
        {
            canvasScriptViz.Children.Clear();
            _rectangles = new List<Rectangle>();
            
            // Update the list of boxes that are active in the current frame.
            UpdateCurrFrameBoxes();
            UpdateCurrFramePositions();

            foreach (Box box in _currFrameBoxes)
            {
                SolidColorBrush fill = new SolidColorBrush();
                SolidColorBrush stroke = new SolidColorBrush();
                
                #region Determine Fill/Stroke Color
                switch (box.BoxType)
                {
                    case "Hurtbox":
                        fill.Color = _hurtboxFillColor;
                        stroke.Color = _hurtboxStrokeColor;
                        break;
                    case "Hitbox":
                        fill.Color = _hitboxFillColor;
                        stroke.Color = _hitboxStrokeColor;
                        break;
                    case "PhysicsBox":
                        fill.Color = _physboxFillColor;
                        stroke.Color = _physboxStrokeColor;
                        break;
                    case "ProxBox":
                        fill.Color = _proxboxFillColor;
                        stroke.Color = _proxboxStrokeColor;
                        break;
                    default:
                        fill.Color = Colors.Gray;
                        stroke.Color = Colors.LightGray;
                        break;
                }
                #endregion

                // TODO: If enabledBoxTypes contains box.BoxType, draw box.
                Rectangle rectangle = new Rectangle()
                {
                    Width = box.Width * BOX_SCALAR,
                    Height = box.Height * BOX_SCALAR,
                    Fill = fill,
                    Stroke = stroke
                };

                canvasScriptViz.Children.Add(rectangle);
                _rectangles.Add(rectangle);
            }

            UpdateBoxPositions();
        }

        private object GenerateBoxData(Box box)
        {
            Grid _item;
            Label _lblX, _lblY, _lblW, _lblH;
            TextBox _tbX, _tbY, _tbW, _tbH;

            SolidColorBrush _textColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LABEL_TEXT_COLOR));

            /*
             * Make a label and textbox for each property of the Box (besides Tick-).
            (nameof(box.X), box.X);
            (nameof(box.Y), box.Y);
            (nameof(box.Width), box.Width);
            (nameof(box.Height), box.Height);
             */

            _item = new Grid();

            #region Create Row and Column Definitions
            ColumnDefinition colDef0 = new ColumnDefinition();
            ColumnDefinition colDef1 = new ColumnDefinition();

            colDef0.Width = new GridLength(3, GridUnitType.Star);
            colDef1.Width = new GridLength(2, GridUnitType.Star);

            RowDefinition rowDef0 = new RowDefinition();
            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();
            RowDefinition rowDef3 = new RowDefinition();

            _item.ColumnDefinitions.Add(colDef0);
            _item.ColumnDefinitions.Add(colDef1);

            _item.RowDefinitions.Add(rowDef0);
            _item.RowDefinitions.Add(rowDef1);
            _item.RowDefinitions.Add(rowDef2);
            _item.RowDefinitions.Add(rowDef3);
            #endregion

            #region Create Item Content
            _lblX = MakeDataBoxLabel(nameof(box.X), _textColor);
            _lblY = MakeDataBoxLabel(nameof(box.Y), _textColor);
            _lblW = MakeDataBoxLabel(nameof(box.Width), _textColor);
            _lblH = MakeDataBoxLabel(nameof(box.Height), _textColor);

            _tbX = MakeDataBoxTextBox(box.X);
            _tbY = MakeDataBoxTextBox(box.Y);
            _tbW = MakeDataBoxTextBox(box.Width);
            _tbH = MakeDataBoxTextBox(box.Height);
            #endregion

            #region Set Rows and Columns
            Grid.SetRow(_lblX, 0);
            Grid.SetRow(_lblY, 1);
            Grid.SetRow(_lblW, 2);
            Grid.SetRow(_lblH, 3);
            Grid.SetColumn(_lblX, 0);
            Grid.SetColumn(_lblY, 0);
            Grid.SetColumn(_lblW, 0);
            Grid.SetColumn(_lblH, 0);

            Grid.SetRow(_tbX, 0);
            Grid.SetRow(_tbY, 1);
            Grid.SetRow(_tbW, 2);
            Grid.SetRow(_tbH, 3);
            Grid.SetColumn(_tbX, 1);
            Grid.SetColumn(_tbY, 1);
            Grid.SetColumn(_tbW, 1);
            Grid.SetColumn(_tbH, 1);
            #endregion

            #region Add Content to Item
            _item.Children.Add(_lblX);
            _item.Children.Add(_tbX);
            _item.Children.Add(_lblY);
            _item.Children.Add(_tbY);
            _item.Children.Add(_lblW);
            _item.Children.Add(_tbW);
            _item.Children.Add(_lblH);
            _item.Children.Add(_tbH);
            #endregion

            return _item;
        }

        private void UndoMoveBoxes()
        {
            if (_flagDragging)
            {
                _flagDragging = false;

                SetCanvasPosition(new Point());
                containerCanvas.ReleaseMouseCapture();
            }
        }
        
        private Label MakeDataBoxLabel(string property, SolidColorBrush color)
        {
            return new Label()
            {
                Content = property,
                Foreground = color
            };
        }

        private TextBox MakeDataBoxTextBox(float value)
        {
            return new TextBox()
            {
                Text = value.ToString(),
                HorizontalAlignment = HorizontalAlignment.Right
            };
        }

        private void SetCanvasPosition(Point position, bool useOriginalPosition = true)
        {
            Canvas.SetLeft(canvasScriptViz, useOriginalPosition? (originalCanvasPos.X + position.X) : position.X);
            Canvas.SetBottom(canvasScriptViz, useOriginalPosition? (originalCanvasPos.Y + position.Y) : position.Y);
        }

        private void UpdateSliderLabel()
        {
            if (lblCurrentFrame != null) lblCurrentFrame.Content = String.Format("{0} / {1}", _currentFrame, _maxFrame);
        }

        private void UpdateBoxPositions()
        {
            if (_rectangles != null)
            {
                foreach (Rectangle rectangle in _rectangles)
                {
                    int index = _rectangles.IndexOf(rectangle);
                    Box box = _currFrameBoxes[index];
                    
                    // Determine position of box based on Positions
                    Point boxOrigin = new Point(canvasScriptViz.ActualWidth / 2 + box.X * BOX_SCALAR,
                                                CANVAS_PADDING + box.Y * BOX_SCALAR + box.Height);
                    Point position = boxOrigin;

                    // TODO: foreach position in positions, check if box position should be adjusted
                    foreach (var pos in _currFramePositions)
                    {
                        if (pos.Flag == POSITION_X_FLAG)
                            position.X += pos.Movement * BOX_SCALAR;
                        else if (pos.Flag == POSITION_Y_FLAG)
                            position.Y += pos.Movement * BOX_SCALAR;
                    }


                    Canvas.SetLeft(rectangle, position.X);
                    Canvas.SetBottom(rectangle, position.Y);
                }
            }
        }

        /// <summary>
        /// Updates the list of all boxes that are active on the current frame.
        /// </summary>
        private void UpdateCurrFrameBoxes()
        {
            _currFrameBoxes = new List<Box>();

            foreach (Box box in _boxes)
            {
                if (_currentFrame.IsBetween(box.TickStart, box.TickEnd - 1))
                    _currFrameBoxes.Add(box);
            }
        }

        private void UpdateCurrFramePositions()
        {
            _currFramePositions = new List<Position>();

            foreach (Position pos in _positions)
            {
                if (_currentFrame.IsBetween(pos.TickStart, pos.TickEnd - 1))
                    _currFramePositions.Add(pos);
            }
        }

        /// <summary>
        /// Sets the Script Box text to a default value/script.
        /// </summary>
        void LoadDefaultScript()
        {
            tbScriptBox.Text = @"	{
	  ""Name"": ""SHOURYU_L"",
	  ""Index"": 930,
	  ""FirstHitboxFrame"": 3,
	  ""LastHitboxFrame"": 11,
	  ""InterruptFrame"": -1,
	  ""TotalTicks"": 40,
	  ""ReturnToOriginalPosition"": 15,
	  ""Slide"": 0.0,
	  ""unk3"": 0.0,
	  ""unk4"": 0.0,
	  ""unk5"": 0.0,
	  ""unk6"": 0.0,
	  ""unk7"": 0.0,
	  ""Flag"": 0,
	  ""unk9"": 0,
	  ""numberOfTypes"": 11,
	  ""unk13"": 0,
	  ""HeaderSize"": 64,
	  ""Unknown12"": 0,
	  ""Unknown13"": 0,
	  ""Unknown14"": 0,
	  ""Unknown15"": 0,
	  ""Unknown16"": 0,
	  ""Unknown17"": 0,
	  ""Unknown18"": 0.0,
	  ""Unknown19"": 0,
	  ""Unknown20"": 0,
	  ""Unknown21"": 0,
	  ""Unknown22"": 0,
	  ""AutoCancels"": [
		{
		  ""TickStart"": 12,
		  ""TickEnd"": 40,
		  ""Condition"": 8,
		  ""MoveIndex"": 936,
		  ""MoveIndexName"": null,
		  ""Unknown1"": 0,
		  ""NumberOfInts"": 0,
		  ""Unknown2"": 0,
		  ""Unknown3"": 0,
		  ""Unknown4"": 0,
		  ""Offset"": 0,
		  ""Ints"": []
		}
	  ],
	  ""Type1s"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 3,
		  ""Flag1"": 32,
		  ""Flag2"": 0
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 1,
		  ""Flag1"": 65536,
		  ""Flag2"": 0
		},
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 40,
		  ""Flag1"": 516,
		  ""Flag2"": 0
		}
	  ],
	  ""Forces"": [
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 7,
		  ""Amount"": 0.12,
		  ""Flag"": ""VerticalSpeed""
		},
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 7,
		  ""Amount"": -0.008,
		  ""Flag"": ""VerticalAcceleration""
		}
	  ],
	  ""Cancels"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 7,
		  ""CancelList"": 76,
		  ""Type"": 3
		},
		{
		  ""TickStart"": 5,
		  ""TickEnd"": 7,
		  ""CancelList"": 76,
		  ""Type"": 8
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 7,
		  ""CancelList"": 77,
		  ""Type"": 3
		},
		{
		  ""TickStart"": 5,
		  ""TickEnd"": 7,
		  ""CancelList"": 77,
		  ""Type"": 8
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 7,
		  ""CancelList"": 36,
		  ""Type"": 3
		},
		{
		  ""TickStart"": 5,
		  ""TickEnd"": 7,
		  ""CancelList"": 36,
		  ""Type"": 8
		}
	  ],
	  ""Others"": null,
	  ""Hitboxes"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 11,
		  ""X"": 0.0,
		  ""Y"": 0.6,
		  ""Z"": 0.0,
		  ""Width"": 2.0,
		  ""Height"": 0.5,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 76,
		  ""Unknown7"": 128,
		  ""Unknown8"": 255,
		  ""NumberOfHits"": 0,
		  ""HitType"": 4,
		  ""JuggleLimit"": 0,
		  ""JuggleIncrease"": 0,
		  ""Flag4"": 0,
		  ""HitboxEffectIndex"": 105,
		  ""Unknown10"": 0,
		  ""Unknown11"": 0,
		  ""Unknown12"": 0
		},
		{
		  ""TickStart"": 3,
		  ""TickEnd"": 4,
		  ""X"": 0.2,
		  ""Y"": 0.0,
		  ""Z"": 0.0,
		  ""Width"": 0.59,
		  ""Height"": 1.1,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 72,
		  ""Unknown7"": 128,
		  ""Unknown8"": 17920,
		  ""NumberOfHits"": 1,
		  ""HitType"": 0,
		  ""JuggleLimit"": 55,
		  ""JuggleIncrease"": 0,
		  ""Flag4"": 0,
		  ""HitboxEffectIndex"": 96,
		  ""Unknown10"": 0,
		  ""Unknown11"": 5,
		  ""Unknown12"": 0
		},
		{
		  ""TickStart"": 4,
		  ""TickEnd"": 5,
		  ""X"": 0.15,
		  ""Y"": 0.32,
		  ""Z"": 0.0,
		  ""Width"": 0.57,
		  ""Height"": 1.2,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 72,
		  ""Unknown7"": 128,
		  ""Unknown8"": 17920,
		  ""NumberOfHits"": 1,
		  ""HitType"": 0,
		  ""JuggleLimit"": 55,
		  ""JuggleIncrease"": 0,
		  ""Flag4"": 0,
		  ""HitboxEffectIndex"": 96,
		  ""Unknown10"": 0,
		  ""Unknown11"": 5,
		  ""Unknown12"": 0
		},
		{
		  ""TickStart"": 5,
		  ""TickEnd"": 6,
		  ""X"": 0.0,
		  ""Y"": 0.5,
		  ""Z"": 0.0,
		  ""Width"": 0.67,
		  ""Height"": 1.36,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 72,
		  ""Unknown7"": 128,
		  ""Unknown8"": 17921,
		  ""NumberOfHits"": 1,
		  ""HitType"": 0,
		  ""JuggleLimit"": 55,
		  ""JuggleIncrease"": 0,
		  ""Flag4"": 0,
		  ""HitboxEffectIndex"": 101,
		  ""Unknown10"": 0,
		  ""Unknown11"": 5,
		  ""Unknown12"": 0
		},
		{
		  ""TickStart"": 7,
		  ""TickEnd"": 11,
		  ""X"": -0.05,
		  ""Y"": 0.65,
		  ""Z"": 0.0,
		  ""Width"": 0.68,
		  ""Height"": 1.6,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 72,
		  ""Unknown7"": 128,
		  ""Unknown8"": 17922,
		  ""NumberOfHits"": 1,
		  ""HitType"": 0,
		  ""JuggleLimit"": 55,
		  ""JuggleIncrease"": 15,
		  ""Flag4"": 0,
		  ""HitboxEffectIndex"": 102,
		  ""Unknown10"": 0,
		  ""Unknown11"": 5,
		  ""Unknown12"": 0
		}
	  ],
	  ""Hurtboxes"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 6,
		  ""X"": -0.45,
		  ""Y"": 0.9,
		  ""Z"": 0.0,
		  ""Width"": 0.9,
		  ""Height"": 0.4,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0,
		  ""Unknown7"": 0,
		  ""Unknown8"": 0,
		  ""Unknown9"": 1,
		  ""Flag1"": 3,
		  ""Flag2"": 0,
		  ""Flag3"": 0,
		  ""Flag4"": 0,
		  ""HitEffect"": 0,
		  ""Unknown10"": 3,
		  ""Unknown11"": 0,
		  ""Unknown12"": 1.0,
		  ""Unknown13"": 0
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 6,
		  ""X"": -0.45,
		  ""Y"": 0.0,
		  ""Z"": 0.0,
		  ""Width"": 0.9,
		  ""Height"": 0.9,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0,
		  ""Unknown7"": 0,
		  ""Unknown8"": 0,
		  ""Unknown9"": 1,
		  ""Flag1"": 3,
		  ""Flag2"": 0,
		  ""Flag3"": 0,
		  ""Flag4"": 0,
		  ""HitEffect"": 0,
		  ""Unknown10"": 3,
		  ""Unknown11"": 0,
		  ""Unknown12"": 1.0,
		  ""Unknown13"": 0
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 6,
		  ""X"": -0.25,
		  ""Y"": 0.0,
		  ""Z"": 0.0,
		  ""Width"": 0.5,
		  ""Height"": 1.0,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0,
		  ""Unknown7"": 0,
		  ""Unknown8"": 0,
		  ""Unknown9"": 1,
		  ""Flag1"": 4,
		  ""Flag2"": 0,
		  ""Flag3"": 0,
		  ""Flag4"": 0,
		  ""HitEffect"": 0,
		  ""Unknown10"": 3,
		  ""Unknown11"": 0,
		  ""Unknown12"": 1.0,
		  ""Unknown13"": 0
		},
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 40,
		  ""X"": -0.35,
		  ""Y"": 0.8,
		  ""Z"": 0.0,
		  ""Width"": 0.7,
		  ""Height"": 1.1,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0,
		  ""Unknown7"": 0,
		  ""Unknown8"": 0,
		  ""Unknown9"": 1,
		  ""Flag1"": 3,
		  ""Flag2"": 0,
		  ""Flag3"": 0,
		  ""Flag4"": 0,
		  ""HitEffect"": 0,
		  ""Unknown10"": 4,
		  ""Unknown11"": 0,
		  ""Unknown12"": 1.0,
		  ""Unknown13"": 0
		},
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 40,
		  ""X"": -0.25,
		  ""Y"": 0.9,
		  ""Z"": 0.0,
		  ""Width"": 0.5,
		  ""Height"": 0.9,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0,
		  ""Unknown7"": 0,
		  ""Unknown8"": 0,
		  ""Unknown9"": 1,
		  ""Flag1"": 4,
		  ""Flag2"": 0,
		  ""Flag3"": 0,
		  ""Flag4"": 0,
		  ""HitEffect"": 0,
		  ""Unknown10"": 4,
		  ""Unknown11"": 0,
		  ""Unknown12"": 1.0,
		  ""Unknown13"": 0
		}
	  ],
	  ""PhysicsBoxes"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 6,
		  ""X"": -0.25,
		  ""Y"": 0.0,
		  ""Z"": 0.0,
		  ""Width"": 0.5,
		  ""Height"": 1.35,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 1
		},
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 11,
		  ""X"": -0.25,
		  ""Y"": 0.199999988,
		  ""Z"": 0.0,
		  ""Width"": 0.5,
		  ""Height"": 1.5999999,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 1
		},
		{
		  ""TickStart"": 11,
		  ""TickEnd"": 40,
		  ""X"": -0.25,
		  ""Y"": 0.199999988,
		  ""Z"": 0.0,
		  ""Width"": 0.5,
		  ""Height"": 1.5999999,
		  ""Unknown1"": 0,
		  ""Unknown2"": 25,
		  ""Unknown3"": 25,
		  ""Unknown4"": 25,
		  ""Unknown5"": 0,
		  ""Unknown6"": 1
		}
	  ],
	  ""Animations"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 4,
		  ""Index"": 101,
		  ""Type"": ""Regular"",
		  ""FrameStart"": 14,
		  ""FrameEnd"": 18,
		  ""Unknown1"": 0,
		  ""Unknown2"": 80
		},
		{
		  ""TickStart"": 4,
		  ""TickEnd"": 13,
		  ""Index"": 101,
		  ""Type"": ""Regular"",
		  ""FrameStart"": 18,
		  ""FrameEnd"": 26,
		  ""Unknown1"": 0,
		  ""Unknown2"": 64
		},
		{
		  ""TickStart"": 13,
		  ""TickEnd"": 40,
		  ""Index"": 101,
		  ""Type"": ""Regular"",
		  ""FrameStart"": 26,
		  ""FrameEnd"": 42,
		  ""Unknown1"": 0,
		  ""Unknown2"": 48
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 31,
		  ""Index"": 29,
		  ""Type"": ""Face"",
		  ""FrameStart"": 0,
		  ""FrameEnd"": 0,
		  ""Unknown1"": 0,
		  ""Unknown2"": 32
		},
		{
		  ""TickStart"": 31,
		  ""TickEnd"": 40,
		  ""Index"": 3,
		  ""Type"": ""Face"",
		  ""FrameStart"": 0,
		  ""FrameEnd"": 0,
		  ""Unknown1"": 0,
		  ""Unknown2"": 16
		}
	  ],
	  ""Type9s"": null,
	  ""SoundEffects"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 1,
		  ""Unknown1"": 0,
		  ""Unknown2"": 4,
		  ""Unknown3"": 1,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 1,
		  ""Unknown1"": 0,
		  ""Unknown2"": 1002,
		  ""Unknown3"": 3,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 1,
		  ""TickEnd"": 2,
		  ""Unknown1"": 0,
		  ""Unknown2"": 711,
		  ""Unknown3"": 2,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 1,
		  ""TickEnd"": 2,
		  ""Unknown1"": 0,
		  ""Unknown2"": 10,
		  ""Unknown3"": 3,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 1,
		  ""TickEnd"": 2,
		  ""Unknown1"": 0,
		  ""Unknown2"": 20,
		  ""Unknown3"": 3,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 16,
		  ""TickEnd"": 17,
		  ""Unknown1"": 0,
		  ""Unknown2"": 10,
		  ""Unknown3"": 3,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 16,
		  ""TickEnd"": 17,
		  ""Unknown1"": 0,
		  ""Unknown2"": 20,
		  ""Unknown3"": 3,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		},
		{
		  ""TickStart"": 22,
		  ""TickEnd"": 23,
		  ""Unknown1"": 0,
		  ""Unknown2"": 11,
		  ""Unknown3"": 3,
		  ""Unknown4"": 0,
		  ""Unknown5"": 0,
		  ""Unknown6"": 0
		}
	  ],
	  ""VisualEffects"": [
		{
		  ""TickStart"": 3,
		  ""TickEnd"": 4,
		  ""Unknown1"": 0,
		  ""Unknown2"": 0,
		  ""Unknown3"": 1,
		  ""Type"": 13,
		  ""Unknown5"": 0,
		  ""AttachPoint"": -1,
		  ""X"": -0.2,
		  ""Y"": 0.0,
		  ""Z"": 0.0,
		  ""Unknown10"": 0,
		  ""Size"": 1.0,
		  ""Unknown12"": 0.0
		}
	  ],
	  ""Positions"": [
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 40,
		  ""Movement"": -0.622657,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 13,
		  ""TickEnd"": 14,
		  ""Movement"": 1.328662,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 14,
		  ""TickEnd"": 15,
		  ""Movement"": 1.338222,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 15,
		  ""TickEnd"": 16,
		  ""Movement"": 1.348126,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 16,
		  ""TickEnd"": 17,
		  ""Movement"": 1.358631,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 17,
		  ""TickEnd"": 18,
		  ""Movement"": 1.369205,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 18,
		  ""TickEnd"": 19,
		  ""Movement"": 1.379814,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 19,
		  ""TickEnd"": 20,
		  ""Movement"": 1.390305,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 20,
		  ""TickEnd"": 21,
		  ""Movement"": 1.400853,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 21,
		  ""TickEnd"": 22,
		  ""Movement"": 1.411539,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 22,
		  ""TickEnd"": 23,
		  ""Movement"": 1.422242,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 23,
		  ""TickEnd"": 24,
		  ""Movement"": 1.432956,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 24,
		  ""TickEnd"": 25,
		  ""Movement"": 1.443575,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 25,
		  ""TickEnd"": 26,
		  ""Movement"": 1.454129,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 26,
		  ""TickEnd"": 27,
		  ""Movement"": 1.464497,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 27,
		  ""TickEnd"": 28,
		  ""Movement"": 1.474652,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 28,
		  ""TickEnd"": 29,
		  ""Movement"": 1.484632,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 29,
		  ""TickEnd"": 30,
		  ""Movement"": 1.494218,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 30,
		  ""TickEnd"": 31,
		  ""Movement"": 1.503627,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 31,
		  ""TickEnd"": 32,
		  ""Movement"": 1.512504,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 32,
		  ""TickEnd"": 33,
		  ""Movement"": 1.520964,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 33,
		  ""TickEnd"": 34,
		  ""Movement"": 1.528987,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 34,
		  ""TickEnd"": 35,
		  ""Movement"": 1.5361,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 35,
		  ""TickEnd"": 36,
		  ""Movement"": 1.542843,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 36,
		  ""TickEnd"": 37,
		  ""Movement"": 1.548472,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 37,
		  ""TickEnd"": 38,
		  ""Movement"": 1.553434,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 38,
		  ""TickEnd"": 39,
		  ""Movement"": 1.557495,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 39,
		  ""TickEnd"": 40,
		  ""Movement"": 1.559596,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 4,
		  ""TickEnd"": 5,
		  ""Movement"": 1.075464,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 5,
		  ""TickEnd"": 6,
		  ""Movement"": 1.141714,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 6,
		  ""TickEnd"": 7,
		  ""Movement"": 1.190735,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 7,
		  ""TickEnd"": 8,
		  ""Movement"": 1.222457,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 8,
		  ""TickEnd"": 9,
		  ""Movement"": 1.247365,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 9,
		  ""TickEnd"": 10,
		  ""Movement"": 1.270112,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 10,
		  ""TickEnd"": 11,
		  ""Movement"": 1.289145,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 11,
		  ""TickEnd"": 12,
		  ""Movement"": 1.303309,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 12,
		  ""TickEnd"": 13,
		  ""Movement"": 1.316009,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 0,
		  ""TickEnd"": 1,
		  ""Movement"": 0.622657,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 1,
		  ""TickEnd"": 2,
		  ""Movement"": 0.775275,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 2,
		  ""TickEnd"": 3,
		  ""Movement"": 0.897873,
		  ""Flag"": 32768
		},
		{
		  ""TickStart"": 3,
		  ""TickEnd"": 4,
		  ""Movement"": 0.990188,
		  ""Flag"": 32768
		}
	  ]
	}"; // TODO: use scriptviz_sampledata.json
        }

        void Exit()
        {
            // Save work, etc. before closing 
            Close();
        }

    }

    public struct Box
    {
        public int TickStart, TickEnd;
        public float X, Y;
        public float Width, Height;
        public string BoxType;
        public float HitType, HitboxEffectIndex;
    }

    public struct Position
    {
        public int TickStart, TickEnd;
        public float Movement;
        public int Flag;
    }
}

public static class Util
{
    public static bool IsBetween<T>(this T item, T start, T end)
    {
        return Comparer<T>.Default.Compare(item, start) >= 0
            && Comparer<T>.Default.Compare(item, end) <= 0;
    }
}
