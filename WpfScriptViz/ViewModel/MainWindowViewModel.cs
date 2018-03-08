using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit.Document;
using Newtonsoft.Json;
using ScriptLib;
using ScriptViz.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScriptViz.ViewModel
{
    public class MainWindowViewModel : VMBase
    {
        #region Variables

        #region Constants

        public const string LABEL_TEXT_COLOR = "#FFababad";

        const bool DEBUG = true;

        const double ORIGINAL_SCRIPT_BOX_COLUMN_SIZE = 3;
        const string DEFAULT_SCRIPT_BOX_TEXT = "Load a script, or paste a script here...";

        #endregion

        #region Lists

        ObservableCollection<VMBase> _viewModels = new ObservableCollection<VMBase>
        {
            new ScriptVisualizerViewModel()
        };

        ObservableCollection<object> _moveListTabs;
        public ObservableCollection<object> MoveListTabs
        {
            get => _moveListTabs;
            set { _moveListTabs = value; RaisePropertyChanged(nameof(MoveListTabs)); }
        }

        #endregion

        #region Selected

        // MOVE
        Move _selectedMove;
        public Move SelectedMove
        {
            get => _selectedMove;
            set
            {
                _selectedMove = value;
                RaisePropertyChanged(nameof(SelectedMove));
            }
        }

        int _selectedMoveIndex;
        public int SelectedMoveIndex
        {
            get => _selectedMoveIndex;
            set
            {
                _selectedMoveIndex = value;
                RaisePropertyChanged(nameof(SelectedMoveIndex));
                RaisePropertyChanged(nameof(SelectedMove));
                SelectedMoveChanged();
            }
        }

        // MOVELIST
        private MoveList _selectedMoveList;
        public MoveList SelectedMoveList
        {
            get => _selectedMoveList;
            set
            {
                _selectedMoveList = value;
                RaisePropertyChanged(nameof(SelectedMoveList));
            }
        }

        private MoveList BackupOfSelectedMoveList { get; set; }

        int _selectedMoveListIndex; // The selected MoveList tab in the Script Info area (index)
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

        // PROPERTY
        public PropertyInfo SelectedProperty { get => SelectedMove.GetAllProperties()[SelectedPropertyIndex]; }

        int _selectedPropertyIndex;
        public int SelectedPropertyIndex
        {
            get => _selectedPropertyIndex;
            set
            {
                _selectedPropertyIndex = (value <= 0) ? 0 : (value + SelectedMove.GetGeneralPropertiesOffset());
                RaisePropertyChanged(nameof(SelectedPropertyIndex));
                RaisePropertyChanged(nameof(SelectedProperty));
            }
        }

        // TYPE'S PROPERTY
        public object SelectedTypeProperty
        {
            get => SelectedProperty.GetValue(SelectedMove);
        }

        int _selectedTypePropertyIndex;
        public int SelectedTypePropertyIndex
        {
            get { return _selectedTypePropertyIndex; }
            set { _selectedTypePropertyIndex = value; RaisePropertyChanged(nameof(SelectedTypePropertyIndex)); }
        }

        #endregion // Selected

        #region Script

        // Script text file as a data object
        //BaseFile _scriptFileObject;

        BACFile bacFile;
        public BACFile BacFile
        {
            get { return bacFile; }
            set { bacFile = value; RaisePropertyChanged(nameof(BacFile)); }
        }

        public string FileName { get; set; }

        // shortcut for accessing TextDocument's text
        string ScriptText { get => ScriptTextFile.Text; set => ScriptTextFile.Text = value; }

        TextDocument _scriptTextFile = new TextDocument();
        public TextDocument ScriptTextFile
        {
            get => _scriptTextFile;
            set
            {
                _scriptTextFile = value; RaisePropertyChanged(nameof(ScriptTextFile));
            }
        }

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
        
        bool _isScriptBoxVisible = true;
        public bool IsScriptBoxVisible
        {
            get => _isScriptBoxVisible;
            set  { _isScriptBoxVisible = value; RaisePropertyChanged("IsScriptBoxVisible"); }
        }

        GridLength _scriptBoxColumnSize = new GridLength(ORIGINAL_SCRIPT_BOX_COLUMN_SIZE, GridUnitType.Star);
        public GridLength ScriptBoxColumnSize {
            get => _scriptBoxColumnSize;
            set  { _scriptBoxColumnSize = value; RaisePropertyChanged("ScriptBoxColumnSize"); }
        }

        #endregion // Script
        
        #region ICommands

        #region Menu Commands
        public ICommand CleanScriptCommand => new RelayCommand(CleanScript);
        public ICommand  ShowScriptCommand => new RelayCommand(ShowScript);
        public ICommand   RemoveBviCommand => new RelayCommand(RemoveBACVERint);
        public ICommand     OpenBacCommand => new RelayCommand(OpenBACFile);
        public ICommand        SaveCommand => new RelayCommand(SaveFile);
        public ICommand        ExitCommand => new RelayCommand(Exit);
        #endregion

        public ICommand CommitChangesCommand => new RelayCommand(CommitChanges);

        public Action CloseAction { get; set; } // Action for calling Close() on a Window

        #endregion // ICommands

        #endregion // Variables


        public MainWindowViewModel()
        {
            #region Debug Actions
            if (DEBUG)
                LoadDefaultScript();
            #endregion
            
            // Set initial text for script box
            ScriptTextFile.Text = DEFAULT_SCRIPT_BOX_TEXT;
        }

        // TODO: Don't use this to display?
        #region Load
        void LoadBacFile()
        {
            // Try to clean the JSON before doing anything with it.
            CleanScript();

            (_viewModels[0] as ScriptVisualizerViewModel).ResetDisplay();

            #region Show Loading
            // TODO
            #endregion

            #region Parse JSON
            // Try to parse JSON out of the text
            // Try to parse as a BACFile

            // Convert the JSON String to a C# object.
            bacFile = JsonConvert.DeserializeObject<BACFile>(ScriptTextFile.Text,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            //var bacFile = (BACFile)_scriptFileObject;

            #endregion
            
            // Make list items for each Move
            EnumerateMoveLists();

            SelectedMoveListIndex = 0; // Makes sure NotifyPropertyChanged event gets raised.
        }

        /// <summary>
        /// Creates a tab for each MoveList in the BACFile. In actuality, creates a list of Strings which causes tabs to be created.
        /// </summary>
        void EnumerateMoveLists()
        {
            if (bacFile == null) return;

            // Make tabs for each MoveList
            MoveListTabs = new ObservableCollection<object>();
            for (int i = 0; i < bacFile.MoveLists.Length; i++)
                MoveListTabs.Add("MoveList " + (i + 1));
        }

        void LoadMoveList()
        {
            if (bacFile == null) return;
            if (SelectedMoveListIndex < 0) SelectedMoveListIndex = 0;
            
            SelectedMoveIndex = 0; // This raises the NotifyPropertyChanged event for SelectedMoveIndex and SelectedMove.
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
                File.WriteAllText(FileName, ScriptText);
            }
            catch
            {
                MessageBox.Show("File could not be saved!");
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
        //TODO: Implement!
        private void ShowScript()
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
            // TODO: Create yes/no dialogue


            // if yes, replace // TODO: Replace with Script property.
            Regex regex = new Regex(@"""BACVERint\d"": (?=0)\d,*\s+");

            // replace AND count the number of replacements
            int count = 0;
            ScriptText = regex.Replace(ScriptText,
                m => {                   // function is called on each match
                    count++;
                    return m.Result(""); // replace the match with "" (delete the text)
                });

            MessageBox.Show(String.Format("Removed {0} lines!", count), "RemoveBACVERint Results");
        }
        #endregion // Remove BACVERint

        #endregion // Script Operations

        #endregion // MenuItem Click Handlers


        // TODO NEXT - Continue checking logic from here (LoadBacFile() -> SelectedMoveListIndex == 0;)
        void SelectedMoveListChanged()
        {
            if (SelectedMoveListIndex == -1) return;

            // TODO: Check if previous movelist changes - if any - were saved.
            SelectedMoveList = (BacFile != null) ? this.BacFile.MoveLists[SelectedMoveListIndex] : null;
            LoadMoveList();

            // Make backup of MoveList
            CreateMoveListBackup();
        }

        private void CreateMoveListBackup()
        {
            // clone the MoveList
            BackupOfSelectedMoveList = JsonConvert.DeserializeObject<MoveList>(JsonConvert.SerializeObject(SelectedMoveList));
        }

        void SelectedMoveChanged()
        {
            if (SelectedMoveList == null) SelectedMove = null;

            int numberOfMoves = SelectedMoveList.Moves.Length;
            if (numberOfMoves > 0 && SelectedMoveIndex.IsBetween(0, numberOfMoves - 1))
                SelectedMove = SelectedMoveList.Moves[SelectedMoveIndex];
            else
                SelectedMove = null;

            //MessageBox.Show("Move changed!");

            // Tell registered listeners that SelectedMove has changed.
            Messenger.Default.Send(SelectedMove);
        }

        #endregion // Event Handling

        #region Control Operations

        private void CommitChanges()
        {
            var selectedMoveOriginal = BackupOfSelectedMoveList.Moves[SelectedMoveIndex];

            if (SelectedMove != selectedMoveOriginal)
            {
                var message = String.Format("SelectedMove.Name backup: {0}, SelectedMove.Name change: {1}", 
                    selectedMoveOriginal.Name, SelectedMove.Name);
                Console.WriteLine(message);

                // Save changes
                // reserialize movelists
                ScriptText = JsonConvert.SerializeObject(BacFile, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                    );

                // reload?
                CreateMoveListBackup();
            }
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
    //public struct Box
    //{
    //    public int TickStart, TickEnd;
    //    public float X, Y;
    //    public float Width, Height;
    //    public string BoxType;
    //    public float HitType, HitboxEffectIndex;
    //}

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
