using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using Client.ChatService;
using MySql.Data.MySqlClient;
using MySql.Data;
using DevExpress.XtraEditors;

namespace Client
{
    public partial class Chat : DevExpress.XtraEditors.XtraForm
    {
        ReceiveClient rc = null;
        string myName;
        private MySqlConnection connection;
        string USER = Properties.Settings.Default.Username;
        public Chat()
        {
            simpleButton1.Enabled = false;
            string connectionString;
            connectionString = "Server=64.94.238.79; database=ghost; Uid=Ryan; Pwd=COREi7;";
            connection = new MySqlConnection(connectionString);
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
            this.txtSend.KeyPress += new KeyPressEventHandler(txtSend_KeyPress);
        }
        void rc_ReceiveMsg(string sender, string msg)
        {
            if (msg.Length > 0)
                txtMsgs.Text += Environment.NewLine + sender + ">" + msg;
        }

        void rc_NewNames(object sender, List<string> names)
        {
            lstClients.Items.Clear();
            foreach (string name in names)
            {
                if (name != myName)
                    lstClients.Items.Add(name);
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (label2.Text.Length > 0)
            {
                txtMsgs.Enabled = true;
                txtSend.Enabled = true;
                btnSend.Enabled = true;

                myName = label2.Text.Trim();

                rc = new ReceiveClient();
                XtraMessageBox.Show("Connected To Chat");
                rc.Start(rc, myName);

                rc.NewNames += new GotNames(rc_NewNames);
                rc.ReceiveMsg += new ReceviedMessage(rc_ReceiveMsg);
            }
        }

        private void SendMessage()
        {
            if (lstClients.Items.Count != 0)
            {
                txtMsgs.Text += Environment.NewLine + myName + ">" + txtSend.Text;
                if (lstClients.SelectedItems.Count == 0)
                    rc.SendMessage(txtSend.Text, myName, lstClients.Items[0].ToString());
                else
                    if (!string.IsNullOrEmpty(lstClients.SelectedItem.ToString()))
                    rc.SendMessage(txtSend.Text, myName, lstClients.SelectedItem.ToString());

                txtSend.Clear();
            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void txtSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;

            if (keyValue == 13)
                SendMessage();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            rc.Stop(myName);
            this.Hide();
            e.Cancel = true;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            string LoginQ = "Select * from ghost.clients WHERE Username=@par1 AND Password=@par2;";
            MySqlCommand LoginCMD = new MySqlCommand(LoginQ, connection);
            LoginCMD.Parameters.AddWithValue("@par1", this.textEdit1.Text);
            LoginCMD.Parameters.AddWithValue("@par2", textEdit2.Text);
            try
            {
                connection.Open();
                LoginCMD.ExecuteNonQuery();
                XtraMessageBox.Show("Welcome - " + textEdit1.Text);
                USER = textEdit1.Text;
                Properties.Settings.Default.Save();
                label2.Text = USER;
                simpleButton1.Enabled = true;
                simpleButton2.Enabled = false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
    }
}
