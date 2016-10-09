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

        //所有配置文件的根路径
        private static String mBasePath = "";

        //测试apk 用来检测是否启动完成
        private static string mTestApkPath = "";
        //发出提示的音乐
        private static String mTipMusicPath = "";


        //当前模拟器的 设备号和端口
        private static String mDevice = "";
        
        //Genymotion启动模拟器的进程
        private static String PROCESS_NAME = "player";
        private static String VBOX_HEAD_NAME = "VBoxHeadless";
        private static String VBOX_NET_NAME = "VBoxNetDHCP";
        private static String VBOX_SVC_NAME = "VBoxSVC";


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

        //循环执行的线程
        private Thread runThread = null;

        //当前设备连接的状态
        private static bool isLaunching = false;//模拟器是否正在启动
        private static bool isConnecting = false;//模拟器否正在连接
        private static bool isConnected = false;//模拟器是否已经连接

        //循环执行次数
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

        //每次循环刷的时间间隔
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

        //默认程序循环是停止的
        private static bool isStop = true;
        //判断vpn是否连接
        private static bool isVpnConnect = false;

        // 需要清除缓存 的包
        private static string[] pkgArray = new string[] { "com.sevenseven.android.ppt", "com.sevenseven.caculator", "com.sevenseven.bill","me.sevenseven.sote","com.sevenseven.account"
        ,  "com.seven.mypurse","com.android.seven.cargas","com.sevensevenlittle","com.seven.cashbook"
        , "com.wjjnote","com.wjj.rabbit","com.Green_light_seven","com.wjj.electric","com.wjj.grad","com.wjj.loop","com.jjtax"
        ,"com.wjjwjj.account","com.wjjwjj.house","com.wjjwjj.moneyspell","me.wjjwjj.sote","com.wjjwjj.android.ppt"};



        //每次安装的时间  如果两分钟内没有安装成功 关闭
        private static int installHour = -1;
        private static int installMin = -1;
        private static int installSec = -1;
        private static int installTime = 120;
        //判断是否 已经开始安装
        private static bool isStartInstall = false;

        

        //每次启动联网的时间  如果两分钟内没有启动联网 关闭
        private static int changeNetHour = -1;
        private static int changeNetMin = -1;
        private static int changeNetSec = -1;
        private static int changeNetTime = 120;
        //判断是否 已经开始连接vpn
        private static bool isStartChangeNet = false;


        public What()
        {
            InitializeComponent();

            What.CheckForIllegalCrossThreadCalls = false;

            
            mBasePath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1);

            mTestApkPath = mBasePath + Constant.Folders.APK_FOLDER_NAME + "\\test.apk";
            mTipMusicPath = mBasePath + Constant.Folders.MUSIC_FOLDER_NAME + "\\tip.wav";

            initModelList();

            //当前时间
            timeLabel.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            //当前鼠标位置
            Point ms = Control.MousePosition;
            postionLabel.Text = "x:" + ms.X + " y:" + ms.Y;

            //第一个模拟器y坐标
            int first = 400;
            //每一个模拟器的高度间隔
            int interval = 40;

            //模拟器一
            tb1_x.Text = "200";
            tb1_y.Text = first.ToString();
            tb1_sx.Text = "683";
            
            //模拟器二
            tb2_x.Text = "200";
            tb2_y.Text = (first + interval* 1).ToString();
            tb2_sx.Text = "683";

            //模拟器三
            tb3_x.Text = "200";
            tb3_y.Text = (first + interval* 2).ToString();
            tb3_sx.Text = "683";

            //模拟器四
            tb4_x.Text = "200";
            tb4_y.Text = (first + interval * 3).ToString();
            tb4_sx.Text = "683";

            //Genymotion 设置模拟器参数页面各个点的坐标
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
        //初始化所有现有的模拟器参数文件
        private void initModelList()
        {
            modelDic = new Dictionary<string, string>();
            string baseFolderPath = mBasePath + Constant.Folders.MODEL_FOLDER_NAME;
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

        //初始化 Genymotion 上 模拟器坐标
        private void initAvdList() {
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
        }
        #endregion


        //停止按钮点击事件
        private void btnStop_Click(object sender, EventArgs e)
        { 

            beginDay = DateTime.Now.Day;
            isStop = !isStop;
        }

        //开始按钮点击事件
        private void startBtn_Click(object sender, EventArgs e)
        {

            //检测Genymotion 的窗口位置 并移动 指定位置   --后来去掉，因为直接设置窗体启动的位置
            //moveGenymotionWin();

            mDevice = deviceTb.Text;
           
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
                //鼠标位置
                Point ms = Control.MousePosition;
                postionLabel.Text = "x:" + ms.X + " y:" + ms.Y;
                //当前时间
                timeLabel.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                //循环次数
                countLabel.Text = runTimes + "";

                if (!isStop)
                {

                    //判断时间间隔  如果超过刷的时间， 修改模拟器参数后 关闭模拟器，进行下一次循环
                    if (getTimeInterval(beginHour, beginMin, beginSec, DateTime.Now) >= shuaInterval || !isToday(beginDay, DateTime.Now))
                    {

                       
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

                    //判断安装测试apk是否超时， 如果超时（说明模拟器启动有问题）关闭模拟器！进入下一次循环
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

                    //判断联网是否超时，     如果超时关闭模拟器！进入下一次循环
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
                   
                    //判断进程有没有启动
                    int val = Util.getProcessStaus(PROCESS_NAME);
                    if (val == Util.PROCESS_NO_START)
                    {
                        if (!isLaunching)
                        {
                            isConnecting = false;
                            isConnected = false;
                            //正在启动中
                            isLaunching = true;
                            LogUtil.LogMessage(log, "-----start launch----------");

                            isShowNotRead();
                            closeAdb();

                            if (avdList != null && avdList.Count > 0)
                            {
                                //随机选择一个模拟器    
                                int radmom = getRadmomVal(avdList.Count);
                                mCurrentAvdNum = avdList[radmom];
                                // mCurrentAvdNum = 1;   //指定选择某一个模拟器 just for test
                                // 该模拟器关键点坐标
                                mTapX = positionXDic[mCurrentAvdNum];
                                mTapSX = positionSXDic[mCurrentAvdNum];
                                mTapY = positionYDic[mCurrentAvdNum];

                                //该模拟器的机型参数     机型分辨率 需要在模拟器启动之前提前设置
                                string avdPropPath = mBasePath + Constant.Folders.AVD_PROPERTY_FOLDER_NAME + "\\" + mCurrentAvdNum + ".txt";
                                runAvdPropPath.Text = avdPropPath;
                                Dictionary<string, string> runAvdDic = readProperty(avdPropPath);
                                if (runAvdDic != null && runAvdDic.Count != 0)
                                {
                                    //把当前模拟器的参数显示出来
                                    //机型
                                    runAvdModel.Text = runAvdDic["ro.product.model"]; 
                                    //sdk版本
                                    runAvdSdk.Text = runAvdDic["sdk_version"];
                                    //屏幕分辨率
                                    string screen_x_y = runAvdDic["screen_x_y"];
                                    runAvdScreen.Text = screen_x_y;
                                    mCurrentScreen = screen_x_y;

                                    //获取imei
                                    imeiLabel.Text = getImei(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\temp\\init.androVM.sh");

                                    string[] temp = screen_x_y.Split(new char[] { 'x' });
                                    if (temp != null && temp.Length == 2)
                                    {   
                                        //设置待启动的模拟器   分辨率是启动之前设置
                                        avdSetting(mTapSX, mTapY, temp[0], temp[1]);
                                    }
                                }
                                
                                

                                //启动模拟器
                                launchHour = DateTime.Now.Hour;
                                launchMin = DateTime.Now.Minute;
                                launchSec = DateTime.Now.Second;

                                if (!isVpnConnect)
                                {
                                    //启动之前 先连接vpn
                                    LogUtil.LogMessage(log, "-----changeNet----------");
                                    isVpnConnect = true;
                                    //连接vpn  连接成功和失败都会有相应回调
                                    changeNet();
                                }
                            }

                        }
                    }
                    else if (val == Util.PROCESS_RUNNING)
                    {
                        //说明模拟器已经启动     正在启动设置为false
                        isLaunching = false;

                        //当模拟器 没有正在连接 也没有已经连接 才进入循环
                        if (!isConnecting && !isConnected)
                        {
                            isConnecting = true;
                            LogUtil.LogMessage(log, "-----start connect----------");
                            //准备 adb 连接模拟器
                            Util.getDevices(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DEVICES_BAT_NAME, this);
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

        /// <summary>
        /// 获取机型的imei
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 准备启动模拟器
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool launch(int x, int y)
        {
            LogUtil.LogMessage(log, "延迟 15s 后启动 模拟器");
            Thread.Sleep(15000);
            //真正点击模拟器
            launchAvd(x, y);
            //判断是否有 其他窗口遮挡， 如果遮挡 关闭之后重新
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


        /// <summary>
        /// 开始vpn连接， 并统计新的ip
        /// </summary>
        private void changeNet()
        {
            LogUtil.LogMessage(log, "-----changeNet----------" + "\n " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //标记开始 连接vpn
            isStartChangeNet = true;
            changeNetHour = DateTime.Now.Hour;
            changeNetMin = DateTime.Now.Minute;
            changeNetSec = DateTime.Now.Second;
            Util.callConnectVpn(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.VPN_C_BAT_NAME, "VPN", "b160", "222", this);
        }

        /// <summary>
        /// 断开vpn 连接
        /// </summary>
        private void closeNet()
        {
            LogUtil.LogMessage(log, "-----closeNet----------");
            Util.callDisconnectVpn(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.VPN_D_BAT_NAME, "VPN", this);
        }

        /// <summary>
        /// 开始刷
        /// </summary>
        /// <param name="screen">屏幕分辨率</param>
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

            }
            else
            {
                LogUtil.LogMessage(log, "启动脚本失败");
                try
                {
                    //关闭循环进程
                    Util.closeProcess(PROCESS_NAME);
                    Util.closeProcess(VBOX_HEAD_NAME);
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
            Util.callPMInstall(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PM_INSTALL_BAT_NAME, mDevice, this);
        }

        static Semaphore semaphore = new Semaphore(1, 1); //同时只允许一线程
        /// <summary>
        /// 清除模拟器 指定包名应用的数据
        /// </summary>
        /// <param name="pkg">包名</param>
        private void clearData(string pkg)
        {
            //包名经常更换，所以pm文件动态生成
            string pmTxtPath = mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\temp\\pm.txt";


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
            Util.callPMClear(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PM_CLEAR_BAT_NAME, mDevice, this);

        }

        /// <summary>
        /// 检测模拟器是否联网的。  这个后来去掉了， 正常启动都会联网
        /// </summary>
        private void callPullTimeFile()
        {
            LogUtil.LogMessage(log, "callPullTimeFile");
            Util.callPullTimeFile(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PULL_TIME_BAT_NAME, mBasePath + Constant.Folders.TEMP_FOLDER_NAME, this);
        }

        private void getPullTimeContent()
        {
            string path = mBasePath + Constant.Folders.TEMP_FOLDER_NAME + "\\time.txt";
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
                    Util.callDeleteTimeFile(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DETETE_TIME_BAT_NAME, this);
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
        /// 从模拟器中 pull出 Xprivacy生成的随机值
        /// </summary>
        private void callPullRandomFile() {
            LogUtil.LogMessage(log, "callPullRandomFile 时间间隔3s");
            Thread.Sleep(3000);
            Util.callPullRandomFile(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PULL_RANDOM_BAT_NAME, mBasePath + Constant.Folders.TEMP_FOLDER_NAME, this);
        }

        /// <summary>
        /// 获取Xprivacy生成的随机值内容
        /// </summary>
        private void getPullRandomContent() {
            string path = mBasePath + Constant.Folders.TEMP_FOLDER_NAME + "\\random";
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                String content = reader.ReadToEnd();
                reader.Close();
                xprivacyLabel.Text = content;
            }
        }

        /// <summary>
        /// 删除 Xprivacy生成随机值的文件
        /// </summary>
        private void callDeleteRandomFile() {
            Util.callDeleteRandomFile(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DETETE_RANDOM_BAT_NAME, this);
        }

        /// <summary>
        /// 准备修改当前模拟器 的机型参数  通过替换 build.prop文件。 一个循环执行结束的时候会修改机型参数，模拟器第一次启动也会修改机型参数
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

                    //创建一个新prop文件
                    propPath = createProp(value, currentAvdNum);
                }
            }
            //用新创建的prop文件 替换掉模拟器的prop文件。 
            Util.callProp(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.PROP_BAT_NAME, mDevice, propPath, this);

        }



        /// <summary>
        /// 所有回调
        /// </summary>
        /// <param name="type"></param>
        public void onProcessOver(int type)
        {

            switch (type)
            {
                #region vpn 连接. 成功 启动模拟器; 不成功 重新进入循环;
                case Constant.ProcessType.TYPE_OF_PROCESS_CONNECT_VPN:
                    //接收vpn 连接的回调
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
                        //标记 vpn连接 结束
                        isStartChangeNet = false;
                        changeNetHour = -1;
                        changeNetMin = -1;
                        changeNetSec = -1;

                        if (content.Contains("已连接") || content.Contains("已经连接"))
                        {
                            LogUtil.LogMessage(log, "-----vpn connect----------");

                            //标记 vpn连接成功
                            isVpnConnect = true;
                            LogUtil.LogMessage(log, "-----launch avd----------");
                            //准备启动模拟器
                            launch(mTapX, mTapY);

                            #region 保存ip
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
                            #endregion
                        }
                        else
                        {
                            LogUtil.LogMessage(log, "-----vpn connect failed----------");
                            //vpn连接失败 会重新进入循环
                            isVpnConnect = false;
                            isLaunching = false;
                            closeNet();
                        }

                    }
                    break;
                #endregion

                #region vpn 断开
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

                    }
                    break;
                #endregion

                #region 获取当前连接的设备. 如果已经连接  开始安装测试apk; 如果没有连接 开始连接;
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

                        // 192开头
                        if (content.Contains("192"))
                        {
                            mDevice = content.Substring(content.LastIndexOf("192"), content.LastIndexOf("5555") - content.LastIndexOf("192") + 4);
                            deviceTb.Text = mDevice;
                        }
                        
                        // 169开头
                        if (content.Contains("169"))
                        {
                            mDevice = content.Substring(content.LastIndexOf("169"), content.LastIndexOf("5555") - content.LastIndexOf("169") + 4);
                            deviceTb.Text = mDevice;
                        }

                        
                        if ((content.Contains("192") || content.Contains("169")) && !content.Contains("offline"))
                        {
                            //说明此时已经 adb 已经连接到了 模拟器
                            isConnecting = false;
                            isConnected = true;

                            LogUtil.LogMessage(log, "Device Connect 开始安装测试包");
                            //开始安装测试包 目的等待模拟器真正进入运行状态
                            startInstall(mTestApkPath);
                           
                        }
                        else
                        {
                            LogUtil.LogMessage(log, "Device Not Connect");
                            // 说明此时 adb 还没有连接到模拟器
                            if (isConnecting)
                            {
                                LogUtil.LogMessage(log, "----延迟2s callConnect----");
                                Thread.Sleep(2000);
                                Util.callConnect(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.CONNECT_BAT_NAME, mDevice, this);
                            }
                        }
                    }
                    break;
                #endregion

                #region 开始连接设备; 连接成功  开始安装测试apk; 连接失败  重新获取当前连接设备;
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
                            //说明此时已经 adb 已经连接到了 模拟器
                            isConnecting = false;
                            isConnected = true;

                            LogUtil.LogMessage(log, "Device Connect 开始安装测试包");
                            //开始安装测试包 目的等待模拟器真正进入运行状态
                            startInstall(mTestApkPath);

                        }
                        else
                        {
                            LogUtil.LogMessage(log, "Device Not Connect");
                            // 开始获取当前连接的设备
                            if (isConnecting)
                            {
                                LogUtil.LogMessage(log, "----延迟2s getDevices----");
                                Thread.Sleep(2000);
                                Util.getDevices(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.DEVICES_BAT_NAME, this);
                            }
                        }
                    }
                    break;
                #endregion

                #region 开始安装测试包
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
                            LogUtil.LogMessage(log, "安装失败");
                            //失败重新安装 直到成功
                            startInstall(mTestApkPath);

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

                #region 从模拟器pull出 Xprivacy生成的随机值文件
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
                            LogUtil.LogMessage(log, "Xprivacy还没生成随机值！");
                            //说明Xprivacy 没有生成随机值，脚本暂时也不能启动
                            //继续等待
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
                #endregion

                #region 获取当前模拟器安装的应用 并清除准备刷的应用的 数据
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
                                //循环遍历 清除要刷的应用的数据
                                if (content.Contains(pkgArray[i]))
                                {
                                    clearData(pkgArray[i]);
                                }
                            }
                        }

                    }
                    break;
                #endregion

                #region 清除准备要刷的应用的数据
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
                #endregion


                #region 修改机型参数
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
                #endregion

                #region 删除Xprivacy生成的随机值文件
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
                #endregion

                case Constant.ProcessType.TYPE_OF_PROCESS_START_REBOOT:

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
                string dataPath = mBasePath + Constant.Folders.IPS_FOLDER_NAME + "\\" + d[0] + ".txt";
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
        /// 根据sdk版本 去获取对应版本的build.prop模板
        /// </summary>
        /// <param name="modelPath">model的文件路径</param>
        /// <param name="currentAvdNum">当前启动的avd</param>
        /// <returns></returns>
        private string createProp(String modelPath, int currentAvdNum)
        {

            string tempPath = mBasePath + Constant.Folders.TEMP_FOLDER_NAME;
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
                    //根据sdk版本  获取对应版本的 模板文件复制到temp里 等待修改
                    string sdkPath = mBasePath + Constant.Folders.SDK_FOLDER_NAME + "\\" + sdk + "\\" + "build.prop";
                    File.Copy(sdkPath, modelFolder + "\\" + "build.prop");
                    //开始修改机型参数， 并保存起来方便后面用到的时候获取
                    if (changeProp(propDic, modelFolder + "\\" + "build.prop") && saveAvdProperty(modelPath, currentAvdNum))
                    {
                        return modelFolder + "\\" + "build.prop";
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 修改Temp中的模板 prop文件
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
        /// 保存已经修改的模拟器的相关参数。下次启动的时候 可以获取到。
        /// </summary>
        /// <param name="currentAvdNum"></param>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        private bool saveAvdProperty(String modelPath, int currentAvdNum)
        {
            string propPath = mBasePath + Constant.Folders.AVD_PROPERTY_FOLDER_NAME + "\\" + currentAvdNum + ".txt";
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

        /// <summary>
        /// 获取一个随机值
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
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

        //打开Genymotion设置页，并設置分辨率
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
            
            
            //TODO 通过代码直接设置avd参数
            return true;
        }

        /// <summary>
        /// 判断 是否弹出了 遮挡  程序执行的 窗口，有则关闭
        /// </summary>
        /// <returns></returns>
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

                    //模拟设置参数以后 需要一定时候才能启动
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
        /// 检测有没有异常窗口弹出，任何阻止程序进行 都要关闭掉
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

        /// <summary>
        /// 检测有没有异常窗口弹出, 任何阻止程序进行 都要关闭掉
        /// </summary>
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

                Util.callCloseAdb(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.ADB_CLOSE_BAT_NAME, this);

            }
            catch (Exception e)
            {
                LogUtil.LogMessage(log, "isShowElse:" + e.Message);
            }


        }

        /// <summary>
        /// 启动模拟器
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
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

        /// <summary>
        /// 启动模拟器里 按键精灵的脚本;   模拟器需要设置 No home bar
        /// </summary>
        /// <param name="screen">不同分辨率不同</param>
        /// <returns></returns>
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

        #region 模拟鼠标点击
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

        #region 模拟键盘输入

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

        //播放音乐
        private void playMusic(string path)
        {
            SoundPlayer sp = new SoundPlayer();
            sp.SoundLocation = path;
            sp.PlayLooping();
        }

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


        /// <summary>
        /// 安装测试apk 为了判断模拟器是不是真正运行起来
        /// </summary>
        /// <param name="apkPath">测试apk路径</param>
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
            Util.callInstall(mBasePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.INSTALL_BAT_NAME, mDevice, apkPath, this);
        }

        //private void startUninstall(String pkgName) {
        //    semaphore.WaitOne();
        //    LogUtil.LogMessage(log, "Devices:" + mDevice + "UnInstall:" + pkgName);
        //    Util.callUninstall(basePath + Constant.Folders.BAT_FOLDER_NAME + "\\" + Constant.Apktool.UNINSTALL_BAT_NAME, mDevice, pkgName, this);
        //}
        #endregion

      

    }
}
