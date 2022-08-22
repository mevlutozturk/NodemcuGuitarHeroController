using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GUTRHCK
{
    public partial class Form1 : Form
    {
        MqttClient mqttClient;
        private string receiverData;
        private string id;
        private string previous_data;
        int[] receiverDataArr = { 0, 0, 0, 0, 0, 0, 0 };
        int[] previousDataArr = { 0, 0, 0, 0, 0, 0, 0 };

        public Form1()
        {
            InitializeComponent(); 
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);


        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            receiverData = serialPort1.ReadLine();
            this.Invoke(new EventHandler(serial_command));

        }

        WindowsInput.InputSimulator KB = new WindowsInput.InputSimulator();
        VAMemory vm = new VAMemory("GH3");
        private void serial_command(object sender, EventArgs e)
        {
            var keyboard = new Keyboard();
            listBox1.Items.Clear();
            listBox1.Items.Add(receiverData);

            receiverData = receiverData.Substring(0, receiverData.Length - 1);

            String[] dataStr = new string[7];

            for (int i = 0; i < receiverData.Length; i++)
            {
                dataStr[i] = receiverData[i].ToString();
            }


            for (int i = 0; i < 7; i++)
            {
                previousDataArr[i] = receiverDataArr[i];
                receiverDataArr[i] = int.Parse(dataStr[i]);
            }

            
            if (receiverDataArr[0] == 1) keyboard.Press(Keyboard.ScanCodeShort.KEY_1);
            if (receiverDataArr[0] != previousDataArr[0] && receiverDataArr[0] == 0) keyboard.Release(Keyboard.ScanCodeShort.KEY_1);

           
            if (receiverDataArr[1] == 1) keyboard.Press(Keyboard.ScanCodeShort.KEY_2);
            if (receiverDataArr[1] != previousDataArr[1] && receiverDataArr[1] == 0) keyboard.Release(Keyboard.ScanCodeShort.KEY_2);

           
            if (receiverDataArr[2] == 1) keyboard.Press(Keyboard.ScanCodeShort.KEY_3);
            if (receiverDataArr[2] != previousDataArr[2] && receiverDataArr[2] == 0) keyboard.Release(Keyboard.ScanCodeShort.KEY_3);

            
            if (receiverDataArr[3] == 1) keyboard.Press(Keyboard.ScanCodeShort.KEY_4);
            if (receiverDataArr[3] != previousDataArr[3] && receiverDataArr[3] == 0) keyboard.Release(Keyboard.ScanCodeShort.KEY_4);

            
            if (receiverDataArr[4] == 1) keyboard.Press(Keyboard.ScanCodeShort.KEY_5);
            if (receiverDataArr[4] != previousDataArr[4] && receiverDataArr[4] == 0) keyboard.Release(Keyboard.ScanCodeShort.KEY_5);

            
            if (receiverDataArr[5] == 1) keyboard.Press(Keyboard.ScanCodeShort.NUMPAD8);
            if (receiverDataArr[5] != previousDataArr[5] && receiverDataArr[5] == 0) keyboard.Release(Keyboard.ScanCodeShort.NUMPAD8);

            
            if (receiverDataArr[6] == 1) keyboard.Press(Keyboard.ScanCodeShort.NUMPAD9);
            if (receiverDataArr[6] != previousDataArr[6] && receiverDataArr[6] == 0) keyboard.Release(Keyboard.ScanCodeShort.NUMPAD9);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Process process = Process.GetProcessesByName("GH3")[0];
                foreach (ProcessModule module in process.Modules)
                {
                    if (module.FileName.IndexOf("GH3.exe") != -1)
                    {
                        id = module.BaseAddress.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please Start The Game First");
                Environment.Exit(0);
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            string[] ports = SerialPort.GetPortNames();
            
            comboBox1.Text = "Port";
            comboBox2.Text = "Baundrate";
            button1.Text = "Send";
            button2.Text = "CONNECT";
            label1.Text = "Your Score";
            label2.Text = "Your Message";
            checkBox1.Text = "Send Score";

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox2.Items.Add("300");
            comboBox2.Items.Add("600");
            comboBox2.Items.Add("1200");
            comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("57600");
            comboBox2.Items.Add("115200");
            try
            {
                mqttClient = new MqttClient("192.168.137.121");
                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                mqttClient.Subscribe(new string[] { "Score/Message" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                mqttClient.Connect("Button");
            }
            catch(Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }
        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(message)));
        }
        

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.BaudRate = Convert.ToInt32(comboBox2.SelectedItem);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (button2.Text == "CONNECT")
                {
                    button2.Text = "DISCONNECT";
                    listBox1.Items.Add("CONNECTION SUCCESS");
                    listBox1.Items.Add("PORT : " + comboBox1.SelectedItem.ToString());
                    listBox1.Items.Add("BAUNDRATE : " + comboBox2.SelectedItem);
                    serialPort1.Open();

                }
                else
                {
                    if (button2.Text == "DISCONNECT")
                        button2.Text = "CONNECT";
                        listBox1.Items.Add("NO CONNECTION");
                        serialPort1.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (mqttClient != null && mqttClient.IsConnected)
                {
                    mqttClient.Publish("Score/Message", Encoding.UTF8.GetBytes(textBox1.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
            
        }
        VAMemory vmemory = new VAMemory("GH3");
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int client = Convert.ToInt32(id);
                int a1 = vmemory.ReadInt32((IntPtr)client + 0x00612FCC);
                int a2 = vmemory.ReadInt32((IntPtr)a1 + 0x4);
                int a3 = vmemory.ReadInt32((IntPtr)a2 + 0x8);
                int a4 = vmemory.ReadInt32((IntPtr)a3 + 0x4);
                int a5 = vmemory.ReadInt32((IntPtr)a4 + 0x73C);
                int a6 = vmemory.ReadInt32((IntPtr)a5 + 0xC);
                int a7 = a6 + 0x22C;
                if (checkBox1.Checked)
                {
                    float r_float = vmemory.ReadFloat((IntPtr)a7);
                    textBox2.Text = r_float.ToString();
                }
                if (mqttClient != null && mqttClient.IsConnected)
                {
                    mqttClient.Publish("Score/Message", Encoding.UTF8.GetBytes(textBox2.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }
    }
}
