using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Media;
using System.Xml;

namespace What
{
    public partial class What : Form, OnProcessListener
    {

        private static String basePath = "";

        //当前模拟器的 端口
        private static String mDevice = "";

        private static String PROCESS_NAME = "player";
        private static String VBOX_HEAD_NAME = "VBoxHeadless";
        private static String VBOX_NET_NAME = "VBoxNetDHCP";
        private static String VBOX_SVC_NAME = "VBoxSVC";


        private static List<CheckBox> checkList = null;
        private static List<int> hourList = new List<int>();

        // avd的数量
        private static List<int> avdList = null;
        // 启动avd 的x坐标
        private static Dictionary<int, int> positionXDic = null;
        // 启动avd 设置的x的坐标
        private static Dictionary<int, int> positionSXDic = null;
        // 启动avd 的y坐标
        private static Dictionary<int, int> positionYDic = null;


        #region  新的流程不需要再安装和卸载包了
        //apk部分
        //private static List<string> apkFolderList = null;
        //private static List<String> totalList = new List<string>();
        //private static List<string> pkgList = new List<string>();
        // 每一个循环安装的总数

        // 每一个循环已经安装的数量
        //private static int installSuccess = 0;
        // 每一个循环卸载apk的数量
        //private static int uninstallSuccess = 0;
        #endregion

        //模拟的全部机型
        private static Dictionary<string, string> modelDic = null;


        private Thread runThread = null;

        //当前设备连接的状态
        private static bool isLaunching = false;
        private static bool isConnecting = false;
        private static bool isConnected = false;

        //启动次数
        private static int runTimes = 0;

        // 从1开始编号
        private static int mCurrentAvdNum = -1;
        private static string mCurrentScreen = "";
        // 新建avd 第一次启动
        private static bool isAvdFirstRun = false;


        // 每次开刷的时间
        private static int beginDay = -1;
        private static int beginHour = -1;
        private static int beginMin = -1;
        private static int beginSec = -1;

        //刷的时间间隔
        private static int shuaInterval = 145;
        

        //每次启动的时间
        private static int launchHour = -1;
        private static int launchMin = -1;
        private static int launchSec = -1;
        private static int responseTime = 180;


        // 随机启动一个 avd
        private static int mTapX = -1;
        private static int mTapSX = -1;
        private static int mTapY = -1;


        private static bool isStop = true;
        private static bool isVpnConnect = false;

        // 需要清除缓存 的包
        private static string[] pkgArray = new string[] { "com.sevenseven.android.ppt", "com.sevenseven.caculator", "com.sevenseven.bill","me.sevenseven.sote","com.sevenseven.account"
        ,  "com.seven.mypurse","com.android.seven.cargas","com.sevensevenlittle","com.seven.cashbook"
        , "com.wjjnote","com.wjj.rabbit","com.Green_light_seven","com.wjj.electric","com.wjj.grad","com.wjj.loop","com.jjtax"
        ,"com.wjjwjj.account","com.wjjwjj.house","com.wjjwjj.moneyspell","me.wjjwjj.sote","com.wjjwjj.android.ppt"};

        //测试apk 用来检测是否启动完成
        private static string testApkPath = "";

        //每次安装的时间  如果两分钟内没有安装成功 关闭
        private static int installHour = -1;
        private static int installMin = -1;
        private static int installSec = -1;
        private static int installTime = 120;
        private static bool isStartInstall = false;

        private static String tipMusicPath = "";

        //每次启动联网的时间  如果两分钟内没有启动联网 关闭
        private static int changeNetHour = -1;
        private static int changeNetMin = -1;
        private static int changeNetSec = -1;
        private static int changeNetTime = 120;
        private static bool isStartChangeNet = false;


        public What()
        {
            InitializeComponent();

            What.CheckForIllegalCrossThreadCalls = false;

            basePath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1);

            initCheckList();

            testApkPath = basePath + Constant.Folders.APK_FOLDER_NAME + "\\test.apk";
            tipMusicPath = basePath + Constant.Folders.MUSIC_FOLDER_NAME + "\\tip.wav";

            //initApkFolderList();
            //initSDKList();
            initModelList();

            deviceTb.Text = mDevice;
            timeLabel.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            Point ms = Control.MousePosition;
            postionLabel.Text = "x:" + ms.X + " y:" + ms.Y;

            int first = 400;
            int interval = 40;

            tb1_x.Text = "200";
            tb1_y.Text = first.ToString();
            tb1_sx.Text = "683";

            tb2_x.Text = "200";
            tb2_y.Text = (first + interval* 1).ToString();
            tb2_sx.Text = "683";

            tb3_x.Text = "200";
            tb3_y.Text = (first + interval* 2).ToString();
            tb3_sx.Text = "683";

            tb4_x.Text = "200";
            tb4_y.Text = (first + interval * 3).ToString();
            tb4_sx.Text = "683";

            setting_custom_x.Text = "267";
            setting_custom_y.Text = "345";

            setting_X_x.Text = "305";
            setting_X_y.Text = "385";

            setting_Y_x.Text = "388";
            setting_Y_y.Text = "385";

            setting_ok_x.Text = "521";
            setting_ok_y.Text = "580";

            setting_dpi_x.Text = "470";
            setting_dpi_y.Text = "385";

            setting_240_x.Text = "470";
            setting_240_y.Text = "452";

            setting_320_x.Text = "470";
            setting_320_y.Text = "470";

            setting_480_x.Text = "470";
            setting_480_y.Text = "485";

            runThread = new Thread(run);
        }


        #region Init
        // 初始化checkList
        private void initCheckList()
        {
            checkList = new List<CheckBox>();
            checkList.Add(cb1);
            checkList.Add(cb2);
            checkList.Add(cb3);
            checkList.Add(cb4);
            checkList.Add(cb5);
            checkList.Add(cb6);
            checkList.Add(cb7);
            checkList.Add(cb8);
            checkList.Add(cb9);
            checkList.Add(cb10);
            checkList.Add(cb11);
            checkList.Add(cb12);
            checkList.Add(cb13);
            checkList.Add(cb14);
            checkList.Add(cb15);
            checkList.Add(cb16);
            checkList.Add(cb17);
            checkList.Add(cb18);
            checkList.Add(cb19);
            checkList.Add(cb20);
            checkList.Add(cb21);
            checkList.Add(cb22);
            checkList.Add(cb23);
            checkList.Add(cb24);
        }



        //初始化配置文件里的
        #region
        //private void initApkFolderList()
        //{
        //    apkFolderList = new List<string>();
        //    string baseFolderPath = basePath + Constant.Folders.APK_FOLDER_NAME;
        //    string[] dirArray = Directory.GetDirectories(baseFolderPath);
        //    if (dirArray != null && dirArray.Length > 0)
        //    {
        //        foreach (string path in dirArray)
        //        {
        //            apkFolderList.Add(path);
        //        }
        //    }
        //}
        #endregion

        //private void initSDKList()
        //{
        //    sdkDic = new Dictionary<int, string>();
        //    string sdkFolderPath = basePath + Constant.Folders.SDK_FOLDER_NAME;
        //    string[] dirArray = Directory.GetDirectories(sdkFolderPath);

        //    if (dirArray != null && dirArray.Length > 0)
        //    {
        //        foreach (string path in dirArray)
        //        {
        //            int sdk = int.Parse(path.Substring(path.LastIndexOf("\\") + 1));
        //            string propPath = path + "\\" + "build.prop";
        //            sdkDic.Add(sdk, propPath);
        //        }
        //    }
        //}

        private void initModelList()
        {
            modelDic = new Dictionary<string, string>();
            string baseFolderPath = basePath + Constant.Folders.MODEL_FOLDER_NAME;
            string[] dirArray = Directory.GetFiles(baseFolderPath);
            if (dirArray != null && dirArray.Length > 0)
            {
                foreach (string path in dirArray)
                {
                    string model = path.Substring(path.LastIndexOf("\\") + 1);
                    modelDic.Add(model, path);
                }
            }
        }
        #endregion

        #region 安装和卸载apk

        //static Semaphore semaphore = new Semaphore(1, 1); //同时只允许一线程
        ////遍历安装包
        //private void installAllApks(int times, List<string> apkFolderList)
        //{
        //    if (apkFolderList == null || apkFolderList.Count == 0)
        //    {
        //        return;
        //    }

        //    string path = apkFolderList[times % apkFolderList.Count];
        //    LogUtil.LogMessage(log, "install:" +path);
        //    string[] apks = Directory.GetFiles(path);
        //    if (apks != null && apks.Length > 0)
        //    {
        //        LogUtil.LogMessage(log, "install:" + apks.Length);
        //        foreach (string apkPath in apks)
        //        {
        //            totalList.Add(apkPath);
        //            pkgList.Add(apkPath.Substring(apkPath.LastIndexOf("\\") + 1, apkPath.LastIndexOf(".apk") - apkPath.LastIndexOf("\\") - 1));
        //        }
        //        //install
        //        LogUtil.LogMessage(log, "install:" + totalList.Count + ":" + installSuccess);
        //        if (totalList.Count > installSuccess)
        //        {
        //            startInstall(totalList[installSuccess]);
        //        }
        //    }
        //}

       

        private void startInstall(String apkPath)
        {
            //  semaphore.WaitOne();
            LogUtil.LogMessage(log, "Devices:" + mDevice + "\n Install:" + apkPath + "\n " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            isStartInstall = false;
            installHour = DateTime.Now.Hour;
            installMin = DateTime.Now.Minute;
            installSec = DateTime.Now.Second;
            LogUtil.LogMessage(log, "3秒安装一次 避免太频繁！");
            Thread.Sleep(3000);
            Util.callInstall(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.INSTALL_BAT_NAME, mDevice, apkPath, this);
        }

        //private void startUninstall(String pkgName) {
        //    semaphore.WaitOne();
        //    LogUtil.LogMessage(log, "Devices:" + mDevice + "UnInstall:" + pkgName);
        //    Util.callUninstall(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.UNINSTALL_BAT_NAME, mDevice, pkgName, this);
        //}
        #endregion

        private void btnStop_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Hello");

            //callPullRandomFile();
            //callDeleteRandomFile();

            //avdSetting(1,1,"1","1");
             Dictionary<string, string> ipDic = Util.getIpInfo();
             if (ipDic != null && ipDic.Count > 0)
             {

                 string ip = ipDic["ip"];
                 MessageBox.Show(ip);
             }

            beginDay = DateTime.Now.Day;
            isStop = !isStop;
        }

        private void startBtn_Click(object sender, EventArgs e)
        {

            //检测Genymotion 的窗口位置 并移动他

            moveGenymotionWin();

            mDevice = deviceTb.Text;
            for (int i = 0; i < checkList.Count; i++)
            {
                if (checkList[i].Checked)
                {
                    hourList.Add(i + 1);
                }
            }

            #region 初始化 模拟器的参数
            avdList = new List<int>();
            positionXDic = new Dictionary<int, int>();
            positionSXDic = new Dictionary<int, int>();
            positionYDic = new Dictionary<int, int>();
            if (!tb1_x.Text.Trim().Equals("") && !tb1_y.Text.Trim().Equals("") && !tb1_sx.Text.Trim().Equals(""))
            {
                avdList.Add(1);
                positionXDic.Add(1, int.Parse(tb1_x.Text));
                positionSXDic.Add(1, int.Parse(tb1_sx.Text));
                positionYDic.Add(1, int.Parse(tb1_y.Text));
            }
            if (!tb2_x.Text.Trim().Equals("") && !tb2_y.Text.Trim().Equals("") && !tb2_sx.Text.Trim().Equals(""))
            {
                avdList.Add(2);
                positionXDic.Add(2, int.Parse(tb2_x.Text));
                positionSXDic.Add(2, int.Parse(tb2_sx.Text));
                positionYDic.Add(2, int.Parse(tb2_y.Text));
            }

            if (!tb3_x.Text.Trim().Equals("") && !tb3_y.Text.Trim().Equals("") && !tb3_sx.Text.Trim().Equals(""))
            {
                avdList.Add(3);
                positionXDic.Add(3, int.Parse(tb3_x.Text));
                positionSXDic.Add(3, int.Parse(tb3_sx.Text));
                positionYDic.Add(3, int.Parse(tb3_y.Text));
            }

            if (!tb4_x.Text.Trim().Equals("") && !tb4_y.Text.Trim().Equals("") && !tb4_sx.Text.Trim().Equals(""))
            {
                avdList.Add(4);
                positionXDic.Add(4, int.Parse(tb4_x.Text));
                positionSXDic.Add(4, int.Parse(tb4_sx.Text));
                positionYDic.Add(4, int.Parse(tb4_y.Text));
            }

            #endregion

            if (runThread != null)
            {
                runThread.Start();
            }


        }

        public void run()
        {

            startBtn.Enabled = false;

            while (true)
            {

                Point ms = Control.MousePosition;
                postionLabel.Text = "x:" + ms.X + " y:" + ms.Y;

                timeLabel.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute;

                countLabel.Text = runTimes + "";

                if (!isStop)
                {

                    //判断时间间隔
                    if (getTimeInterval(beginHour, beginMin, beginSec, DateTime.Now) >= shuaInterval || !isToday(beginDay, DateTime.Now))
                    {

                        //TODO 修改本模拟器的参数
                        LogUtil.LogMessage(log, "脚本执行时间到 开始修改参数： " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        beginDay = DateTime.Now.Day;
                        beginHour = -1;
                        beginMin = -1;
                        beginSec = -1;

                        //获取安装的包 并清除数据
                        
                        LogUtil.LogMessage(log, "修改机型信息。。");
                        startChangeProp(mCurrentAvdNum);

                        #region 卸载掉安装包
                        //LogUtil.LogMessage(log, "脚本执行时间到 开始卸载： " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        //beginHour = -1;
                        //beginMin = -1;
                        //beginSec = -1;
                        //if (pkgList != null && pkgList.Count > 0) {
                        //    if (pkgList.Count > uninstallSuccess)
                        //    {
                        //        startUninstall(pkgList[uninstallSuccess]);
                        //    }
                        //}
                        #endregion

                    }

                    if (getTimeInterval(installHour, installMin, installSec, DateTime.Now) >= installTime && !isStartInstall)
                    {
                        LogUtil.LogMessage(log, "******成功检测一次 没有启动安装*****");

                        //playMusic(tipMusicPath);
                        beginDay = DateTime.Now.Day;
                        beginHour = -1;
                        beginMin = -1;
                        beginSec = -1;
                        closeAvd(mCurrentScreen);
                        //callDeleteRandomFile();
                    }

                    if (getTimeInterval(changeNetHour, changeNetMin, changeNetSec, DateTime.Now) >= changeNetTime && isStartChangeNet) { 
                        //如果启动了联网， 且两分钟内没有结果
                        LogUtil.LogMessage(log, "******成功检测一次 联网超时的情况*****");

                        changeNetHour = -1;
                        changeNetMin = -1;
                        changeNetSec = -1;

                        beginDay = DateTime.Now.Day;
                        beginHour = -1;
                        beginMin = -1;
                        beginSec = -1;
                        closeAvd(mCurrentScreen);
                    }


                    isShowWindow();
                    isShowElse();

                    int val = Util.getProcessStaus(PROCESS_NAME);
                    if (val == Util.PROCESS_NO_START)
                    {
                        if (!isLaunching)
                        {
                            isConnecting = false;
                            isConnected = false;
                            isLaunching = true;
                            LogUtil.LogMessage(log, "-----start launch----------");

                            isShowNotRead();
                            closeAdb();

                            if (avdList != null && avdList.Count > 0)
                            {
                                //每次随机一个 sdk版本
                                int radmom = getRadmomVal(avdList.Count);
                                mCurrentAvdNum = avdList[radmom];
                               // mCurrentAvdNum = 1;
                                mTapX = positionXDic[mCurrentAvdNum];
                                mTapSX = positionSXDic[mCurrentAvdNum];
                                mTapY = positionYDic[mCurrentAvdNum];

                                string avdPropPath = basePath + Constant.Folders.AVD_PROPERTY_FOLDER_NAME + "\\" + mCurrentAvdNum + ".txt";
                                runAvdPropPath.Text = avdPropPath;
                                Dictionary<string, string> runAvdDic = readProperty(avdPropPath);
                                if (runAvdDic != null && runAvdDic.Count != 0)
                                {
                                  
                                    runAvdModel.Text = runAvdDic["ro.product.model"];
                                    runAvdSdk.Text = runAvdDic["sdk_version"];
                                    string screen_x_y = runAvdDic["screen_x_y"];
                                    runAvdScreen.Text = screen_x_y;
                                    mCurrentScreen = screen_x_y;

                                    imeiLabel.Text = getImei(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\temp\\init.androVM.sh");

                                    string[] temp = screen_x_y.Split(new char[] { 'x' });
                                    if (temp != null && temp.Length == 2)
                                    {
                                        avdSetting(mTapSX, mTapY, temp[0], temp[1]);
                                    }
                                }
                                
                                

                                //启动模拟器
                                launchHour = DateTime.Now.Hour;
                                launchMin = DateTime.Now.Minute;
                                launchSec = DateTime.Now.Second;

                                if (!isVpnConnect)
                                {
                                    LogUtil.LogMessage(log, "-----changeNet----------");
                                    isVpnConnect = true;
                                    changeNet();
                                }
                                //else
                                //{
                                //    LogUtil.LogMessage(log, "-----closeNet  changeNet----------");
                                //    closeNet();
                                //    changeNet();
                                //}
                            }

                        }
                    }
                    else if (val == Util.PROCESS_RUNNING)
                    {

                        isLaunching = false;


                        if (!isConnecting && !isConnected)
                        {
                            isConnecting = true;
                            LogUtil.LogMessage(log, "-----start connect----------");
                            Util.getDevices(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DEVICES_BAT_NAME, this);
                        }
                    }
                    else
                    {

                        if (getTimeInterval(launchHour, launchMin, launchSec, DateTime.Now) > responseTime)
                        {

                            //launchHour = -1;
                            //launchMin = -1;
                            //launchSec = -1;

                            //isLaunching = false;
                            LogUtil.LogMessage(log, "-----三分钟未启动成功！！！！----------");
                            //try
                            //{
                            //    Util.closeProcess(PROCESS_NAME);
                            //    Util.closeProcess(VBOX_HEAD_NAME);
                            //    Util.closeProcess(VBOX_NET_NAME);
                            //}
                            //catch (Exception e)
                            //{

                            //}


                        }
                    }
                }
            }
        }

        private string getImei(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "";
            }
            StreamReader sr = new StreamReader(filePath);
            string str = "";
            while ((str = sr.ReadLine()) != null)
            {
                //  MessageBox.Show(str);
                if (str.Contains("genyd.device.id"))
                {
                    str = str.Replace("setprop genyd.device.id", "").Replace("\"", "").Trim();
                    return str;
                }
            }
            sr.Close();
            return str;
        }

        // 判断有没有启动成功
        private bool launch(int x, int y)
        {
            LogUtil.LogMessage(log, "yan chi 10 ... Tet");
            Thread.Sleep(15000);
            launchAvd(x, y);
            if (!isShowWindow())
            {
                //如果没有展示 启动成功
                return true;
            }
            else
            {
                //展示了 等10s之后再启动
                Thread.Sleep(10000);
                return launch(x, y);
            }
        }

        // 改变ip 并统计
        // 改变ip 并统计
        private void changeNet()
        {
            LogUtil.LogMessage(log, "-----changeNet----------" + "\n " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            isStartChangeNet = true;
            changeNetHour = DateTime.Now.Hour;
            changeNetMin = DateTime.Now.Minute;
            changeNetSec = DateTime.Now.Second;
            Util.callConnectVpn(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.VPN_C_BAT_NAME, "VPN", "b160", "222", this);
        }

        private void closeNet()
        {
            LogUtil.LogMessage(log, "-----closeNet----------");
            Util.callDisconnectVpn(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.VPN_D_BAT_NAME, "VPN", this);
        }

        //开刷
        private void startShua(string screen)
        {

            LogUtil.LogMessage(log, "获取安装的包 并清除数据");
            getInstall();
            if (startJiaoben(screen))
            {
                LogUtil.LogMessage(log, "启动脚本成功  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                // 开始计时 大概 3- 5分钟 后进入下一个循环
                beginDay = DateTime.Now.Day;
                beginHour = DateTime.Now.Hour;
                beginMin = DateTime.Now.Minute;
                beginSec = DateTime.Now.Second;

               
                // 判断是否联网成功  如果成功否则结束avd
                // 
                //Thread.Sleep(25000);
                //LogUtil.LogMessage(log, "延迟 25s 来检测是否联网成功！call pull file ");
                //callPullTimeFile();

                LogUtil.LogMessage(log, "测试不检测是否联网");
            }
            else
            {
                LogUtil.LogMessage(log, "启动脚本失败");
                try
                {
                    Util.closeProcess(PROCESS_NAME);
                    Util.closeProcess(VBOX_HEAD_NAME);
                  //  Util.closeProcess(VBOX_NET_NAME);
                 //   Util.closeProcess(VBOX_SVC_NAME);
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// 获取 当前avd 安装的应用
        /// </summary>
        private void getInstall()
        {
            Util.callPMInstall(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PM_INSTALL_BAT_NAME, mDevice, this);
        }

        static Semaphore semaphore = new Semaphore(1, 1); //同时只允许一线程
        private void clearData(string pkg)
        {

            //string pmBatPath = basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PM_BAT_NAME;
            string pmTxtPath = basePath + Constant.Folders.BAT_FOLDER_NAME + "\\temp\\pm.txt";


            string content = "pm clear " + pkg + "\nexit\n";

            if (!File.Exists(pmTxtPath))
            {
                FileStream stream = File.Create(pmTxtPath);
                stream.Close();
            }

            StreamWriter writer = new StreamWriter(pmTxtPath);
            writer.Write(content);
            writer.Flush();
            writer.Close();
            //清除一个应用的数据
            semaphore.WaitOne();
            Util.callPMClear(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PM_CLEAR_BAT_NAME, mDevice, this);

        }

        private void callPullTimeFile()
        {
            LogUtil.LogMessage(log, "callPullTimeFile");
            Util.callPullTimeFile(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PULL_TIME_BAT_NAME, basePath + Constant.Folders.TEMP_FOLDER_NAME, this);
        }

        private void getPullTimeContent()
        {
            string path = basePath + Constant.Folders.TEMP_FOLDER_NAME + "\\time.txt";
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                String content = reader.ReadToEnd();
                reader.Close();

                if (!content.Trim().Equals(""))
                {
                    //启动成功
                    LogUtil.LogMessage(log, "脚本启动成功 并且联网成功 :" + content);
                }
                else
                {
                    //脚本启动成功  但是联网失败
                    LogUtil.LogMessage(log, "脚本启动成功 并且联网失败");
                    callDeleteRandomFile();
                    //closeAvd(mCurrentScreen);
                }
                try
                {
                    File.Delete(path);
                    Util.callDeleteTimeFile(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DETETE_TIME_BAT_NAME, this);
                }
                catch (Exception e)
                {

                }


            }
            else
            {
                //关闭模拟器
                LogUtil.LogMessage(log, "脚本启动失败");
                callDeleteRandomFile();
                //closeAvd(mCurrentScreen);
            }


        }

        /// <summary>
        /// pull random file
        /// </summary>
        private void callPullRandomFile() {
            LogUtil.LogMessage(log, "callPullRandomFile 时间间隔3s");
            Thread.Sleep(3000);
            Util.callPullRandomFile(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PULL_RANDOM_BAT_NAME, basePath + Constant.Folders.TEMP_FOLDER_NAME, this);
        }

        private void getPullRandomContent() {
            string path = basePath + Constant.Folders.TEMP_FOLDER_NAME + "\\random";
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                String content = reader.ReadToEnd();
                reader.Close();
                xprivacyLabel.Text = content;
            }
        }

        private void callDeleteRandomFile() {
            Util.callDeleteRandomFile(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DETETE_RANDOM_BAT_NAME, this);
        }

        /// <summary>
        /// 准备修改当前avd 的参数
        /// </summary>
        /// <param name="currentAvdNum"></param>
        private void startChangeProp(int currentAvdNum)
        {
            //随机一个机型更改本机的参数
            string propPath = "";
            if (modelDic != null && modelDic.Count > 0)
            {
                int radom = getRadmomVal(modelDic.Count - 1);
                List<string> keyList = new List<string>(modelDic.Keys);
                if (keyList.Count > radom)
                {
                    string key = keyList[radom];
                    string value = modelDic[key];

                    //创建一个prop
                    propPath = createProp(value, currentAvdNum);
                }
            }

            Util.callProp(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PROP_BAT_NAME, mDevice, propPath, this);

        }




        public void onProcessOver(int type)
        {

            switch (type)
            {
                case Constant.ProcessType.TYPE_OF_PROCESS_GET_DEVICES:
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;

                            }
                        }

                        LogUtil.LogMessage(log, content);

                        if (content.Contains("192"))
                        {
                            mDevice = content.Substring(content.LastIndexOf("192"), content.LastIndexOf("5555") - content.LastIndexOf("192") + 4);
                            deviceTb.Text = mDevice;
                        }

                        if (content.Contains("169"))
                        {
                            mDevice = content.Substring(content.LastIndexOf("169"), content.LastIndexOf("5555") - content.LastIndexOf("169") + 4);
                            deviceTb.Text = mDevice;
                        }

                        if ((content.Contains("192") || content.Contains("169")) && !content.Contains("offline"))
                        {
                            isConnecting = false;
                            isConnected = true;

                            LogUtil.LogMessage(log, "Device Connect 开始安装测试包");
                            #region install
                            startInstall(testApkPath);
                            //installAllApks(runTimes, apkFolderList);
                            #endregion
                        }
                        else
                        {
                            LogUtil.LogMessage(log, "Device Not Connect");
                            // connect
                            if (isConnecting)
                            {
                                LogUtil.LogMessage(log, "----延迟2s callConnect----");
                                Thread.Sleep(2000);
                                Util.callConnect(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.CONNECT_BAT_NAME, mDevice, this);
                            }
                        }
                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_START_CONNECT:


                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        if ((content.Contains("192") || content.Contains("169")) && !content.Contains("offline") && !content.Contains("unable"))
                        {
                            isConnecting = false;
                            isConnected = true;

                            LogUtil.LogMessage(log, "Device Connect 开始安装测试包");
                            #region install
                            startInstall(testApkPath);
                            //installAllApks(runTimes, apkFolderList);
                            #endregion

                        }
                        else
                        {
                            LogUtil.LogMessage(log, "Device Not Connect");
                            // getDevices
                            if (isConnecting)
                            {
                                LogUtil.LogMessage(log, "----延迟2s getDevices----");
                                Thread.Sleep(2000);
                                Util.getDevices(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DEVICES_BAT_NAME, this);
                            }
                        }
                    }
                    break;
                #region
                case Constant.ProcessType.TYPE_OF_PROCESS_START_INSTALL:
                    // semaphore.Release();

                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        LogUtil.LogMessage(log, content);
                        if (content.ToLower().Contains("failure") || content.ToLower().Contains("error"))
                        {
                            isStartInstall = true;
                            installHour = -1;
                            installMin = -1;
                            installSec = -1;
                            //失败重新安装 直到成功 
                            LogUtil.LogMessage(log, "安装失败");
                            startInstall(testApkPath);

                        }
                        else if (content.ToLower().Contains("success"))
                        {
                            isStartInstall = true;
                            installHour = -1;
                            installMin = -1;
                            installSec = -1;
                            LogUtil.LogMessage(log, "--等待-- 启动脚本");

                            if (isAvdFirstRun)
                            {
                                LogUtil.LogMessage(log, "第一次 直接修改配置 下次启动");
                                //第一次 直接修改配置 下次启动
                                startChangeProp(mCurrentAvdNum);
                            }
                            else
                            {

                                // 检测 Xprivacy 是否生成随机值， 如果成功说明 也可以启动脚本了
                                callPullRandomFile();

                            }
                        }
                        else
                        {

                            LogUtil.LogMessage(log, "-----------未知------");

                            if (content.Trim().Equals(""))
                            {
                                LogUtil.LogMessage(log, "-----------log is empty---------");
                            }
                            else
                            {
                                LogUtil.LogMessage(log, "-----------log is" + content + "---------");
                            }

                            //closeAvd(mCurrentScreen);
                        }
                    }

                    break;
                //case Constant.ProcessType.TYPE_OF_PROCESS_UNINSTALL_APP:
                //    semaphore.Release();
                //    if (Util.myThread != null)
                //    {
                //        List<String> logList = Util.myThread.getLog();
                //        String content = "";
                //        if (logList != null && logList.Count > 0)
                //        {
                //            foreach (String logs in logList)
                //            {
                //                content = content + logs;
                //            }
                //        }

                //        if (content.ToLower().Contains("success")) {
                //            uninstallSuccess = uninstallSuccess + 1;

                //            LogUtil.LogMessage(log, "UnInstall Success---> " + pkgList.Count + ":" + uninstallSuccess);

                //            if (uninstallSuccess == pkgList.Count && pkgList.Count != 0)
                //            {
                //                //卸载完全 清除数据

                //                uninstallSuccess = 0;
                //                pkgList.Clear();

                //                LogUtil.LogMessage(log, "UnInstall All Apks \n 修改本机参数");
                //                // 无论是重启 还是 重新启动一个新的模拟器都要 修改掉本机的参数
                //                //随机一个机型更改本机的参数


                //                string propPath = "";
                //                if (modelDic != null && modelDic.Count > 0)
                //                {
                //                    int radom = getRadmomVal(modelDic.Count - 1);
                //                    List<string> keyList = new List<string>(modelDic.Keys);
                //                    if (keyList.Count > radom)
                //                    {
                //                        string key = keyList[radom];
                //                        string value = modelDic[key];

                //                        modelLabel.Text = value;

                //                        //创建一个prop
                //                        propPath = createProp(value, mCurSdkVersion);
                //                    }
                //                }

                //                Util.callProp(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PROP_BAT_NAME, mDevice, propPath, this);

                //            }
                //            else {
                //                if (pkgList.Count > uninstallSuccess)
                //                {
                //                    startUninstall(pkgList[uninstallSuccess]);
                //                }
                //            }
                //        }
                //    }
                //    break;
                #endregion
                case Constant.ProcessType.TYPE_OF_PROCESS_CHANGE_PROP:

                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        if (content.Contains("fail"))
                        {
                            LogUtil.LogMessage(log, content);
                        }
                        else
                        {
                            LogUtil.LogMessage(log, "-----change prop success ！ 准备删除 Xprivacy 生成的随机值！----------");

                            callDeleteRandomFile();

                        }
                    }
                    break;

                case Constant.ProcessType.TYPE_OF_PROCESS_START_REBOOT:

                    break;

                #region vpn 连接
                case Constant.ProcessType.TYPE_OF_PROCESS_CONNECT_VPN:

                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }


                        LogUtil.LogMessage(log, content);
                        isStartChangeNet = false;
                        changeNetHour = -1;
                        changeNetMin = -1;
                        changeNetSec = -1;

                        if (content.Contains("已连接") || content.Contains("已经连接"))
                        {
                            LogUtil.LogMessage(log, "-----vpn connect----------");

                   
                            isVpnConnect = true;
                            LogUtil.LogMessage(log, "-----launch avd----------");
                            launch(mTapX, mTapY);

                            //Dictionary<string, string> ipDic = Util.getIpInfo();
                            //if (ipDic != null && ipDic.Count > 0)
                            //{

                            //    string ip = ipDic["ip"];
                            //    if (!ip.Trim().Contains("60.166."))
                            //    {
                            //        string location = ipDic["location"];
                            //        ipLabel.Text = ip + "  " + location;
                            //        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            //        saveIp(ip, location, time);
                            //        isVpnConnect = true;
                            //        LogUtil.LogMessage(log, "-----launch avd----------");
                            //        launch(mTapX, mTapY);
                            //    }
                            //    else
                            //    {
                            //        LogUtil.LogMessage(log, "-----get Ip info failed----------");
                            //        isVpnConnect = false;
                            //        isLaunching = false;
                            //        closeNet();

                            //        //isShowWindow();
                            //    }
                            //}
                            //else
                            //{
                            //    LogUtil.LogMessage(log, "-----get Ip info failed----------");
                            //    isVpnConnect = false;
                            //    isLaunching = false;
                            //    closeNet();

                            //    //isShowWindow();
                            //}
                        }
                        else
                        {
                            LogUtil.LogMessage(log, "-----vpn connect failed----------");
                            isVpnConnect = false;
                            isLaunching = false;
                            closeNet();

                            // isShowWindow();
                            //closeNet();
                            //changeNet();
                        }

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_DISCONNECT_VPN:
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }


                        //if (content.Contains("已连接"))
                        //{
                        //    MessageBox.Show("Success!");
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Failed!");
                        //}

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_PULL_TIME:
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        LogUtil.LogMessage(log, "Pull:" + content);

                        if (content.Contains("not exist"))
                        {
                            //说明脚本没有启动成功
                            //关闭模拟器重新启动
                            LogUtil.LogMessage(log, "脚本启动失败或者联网失败的！");
                            callDeleteRandomFile();
                            //closeAvd(mCurrentScreen);
                        }
                        else
                        {
                            getPullTimeContent();
                        }

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_PULL_RANDOM:
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        LogUtil.LogMessage(log, "Pull:" + content);

                        if (content.Contains("not exist"))
                        {
                            //说明Xprivacy 没有生成随机值，脚本暂时也不能启动
                            //继续等待
                            LogUtil.LogMessage(log, "Xprivacy还没生成随机值！");
                            callPullRandomFile();
                        }
                        else
                        {
                            // 说明Xprivacy生成了随机值，脚本也可以启动了
                            //开刷
                            LogUtil.LogMessage(log, "Xprivacy生成随机值！");
                            getPullRandomContent();
                            LogUtil.LogMessage(log, "延迟 5s 开刷");
                            Thread.Sleep(5000);
                            startShua(mCurrentScreen);
                           
                        }

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_DELETE_TIME:
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        LogUtil.LogMessage(log, "Delete:" + content);

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_DELETE_RANDOM:
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        LogUtil.LogMessage(log, "Delete:" + content);
                        //主动关闭模拟器
                        if (isAvdFirstRun)
                        {
                            try
                            {
                                LogUtil.LogMessage(log, "close vpn");
                                isVpnConnect = false;
                                isLaunching = false;
                                closeNet();
                                ipLabel.Text = "";

                                Util.closeProcess(PROCESS_NAME);
                                Util.closeProcess(VBOX_HEAD_NAME);
                                // Util.closeProcess(VBOX_NET_NAME);
                                // Util.closeProcess(VBOX_SVC_NAME);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else
                        {
                            LogUtil.LogMessage(log, "延迟 2s ....");
                            Thread.Sleep(2000);
                            closeAvd(mCurrentScreen);
                        }
                        runTimes++;
                        mCurrentAvdNum = -1;

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_PM_CLEAR:
                    semaphore.Release();
                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }

                        LogUtil.LogMessage(log, "PM Clear:" + content);

                    }
                    break;
                case Constant.ProcessType.TYPE_OF_PROCESS_PM_INSTALL:

                    if (Util.myThread != null)
                    {
                        List<String> logList = Util.myThread.getLog();
                        String content = "";
                        if (logList != null && logList.Count > 0)
                        {
                            foreach (String logs in logList)
                            {
                                content = content + logs;
                            }
                        }
                        LogUtil.LogMessage(log, "PM Install:" + content);

                        if (pkgArray != null && pkgArray.Length > 0)
                        {
                            for (int i = 0; i < pkgArray.Length; i++)
                            {
                                if (content.Contains(pkgArray[i]))
                                {
                                    clearData(pkgArray[i]);
                                }
                            }
                        }

                    }
                    break;

                #endregion
            }
        }

        /// <summary>
        /// 保存每次更换的ip
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="location"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool saveIp(string ip, string location, string time)
        {
            string[] d = time.Split(new char[] { ' ' });
            if (d != null && d.Length == 2)
            {
                string dataPath = basePath + Constant.Folders.IPS_FOLDER_NAME + "\\" + d[0] + ".txt";
                if (!File.Exists(dataPath))
                {
                    FileStream stream = File.Create(dataPath);
                    stream.Close();
                }
                StreamWriter writer = new StreamWriter(dataPath, true);
                writer.Write(time + "  " + ip + "  " + location + "\n");
                writer.Flush();
                writer.Close();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 修改Prop 
        /// </summary>
        /// <param name="modelPath">model的文件路径</param>
        /// <param name="currentAvdNum">当前启动的avd</param>
        /// <returns></returns>
        private string createProp(String modelPath, int currentAvdNum)
        {

            string tempPath = basePath + Constant.Folders.TEMP_FOLDER_NAME;
            if (File.Exists(modelPath))
            {

                string modelName = modelPath.Substring(modelPath.LastIndexOf("\\") + 1);
                //temp文件夹存放各个model 的地方
                string modelFolder = tempPath + "\\" + modelName;

                if (Directory.Exists(modelFolder))
                {
                    if (saveAvdProperty(modelPath, currentAvdNum))
                    {
                        return modelFolder + "\\" + "build.prop";
                    }
                }
                else
                {
                    //在temp中创建机型
                    Directory.CreateDirectory(modelFolder);

                    Dictionary<string, string> propDic = readProperty(modelPath);
                    int sdk = int.Parse(propDic["sdk_version"]);
                    //在temp中创建
                    string sdkPath = basePath + Constant.Folders.SDK_FOLDER_NAME + "\\" + sdk + "\\" + "build.prop";
                    File.Copy(sdkPath, modelFolder + "\\" + "build.prop");
                    //并且修改好
                    if (changeProp(propDic, modelFolder + "\\" + "build.prop") && saveAvdProperty(modelPath, currentAvdNum))
                    {
                        return modelFolder + "\\" + "build.prop";
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 修改prop
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        /// <returns></returns>
        private bool changeProp(Dictionary<string, string> propDic, String toPath)
        {
            if (propDic != null && propDic.Count > 0 && File.Exists(toPath))
            {
                StreamReader reader = new StreamReader(toPath);
                String content = reader.ReadToEnd();
                reader.Close();
                foreach (var item in propDic)
                {
                    if (content.Contains(item.Key))
                    {
                        content = content.Replace(item.Key, item.Key + "=" + item.Value);
                    }
                }

                StreamWriter writer = new StreamWriter(toPath);
                writer.Write(content);
                writer.Flush();
                writer.Close();
                return true;

            }
            return false;
        }

        /// <summary>
        /// 保存下次avd信息
        /// </summary>
        /// <param name="currentAvdNum"></param>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        private bool saveAvdProperty(String modelPath, int currentAvdNum)
        {
            string propPath = basePath + Constant.Folders.AVD_PROPERTY_FOLDER_NAME + "\\" + currentAvdNum + ".txt";
            try
            {
                if (File.Exists(propPath))
                {
                    //删除后重新创建
                    File.Delete(propPath);
                    File.Copy(modelPath, propPath);
                    return true;
                }
                else
                {
                    //创建一个文件改写
                    File.Copy(modelPath, propPath);
                    return true;
                }
            }
            catch (Exception e)
            {

            }
            return false;
        }

        /// <summary>
        /// 读取一个property内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Dictionary<string, string> readProperty(string filePath)
        {
            Dictionary<string, string> contentDictionary = new Dictionary<string, string>();
            if (!File.Exists(filePath))
            {
                return contentDictionary;
            }
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream, Encoding.Default);
                fileStream.Seek(0, SeekOrigin.Begin);
                string content = streamReader.ReadLine();
                while (content != null)
                {
                    if (content.Contains("="))
                    {
                        string key = content.Substring(0, content.LastIndexOf("=")).Trim();
                        string value = content.Substring(content.LastIndexOf("=") + 1).Trim();
                        if (!contentDictionary.ContainsKey(key))
                        {
                            contentDictionary.Add(key, value);
                        }
                    }
                    content = streamReader.ReadLine();
                }
            }
            catch
            {
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
            return contentDictionary;
        }

        //获取一个随机值
        private int getRadmomVal(int max)
        {
            Random rd = new Random();
            return rd.Next(0, max);
        }

        //获取两个时间点的时间间隔
        private int getTimeInterval(int bH, int bM, int bS, DateTime e)
        {
            if (e != null)
            {
                if (bH <= 0 && bM <= 0 && bS <= 0)
                {
                    return -1;
                }
                return ((e.Hour - bH) * 60 + e.Minute - bM) * 60 + e.Second - bS;
            }
            return -1;
        }

        //判断是否同一天
        private bool isToday(int bD, DateTime e)
        {
            if (e != null)
            {
                if (bD != e.Day)
                {
                    return false;
                }
            }
            return true;
        }

        #region 模拟点击和输入

        //打开设置
        private bool avdSetting(int x, int y , string s_x, string s_y)
        {
            //通过模拟点击设置avd参数

            //打开
            Thread.Sleep(1000);
            doubleClick(x, y);
            Thread.Sleep(10000);
            //点击custom
            click(int.Parse(setting_custom_x.Text), int.Parse(setting_custom_y.Text));
            Thread.Sleep(2000);
            doubleClick(int.Parse(setting_X_x.Text), int.Parse(setting_X_y.Text));
            Thread.Sleep(2000);
            inputStr("genymotion", s_x);
            Thread.Sleep(2000);
            doubleClick(int.Parse(setting_Y_x.Text), int.Parse(setting_Y_y.Text));
            Thread.Sleep(2000);
            inputStr("genymotion", s_y);
            Thread.Sleep(2000);
            click(int.Parse(setting_dpi_x.Text), int.Parse(setting_dpi_y.Text));
            Thread.Sleep(2000);
            if (s_x.Trim().Equals("480"))
            {
                click(int.Parse(setting_240_x.Text), int.Parse(setting_240_y.Text));
            }
            else if (s_x.Trim().Equals("720") || s_x.Trim().Equals("768"))
            {
                click(int.Parse(setting_320_x.Text), int.Parse(setting_320_y.Text));
            }
            else if (s_x.Trim().Equals("1080"))
            {
                click(int.Parse(setting_480_x.Text), int.Parse(setting_480_y.Text));
            }
            Thread.Sleep(2000);
            click(int.Parse(setting_ok_x.Text), int.Parse(setting_ok_y.Text));
            Thread.Sleep(10000);
            
            
            //通过代码直接设置avd参数
            //String path = "C:\\Users\\Administrator\\VirtualBox VMs\\Google Nexus 4 - 4.1.1 - API 16 - 768x1280_11";
            //String vboxName = "Google Nexus 4 - 4.1.1 - API 16 - 768x1280_11.vbox";
            //String vbox_prevName = "Google Nexus 4 - 4.1.1 - API 16 - 768x1280_11.vbox-prev";

            //String vboxPath = path + "\\"+vboxName;
            //String vbox_prevPath = path + "\\" + vbox_prevName;

            //if (!File.Exists(vboxPath))
            //{
            //    MessageBox.Show("File Not Exist! " + "\n" + vboxPath);
            //}
            //else
            //{
            //    StreamReader reader = new StreamReader(vboxPath);
            //    String content = reader.ReadToEnd();
            //    content = content.Replace("xmlns=\"http://www.innotek.de/VirtualBox-settings\"", "");
            //    reader.Close();
            //    MessageBox.Show(content);

            //    StreamWriter writer = new StreamWriter(vboxPath);
            //    writer.Write(content);
            //    writer.Flush();
            //    writer.Close();

            //    try
            //    {
            //        XmlDocument vboxXml = new XmlDocument();
            //        vboxXml.Load(vboxPath);


            //        XmlNodeList nodeList = vboxXml.SelectNodes("VirtualBox/Machine/Hardware/GuestProperties/GuestProperty");
            //        foreach (XmlNode node in nodeList)
            //        {
            //            if (node.Attributes["name"].Value.Equals("vbox_dpi"))
            //            {
            //                if (s_x.Trim().Equals("480"))
            //                {
            //                    node.Attributes["value"].Value = "240";
            //                }
            //                else if (s_x.Trim().Equals("720") || s_x.Trim().Equals("768"))
            //                {
            //                    node.Attributes["value"].Value = "320";
            //                }
            //                else if (s_x.Trim().Equals("1080"))
            //                {
            //                    node.Attributes["value"].Value = "480";
            //                }
            //            }

            //            if (node.Attributes["name"].Value.Equals("vbox_graph_mode"))
            //            {
            //                node.Attributes["value"].Value = s_x + "x" + s_y + "-16";
            //            }
            //        }

            //    }
            //    catch (Exception e) {
            //        MessageBox.Show(e.Message);                
            //    }
                
               
            //}

            //if (!File.Exists(vbox_prevPath))
            //{
            //    MessageBox.Show("File Not Exist!" + "\n" + vbox_prevPath);
            //}
            //else
            //{
            //    XmlDocument vbox_prevXml = new XmlDocument();
            //    vbox_prevXml.Load(vbox_prevPath);
            //}

            






            return true;
        }

        //判断 弹出了 模拟器正在处理设置的窗口
        private bool isShowWindow()
        {
            Process[] ps = Process.GetProcessesByName("genymotion");
            if (ps != null && ps.Length > 0)
            {
                if (ps[0].MainWindowHandle != IntPtr.Zero)
                {
                    IntPtr errorHandle = FindWindow("QWidget", "Error");
                    if (errorHandle != IntPtr.Zero)
                    {
                        SendMessage(errorHandle, 0x10, 0, 0);
                    }

                    IntPtr versionHandle = FindWindow("QWidget", "Different versions");
                    if (versionHandle != IntPtr.Zero)
                    {
                        SendMessage(versionHandle, 0x10, 0, 0);
                    }

                    IntPtr vHandle = FindWindow("QWidget", "Virtual device configuration");
                    if (vHandle != IntPtr.Zero)
                    {
                        SendMessage(vHandle, 0x10, 0, 0);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 检测有没有异常窗口弹出
        /// </summary>
        private void isShowElse()
        {
            try
            {
                Process[] ps = Process.GetProcessesByName("WerFault");
                foreach (Process p in ps)
                {
                    p.Kill();
                }

            }
            catch (Exception e)
            {
                LogUtil.LogMessage(log, "isShowElse:" + e.Message);
            }
        }

        private void isShowNotRead()
        {
            try
            {
                Process[] ps = Process.GetProcessesByName("csrss");
                LogUtil.LogMessage(log, "csrss:" + ps.Length);

                IntPtr notReadHandle = FindWindow(null, "adb.exe - 应用程序错误");
                if (notReadHandle != IntPtr.Zero)
                {
                    LogUtil.LogMessage(log, "find not read  并准备关闭");
                    click(1050, 623);

                }
                else
                {
                    LogUtil.LogMessage(log, "not find not read");
                }

                IntPtr dummyHandle = FindWindow(null, "Dummy: player.exe - 应用程序错误");
                if (dummyHandle != IntPtr.Zero)
                {
                    LogUtil.LogMessage(log, "find dummy  并准备关闭");
                    click(1050, 623);

                }
                else
                {
                    LogUtil.LogMessage(log, "not find dummy");
                }

            }
            catch (Exception e)
            {
                LogUtil.LogMessage(log, "isShowNotRead:" + e.Message);
            }
        }

        /// <summary>
        /// 关闭adb
        /// </summary>
        private void closeAdb()
        {
            LogUtil.LogMessage(log, "callCloseAdb");
            try
            {
                Process[] ps = Process.GetProcessesByName("adb");
                foreach (Process p in ps)
                {
                    p.Kill();
                }

                Util.callCloseAdb(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.ADB_CLOSE_BAT_NAME, this);

            }
            catch (Exception e)
            {
                LogUtil.LogMessage(log, "isShowElse:" + e.Message);
            }


        }

        //启动avd
        private void launchAvd(int x, int y)
        {
            doubleClick(x, y);
        }

        //关闭avd
        private void closeAvd(string screen)
        {

            LogUtil.LogMessage(log, "closeAvd");

            beginHour = -1;
            beginMin = -1;
            beginSec = -1;

            changeNetHour = -1;
            changeNetMin = -1;
            changeNetSec = -1;

            installHour = -1;
            installMin = -1;
            installSec = -1;

            isLaunching = false;
            isVpnConnect = false;
            closeNet();
            ipLabel.Text = "";

            switch (screen)
            {
                case Constant.Screen.SCREEN_480x800:
                    click(1233, 129);
                    break;
                case Constant.Screen.SCREEN_480x854:
                    click(1236, 101);
                    break;
                case Constant.Screen.SCREEN_720x1280:
                    click(1267, 11);
                    break;
                case Constant.Screen.SCREEN_768x1280:
                    click(1283, 10);
                    break;
                case Constant.Screen.SCREEN_1080x1920:
                    click(1266, 10);
                    break;
                default:
                    return;
            }

            try
            {
                Util.closeProcess(PROCESS_NAME);
                Util.closeProcess(VBOX_HEAD_NAME);
                // Util.closeProcess(VBOX_NET_NAME);
               // Util.closeProcess(VBOX_SVC_NAME);


            }
            catch (Exception e)
            {

            }


        }

        //启动脚本
        private bool startJiaoben(string screen)
        {
            try
            {
                switch (screen)
                {
                    case Constant.Screen.SCREEN_480x800:
                        // 两次Home
                        click(1233, 911);
                        click(1233, 911);
                        Thread.Sleep(2000);
                        //icon
                        click(788, 314);
                        Thread.Sleep(8000);
                        //脚本
                        click(963, 723);
                        Thread.Sleep(2000);
                        //手机
                        click(854, 394);
                        Thread.Sleep(1000);
                        //加载
                        click(909, 475);
                        Thread.Sleep(2000);
                        //启动
                        click(1020, 563);
                        return true;
                    case Constant.Screen.SCREEN_480x854:
                        // 两次Home
                        click(1233, 902);
                        click(1233, 902);
                        Thread.Sleep(2000);
                        //icon
                        click(788, 314);
                        Thread.Sleep(8000);
                        //脚本
                        click(963, 723);
                        Thread.Sleep(2000);
                        //手机
                        click(854, 370);
                        Thread.Sleep(1000);
                        //加载1
                        click(909, 448);
                        Thread.Sleep(2000);
                        //启动
                        click(1020, 555);
                        return true;
                    case Constant.Screen.SCREEN_720x1280:
                        // 两次Home
                        click(1265, 956);
                        click(1265, 956);
                        Thread.Sleep(2000);
                        //icon
                        click(768, 268);
                        Thread.Sleep(8000);
                        //脚本
                        click(924, 790);
                        Thread.Sleep(2000);
                        //手机
                        click(822, 278);
                        Thread.Sleep(1000);
                        //加载
                        click(887, 367);
                        Thread.Sleep(2000);
                        //启动
                        click(1047, 525);
                        return true;
                    case Constant.Screen.SCREEN_768x1280:
                        // 两次Home
                        click(1282, 956);
                        click(1282, 956);
                        Thread.Sleep(2000);
                        //icon
                        click(770, 272);
                        Thread.Sleep(8000);
                        //脚本
                        click(907, 795);
                        Thread.Sleep(2000);
                        //手机
                        click(822, 285);
                        Thread.Sleep(1000);
                        //加载
                        click(883, 367);
                        Thread.Sleep(2000);
                        //启动
                        click(1067, 525);
                        return true;
                    case Constant.Screen.SCREEN_1080x1920:
                        // 两次Home
                        click(1264, 958);
                        click(1264, 958);
                        Thread.Sleep(2000);
                        //icon
                        click(767, 274);
                        Thread.Sleep(8000);
                        //脚本
                        click(940, 795);
                        Thread.Sleep(2000);
                        //手机
                        click(822, 285);
                        Thread.Sleep(1000);
                        //加载
                        click(889, 365);
                        Thread.Sleep(2000);
                        //启动
                        click(1045, 525);
                        return true;
                    default:
                        return false;
                }


            }
            catch (Exception e)
            {

            }
            return false;
        }

        #endregion

        #region
        //双击
        private void doubleClick(int x, int y)
        {
            LogUtil.LogMessage(log, "双：" + x + "  " + y);
            SetCursorPos(x, y);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        //单击
        private void click(int x, int y)
        {
            LogUtil.LogMessage(log, "单：" + x + "  " + y);
            SetCursorPos(x, y);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags,
            int dx, int dy, uint data, UIntPtr extraInfo);

        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }
        #endregion

        #region 模拟输入

        private void inputStr(String processName, String text)
        {
            Process[] pro = Process.GetProcessesByName(processName);
            if (pro != null && pro.Length > 0)
            {
                if (pro[0].MainWindowHandle != IntPtr.Zero)
                {
                    IntPtr vHandle = FindWindow("QWidget", "Configure virtual device");
                    if (vHandle != IntPtr.Zero)
                    {
                        if (!text.Trim().Equals(""))
                        {
                            char[] t = text.ToCharArray();
                            foreach (char c in t)
                            {
                                SendMessage(vHandle, 0x0100, (int)c, 0);
                                SendMessage(vHandle, 0x0101, (int)c, 0);
                            }
                        }
                    }
                    //else {
                    //    MessageBox.Show("没找到子窗口");
                    //}

                }
                //else {
                //    MessageBox.Show("没找到父窗口");
                //}
            }

        }

        [DllImport("User32.DLL")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int param, int para);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.DLL")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        public const uint WM_SETTEXT = 0x000C;

        private void moveGenymotionWin()
        {
            Process[] ps = Process.GetProcessesByName("genymotion");
            if (ps != null && ps.Length > 0)
            {
                if (ps[0].MainWindowHandle != IntPtr.Zero)
                {
                    IntPtr genymotionHandle = FindWindow("QWidget", "Genymotion for personal use");
                    if (genymotionHandle != IntPtr.Zero)
                    {
                        RECT rect = new RECT();
                        GetWindowRect(genymotionHandle, ref rect);
                        //找到窗体
                        MoveWindow(genymotionHandle, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, false);
                    }

                }
            }
        }

        [DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        #endregion


        private void playMusic(string path)
        {
            SoundPlayer sp = new SoundPlayer();
            sp.SoundLocation = path;
            sp.PlayLooping();
        }

       

      

    }
}
