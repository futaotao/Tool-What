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

        /// <summary>
        /// 获取两个时间间隔  单位：秒
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        //public static int getTimeInterval(DateTime beginTime, DateTime endTime)
        //{
            
        //    TimeSpan ts = endTime.Subtract(beginTime);
        //    return (int)(ts.TotalSeconds);
        //}

        /// <summary>
        /// 获取两个时间间隔  单位：秒
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int getTimeInterval(String beginTime, DateTime endTime)
        {
            if (String.IsNullOrEmpty(beginTime)) {
                return 0;
            }

            DateTime begin = Convert.ToDateTime(beginTime);
            TimeSpan ts = endTime.Subtract(begin);
            return (int)(ts.TotalSeconds);
        }

        
        /// <summary>
        /// 获取当前连接的设备
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// adb 开始连接设备
        /// </summary>
        /// <param name="batPath">bat路径</param>
        /// <param name="device">设备号</param>
        /// <param name="onProcessListener"></param>
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

       /// <summary>
       /// 安装apk
       /// </summary>
       /// <param name="batPath">bat路径</param>
       /// <param name="device">设备号</param>
       /// <param name="apkPath">apk路径</param>
       /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 卸载apk
        /// </summary>
        /// <param name="batPath">bat路径</param>
        /// <param name="device">设备号</param>
        /// <param name="pkg">包名</param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 修改build.prop 以及 imei
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="device"></param>
        /// <param name="propPath"></param>
        /// <param name="onProcessListener"></param>
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

       


        /// <summary>
        /// 连接vpn
        /// </summary>
        /// <param name="batPath">bat路径</param>
        /// <param name="vpnName">VPN的名称</param>
        /// <param name="userName">VPN账号名</param>
        /// <param name="userPassword">VPN密码</param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 断开vpn
        /// </summary>
        /// <param name="batPath">bat路径</param>
        /// <param name="vpnName">VPN名称</param>
        /// <param name="onProcessListener"></param>
        public static void callDisconnectVpn(String batPath, String vpnName, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { vpnName }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_DISCONNECT_VPN);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }

       /// <summary>
       /// 关闭adb
       /// </summary>
       /// <param name="batPath"></param>
       /// <param name="onProcessListener"></param>
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


        /// <summary>
        /// 从模拟器中pull出 Xprivacy生成的随机值文件
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 从模拟器中 pull出 按键精灵生成Share文件
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="onProcessListener"></param>
        public static void callPullAnjianFile(String batPath, String targetPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { targetPath }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_PULL_ANJIAN);
                myThread.start();
            }
            catch (Exception e)
            {
            }
        }


        /// <summary>
        /// 删除Xprivacy生成的随机值文件
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="onProcessListener"></param>
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

        /*******************************************************************************************/
        
        #region  弃用代码

        /**

        /// <summary>
        /// 查看所有安装的apk 没有用到
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="onProcessListener"></param>
        public static void callApps(String batPath, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_GET_APPS);
                myThread.start();
            }
            catch (Exception e)
            {

            }
        }



        /// <summary>
        /// 获取当前模拟器安装的应用
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="device"></param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 清除模拟器应用数据
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="device"></param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// pull  按键精灵生成的 time文件。用来判断模拟器是否联网成功
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 删除按键精灵生成time文件
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="onProcessListener"></param>
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

        /// <summary>
        /// 重启模拟器  监听不到
        /// </summary>
        /// <param name="batPath"></param>
        /// <param name="device"></param>
        /// <param name="onProcessListener"></param>
        public static void callReboot(String batPath, String device, OnProcessListener onProcessListener)
        {
            if (onProcessListener == null)
            {
                return;
            }

            try
            {
                myThread = new MyThread(ThreadUtil.getBatOrExeStartInfo(batPath, new String[] { device }), onProcessListener, Constant.ProcessType.TYPE_OF_PROCESS_START_REBOOT);
                myThread.start();
            }
            catch (Exception e)
            {

            }
        }

         */
        #endregion
    }
}
