using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ping
{
    internal class IPTools
    {
        /// <summary>
        /// 获取网卡接口所在的ip地址
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<string, string, string>> getInterfaceIP()
        {
            List<Tuple<string, string, string>> ips = new List<Tuple<string, string, string>>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) // 以太网
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    //获取单播地址集
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            string ipv4 = ipadd.Address.ToString();//获取ip
                            if (ipv4.StartsWith("169.254"))
                            {
                                continue;
                            }
                            ips.Add(Tuple.Create(ipv4, adapter.Name, adapter.Id));
                        }
                        else if (ipadd.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            //本机IPV6 地址
                        }
                    }
                }
            }
            return ips;
        }

        /// <summary>
        /// ping传入的ip地址，返回ping的结果
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool ping(string ipAddress)
        {
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(ipAddress, 200);
                return (reply.Status == IPStatus.Success);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.StackTrace?.ToString());
            }
            return false;
        }

        /// <summary>
        /// 获取ip地址对应的主机信息
        /// </summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> getResult(List<string> ips)
        {
            var res = new List<Tuple<string, string>>();
            foreach(string ip in ips)
            {
                var isPing = ping(ip);
                if (isPing)
                {
                    try
                    {
                        IPHostEntry he = Dns.GetHostEntry(ip);
                        res.Add(Tuple.Create(ip, he.HostName));
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        res.Add(Tuple.Create(ip, "未知主机"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                        continue;
                    }
                }
            }
            return res;
        }


        public static Tuple<bool, string> GetDomainIP(string url)
        {
            string ipAddress = url;
            if (!ipAddress.StartsWith("http"))
            {
                ipAddress = "http://" + ipAddress;
            }
            string p = @"(http|https)://(?<domain>[^(:|/]*)";
            Regex reg = new Regex(p, RegexOptions.IgnoreCase);
            Match m = reg.Match(ipAddress);
            try
            {
                string Result = m.Groups["domain"].Value;//提取域名地址 
                Console.WriteLine(Result);
                IPHostEntry host = Dns.GetHostByName(Result);//域名解析的IP地址
                Console.WriteLine(host.AddressList);
                IPAddress ip = host.AddressList[0];
                string rIP = ip.ToString();
                return Tuple.Create(true, rIP);
            }
            catch
            {
                return Tuple.Create(false, "请输入正确的域名,或者您的电脑没有联互联网");
            }
        }


    }

    
}
