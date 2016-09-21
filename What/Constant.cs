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
            public const String IPS_FOLDER_NAME = "ips";
            public const String AVD_PROPERTY_FOLDER_NAME = "avds";
            public const String APK_FOLDER_NAME = "apks";
            public const String BAT_FOLDER_NAME = "bats";
            public const String MUSIC_FOLDER_NAME = "music";

            public const String SDK_FOLDER_NAME = "sdks";
            public const String MODEL_FOLDER_NAME = "models";
            public const String TEMP_FOLDER_NAME = "temp";

           
        }

        public class Apktool {
            public const String DEVICES_BAT_NAME = "devices.bat";
            public const String CONNECT_BAT_NAME = "connect.bat";
            public const String INSTALL_BAT_NAME = "install.bat";
            public const String PROP_BAT_NAME = "prop.bat";
            public const String APPS_BAT_NAME = "apps.bat";
            public const String UNINSTALL_BAT_NAME = "uninstall.bat";

            public const String REBOOT_BAT_NAME = "reboot.bat";
            public const String VPN_C_BAT_NAME = "vpn_c.bat";
            public const String VPN_D_BAT_NAME = "vpn_d.bat";

            public const String PULL_TIME_BAT_NAME = "pull_time.bat";
            public const String DETETE_TIME_BAT_NAME = "delete_time.bat";

            public const String PULL_RANDOM_BAT_NAME = "pull_random.bat";
            public const String DETETE_RANDOM_BAT_NAME = "delete_random.bat";

            public const String PM_CLEAR_BAT_NAME = "pm_clear.bat";
            public const String PM_INSTALL_BAT_NAME = "pm_install.bat";

            public const String ADB_CLOSE_BAT_NAME = "adb_close.bat                                                                                    ";
        }

        public class ProcessType
        {
            public const int TYPE_OF_PROCESS_START_UNKNOWN = 0x0000;

            public const int TYPE_OF_PROCESS_START_UNPACK_APK = 0x0001;

            public const int TYPE_OF_PROCESS_START_PACK_APK = 0x0002;

            public const int TYPE_OF_PROCESS_START_OPEN_FOLDER = 0x0003;

            // new
            public const int TYPE_OF_PROCESS_GET_DEVICES = 0x0004;

            public const int TYPE_OF_PROCESS_START_CONNECT = 0x0005;

            public const int TYPE_OF_PROCESS_START_INSTALL = 0x0006;

            public const int TYPE_OF_PROCESS_CHANGE_PROP = 0x0007;

            public const int TYPE_OF_PROCESS_GET_APPS = 0x0008;

            public const int TYPE_OF_PROCESS_UNINSTALL_APP = 0x0009;

            public const int TYPE_OF_PROCESS_START_REBOOT = 0x00010;

            

           
            
            
            
            public const int TYPE_OF_PROCESS_CONNECT_VPN = 0x00011;

            public const int TYPE_OF_PROCESS_DISCONNECT_VPN = 0x00012;

            public const int TYPE_OF_PROCESS_PULL_TIME = 0x00013;
            public const int TYPE_OF_PROCESS_DELETE_TIME = 0x00014;

            public const int TYPE_OF_PROCESS_PM_CLEAR = 0x00015;
            public const int TYPE_OF_PROCESS_PM_INSTALL = 0x00016;

            public const int TYPE_OF_PROCESS_CLOSE_ADB = 0x00017;

            public const int TYPE_OF_PROCESS_PULL_RANDOM = 0x00018;
            public const int TYPE_OF_PROCESS_DELETE_RANDOM = 0x00019;
            
        }
    }
}
