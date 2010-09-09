//Copyright (c) Microsoft Corporation.  All rights reserved.

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation
{
    /// <summary>
    /// Implements a CommandLink button that can be used in WPF user interfaces.
    /// </summary>
    public partial class CommandLink : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public CommandLink()
        {
            // Throw PlatformNotSupportedException if the user is not running Vista or beyond
            CoreHelpers.ThrowIfNotVista();

            this.DataContext = this;
            InitializeComponent();
            this.button.Click += new RoutedEventHandler(button_Click);
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            e.Source = this;
            if (Click != null)
                Click(sender, e);
        }

        RoutedUICommand command;

        /// <summary>
        /// Routed UI command to use for this button
        /// </summary>
        public RoutedUICommand Command
        {
            get { return command; }
            set { command = value; }
        }

        /// <summary>
        /// Occurs when the control is clicked.
        /// </summary>
        public event RoutedEventHandler Click;

        private string link;

        /// <summary>
        /// Specifies the main instruction text
        /// </summary>
        public string Link
        {
            get { return link; }
            set
            {
                link = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Link"));
                }
            }
        }
        private string note;

        /// <summary>
        /// Specifies the supporting note text
        /// </summary>
        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Note"));
                }
            }
        }
        private ImageSource icon;

        /// <summary>
        /// Icon to set for the command link button
        /// </summary>
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Icon"));
                }
            }
        }

        /// <summary>
        /// Indicates if the button is in a checked state
        /// </summary>
        public bool? IsCheck
        {
            get
            {
                return button.IsChecked;
            }
            set { button.IsChecked = value; }
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Indicates whether this feature is supported on the current platform.
        /// </summary>
        public static bool IsPlatformSupported
        {
            get
            {
                // We need Windows Vista onwards ...
                return CoreHelpers.RunningOnVista;
            }
        }
    }
}