using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.ComponentModel;
using System.Windows;

namespace ScriptViz.Util
{
    public class ScriptBox : TextEditor, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public new static DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(TextDocument), typeof(ScriptBox),
                new PropertyMetadata((obj, args) =>
                {
                    var target = (ScriptBox)obj;
                    target.Document = (TextDocument)args.NewValue;
                })
        );

        public new string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public new TextDocument Document
        {
            get => base.Document;
            set => base.Document = value;
        }

        public int Length => base.Text.Length;

        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged("Length");
            base.OnTextChanged(e);
        }
    }
}
