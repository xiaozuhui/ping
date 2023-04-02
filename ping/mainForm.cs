using System;
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
            initListView();
        }

        private void initListView()
        {
            var interfaceInfos = IPTools.getInterfaceIP();
            foreach (var interfaceInfo in interfaceInfos)
            {
                ListViewItem listViewItemIP = new ListViewItem(interfaceInfo.Item1);
                var item = this.localInterfacesListView.Items.Add(listViewItemIP);
                item.SubItems.Add(interfaceInfo.Item2);
                item.SubItems.Add(interfaceInfo.Item3);
            }
        }

        private void localInterfacesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = localInterfacesListView.SelectedItems;
            if (selectedItems.Count >= 1)
            {
                this.textBox1.Text = selectedItems[0].Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
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
                catch(Exception err)
                {
                    Console.WriteLine(err.StackTrace);
                    MessageBox.Show(err.Message);
                }
            } 
            else
            {
                MessageBox.Show("ip: " + this.textBox1.Text + " , 无法解析");
            }
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
    }
}
