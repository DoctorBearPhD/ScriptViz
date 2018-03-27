using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit.Document;
using Newtonsoft.Json;
using ScriptLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.CommandWpf;

namespace ScriptViz.ViewModel
{
    public class MainWindowViewModel : VMBase
    {
        #region Variables

        #region Constants

        public const string LABEL_TEXT_COLOR = "#FFababad";

        private const bool DEBUG = true;

        private const double ORIGINAL_SCRIPT_BOX_COLUMN_SIZE = 2;
        private const string DEFAULT_SCRIPT_BOX_TEXT = "Load a script and it will be displayed here...";

        #endregion

        #region Lists

        private readonly ObservableCollection<VMBase> _viewModels = new ObservableCollection<VMBase>
        {
            new ScriptVisualizerViewModel()
        };

        private readonly ObservableCollection<TabItemViewModel> _moveListTabs = new ObservableCollection<TabItemViewModel>();
        public ObservableCollection<TabItemViewModel> MoveListTabs => 
            _moveListTabs ?? new ObservableCollection<TabItemViewModel>();

        #endregion

        #region Selected

        // MOVELIST
        private int _selectedTabIndex; // The selected Tab in the Script Info area (index)
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                RaisePropertyChanged(nameof(SelectedTabIndex));

                // | MoveList 1 |    | MoveList 2 |    | HitboxEffectses |
                if ( _selectedTabIndex !=  MoveListTabs.Count - 1 ) // if not the last tab (the HitboxEffectses tab)
                {
                    SelectedMoveListIndex = _selectedTabIndex;
                }
            }
        }

        private int _selectedMoveListIndex;
        public int SelectedMoveListIndex
        {
            get => _selectedMoveListIndex;
            set
            {
                _selectedMoveListIndex = value;
                RaisePropertyChanged(nameof(SelectedMoveListIndex));
                SelectedMoveListChanged();
            }
        }

        #endregion // Selected

        #region Script

        // Script text file as a data object
        //BaseFile _scriptFileObject;

        private BACFile _bacFile;
        public BACFile BacFile
        {
            get => _bacFile;
            set { _bacFile = value; RaisePropertyChanged(nameof(BacFile)); }
        }

        public string FileName { get; set; }

        // shortcut for accessing TextDocument's text
        private string ScriptText { get => ScriptTextFile.Text; set => ScriptTextFile.Text = value; }

        private TextDocument _scriptTextFile = new TextDocument();
        public TextDocument ScriptTextFile
        {
            get => _scriptTextFile;
            set
            {
                _scriptTextFile = value; RaisePropertyChanged(nameof(ScriptTextFile));
            }
        }

        private bool _isScriptLoaded;
        public bool IsScriptLoaded
        {
            get => _isScriptLoaded;
            set
            {
                _isScriptLoaded = value;
                RaisePropertyChanged(nameof(IsScriptLoaded));
            }
        }

        private GridLength _scriptBoxColumnSize = new GridLength(0);
        public GridLength ScriptBoxColumnSize {
            get => _scriptBoxColumnSize;
            set  { _scriptBoxColumnSize = value; RaisePropertyChanged(nameof(ScriptBoxColumnSize)); }
        }

        #endregion // Script

        #region Preferences

        private bool _isScriptBoxVisible;
        public bool IsScriptBoxVisible
        {
            get => _isScriptBoxVisible;
            set
            {
                _isScriptBoxVisible = value;
                RaisePropertyChanged(nameof(IsScriptBoxVisible));
                Messenger.Default.Send(IsScriptBoxVisible, nameof(IsScriptBoxVisible));
            }
        }

        private bool _isUnknownsVisible = true;
        public bool IsUnknownsVisible
        {
            get => _isUnknownsVisible;
            set
            {
                _isUnknownsVisible = value;
                RaisePropertyChanged(nameof(IsUnknownsVisible));
                Messenger.Default.Send(IsUnknownsVisible, nameof(IsUnknownsVisible));
            }
        }


        private bool _saveWithBACVERint = true;
        public bool SaveWithBACVERint
        {
            get => _saveWithBACVERint;
            set { _saveWithBACVERint = value; RaisePropertyChanged(nameof(SaveWithBACVERint)); }
        }



        #endregion // Preferences

        #region ICommands

        #region Menu Commands
        public ICommand CleanScriptCommand => new RelayCommand(CleanScript);
        public ICommand   RemoveBviCommand => new RelayCommand(RemoveBACVERint);
        public ICommand     OpenBacCommand => new RelayCommand(OpenBACFile);
        public ICommand        SaveCommand => new RelayCommand(SaveFile);
        public ICommand        ExitCommand => new RelayCommand(Exit);
        #endregion

        public Action CloseAction { get; set; } // Action for calling Close() on a Window

        #endregion // ICommands

        #region Misc

        private string _numberOfTypes;
        public string NumberOfTypes
        {
            get => _numberOfTypes ?? "N/A";
            set { _numberOfTypes = value; RaisePropertyChanged(nameof(NumberOfTypes)); }
        }


        #endregion // Misc

        #endregion // Variables


        public MainWindowViewModel()
        {
            #region Debug Actions
            if (DEBUG)
                LoadDefaultScript();
            #endregion
            
            // Set initial text for script box
            ScriptTextFile.Text = DEFAULT_SCRIPT_BOX_TEXT;

            Messenger.Default.Register<Move>(this, move => NumberOfTypes = move?.numberOfTypes.ToString());
            Messenger.Default.Register<bool>(this, nameof(IsScriptBoxVisible), ShowScript);
        }

        #region Load

        private void LoadBacFile()
        {
            // Try to clean the JSON before doing anything with it.
            CleanScript();

            (_viewModels[0] as ScriptVisualizerViewModel)?.ResetDisplay();

            #region Show Loading
            // TODO
            #endregion

            #region Parse JSON
            // Try to parse JSON out of the text
            // Try to parse as a BACFile

            // Convert the JSON String to a C# object.
            _bacFile = JsonConvert.DeserializeObject<BACFile>(ScriptTextFile.Text);

            //var bacFile = (BACFile)_scriptFileObject;

            #endregion
            
            // Make list items for each Move
            CreateTabs();

            SelectedMoveListIndex = 0; // Makes sure NotifyPropertyChanged event gets raised.
        }

        /// <summary>
        /// Creates a tab for each MoveList in the BACFile and creates a HitboxEffectses tab. 
        /// TODO: Should rename it to something that has less to do with the UI.
        /// </summary>
        private void CreateTabs()
        {
            if (_bacFile == null) return;

            MoveListTabs.Clear();

            // Make tabs for each MoveList
            for (int i = 0; i < _bacFile.MoveLists.Length; i++)
                MoveListTabs.Add(new MoveListViewModel { Header = "MoveList " + (i + 1), Content = BacFile.MoveLists[i] } );

            // Make a tab for the list of HitboxEffects objects
            MoveListTabs.Add(new HitboxEffectsesViewModel { Header = "HitboxEffectses", Content = BacFile.HitboxEffectses, IsUnknownsVisible = this.IsUnknownsVisible } );

        }


        #endregion // Load

        #region Event Handling

        #region MenuItem Click Handlers
        
        // TODO - Use OpenBacFile for BAC files, OpenBcmFile for BCM files, etc.
        #region Open File
        public void OpenBACFile()
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
                //null the previous script file object
                //_scriptFileObject = null;

                // read the file in
                FileName = openFileDialog.FileName;

                ScriptTextFile = new TextDocument(ICSharpCode.AvalonEdit.Utils.FileReader.OpenFile(FileName, System.Text.Encoding.UTF8).ReadToEnd());
                IsScriptLoaded = true;
                
                LoadBacFile();
            }
        }
        #endregion

        public void SaveFile()
        {
            //ScriptText = JsonConvert.SerializeObject(BacFile, Formatting.Indented);
            Console.WriteLine("Saving...");
            try
            {
                // Save changes
                // reserialize movelists
                ScriptText = JsonConvert.SerializeObject(BacFile, Formatting.Indented);

                if (!SaveWithBACVERint)
                    RemoveBACVERint(false);

                File.WriteAllText(FileName, ScriptText);
            }
            catch (JsonSerializationException jse)
            {
                MessageBox.Show("Json could not be serialized! Exception: \n" + jse);
                return;
            }
            catch (IOException)
            {
                MessageBox.Show("File could not be saved! (Is file in use?)");
                return;
            }
            Console.WriteLine("Save complete.");
        }

        #region Exit
        public void Exit()
        {
            // Save work, etc. before closing 

            // Close window
            CloseAction();
        }
        #endregion

        #region Script Operations

        #region Show Script

        private void ShowScript(bool visibility)
        {
            if (IsScriptBoxVisible)
            {
                ScriptBoxColumnSize = new GridLength(ORIGINAL_SCRIPT_BOX_COLUMN_SIZE, GridUnitType.Star);
            }
            else
                ScriptBoxColumnSize = new GridLength(0);
        }

        #endregion // Show Script

        #region Try to Clean JSON
        public void CleanScript()
        {
            int countCurly = 0;
            int countSquare = 0;

            Regex regex = new Regex(@",(\s*)}");
            ScriptText = regex.Replace(ScriptText, m =>
            {
                countCurly++;
                return m.Result(@"$1}");
            });

            regex = new Regex(@",(\s*)]");
            ScriptText = regex.Replace(ScriptText, m =>
            {
                countCurly++;
                return m.Result(@"$1]");
            });

            //File.CreateText("temp.json").Write(Script);

            if (countCurly > 0 || countSquare > 0)
            {
                string msg = String.Format("Cleaned {0} instances of \"}}\" errors and {1} instances of \"]\" errors.", 
                    countCurly, countSquare);

                MessageBox.Show(msg);
            }
        }
        #endregion

        #region Remove BACVERint
        public void RemoveBACVERint()
        {
            RemoveBACVERint(true);
        }

        public void RemoveBACVERint(bool showMessage)
        {
            // TODO: Create yes/no dialogue


            // if yes, replace // TODO: Replace with Script property.
            Regex regex = new Regex(@"""BACVERint\d"": (?=0)\d,*\s+");

            if (!showMessage)
                // replace without counting
                ScriptText = regex.Replace(ScriptText, "");
            else
            {
                // replace AND count the number of replacements
                int count = 0;
                ScriptText = regex.Replace(ScriptText,
                    m => {               // function is called on each match
                    count++;
                    return m.Result(""); // replace the match with "" (delete the text)
                });

                MessageBox.Show($"Removed {count} lines!", "RemoveBACVERint Results");
            }
        }
        #endregion // Remove BACVERint

        #endregion // Script Operations

        #endregion // MenuItem Click Handlers

        private void SelectedMoveListChanged()
        {
            if (SelectedMoveListIndex == -1) return;

            // TODO: Check if previous movelist changes - if any - were saved.
            Messenger.Default.Send<MoveList>( this.BacFile?.MoveLists[SelectedMoveListIndex] );
            //LoadMoveList();
        }

        #endregion // Event Handling

        #region Debug Methods
        /// <summary>
        /// Sets the Script Box text to a default value/script.
        /// </summary>
        private void LoadDefaultScript()
        {
            // TODO: use scriptviz_sampledata.json
        }
        #endregion
    }

    /// <summary>
    /// A logical representation of a System.Windows.Shapes.Rectangle
    /// </summary>
    public class Rect : INotifyPropertyChanged
    {
        public double X      { get; set; }
        public double Y      { get; set; }
        public double Width  { get; set; }
        public double Height { get; set; }
        public Brush  Fill   { get; set; }
        public Brush  Stroke { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
