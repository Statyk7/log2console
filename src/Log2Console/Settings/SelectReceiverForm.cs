using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Log2Console.Receiver;


namespace Log2Console.Settings
{
    public partial class SelectReceiverForm : Form
    {
        public string SelectedReceiverTypeName { get; protected set; }


        public SelectReceiverForm()
        {
            InitializeComponent();

            // Populate Receivers
            Dictionary<string, Type> receiverTypes = ReceiverFactory.Instance.ReceiverTypes;
            foreach (KeyValuePair<string, Type> kvp in receiverTypes)
                receiverTypeComboBox.Items.Add(kvp.Key);
            receiverTypeComboBox.SelectedIndex = 0;
        }

        private void receiverTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedReceiverTypeName = receiverTypeComboBox.Text;
        }
    }
}
