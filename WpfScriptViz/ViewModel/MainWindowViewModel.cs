using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScriptLib;
using ScriptViz.Command;
using ScriptViz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScriptViz.ViewModel
{
    public class MainWindowViewModel : VMBase
    {
        MainWindowModel model;

        #region Lists

        public ObservableCollection<Box> CurrFrameBoxes;

        ObservableCollection<Box> _boxes;
        ObservableCollection<Rectangle> _rectangles;

        List<Position>  _positions;
        List<Position>  _currFramePositions;
        #endregion

        StreamReader file;

        double _currentFrame;
        double _maxFrame;

        public double CurrentFrame
        {
            get => _currentFrame;
            set
            {
                _currentFrame = value;
                RaisePropertyChanged("CurrentFrame");
            }
        }

        public double MaxFrame
        {
            get => _maxFrame;
            set
            {
                _maxFrame = value;
                RaisePropertyChanged("MaxFrame");
            }
        }

        public const string LABEL_TEXT_COLOR = "#FFababad";
        public const float BOX_SCALAR = 100;
        public const float CANVAS_PADDING = 15;

        const int POSITION_X_FLAG = 32768,
                  POSITION_Y_FLAG = 65536;

        const bool DEBUG = true;

        private Color _hurtboxFillColor, _hurtboxStrokeColor,
                      _hitboxFillColor, _hitboxStrokeColor,
                      _physboxFillColor, _physboxStrokeColor,
                      _proxboxFillColor, _proxboxStrokeColor;

        bool _isScriptLoaded;

        public bool IsScriptLoaded
        {
            get { return _isScriptLoaded; }
            set
            {
                _isScriptLoaded = value;
                RaisePropertyChanged("IsScriptLoaded");
            }
        }

        public ObservableCollection<Rectangle> Rectangles
        {
            get => _rectangles;
            set { _rectangles = value; RaisePropertyChanged("Rectangles"); }
        }
        public ObservableCollection<Box> Boxes
        {
            get => _boxes;
            set { _boxes = value; RaisePropertyChanged("Boxes"); }
        }

        string _script;

        #region View Properties

        double _canvasHorizontalPosition;

        public double CanvasHorizontalPosition
        {
            get { return _canvasHorizontalPosition; }
            set { _canvasHorizontalPosition = value; RaisePropertyChanged("CanvasHorizontalPosition"); }
        }

        #region ICommands

        public ICommand ShowScriptCommand => new RelayCommand(ShowScript);
        public ICommand  RemoveBviCommand => new RelayCommand(RemoveBACVERint);
        public ICommand       OpenCommand => new RelayCommand(OpenFile);
        public ICommand       ExitCommand => new RelayCommand(Exit);

        public Action CloseAction { get; set; } // Action for calling Close() on a Window

        #endregion // ICommands

        #endregion // View Properties

        public MainWindowViewModel()
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
            #endregion

            #region Debug Actions
            if (DEBUG)
                LoadDefaultScript();
            #endregion

            // instantiate the Model
            model = new MainWindowModel();
        }

        #region Box Updates

        private void UpdateVisualizer()
        {
            #region Reset Display
            //SetCanvasPosition(new Point(), false);

            _boxes = new ObservableCollection<Box>();
            _positions = new List<Position>();
            _currentFrame = 0;
            _maxFrame = 0;
            #endregion

            #region Get Script
            
            #endregion

            #region Parse JSON
            // Try to parse JSON out of the text
            // Try to parse as a JObject
            if (_script.TrimStart().StartsWith("{"))
            {
                CleanScript();


                // Convert the JSON String to a C# object.
                var jobj = JsonConvert.DeserializeObject<dynamic>(_script,
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
                if (_boxes.Count == 0)
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
                }
                #endregion

                DrawBoxes();
            }
            else
            {
                // Script didn't start with "{"
            }
            #endregion
        }

        private void DrawBoxes()
        {
            // Clear the canvas
            Rectangles = new ObservableCollection<Rectangle>();

            // Update the list of boxes that are active in the current frame.
            UpdateCurrFrameBoxes();
            UpdateCurrFramePositionModifiers();

            // For each Box, draw a Rectangle
            foreach (Box box in CurrFrameBoxes)
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

                // TODO: If enabledBoxTypes contains box.BoxType, draw box. For enabling/disabling visibility of Boxes.
                var rectangle = new Rectangle()
                {
                    Width = box.Width * BOX_SCALAR,
                    Height = box.Height * BOX_SCALAR,
                    Fill = fill,
                    Stroke = stroke
                };

                Rectangles.Add(rectangle);
            }

            UpdateRectangleLocations();
        }

        public void UpdateRectangleLocations()
        {
            if (Rectangles != null)
            {
                for (int i = 0; i < Rectangles.Count; i++)
                {
                    Rectangle rectangle = Rectangles[i];
                    Box box = CurrFrameBoxes[i];

                    // Modify position of box, based on Positions
                    Point boxScaledLocation = new Point(box.X * BOX_SCALAR, 
                                                        box.Y * BOX_SCALAR + box.Height);
                    Point boxOrigin = new Point(boxScaledLocation.X,
                                                CANVAS_PADDING + boxScaledLocation.Y);

                    Point location = boxOrigin;

                    foreach (var pos in _currFramePositions)
                    {
                        if (pos.Flag == POSITION_X_FLAG)
                            location.X += pos.Movement * BOX_SCALAR;
                        else if (pos.Flag == POSITION_Y_FLAG)
                            location.Y += pos.Movement * BOX_SCALAR;
                    }

                    Canvas.SetLeft(rectangle, location.X);
                    Canvas.SetBottom(rectangle, location.Y);
                }
            }
        }

        #region Generate Script Data Fields
        
        // TODO: Replace with a Template
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

        #endregion

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
        #endregion // Box Updates

        #region Event Handling

        #region MenuItem Click Handlers
        
        #region Open File
        public void OpenFile()
        {
            // Create Open File dialogue
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                FileName = "BAC_*",
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                if (file != null) file.Close(); // close the old file

                // read the file in
                string fileName = openFileDialog.FileName;

                file = new StreamReader(fileName);
                
                _script = file.ReadToEnd();
                IsScriptLoaded = true;

                file.Close();
                // Reset Display
                // TODO?
            }
        }
        #endregion

        #region Exit
        public void Exit()
        {
            // Save work, etc. before closing 

            // Close window
            CloseAction();
        }
        #endregion
        
        #region Remove BACVERint
        public void RemoveBACVERint()
        {
            // TODO: Create yes/no dialogue


            // if yes, replace // TODO: Replace with Script property.
            Regex regex = new Regex(@"""BACVERint\d"": (?=0)\d,\s+");

            // replace AND count the number of replacements
            int count = 0;
            _script = regex.Replace(_script,
                m => {                   // function is called on each match
                    count++;
                    return m.Result(""); // replace the match with "" (delete the text)
                });

            MessageBox.Show(String.Format("Removed {0} lines!", count), "RemoveBACVERint Results");
        }
        #endregion

        #endregion // MenuItem Click Handlers

        #region Frame Change
        public void GoToPreviousFrame()
        {
            if (CurrentFrame > 0) CurrentFrame--;
        }

        public void GoToNextFrame()
        {
            if (CurrentFrame < _maxFrame) CurrentFrame++;
        }
        #endregion

        #endregion // Event Handling

        #region Script Operations

        public void CleanScript()
        {
            #region Try to Clean JSON
            int countCurly = 0;
            int countSquare = 0;

            Regex regex = new Regex(@",(\s*)}");
            _script = regex.Replace(_script, m =>
            {
                countCurly++;
                return m.Result(@"\1\}");
            });

            regex = new Regex(@",(\s*)]");
            _script = regex.Replace(_script, m =>
            {
                countCurly++;
                return m.Result(@"\1\]");
            });

            string msg = String.Format("Cleaned {0} instances of \"}}\" errors and {1} instances of \"]\" errors.", countCurly, countSquare);
            MessageBox.Show(msg);
            #endregion
        }

        #endregion

        #region Control Operations

        private void ShowScript()
        {
            MessageBox.Show("Show Script button clicked!");
            //dpScriptBoxGroup.Visibility = menuitemShowScriptBox.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            //gridContent.RowDefinitions[1].Height = new GridLength(menuitemShowScriptBox.IsChecked ? 1 : 0, GridUnitType.Star);
        }

        #endregion

        #region Debug Methods
        /// <summary>
        /// Sets the Script Box text to a default value/script.
        /// </summary>
        void LoadDefaultScript()
        {
            // TODO: use scriptviz_sampledata.json
        }
        #endregion
    }

    /// <summary>
    /// A logical representation of a script-based Box
    /// </summary>
    public struct Box
    {
        public int TickStart, TickEnd;
        public float X, Y;
        public float Width, Height;
        public string BoxType;
        public float HitType, HitboxEffectIndex;
    }

    /// <summary>
    /// A logical representation of a System.Windows.Shapes.Rectangle
    /// </summary>
    public struct Rect
    {
        public float X, Y;
        public float Width, Height;
    }
}
