using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;

using Log2Console.Receiver;


namespace Log2Console.Settings
{
    public partial class SettingsForm : Form
    {
        public SettingsForm(UserSettings userSettings, IReceiver receiver)
        {
            InitializeComponent();

			// UI Settings
            UserSettings = userSettings;

			// Receiver
			Dictionary<string, Type> receiverTypes = ReceiverFactory.Instance.ReceiverTypes;
			foreach (KeyValuePair<string, Type>  kvp in receiverTypes)
				receiverTypeComboBox.Items.Add(kvp.Key);

			if (receiver != null)
			{
				Receiver = receiver;

				Type receiverType = receiver.GetType();
				receiverTypeComboBox.SelectedIndex = 
					receiverTypeComboBox.FindString(receiverType.FullName);
			}
        }


        public UserSettings UserSettings
        {
            get { return settingsPropertyGrid.SelectedObject as UserSettings; }
            set { settingsPropertyGrid.SelectedObject = value; }
        }

		public IReceiver Receiver
		{
			get { return receiverPropertyGrid.SelectedObject as IReceiver; }
			set { receiverPropertyGrid.SelectedObject = value; }
		}


		private void receiverTypeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ((Receiver != null) && 
				Receiver.GetType().FullName.Equals(receiverTypeComboBox.SelectedText))
			{
				return;
			}

			// Instantiates a new receiver based on the selected type
			IReceiver receiver =
				ReceiverFactory.Instance.Create(receiverTypeComboBox.Text);

			Receiver = receiver;
		}
    }
}