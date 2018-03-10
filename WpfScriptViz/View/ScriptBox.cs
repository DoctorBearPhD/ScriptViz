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

        new public static DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(TextDocument), typeof(ScriptBox),
                new PropertyMetadata((obj, args) =>
                {
                    ScriptBox target = (ScriptBox)obj;
                    target.Document = (TextDocument)args.NewValue;
                })
        );

        new public string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        new public TextDocument Document
        {
            get => base.Document;
            set => base.Document = value;
        }

        public int Length { get { return base.Text.Length; } }

        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged("Length");
            base.OnTextChanged(e);
        }
    }
}
