using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PfeLibrary;

namespace PFEProject
{
    public partial class cvsshowresult : Form
    {
        public cvsshowresult()
        {
            InitializeComponent();
            listBox1.Items.Clear();
            listBox1.Update();
        }

        private void cvsshowresult_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, Dictionary<int, List<int>>> pair in SharedValues.AUCollection)
            {
                listBox1.Items.Add(pair.Key);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string s = string.Empty;
            foreach (var AU in SharedValues.AUCollection[listBox1.SelectedItem.ToString()])
            {
                // listView1.Items.Add(new ListViewItem());
                ListViewItem lv = new ListViewItem(AU.Key.ToString());
                foreach (int i in AU.Value)
                {
                    s += "    " + i.ToString();
                }

                lv.SubItems.Add(s);
                listView1.Items.Add(lv);

                s = string.Empty;
            }
        } 
    }
}
