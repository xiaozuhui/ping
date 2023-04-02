using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ping
{
    public partial class mainForm : Form
    {
        delegate void AsynUpdateUI(string remoteIP, string remoteName);

        public mainForm()
        {
            InitializeComponent();
            //initListView();
        }

        /*private void initListView()
        {
            var interfaceInfos = IPTools.getInterfaceIP();
            foreach (var interfaceInfo in interfaceInfos)
            {
                ListViewItem listViewItemIP = new ListViewItem(interfaceInfo.Item1);
                var item = this.localInterfacesListView.Items.Add(listViewItemIP);
                item.SubItems.Add(interfaceInfo.Item2);
                item.SubItems.Add(interfaceInfo.Item3);
            }
        }*/

        /*private void localInterfacesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = localInterfacesListView.SelectedItems;
            if (selectedItems.Count >= 1)
            {
                this.textBox1.Text = selectedItems[0].Text;
            }
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            /*if (this.textBox1.Text == "")
            {
                MessageBox.Show("参数IP为空");
                return;
            }
            var domainIP = IPTools.GetDomainIP(this.textBox1.Text);
            if (domainIP.Item1)
            {
                var ip = domainIP.Item2;
                string[] ipItemList = ip.Split('.');
                var pre = string.Join(".", ipItemList.Take(ipItemList.Length - 1));
                try
                {
                    PingRemoteIP dataWrite = new PingRemoteIP();//实例化一个写入数据的类
                    dataWrite.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托
                    dataWrite.TaskCallBack += Accomplish;//绑定完成任务要调用的委托
                    Thread thread = new Thread(new ParameterizedThreadStart(dataWrite.runTask));
                    thread.IsBackground = true;
                    thread.Start(pre);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.StackTrace);
                    MessageBox.Show(err.Message);
                }
            }
            else
            {
                MessageBox.Show("ip: " + this.textBox1.Text + " , 无法解析");
            }*/
        }

        private void UpdataUIStatus(string remoteIP, string remoteName)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate (string ip, string name)
                {
                    ListViewItem listViewItemIP = new ListViewItem(ip);
                    var item = this.listView1.Items.Add(listViewItemIP);
                    item.SubItems.Add(name);
                }), remoteIP, remoteName);
            }
            else
            {
                ListViewItem listViewItemIP = new ListViewItem(remoteIP);
                var item = this.listView1.Items.Add(listViewItemIP);
                item.SubItems.Add(remoteName);
            }
        }

        private void Accomplish()
        {
            //还可以进行其他的一些完任务完成之后的逻辑处理
            MessageBox.Show("搜索完毕");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string maskStr = parseMaskedText(this.maskedTextBox2.Text);
            if (maskStr == "")
            {
                MessageBox.Show("参数IP为空");
                return;
            }

            var domainIP = IPTools.GetDomainIP(maskStr);
            if (domainIP.Item1)
            {
                var ip = domainIP.Item2;
                string[] ipItemList = ip.Split('.');
                var pre = string.Join(".", ipItemList.Take(ipItemList.Length - 1));
                try
                {
                    PingRemoteIP dataWrite = new PingRemoteIP();//实例化一个写入数据的类
                    dataWrite.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托
                    dataWrite.TaskCallBack += Accomplish;//绑定完成任务要调用的委托
                    Thread thread = new Thread(new ParameterizedThreadStart(dataWrite.runTask));
                    thread.IsBackground = true;
                    thread.Start(pre);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.StackTrace);
                    MessageBox.Show(err.Message);
                }
            }
            else
            {
                MessageBox.Show("ip: " + this.maskedTextBox2.Text + " , 无法解析");
            }
        }

        private string parseMaskedText(string maskedText)
        {
            if (maskedText == null)
            {
                return "";
            }
            string[] maskList = maskedText.Split('.');
            List<string> res = new List<string>();
            foreach (string str in maskList)
            {
                string s = str.Trim(new char[] { ' ', '_', '\t', '\n' });
                if (s == null || s.Equals(""))
                {
                    continue;
                }
                res.Add(s);
            }
            if (res.Count > 3)
            {
                return string.Join(".", res);
            }
            else if (res.Count == 3)
            {
                res.Add("1");
                return string.Join(".", res);
            }
            else
            {
                // TODO 进入特殊的搜索逻辑
            }
            return "";
        }
    }
}
