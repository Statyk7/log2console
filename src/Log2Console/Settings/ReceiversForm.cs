using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Log2Console.Receiver;


namespace Log2Console.Settings
{
    public partial class ReceiversForm : Form
    {
        public List<IReceiver> AddedReceivers { get; protected set; }
        public List<IReceiver> RemovedReceivers { get; protected set; }


        public ReceiversForm(IEnumerable<IReceiver> receivers)
        {
            AddedReceivers = new List<IReceiver>();
            RemovedReceivers = new List<IReceiver>();

            InitializeComponent();

            // Populate Receiver Types
            Dictionary<string, ReceiverFactory.ReceiverInfo> receiverTypes = ReceiverFactory.Instance.ReceiverTypes;
            foreach (KeyValuePair<string, ReceiverFactory.ReceiverInfo> kvp in receiverTypes)
            {
                ToolStripItem item = addReceiverCombo.DropDownItems.Add(kvp.Value.Name);
                item.Tag = kvp.Value;
            }

            // Populate Existing Receivers
            foreach (IReceiver receiver in receivers)
                AddReceiver(receiver);
        }

        private void AddReceiver(IReceiver receiver)
        {
            Type receiverType = receiver.GetType();
            ListViewItem lvi = receiversListView.Items.Add(ReceiverUtils.GetTypeDescription(receiverType));
            lvi.Tag = receiver;
            lvi.Selected = true;
        }


        private void addReceiverCombo_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ReceiverFactory.ReceiverInfo info = e.ClickedItem.Tag as ReceiverFactory.ReceiverInfo;
            if (info != null)
            {
                // Instantiates a new receiver based on the selected type
                IReceiver receiver = ReceiverFactory.Instance.Create(info.Type.FullName);

                AddedReceivers.Add(receiver);
                AddReceiver(receiver);
            }
        }

        private void removeReceiverBtn_Click(object sender, EventArgs e)
        {
            IReceiver receiver = GetSelectedReceiver();
            if (receiver == null)
                return;

            DialogResult dr = MessageBox.Show(this, "Confirm Delete?", "Confirmation", MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr != DialogResult.Yes)
                return;

            receiversListView.Items.Remove(GetSelectedItem());

            if (AddedReceivers.Find(r => r == receiver) != null)
                AddedReceivers.Remove(receiver);
            else
                RemovedReceivers.Add(receiver);
        }

        private void receiversListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            IReceiver receiver = GetSelectedReceiver();

            removeReceiverBtn.Enabled = (receiver != null);
            receiverPropertyGrid.SelectedObject = receiver;

			if (receiver != null)
				sampleClientConfigTextBox.Text = receiver.SampleClientConfig;
        }

        private ListViewItem GetSelectedItem()
        {
            if (receiversListView.SelectedItems.Count > 0)
                return receiversListView.SelectedItems[0];
            return null;
        }

        private IReceiver GetSelectedReceiver()
        {
            if (receiversListView.SelectedItems.Count <= 0)
                return null;

            ListViewItem lvi = GetSelectedItem();
            return (lvi == null) ? null : lvi.Tag as IReceiver;
        }
    }
}
