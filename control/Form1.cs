using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace control
{
    public partial class Form1 : Form
    {
        List<byte> recvList = new List<byte>();
        
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;
            comboBox2.SelectedIndex = 1;
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] rxbuf = new byte[32];
            int i;
            int len = 0;
            int buf_len = serialPort1.BytesToRead;
            string str = serialPort1.ReadLine();
            rxbuf = System.Text.Encoding.ASCII.GetBytes(str);

            //byte[] buf = new byte[buf_len];
            //serialPort1.Read(buf, 0, buf_len);
            //recvList.AddRange(buf);
            //if (recvList.Count > 64)
            //{
            //    recvList.Clear();

            //}
            //len = recvList.Count;
            //for (i=0;i< len; i++)
            //{
            //    rxbuf[i] = recvList[i];
            //}
            if ((rxbuf[0] != 0x48) || (rxbuf[1] != 0x4C))
            {
                return;
            }
            switch(rxbuf[3])
            {
                case 0x01:
                    textBox1.Text = rxbuf[4].ToString();
                    break;
                case 0x02:
                    textBox3.Text += "组号设置成功\r\n";
                    break;
                case 0x03:
                    textBox2.Text = rxbuf[4].ToString();
                    break;
                case 0x04:
                    textBox3.Text += "设备号设置成功\r\n";
                    break;
                default: 
                    break;
            }
                serialPort1.DiscardInBuffer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "打开")
            {
                try
                {
                    serialPort1.PortName = (string)comboBox1.SelectedItem;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                    serialPort1.DataBits = 8;
                    serialPort1.Parity = Parity.None;
                    serialPort1.StopBits = StopBits.One;
                    try
                    {
                        serialPort1.Open();
                    }
                    catch
                    {
                        MessageBox.Show("串口打开失败", "错误提示");
                    }
                    if (serialPort1.IsOpen == true)
                    {
                        button1.Text = "关闭";
                    }
                }
                catch
                {
                    MessageBox.Show("请选择正确配置", "错误提示");
                }

            }
            else
            {
                serialPort1.Close();
                button1.Text = "打开";
            }
        }
        private void serialDataSend(byte cmd, byte[] data,byte len) 
        {
            int i;
            int len1;
            byte[] buffer = new byte[32];
            if(serialPort1.IsOpen==false)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            buffer[0] = 0x48;
            buffer[1] = 0x4C;
            if(tabControl1.SelectedTab.Name == tabPage1.Name)
            {
                buffer[2] = 0x01;
            }
            else
            {
                buffer[2] = 0x02;
            }
            buffer[3] = cmd;
            for(i = 0; i < len;i++)
            {
                buffer[4+i] = data[i];
            }
            len1 = i + 6;
            buffer[len1 - 1] = 0x0A;
            buffer[len1 - 2] = 0x0D;
            serialPort1.Write(buffer, 0, len1);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            serialDataSend(0x01, null, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[1];
            try
            {
                data[0] = byte.Parse(textBox1.Text);
                serialDataSend(0x02, data, 1);
            }
            catch
            {
                MessageBox.Show("请输入正确数据");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialDataSend(0x03, null, 0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[1];
            try
            {
                data[0] = byte.Parse(textBox2.Text);
                serialDataSend(0x04, data, 1);
            }
            catch
            {
                MessageBox.Show("请输入正确数据");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = string.Empty;
        }
    }
}
