using System;
using System.Drawing;
using System.Windows.Forms;

namespace DanTup.BrowserSelector.Selector
{
    public partial class SelectorWindow : Form
    {
        private readonly string _urlToOpen;

        public SelectorWindow(string url)
        {
            _urlToOpen = url;
            var urlTitle = _urlToOpen.Length > 80 ? _urlToOpen.Substring(0, 75) + " ..." : _urlToOpen;
            
            InitializeComponent();
            
            listBox1.Items.Clear();
            contextMenuStrip1.Items.Clear();

            var headFont = new Font(contextMenuStrip1.Font, FontStyle.Bold);
            var urlFont = new Font(contextMenuStrip1.Font.FontFamily, 6, FontStyle.Underline);
            
            contextMenuStrip1.Items.Add(new ToolStripLabel("Open with") {ForeColor = Color.Blue, Font = headFont, AutoToolTip = false });
            contextMenuStrip1.Items.Add(new ToolStripLabel(urlTitle) {ForeColor = Color.Gray, Font = urlFont, ToolTipText = _urlToOpen, AutoToolTip = false });
            contextMenuStrip1.Items.Add(new ToolStripSeparator());

            var browsers = ConfigReader.GetBrowsers();
            foreach (var browser in browsers)
            {
                listBox1.Items.Add(browser.Value);

                var cItem = contextMenuStrip1.Items.Add(browser.Key);
                cItem.Tag = browser.Value;
                cItem.AutoToolTip = false;
            }

            contextMenuStrip1.MouseWheel += new MouseEventHandler(this.mouseWheel_UpDown);
            contextMenuStrip1.ShowItemToolTips = true;
        }

        private void mouseWheel_UpDown(object sender, MouseEventArgs e)
        {
            // System.Windows.Forms.MessageBox.Show($"Delta = ${e.Delta}");

            if (e.Delta > 0) {
                var currInd = listBox1.SelectedIndex;
                if (currInd > 0) {
                    listBox1.SelectedIndex -= 1;
                    contextMenuStrip1.Items[listBox1.SelectedIndex+3].Select();
                }
            }
            if (e.Delta < 0) {
                var currInd = listBox1.SelectedIndex;
                if (currInd < listBox1.Items.Count - 1) {
                    listBox1.SelectedIndex += 1;
                    contextMenuStrip1.Items[listBox1.SelectedIndex+3].Select();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.ShowMenu();
        }

        internal void ShowMenu()
        {
            contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            contextMenuStrip1.Focus();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var browser = listBox1.SelectedItem as Browser;

            if (browser != null)
            {
                Program.OpenUrlInBrowser(_urlToOpen, browser);
            }
            else
            {
                MessageBox.Show(this, ":-((");
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SelectorWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) {
                contextMenuStrip1.Items[listBox1.SelectedIndex+3].PerformClick();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var browser = e.ClickedItem.Tag as Browser;
            
            if (browser != null)
            {
                Program.OpenUrlInBrowser(_urlToOpen, browser);
            }
            else
            {
                MessageBox.Show(this, ":-((");
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
