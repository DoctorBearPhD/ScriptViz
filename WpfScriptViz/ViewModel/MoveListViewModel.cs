using GalaSoft.MvvmLight.Messaging;
using ScriptLib;
using System.Reflection;

namespace ScriptViz.ViewModel
{
    public class MoveListViewModel : TabItemViewModel
    {
        #region Variables

        public MoveList SelectedMoveList
        {
            get => Content as MoveList;
        }

        public Move SelectedMove => SelectedMoveList.Moves[SelectedMoveIndex];

        int _selectedMoveIndex;
        public int SelectedMoveIndex
        {
            get => _selectedMoveIndex;
            set
            {
                if (value < 0) return;

                _selectedMoveIndex = value;
                RaisePropertyChanged(nameof(SelectedMoveIndex)); // Notifies connected UI elements that SelectedMoveIndex has changed
                RaisePropertyChanged(nameof(SelectedMove));

                //if (SelectedMoveList == null) SelectedMove = null;

                //int numberOfMoves = SelectedMoveList.Moves.Length;
                //if (numberOfMoves > 0 && SelectedMoveIndex.IsBetween(0, numberOfMoves - 1)) // "if index is valid"
                //    SelectedMove = SelectedMoveList.Moves[SelectedMoveIndex];
                //else
                //    SelectedMove = null;

                Messenger.Default.Send(SelectedMove);
            }
        }

        // PROPERTY
        public PropertyInfo SelectedProperty
        {
            get => (SelectedPropertyIndex <= 0) ? null : SelectedMove.GetAllProperties()[SelectedPropertyIndex];
        }

        int _selectedPropertyIndex;
        public int SelectedPropertyIndex
        {
            get => _selectedPropertyIndex;
            set
            {
                _selectedPropertyIndex = (value <= 0) ? value : (value + SelectedMove.GetGeneralPropertiesOffset());
                RaisePropertyChanged(nameof(SelectedPropertyIndex));
                RaisePropertyChanged(nameof(SelectedProperty));
            }
        }

        // TYPE'S PROPERTY
        public object SelectedTypeProperty => SelectedProperty?.GetValue(SelectedMove);

        int _selectedTypePropertyIndex;
        public int SelectedTypePropertyIndex
        {
            get => _selectedTypePropertyIndex;
            set { _selectedTypePropertyIndex = value; RaisePropertyChanged(nameof(SelectedTypePropertyIndex)); }
        }

        #endregion // Variables

        #region Constructor

        public MoveListViewModel()
        {
            
        }

        #endregion

        #region Load MoveList



        #endregion

    }
}
