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
        
        private double _currentFrame;
        private double _maxFrame;

        public const string LABEL_TEXT_COLOR = "#FFababad";
        public const float BOX_SCALAR = 100;
        public const float CANVAS_PADDING = 15;

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

            if (DEBUG) {
                LoadDefaultScript();
            }
        }

        #region Event Responses

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            #region Reset Display
            spDisplayContainer.Children.Clear();

            SetCanvasPosition(new Point(), false);

            _boxes = new List<Box>(); // TODO: Define different hitbox types
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

                    _maxFrame = jobj.TotalTicks;
                    
                    #region Check For Boxes
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
                            box.BoxType = (box.HitboxEffectIndex == -1) ? "ProxBox": "Hitbox";

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
            // update canvas size
            canvasScriptViz.Width = e.NewSize.Width;
            canvasScriptViz.Height = e.NewSize.Height;

            // update boxes
            if (_boxes != null)
                UpdateBoxPositions(sender, e);
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

        private void DrawBoxes()
        {
            canvasScriptViz.Children.Clear();
            _rectangles = new List<Rectangle>();
            
            UpdateCurrFrameBoxes();

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

                Canvas.SetLeft(rectangle, canvasScriptViz.ActualWidth / 2 + box.X * BOX_SCALAR);
                Canvas.SetBottom(rectangle, CANVAS_PADDING + box.Y * BOX_SCALAR + box.Height);

                canvasScriptViz.Children.Add(rectangle);
                _rectangles.Add(rectangle);
            }
        }

        private void UpdateCurrFrameBoxes()
        {
            _currFrameBoxes = new List<Box>();

            foreach (Box box in _boxes)
            {
                if (_currentFrame.IsBetween(box.TickStart, box.TickEnd))
                    _currFrameBoxes.Add(box);
            }
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

        #region Exit Handling
        private void menuitemExit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        void Exit()
        {
            Close();
        }
        #endregion

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

        private void UpdateSliderLabel()
        {
            if (lblCurrentFrame != null) lblCurrentFrame.Content = String.Format("{0} / {1}", _currentFrame, _maxFrame);
        }

        private void SetCanvasPosition(Point position, bool useOriginalPosition = true)
        {
            Canvas.SetLeft(canvasScriptViz, useOriginalPosition? (originalCanvasPos.X + position.X) : position.X);
            Canvas.SetBottom(canvasScriptViz, useOriginalPosition? (originalCanvasPos.Y + position.Y) : position.Y);
        }

        void LoadDefaultScript()
        {
            tbScriptBox.Text = @"{
          ""Name"": ""STAND"",
          ""Index"": 0,
          ""FirstHitboxFrame"": -1,
          ""LastHitboxFrame"": 361,
          ""InterruptFrame"": -1,
          ""TotalTicks"": 1679,
          ""ReturnToOriginalPosition"": 19,
          ""Slide"": 0.0,
          ""unk3"": 0.0,
          ""unk4"": 0.0,
          ""unk5"": 0.0,
          ""unk6"": 0.0,
          ""unk7"": 0.0,
          ""Flag"": 0,
          ""unk9"": 0,
          ""numberOfTypes"": 8,
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
          ""AutoCancels"": null,
          ""Type1s"": null,
          ""Forces"": null,
          ""Cancels"": null,
          ""Others"": [
            {
              ""TickStart"": 931,
              ""TickEnd"": 932,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 108,
              ""Ints"": [
                2,
                2,
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
    },
            {
              ""TickStart"": 1002,
              ""TickEnd"": 1003,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 160,
              ""Ints"": [
                2,
                2,
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
},
            {
              ""TickStart"": 1071,
              ""TickEnd"": 1072,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 212,
              ""Ints"": [
                2,
                2,
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            },
            {
              ""TickStart"": 1109,
              ""TickEnd"": 1110,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 264,
              ""Ints"": [
                1,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            },
            {
              ""TickStart"": 1160,
              ""TickEnd"": 1161,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 316,
              ""Ints"": [
                1,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            },
            {
              ""TickStart"": 1215,
              ""TickEnd"": 1216,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 368,
              ""Ints"": [
                8,
                0,
                10,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            },
            {
              ""TickStart"": 1407,
              ""TickEnd"": 1408,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 420,
              ""Ints"": [
                10,
                5,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            },
            {
              ""TickStart"": 1454,
              ""TickEnd"": 1455,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 472,
              ""Ints"": [
                10,
                5,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            },
            {
              ""TickStart"": 1516,
              ""TickEnd"": 1517,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 6,
              ""Unknown2"": 0,
              ""NumberOfInts"": 16,
              ""Offset"": 524,
              ""Ints"": [
                12,
                12,
                12,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
              ]
            }
          ],
          ""Hitboxes"": [
            {
              ""TickStart"": 1520,
              ""TickEnd"": 1525,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""X"": 0.0,
              ""Y"": 1.4,
              ""Z"": 0.0,
              ""Width"": 1.15,
              ""Height"": 0.6,
              ""Unknown1"": 0,
              ""Unknown2"": 25,
              ""Unknown3"": 25,
              ""Unknown4"": 25,
              ""Unknown5"": 0,
              ""Unknown6"": 8,
              ""Unknown7"": 128,
              ""Unknown8"": 7680,
              ""NumberOfHits"": 1,
              ""HitType"": 0,
              ""JuggleLimit"": 0,
              ""JuggleIncrease"": 1,
              ""Flag4"": 0,
              ""HitboxEffectIndex"": 2,
              ""Unknown10"": 0,
              ""Unknown11"": 0,
              ""Unknown12"": 0
            }
		  ],
          ""Hurtboxes"": [
            {
              ""TickStart"": 0,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""X"": -0.4,
              ""Y"": 1.8,
              ""Z"": 0.0,
              ""Width"": 0.9,
              ""Height"": 0.45,
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
              ""Unknown10"": 2,
              ""Unknown11"": 0,
              ""Unknown12"": 1.0,
              ""Unknown13"": 0
            },
            {
              ""TickStart"": 0,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""X"": -0.4,
              ""Y"": 0.9,
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
              ""Unknown10"": 1,
              ""Unknown11"": 0,
              ""Unknown12"": 1.0,
              ""Unknown13"": 0
            },
            {
              ""TickStart"": 0,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""X"": -0.4,
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
              ""Unknown10"": 0,
              ""Unknown11"": 0,
              ""Unknown12"": 1.0,
              ""Unknown13"": 0
            },
            {
              ""TickStart"": 0,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""X"": -0.3,
              ""Y"": 0.0,
              ""Z"": 0.0,
              ""Width"": 0.7,
              ""Height"": 1.65,
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
              ""Unknown10"": 1,
              ""Unknown11"": 0,
              ""Unknown12"": 1.0,
              ""Unknown13"": 0
            }
          ],
          ""PhysicsBoxes"": [
            {
              ""TickStart"": 0,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""X"": -0.3,
              ""Y"": 0.0,
              ""Z"": 0.0,
              ""Width"": 0.7,
              ""Height"": 1.65,
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
              ""TickEnd"": 281,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Regular"",
              ""FrameStart"": 0,
              ""FrameEnd"": 281,
              ""Unknown1"": 0,
              ""Unknown2"": 848
            },
            {
              ""TickStart"": 281,
              ""TickEnd"": 562,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Regular"",
              ""FrameStart"": 0,
              ""FrameEnd"": 281,
              ""Unknown1"": 0,
              ""Unknown2"": 832
            },
            {
              ""TickStart"": 562,
              ""TickEnd"": 843,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Regular"",
              ""FrameStart"": 0,
              ""FrameEnd"": 281,
              ""Unknown1"": 0,
              ""Unknown2"": 816
            },
            {
              ""TickStart"": 843,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 30,
              ""Type"": ""Regular"",
              ""FrameStart"": 0,
              ""FrameEnd"": 836,
              ""Unknown1"": 0,
              ""Unknown2"": 800
            },
            {
              ""TickStart"": 0,
              ""TickEnd"": 882,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 784
            },
            {
              ""TickStart"": 882,
              ""TickEnd"": 892,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 21,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 768
            },
            {
              ""TickStart"": 892,
              ""TickEnd"": 899,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 752
            },
            {
              ""TickStart"": 899,
              ""TickEnd"": 901,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 736
            },
            {
              ""TickStart"": 901,
              ""TickEnd"": 907,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 720
            },
            {
              ""TickStart"": 907,
              ""TickEnd"": 909,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 704
            },
            {
              ""TickStart"": 909,
              ""TickEnd"": 915,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 688
            },
            {
              ""TickStart"": 915,
              ""TickEnd"": 917,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 672
            },
            {
              ""TickStart"": 917,
              ""TickEnd"": 927,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 656
            },
            {
              ""TickStart"": 927,
              ""TickEnd"": 943,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 640
            },
            {
              ""TickStart"": 943,
              ""TickEnd"": 950,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 624
            },
            {
              ""TickStart"": 950,
              ""TickEnd"": 952,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 608
            },
            {
              ""TickStart"": 952,
              ""TickEnd"": 960,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 592
            },
            {
              ""TickStart"": 960,
              ""TickEnd"": 982,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 576
            },
            {
              ""TickStart"": 982,
              ""TickEnd"": 1002,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 21,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 560
            },
            {
              ""TickStart"": 1002,
              ""TickEnd"": 1012,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 544
            },
            {
              ""TickStart"": 1012,
              ""TickEnd"": 1017,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 528
            },
            {
              ""TickStart"": 1017,
              ""TickEnd"": 1023,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 512
            },
            {
              ""TickStart"": 1023,
              ""TickEnd"": 1025,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 496
            },
            {
              ""TickStart"": 1025,
              ""TickEnd"": 1031,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 480
            },
            {
              ""TickStart"": 1031,
              ""TickEnd"": 1058,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 464
            },
            {
              ""TickStart"": 1058,
              ""TickEnd"": 1064,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 448
            },
            {
              ""TickStart"": 1064,
              ""TickEnd"": 1066,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 432
            },
            {
              ""TickStart"": 1066,
              ""TickEnd"": 1072,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 416
            },
            {
              ""TickStart"": 1072,
              ""TickEnd"": 1092,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 400
            },
            {
              ""TickStart"": 1092,
              ""TickEnd"": 1099,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 384
            },
            {
              ""TickStart"": 1099,
              ""TickEnd"": 1101,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 368
            },
            {
              ""TickStart"": 1101,
              ""TickEnd"": 1108,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 352
            },
            {
              ""TickStart"": 1108,
              ""TickEnd"": 1145,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 6,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 336
            },
            {
              ""TickStart"": 1145,
              ""TickEnd"": 1160,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 21,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 320
            },
            {
              ""TickStart"": 1160,
              ""TickEnd"": 1168,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 13,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 304
            },
            {
              ""TickStart"": 1168,
              ""TickEnd"": 1183,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 21,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 288
            },
            {
              ""TickStart"": 1183,
              ""TickEnd"": 1203,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 272
            },
            {
              ""TickStart"": 1203,
              ""TickEnd"": 1224,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 19,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 256
            },
            {
              ""TickStart"": 1224,
              ""TickEnd"": 1279,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 9,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 240
            },
            {
              ""TickStart"": 1279,
              ""TickEnd"": 1284,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 13,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 224
            },
            {
              ""TickStart"": 1284,
              ""TickEnd"": 1350,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 208
            },
            {
              ""TickStart"": 1350,
              ""TickEnd"": 1354,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 13,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 192
            },
            {
              ""TickStart"": 1354,
              ""TickEnd"": 1394,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 176
            },
            {
              ""TickStart"": 1394,
              ""TickEnd"": 1402,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 21,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 160
            },
            {
              ""TickStart"": 1402,
              ""TickEnd"": 1414,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 29,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 144
            },
            {
              ""TickStart"": 1414,
              ""TickEnd"": 1436,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 21,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 128
            },
            {
              ""TickStart"": 1436,
              ""TickEnd"": 1471,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 2,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 112
            },
            {
              ""TickStart"": 1471,
              ""TickEnd"": 1481,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 12,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 96
            },
            {
              ""TickStart"": 1481,
              ""TickEnd"": 1489,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 1,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 80
            },
            {
              ""TickStart"": 1489,
              ""TickEnd"": 1499,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 7,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 64
            },
            {
              ""TickStart"": 1499,
              ""TickEnd"": 1509,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 19,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 48
            },
            {
              ""TickStart"": 1509,
              ""TickEnd"": 1559,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 12,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 32
            },
            {
              ""TickStart"": 1559,
              ""TickEnd"": 1679,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Index"": 0,
              ""Type"": ""Face"",
              ""FrameStart"": 0,
              ""FrameEnd"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 16
            }
          ],
          ""Type9s"": [
            {
              ""TickStart"": 0,
              ""TickEnd"": 5,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 2,
              ""Unknown2"": 0,
              ""Unknown3"": 0.2
            },
            {
              ""TickStart"": 1559,
              ""TickEnd"": 1560,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 2.0
            },
            {
              ""TickStart"": 1560,
              ""TickEnd"": 1561,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.948718
            },
            {
              ""TickStart"": 1561,
              ""TickEnd"": 1562,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.8974359
            },
            {
              ""TickStart"": 1562,
              ""TickEnd"": 1563,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.84615386
            },
            {
              ""TickStart"": 1563,
              ""TickEnd"": 1564,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.79487181
            },
            {
              ""TickStart"": 1564,
              ""TickEnd"": 1565,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.74358976
            },
            {
              ""TickStart"": 1565,
              ""TickEnd"": 1566,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.69230771
            },
            {
              ""TickStart"": 1566,
              ""TickEnd"": 1567,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.64102566
            },
            {
              ""TickStart"": 1567,
              ""TickEnd"": 1568,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.58974361
            },
            {
              ""TickStart"": 1568,
              ""TickEnd"": 1569,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.53846157
            },
            {
              ""TickStart"": 1569,
              ""TickEnd"": 1570,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.48717952
            },
            {
              ""TickStart"": 1570,
              ""TickEnd"": 1571,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.43589735
            },
            {
              ""TickStart"": 1571,
              ""TickEnd"": 1572,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.38461542
            },
            {
              ""TickStart"": 1572,
              ""TickEnd"": 1573,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.33333325
            },
            {
              ""TickStart"": 1573,
              ""TickEnd"": 1574,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.28205132
            },
            {
              ""TickStart"": 1574,
              ""TickEnd"": 1575,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.23076916
            },
            {
              ""TickStart"": 1575,
              ""TickEnd"": 1576,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.17948723
            },
            {
              ""TickStart"": 1576,
              ""TickEnd"": 1577,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.12820506
            },
            {
              ""TickStart"": 1577,
              ""TickEnd"": 1578,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.07692313
            },
            {
              ""TickStart"": 1578,
              ""TickEnd"": 1579,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.025641
            },
            {
              ""TickStart"": 1579,
              ""TickEnd"": 1580,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.9743589
            },
            {
              ""TickStart"": 1580,
              ""TickEnd"": 1581,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.923076868
            },
            {
              ""TickStart"": 1581,
              ""TickEnd"": 1582,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.8717948
            },
            {
              ""TickStart"": 1582,
              ""TickEnd"": 1583,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.8205128
            },
            {
              ""TickStart"": 1583,
              ""TickEnd"": 1584,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.7692307
            },
            {
              ""TickStart"": 1584,
              ""TickEnd"": 1585,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.7179487
            },
            {
              ""TickStart"": 1585,
              ""TickEnd"": 1586,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.6666666
            },
            {
              ""TickStart"": 1586,
              ""TickEnd"": 1587,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.6153846
            },
            {
              ""TickStart"": 1587,
              ""TickEnd"": 1588,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.56410253
            },
            {
              ""TickStart"": 1588,
              ""TickEnd"": 1589,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.5128205
            },
            {
              ""TickStart"": 1589,
              ""TickEnd"": 1590,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.461538434
            },
            {
              ""TickStart"": 1590,
              ""TickEnd"": 1591,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.4102564
            },
            {
              ""TickStart"": 1591,
              ""TickEnd"": 1592,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.358974338
            },
            {
              ""TickStart"": 1592,
              ""TickEnd"": 1593,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.3076923
            },
            {
              ""TickStart"": 1593,
              ""TickEnd"": 1594,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.256410241
            },
            {
              ""TickStart"": 1594,
              ""TickEnd"": 1595,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.2051282
            },
            {
              ""TickStart"": 1595,
              ""TickEnd"": 1596,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.153846145
            },
            {
              ""TickStart"": 1596,
              ""TickEnd"": 1597,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1025641
            },
            {
              ""TickStart"": 1597,
              ""TickEnd"": 1598,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.05128205
            },
            {
              ""TickStart"": 1598,
              ""TickEnd"": 1599,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 1099,
              ""TickEnd"": 1100,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1
            },
            {
              ""TickStart"": 1100,
              ""TickEnd"": 1101,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 1064,
              ""TickEnd"": 1065,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1
            },
            {
              ""TickStart"": 1065,
              ""TickEnd"": 1066,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 1023,
              ""TickEnd"": 1024,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1
            },
            {
              ""TickStart"": 1024,
              ""TickEnd"": 1025,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 950,
              ""TickEnd"": 951,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1
            },
            {
              ""TickStart"": 951,
              ""TickEnd"": 952,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 915,
              ""TickEnd"": 916,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.2
            },
            {
              ""TickStart"": 916,
              ""TickEnd"": 917,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 907,
              ""TickEnd"": 908,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1
            },
            {
              ""TickStart"": 908,
              ""TickEnd"": 909,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 899,
              ""TickEnd"": 900,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.1
            },
            {
              ""TickStart"": 900,
              ""TickEnd"": 901,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            },
            {
              ""TickStart"": 0,
              ""TickEnd"": 1,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 1.0
            },
            {
              ""TickStart"": 1,
              ""TickEnd"": 2,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.9655172
            },
            {
              ""TickStart"": 2,
              ""TickEnd"": 3,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.9310345
            },
            {
              ""TickStart"": 3,
              ""TickEnd"": 4,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.8965517
            },
            {
              ""TickStart"": 4,
              ""TickEnd"": 5,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.862068951
            },
            {
              ""TickStart"": 5,
              ""TickEnd"": 6,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.8275862
            },
            {
              ""TickStart"": 6,
              ""TickEnd"": 7,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.793103456
            },
            {
              ""TickStart"": 7,
              ""TickEnd"": 8,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.7586207
            },
            {
              ""TickStart"": 8,
              ""TickEnd"": 9,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.7241379
            },
            {
              ""TickStart"": 9,
              ""TickEnd"": 10,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.6896552
            },
            {
              ""TickStart"": 10,
              ""TickEnd"": 11,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.6551724
            },
            {
              ""TickStart"": 11,
              ""TickEnd"": 12,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.620689631
            },
            {
              ""TickStart"": 12,
              ""TickEnd"": 13,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.5862069
            },
            {
              ""TickStart"": 13,
              ""TickEnd"": 14,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.551724136
            },
            {
              ""TickStart"": 14,
              ""TickEnd"": 15,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.517241359
            },
            {
              ""TickStart"": 15,
              ""TickEnd"": 16,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.482758641
            },
            {
              ""TickStart"": 16,
              ""TickEnd"": 17,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.448275864
            },
            {
              ""TickStart"": 17,
              ""TickEnd"": 18,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.4137931
            },
            {
              ""TickStart"": 18,
              ""TickEnd"": 19,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.379310369
            },
            {
              ""TickStart"": 19,
              ""TickEnd"": 20,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.3448276
            },
            {
              ""TickStart"": 20,
              ""TickEnd"": 21,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.310344815
            },
            {
              ""TickStart"": 21,
              ""TickEnd"": 22,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.2758621
            },
            {
              ""TickStart"": 22,
              ""TickEnd"": 23,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.241379321
            },
            {
              ""TickStart"": 23,
              ""TickEnd"": 24,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.206896544
            },
            {
              ""TickStart"": 24,
              ""TickEnd"": 25,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.172413826
            },
            {
              ""TickStart"": 25,
              ""TickEnd"": 26,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.137931049
            },
            {
              ""TickStart"": 26,
              ""TickEnd"": 27,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.103448272
            },
            {
              ""TickStart"": 27,
              ""TickEnd"": 28,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0689654946
            },
            {
              ""TickStart"": 28,
              ""TickEnd"": 29,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0344827771
            },
            {
              ""TickStart"": 29,
              ""TickEnd"": 30,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 4,
              ""Unknown2"": 2,
              ""Unknown3"": 0.0
            }
          ],
          ""SoundEffects"": [
            {
              ""TickStart"": 876,
              ""TickEnd"": 877,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 876,
              ""TickEnd"": 877,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 894,
              ""TickEnd"": 895,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1600,
              ""Unknown3"": 514,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 931,
              ""TickEnd"": 932,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 25,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 931,
              ""TickEnd"": 932,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1004,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 958,
              ""TickEnd"": 959,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 958,
              ""TickEnd"": 959,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1001,
              ""TickEnd"": 1002,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 26,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1001,
              ""TickEnd"": 1002,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1004,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1019,
              ""TickEnd"": 1020,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1019,
              ""TickEnd"": 1020,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1070,
              ""TickEnd"": 1071,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 27,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1070,
              ""TickEnd"": 1071,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1004,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1093,
              ""TickEnd"": 1094,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1093,
              ""TickEnd"": 1094,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1109,
              ""TickEnd"": 1110,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 25,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1123,
              ""TickEnd"": 1124,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1123,
              ""TickEnd"": 1124,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1160,
              ""TickEnd"": 1161,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 26,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1160,
              ""TickEnd"": 1161,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1004,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1176,
              ""TickEnd"": 1177,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 28,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1196,
              ""TickEnd"": 1197,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 10,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1196,
              ""TickEnd"": 1197,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 20,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1208,
              ""TickEnd"": 1209,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1418,
              ""Unknown3"": 2,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1215,
              ""TickEnd"": 1216,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1005,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1328,
              ""TickEnd"": 1329,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1328,
              ""TickEnd"": 1329,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1398,
              ""TickEnd"": 1399,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 202,
              ""Unknown3"": 2,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1399,
              ""TickEnd"": 1400,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 29,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1485,
              ""TickEnd"": 1486,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1485,
              ""TickEnd"": 1486,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1508,
              ""TickEnd"": 1509,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 30,
              ""Unknown3"": 1,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1509,
              ""TickEnd"": 1510,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 10,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1509,
              ""TickEnd"": 1510,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 20,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1513,
              ""TickEnd"": 1514,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 1417,
              ""Unknown3"": 2,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1549,
              ""TickEnd"": 1550,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 11,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            },
            {
              ""TickStart"": 1549,
              ""TickEnd"": 1550,
              ""BACVERint1"": 0,
              ""BACVERint2"": 0,
              ""BACVERint3"": 0,
              ""BACVERint4"": 0,
              ""Unknown1"": 0,
              ""Unknown2"": 21,
              ""Unknown3"": 3,
              ""Unknown4"": 0,
              ""Unknown5"": 0,
              ""Unknown6"": 0
            }
          ],
          ""VisualEffects"": null,
          ""Positions"": null
}"; // TODO: use scriptviz_sampledata.json
        }

        private void UpdateBoxPositions(object sender, SizeChangedEventArgs e)
        {
            if (_rectangles != null)
            {
                foreach (Rectangle rectangle in _rectangles)
                {
                    int index = _rectangles.IndexOf(rectangle);
                    Box box = _currFrameBoxes[index];

                    Canvas.SetLeft(rectangle, canvasScriptViz.ActualWidth / 2 + box.X*BOX_SCALAR);
                    Canvas.SetBottom(rectangle, CANVAS_PADDING + box.Y * BOX_SCALAR + box.Height);
                }
            }
        }

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

    }

    public struct Box
    {
        public float X, Y;
        public float Width, Height;
        public int TickStart, TickEnd;
        public string BoxType;
        public float HitboxEffectIndex;
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
