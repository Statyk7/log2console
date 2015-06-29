using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace Log2Console
{
    public static class utils
    {
        public static void Export2Excel(ListView listView, string FileName)
        {

                using (StreamWriter sw = new StreamWriter(FileName, false))
                {
                    sw.AutoFlush = true;
                    StringBuilder sb = new StringBuilder();
                    foreach (ColumnHeader ch in listView.Columns)
                    {
                        sb.Append(ch.Text + ",");
                    }
                    sb.AppendLine();
                    foreach (ListViewItem lvi in listView.Items)
                    {
                        foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                        {
                            if (lvs.Text.Trim() == string.Empty)
                                sb.Append(" ,");
                            else
                                sb.Append(lvs.Text + ",");
                        }
                        sb.AppendLine();
                    }
                    
                    sw.Write(sb.ToString());
                    sw.Close();
                }
                FileInfo fil = new FileInfo(FileName);
                if (fil.Exists == true)
                    MessageBox.Show("Process Completed", "Export to Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}