using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace What
{
    class ThreadUtil
    {
        /// <summary>
        ///  调用 bat文件 或者 exe 文件
        /// </summary>
        /// <param name="path">bat或者exe 的路径</param>
        /// <param name="args">bat或者exe 的参数</param>
        /// <returns>Log</returns>

        public static List<String> callBatOrExe(String path, String[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = path;
            String arguments = "";
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    arguments = arguments + args[i] + " ";
                }
                startInfo.Arguments = arguments;
            }

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            List<String> list = new List<String>();
            list.Add(process.StandardOutput.ReadToEnd());
            list.Add(process.StandardError.ReadToEnd());
            return list;
        }

        /// <summary>
        ///  调用 bat文件 或者 exe 文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <returns>ProcessStartInfo</returns>
        public static ProcessStartInfo getBatOrExeStartInfo(String path, String[] args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = path;
            String arguments = "";
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    arguments = arguments + args[i] + " ";
                }
                startInfo.Arguments = arguments;
            }

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            return startInfo;
        }

        public static ProcessStartInfo openFolder(String filePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "explorer";
            startInfo.Arguments = @"/select," + filePath;
            return startInfo;
        }
    }
}
