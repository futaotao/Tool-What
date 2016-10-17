using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace What
{
    class Constant
    {

        public class Screen {

            public const string SCREEN_UNKNOWN = "unknown";

            public const string SCREEN_480x800 = "480x800";
            public const string SCREEN_480x854 = "480x854";
            public const string SCREEN_720x1280 = "720x1280";
            public const string SCREEN_768x1280 = "768x1280";
            public const string SCREEN_1080x1920 = "1080x1920";

       }

        public class Folders {
            //保存每次生成的ip
            public const String IPS_FOLDER_NAME = "ips";
            //模拟器对应的机型信息。
            public const String AVD_PROPERTY_FOLDER_NAME = "avds";
            //测试apk的保存路径  用于检测模拟器是否启动的
            public const String APK_FOLDER_NAME = "apks";
            //批处理文件保存路径
            public const String BAT_FOLDER_NAME = "bats";
            //提示音保存路径
            public const String MUSIC_FOLDER_NAME = "music";
            //不同sdk版本的 build.prop文件 模板
            public const String SDK_FOLDER_NAME = "sdks";
            //真实机型的参数 以备修改模拟器的参数
            public const String MODEL_FOLDER_NAME = "models";
            //已经修改好的 机型参数 防止多次生成
            public const String TEMP_FOLDER_NAME = "temp";

           
        }
        
        /// <summary>
        /// 所有的批处理文件
        /// </summary>
        public class Apktool {
            //获取连接设备
            public const String DEVICES_BAT_NAME = "devices.bat";
            //adb 连接设备
            public const String CONNECT_BAT_NAME = "connect.bat";

            //安装
            public const String INSTALL_BAT_NAME = "install.bat";
            //修改模拟器 build.prop 和 init.androVM.sh
            public const String PROP_BAT_NAME = "prop.bat";
            
            //卸载
            public const String UNINSTALL_BAT_NAME = "uninstall.bat";

          
            
            //连接vpn
            public const String VPN_C_BAT_NAME = "vpn_c.bat";
            //断开vpn
            public const String VPN_D_BAT_NAME = "vpn_d.bat";



            //从模拟器中 pull出Xprivacy生成的随机值文件
            public const String PULL_RANDOM_BAT_NAME = "pull_random.bat";
            //删除 Xprivacy生成的 随机值文件
            //以及按键精灵v3.1.2 生成的 保存悬浮窗坐标的文件
            public const String DETETE_RANDOM_BAT_NAME = "delete_random.bat";

            public const String ADB_CLOSE_BAT_NAME = "adb_close.bat";

           


            /*** 已经弃用 ***/
            //public const String APPS_BAT_NAME = "apps.bat";
            //清除模拟器某一个应用的数据
            //public const String PM_CLEAR_BAT_NAME = "pm_clear.bat";
            //获取当前模拟器安装的应用
            //public const String PM_INSTALL_BAT_NAME = "pm_install.bat";
            //public const String REBOOT_BAT_NAME = "reboot.bat";
            //public const String PULL_TIME_BAT_NAME = "pull_time.bat";
            //public const String DETETE_TIME_BAT_NAME = "delete_time.bat";


        }

        /// <summary>
        /// 回调标识
        /// </summary>
        public class ProcessType
        {
            public const int TYPE_OF_PROCESS_START_UNKNOWN = 0x0000;

            //解包
            public const int TYPE_OF_PROCESS_START_UNPACK_APK = 0x0001;

            //打包
            public const int TYPE_OF_PROCESS_START_PACK_APK = 0x0002;

            //打开文件夹
            public const int TYPE_OF_PROCESS_START_OPEN_FOLDER = 0x0003;

            // new
            public const int TYPE_OF_PROCESS_GET_DEVICES = 0x0004;

            public const int TYPE_OF_PROCESS_START_CONNECT = 0x0005;

            public const int TYPE_OF_PROCESS_START_INSTALL = 0x0006;

            public const int TYPE_OF_PROCESS_CHANGE_PROP = 0x0007;

            //public const int TYPE_OF_PROCESS_GET_APPS = 0x0008;

            public const int TYPE_OF_PROCESS_UNINSTALL_APP = 0x0009;

            //public const int TYPE_OF_PROCESS_START_REBOOT = 0x00010;

            

           
            
            
            //连接vpn回调
            public const int TYPE_OF_PROCESS_CONNECT_VPN = 0x00011;
            //断开vpn回调
            public const int TYPE_OF_PROCESS_DISCONNECT_VPN = 0x00012;

            //public const int TYPE_OF_PROCESS_PULL_TIME = 0x00013;
            //public const int TYPE_OF_PROCESS_DELETE_TIME = 0x00014;

            //public const int TYPE_OF_PROCESS_PM_CLEAR = 0x00015;
            //public const int TYPE_OF_PROCESS_PM_INSTALL = 0x00016;

            public const int TYPE_OF_PROCESS_CLOSE_ADB = 0x00017;

            public const int TYPE_OF_PROCESS_PULL_RANDOM = 0x00018;
            public const int TYPE_OF_PROCESS_DELETE_RANDOM = 0x00019;
            
        }
    }
}
