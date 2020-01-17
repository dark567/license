using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyFileGenerator
{
    public partial class FormKeyTest : Form
    {
        RijndaelManaged Rijndael;

        string GetMotherBoardID()
        {
            string MotherBoardID = string.Empty;
            SelectQuery query = new SelectQuery("Win32_BaseBoard");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator();
            while (enumerator.MoveNext())
            {
                ManagementObject info = (ManagementObject)enumerator.Current;
                MotherBoardID = info["SerialNumber"].ToString().Trim();
            }
            return MotherBoardID;
        }
        //Метод для получения ProcessorID
        //string GetProcessorID()
        //{
        //    string ProcessorID = string.Empty;
        //    SelectQuery query = new SelectQuery("Win32_processor");
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
        //    ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        ManagementObject info = (ManagementObject)enumerator.Current;
        //        ProcessorID = info["processorId"].ToString().Trim();
        //    }
        //    return ProcessorID;
        //}
        //Метод для получения VolumeSerial("C:\")
        string GetVolumeSerial(string strDriveLetter = "C")
        {
            ManagementObject VolumeSerial = new ManagementObject(string.Format("win32_logicaldisk.deviceid=\"{0}:\"", strDriveLetter));
            VolumeSerial.Get();
            return VolumeSerial["VolumeSerialNumber"].ToString().Trim();
        }

        public FormKeyTest()
        {
            InitializeComponent();
            Rijndael = new RijndaelManaged();
            this.Load += new EventHandler(Form1_Load);
            button1.Click += new EventHandler(button1_Click);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            label1.Text = null;
            this.Text = "KeyFileGenerator for MON08 v1.0.0.0 by Dark";
            //Данные с целевого компьютера
            string date = DateTime.Now.ToShortDateString().ToString();
            string number = GetMotherBoardID() + /*GetProcessorID() +*/ GetVolumeSerial() + "|" + dateTimePicker1.Value.ToShortDateString();
            textBox1.Text = number;
        }
        void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Строка данных пуста!";
                return;
            }
            else
            {
                label1.Text = null;
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = "keyfile";
                dialog.Filter = "dat files(*.dat)|*.dat";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    GetKeyFile(textBox1.Text, dialog.FileName);
                    MessageBox.Show("Файл ключа создан.", "Key Generator");
                }
            }
        }
        public void GetKeyFile(string inString, string path)
        {
            byte[] key = new byte[0x20];
            for (int i = 0; i <= 0x1f; i++)
                key[i] = 0x1f;
            Rijndael.Key = key;
            ICryptoTransform transformer = Rijndael.CreateEncryptor();
            using (FileStream fs = File.Open(path, FileMode.OpenOrCreate))
            {
                fs.Write(Rijndael.IV, 0, Rijndael.IV.Length);
                CryptoStream cs = new CryptoStream(fs, transformer, CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);
                sw.Write(inString);
                sw.Flush();
                cs.FlushFinalBlock();
                sw.Close();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
           // MessageBox.Show(dateTimePicker1.Value.ToShortDateString(), "Key Generator");
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            string number = GetMotherBoardID() + /*GetProcessorID() +*/ GetVolumeSerial() + "|" + dateTimePicker1.Value.ToShortDateString();
            textBox1.Text = number;
        }
    }
}

