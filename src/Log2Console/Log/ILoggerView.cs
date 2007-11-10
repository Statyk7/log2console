using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Log2Console.Log
{
    /// <summary>
    /// Defines methods for allowing a LoggerItem to be shown in the UI.
    /// </summary>
    public interface ILoggerView
    {
        /// <summary>
        /// Clears this view and all child views.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the new logger view as a child of the current view and returns the new view.
        /// </summary>
        /// <param name="text">The text to initialize the view with.</param>
        /// <param name="logger">The logger that this instance is a view of.</param>
        /// <returns></returns>
        ILoggerView AddNew(string text, LoggerItem logger);

        /// <summary>
        /// Gets or sets the text of the view. The text is what is shown to the user.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ILoggerView"/> is shown
        /// as highlighted.
        /// </summary>
        /// <value><c>true</c> if highlighted; otherwise, <c>false</c>.</value>
        bool Highlight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ILoggerView"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }
    }
}
