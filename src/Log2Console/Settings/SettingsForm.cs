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
        public SettingsForm(UserSettings userSettings)
        {
            InitializeComponent();

			// UI Settings
            UserSettings = userSettings;

			// Receiver
			Dictionary<string, Type> receiverTypes = ReceiverFactory.Instance.ReceiverTypes;
			foreach (KeyValuePair<string, Type>  kvp in receiverTypes)
				receiverTypeComboBox.Items.Add(kvp.Key);

			if (userSettings.Receiver != null)
			{
				Type receiverType = userSettings.Receiver.GetType();
				receiverTypeComboBox.SelectedIndex = 
					receiverTypeComboBox.FindString(receiverType.FullName);
			}

            // Set the event handler only after having set the receiver
            this.receiverTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.receiverTypeComboBox_SelectedIndexChanged);
        }


        public UserSettings UserSettings
        {
            get { return settingsPropertyGrid.SelectedObject as UserSettings; }
            set
            {
                settingsPropertyGrid.SelectedObject = value;
                if (value.Receiver != null)
                    SetReceiver(value.Receiver);
            }
        }

        private void SetReceiver(IReceiver receiver)
        {
            if (receiver != null)
                sampleClientConfigTextBox.Text = receiver.SampleClientConfig;
            receiverPropertyGrid.SelectedObject = receiver;

            if (UserSettings != null)
                UserSettings.Receiver = receiver;
        }

		private void receiverTypeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            if ((UserSettings != null) && (UserSettings.Receiver != null) &&
                UserSettings.Receiver.GetType().FullName.Equals(receiverTypeComboBox.SelectedText))
			{
				return;
			}

			// Instantiates a new receiver based on the selected type
			IReceiver receiver =
				ReceiverFactory.Instance.Create(receiverTypeComboBox.Text);

            SetReceiver(receiver);
		}
    }
}