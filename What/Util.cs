using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO;


namespace What
{
    class Util
    {

        public static MyThread myThread = null;

        //检查时间是否开刷
        public static int checkTime(List<int> hourList, List<int> minList)
        {

            if (hourList != null && hourList.Count > 0 && minList != null && minList.Count > 0)
            {
                //每次刷一个小时  半个小时的时会替换一次包
                int hour = DateTime.Now.Hour;
                int min = DateTime.Now.Minute;
                foreach (int h in hourList)
                {
                    if (h == hour)
                    {
                        foreach (int m in minList) {
                            if (m == min)
                            {
                                if (minList.Count > 1)
                                {
                                    return min;
                                }
                                else {
                                    return hour;
                                }
                            }
                        }
                        
                    }
                }
            }

            return -1;
        }

        
        //获取当前连接的设备
        public static void getDevices(String batPath, OnProcessListener onProcessListener) {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[]{}), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_GET_DEVICES);
                myThread.start();
            }
            catch (Exception e) { 
            
            }
        }

        //连接设备
        public static void callConnect(String batPath, String device, OnProcessListener onProcessListener)
        {
            if(onProcessListener == null ){
            return;
            }
            try{
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_START_CONNECT);
                myThread.start();
            }catch(Exception e){
            }
        }

        //install apk
        public static void callInstall(String batPath,String device, String apkPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null) {
                return;
            }
            try
            {
                
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[]{ device, apkPath }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_START_INSTALL);
                myThread.start();
            }
            catch (Exception e) { 
            
            }
        }

        //查看所有安装的apk
        public static void callApps(String batPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[]{}), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_GET_APPS);
                myThread.start();
            }
            catch (Exception e)
            {

            }
        }

        //卸载安装包
        public static void callUninstall(String batPath, String device, String pkg, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device, pkg }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_UNINSTALL_APP);
                myThread.start();
            }
            catch (Exception e)
            {

            }
        }

        //启动修改prop
        public static void callProp(String batPath, String device, String propPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device, propPath }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_CHANGE_PROP);
                myThread.start();
            }
            catch (Exception e)
            {

            }
        }

        //重启
        public static void callReboot(String batPath, String device, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null) {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device}), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_START_REBOOT);
                myThread.start();
            }
            catch (Exception e) { 
            
            }
        }


        //连接vpn
        public static void callConnectVpn(String batPath, String vpnName, String userName, String userPassword, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { vpnName, userName, userPassword }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_CONNECT_VPN);
                myThread.start();
            }
            catch (Exception e) { 
            }
        }

        public static void callPullTimeFile(String batPath, String targetPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { targetPath }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_PULL_TIME);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        public static void callDeleteTimeFile(String batPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_DELETE_TIME);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        public static void callPullRandomFile(String batPath, String targetPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { targetPath }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_PULL_RANDOM);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        public static void callDeleteRandomFile(String batPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_DELETE_RANDOM);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        public static void callPMClear(String batPath, String device, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_PM_CLEAR);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        public static void callPMInstall(String batPath, String device, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_PM_INSTALL);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        //断开vpn
        public static void callDisconnectVpn(String batPath, String vpnName, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { vpnName}), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_DISCONNECT_VPN);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        //关闭adb
        public static void callCloseAdb(String batPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_CLOSE_ADB);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

        //获取ip
        public static Dictionary<string, string> getIpInfo()
        {
            string url = "http://www.ip.cn/";
            Dictionary<string, string> dic = null;
            try
            {
                dic = new Dictionary<string, string>();
                WebRequest wr = WebRequest.Create(url);
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.UTF8);
                string all = sr.ReadToEnd(); //读取网站的数据
                sr.Close();
                s.Close();
                int isp = all.IndexOf("<code>") + 6;
                int iep = all.IndexOf("</code>");
                string _ip = all.Substring(isp, iep - isp);
                dic.Add("ip", _ip.Trim());
                int lsp = all.IndexOf("来自：") + 3;
                int lep = all.IndexOf("</p><p>GeoIP:");
                string _location = all.Substring(lsp, lep - lsp);
                dic.Add("location", _location);
            }
            catch(Exception e)
            {
               

            }
            return dic;
        }

        //当前进程未启动
        public const int PROCESS_NO_START = 0x0000;
        public const int PROCESS_RUNNING = 0x0001;
        public const int PROCESS_DEAD = 0x0002;
        // 判断当前进程的状态
        public static int getProcessStaus(String name) {
            Process[] temp = Process.GetProcessesByName(name);
            if (temp != null && temp.Length > 0)
            {
                foreach (Process p in temp) {
                    if (!p.Responding) {
                        return PROCESS_DEAD;
                    }
                }
                return PROCESS_RUNNING;
            }
            else {
                return PROCESS_NO_START;
            }
        }

        //启动一个应用程序
        public static void launchProcess(string name) {
            try
            {
                Process.Start(name);
            }
            catch(Exception e) { 
            }
        }
        
        //关闭一个程序
        public static void closeProcess(String name) { 
             Process[] temp = Process.GetProcessesByName(name);
             if (temp != null && temp.Length > 0)
             {
                 foreach (Process p in temp)
                 {
                     p.Kill();
                 }
             }
        }
    }
}
