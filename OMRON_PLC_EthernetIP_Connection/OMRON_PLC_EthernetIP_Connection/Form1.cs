using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

/// <summary>
/// Created by github.com/robototem
/// @robototem
/// </summary>

namespace OMRON_PLC_EthernetIP_Connection
{
    public partial class Form1 : Form
    {
        Thread omronThread;
        
        public bool _plcConnectionStatus = false;

        public Form1()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false; // Allows us to update UI alongside threads
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Configure an IP");
            Log("Configure DM Area to Write");
            Log("Configure DM Area to Read");
            Log("Only '1' or 4 bits will be read.");
        }

        /// <summary>
        /// The button for starting PLC connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _plcConnectionStatus = PlcOmronHelper.Connect(textBoxPLC_IP.Text, 9600);
            }
            catch (Exception ex)
            {
                Log("(1001) " + ex);
            }

            if (_plcConnectionStatus)
            {
                Log("(1000) PLC connection has been made.");

                omronThread = new Thread(UpdateOmron);
                omronThread.Start();
            }
            else
            {
                Log("(1001) PLC connection was failed.");
            }
        }


        private void UpdateOmron()
        {
            while (true)
            {
                Thread.Sleep(20);

                ConnectionTrial();

            }
        }

        /// <summary>
        /// It tries to connect if the connection is lost.
        /// </summary>
        private void ConnectionTrial()
        {
            try
            {
                if (!_plcConnectionStatus)
                {
                    Thread.Sleep(500);
                    Log("(1001) PLC connection is lost. Trying again.");

                    _plcConnectionStatus = PlcOmronHelper.Connect(textBoxPLC_IP.Text, 9600);

                    if (_plcConnectionStatus)
                    {
                        Log("(1000) PLC connection has been after trials.");
                    }

                }
            }
            catch (Exception ex)
            {
                Log("(1001) " + ex);
            }

        }

        private void Log(string mymessage)
        {
            // (1000) SUCCESS message
            // (1001) ERROR message
            if (mymessage.Substring(0, 6) == "(1000)")
            {
                string newmessage = DateTime.Now + ": " + mymessage + "\n";

                richTextBoxLog.Select(richTextBoxLog.TextLength, newmessage.Length);
                richTextBoxLog.SelectionColor = Color.Green;
                richTextBoxLog.AppendText(newmessage);
            }
            else if (mymessage.Substring(0, 6) == "(1001)")
            {
                string newmessage = DateTime.Now + ": " + mymessage + "\n";

                richTextBoxLog.Select(richTextBoxLog.TextLength, newmessage.Length);
                richTextBoxLog.SelectionColor = Color.Red;
                richTextBoxLog.AppendText(newmessage);
            }
            else
            {
                string newmessage = DateTime.Now + ": " + mymessage + "\n";
                richTextBoxLog.Select(richTextBoxLog.TextLength, newmessage.Length);
                richTextBoxLog.SelectionColor = Color.Black;
                richTextBoxLog.AppendText(newmessage);
            }
        }

        /// <summary>
        /// It writes to defined PLC Data Memory Area specified text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWrite_Click(object sender, EventArgs e)
        {
            if (_plcConnectionStatus)
            {
                try
                {
                    PlcOmronHelper.Write(int.Parse(textBoxDMwrite.Text), textBoxWhatToWrite.Text);
                    Log("(1000) Write command has been sent.");
                }
                catch (Exception ex)
                {
                    Log("(1001) " + ex);
                }
            }
        }


        /// <summary>
        /// 1 DM area is to be read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRead_Click(object sender, EventArgs e)
        {
            if (_plcConnectionStatus)
            {
                try
                {
                    int data_to_read = 1;
                    int data_need_to_be_read = data_to_read * 4;

                    string read_data = PlcOmronHelper.Read(4540, data_to_read);

                    if (read_data != null)
                    {
                        if (read_data.Length == data_need_to_be_read)
                        {
                            textBoxReadData.Text = read_data.Substring(0, 1);
                            Log("(1000) Read command has been sent.");
                        }
                    }
                    else
                    {
                        Log("(1001) Mismatch error.");
                    }
                }
                catch (Exception ex)
                {
                    Log("(1001) " + ex);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (omronThread.IsAlive)
            {
                omronThread.Abort();
            }
        }
    }
}
