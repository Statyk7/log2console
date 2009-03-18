using System.Windows.Forms;
using System.Drawing;

using Log2Console.Log;


namespace Log2Console.UI
{
    internal class TreeViewLoggerView : ILoggerView
    {
        public TreeViewLoggerView(TreeView treeView)
        {
            this._treeView = treeView;
            this._isRoot = true;
        }

        private TreeViewLoggerView(TreeView treeView, TreeNode node)
        {
            this._treeView = treeView;
            this._node = node;
            this._isRoot = false;
        }


        #region ILoggerView Members

        /// <summary>
        /// Clears this view and all child views.
        /// </summary>
        public void Clear()
        {
            if (this._isRoot)
            {
                try
                {
                    this._treeView.BeginUpdate();
                    this._treeView.Nodes.Clear();
                }
                finally
                {
                    this._treeView.EndUpdate();
                }
            }
            else
            {
                try
                {
                    this._node.TreeView.BeginUpdate();
                    this._node.Nodes.Clear();
                }
                finally
                {
                    this._node.TreeView.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Adds the new logger view as a child of the current view and returns the new view.
        /// </summary>
        /// <param name="text">The text to initialize the view with.</param>
        /// <param name="logger">The logger that this instance is a view of.</param>
        /// <returns></returns>
        public ILoggerView AddNew(string text, LoggerItem logger)
        {
            // Creating a new node.
            TreeNode node = new TreeNode(text);
            node.Checked = true;
            node.Tag = logger;

            if (_isRoot)
            {
                this._treeView.Nodes.Add(node);
            }
            else
            {
                this._node.Nodes.Add(node);
            }

            node.EnsureVisible();

            return new TreeViewLoggerView(this._treeView, node);
        }

        /// <summary>
        /// Gets or sets the text of the view. The text is what is shown to the user.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (this._isRoot ? "(root)" : this._node.Text);
            }
            set
            {
                if (!this._isRoot)
                {
                    this._node.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ILoggerView"/> is shown
        /// as highlighted.
        /// </summary>
        /// <value><c>true</c> if highlighted; otherwise, <c>false</c>.</value>
        public bool Highlight
        {
            get
            {
                return (this._isRoot ? false : this._node.BackColor == Color.LightBlue);
            }
            set
            {
                if (!this._isRoot)
                {
                    if (value)
                        this._node.BackColor = Color.LightBlue;
                    else
                        this._node.BackColor = Color.Transparent;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ILoggerView"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get
            {
                return (this._isRoot ? true : this._node.Checked);
            }
            set
            {
                if (!this._isRoot)
                {
                    this._node.Checked = value;
                }
            }
        }

        #endregion


        #region Private Members

        private TreeView _treeView;
        private TreeNode _node;
        private bool _isRoot = false;

        #endregion 


    }
}
