using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ping
{
    internal class PingRemoteIP
    {
        public delegate void UpdateUI(string remoteIp, string remoteName);//声明一个更新主线程的委托
        public UpdateUI UpdateUIDelegate;

        public delegate void AccomplishTask();//声明一个在完成任务时通知主线程的委托
        public AccomplishTask TaskCallBack;
        public void runTask(object prefix)
        {
            string pre = (string)prefix;
            for (int i = 2; i < 255; i++)
            {
                var remoteIP = pre + "." + i.ToString();
                var res = IPTools.ping(remoteIP);
                if (res)
                {
                    /*ListViewItem listViewItemIP = new ListViewItem(remoteIP);
                    var item = this.listView1.Items.Add(listViewItemIP);*/
                    try
                    {
                        IPHostEntry he = Dns.GetHostEntry(remoteIP);
                        
                        Console.WriteLine("ip为{0}的设备名称是：{1}", remoteIP, he.HostName);
                        /*item.SubItems.Add(he.HostName);*/
                        UpdateUIDelegate(remoteIP, he.HostName);
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        Console.WriteLine("ip为{0}的设备名称是：{1}", remoteIP, "未知主机");
                        /*item.SubItems.Add("未知主机");*/
                        UpdateUIDelegate(remoteIP, "未知主机");
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.StackTrace);
                    }
                }
            }
            TaskCallBack();
        }
    }
}
