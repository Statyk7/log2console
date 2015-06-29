using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace Log2Console.ICSharpCodeExtension.Actions
{
    public class GotoLineNumber : AbstractEditAction
    {
        private readonly int _line;

        public GotoLineNumber(int line)
        {
            _line = line;
        }

        public override void Execute(TextArea textArea)
        {
            TextLocation position = textArea.Caret.Position;
            position.Line = _line;

            textArea.Caret.Position = position;
            textArea.SelectionManager.ClearSelection();
            textArea.SetDesiredColumn();
            
        }
    }
}
