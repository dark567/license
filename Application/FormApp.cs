using System;
using System.Windows.Forms;

namespace Appl
{
    public partial class FormApp : Form
    {
        public FormApp()
        {
            InitializeComponent();

            this.Load += new EventHandler(Form1_Load);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CryptoClass crypto = new CryptoClass();
            if (!crypto.Form_LoadTrue()) Close();

            string date = crypto.GetDecodeKey("keyfile.dat").Substring(crypto.GetDecodeKey("keyfile.dat").IndexOf("|") + 1);

            
            if (DateTime.Parse(date) < DateTime.Now) Close();

            string decryptstring = crypto.GetDecodeKey("keyfile.dat");
            int number = decryptstring.IndexOf("|");
            decryptstring = decryptstring.Substring(0, number);

            textBox1.Text = crypto.GetDecodeKey("keyfile.dat");
            textBox2.Text = decryptstring;
            textBox3.Text = date;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
