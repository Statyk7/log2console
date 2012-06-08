using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace Log2Console.ICSharpCodeExtension.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;


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
