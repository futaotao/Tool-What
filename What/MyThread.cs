using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace What
{
    class MyThread
    {
        private ProcessStartInfo startInfo;
        private Thread thread = null;
        private List<String> logList = null;
        private OnProcessListener listener = null;
        private int processType = Constant.ProcessType.TYPE_OF_PROCESS_START_UNKNOWN;

        public MyThread(ProcessStartInfo startInfo, OnProcessListener listener, int processType)
        {
            this.startInfo = startInfo;
            this.listener = listener;
            this.processType = processType;
            thread = new Thread(new ThreadStart(startProcess));
            thread.Name = "StartProcess";
            logList = new List<String>();

        }

        public void start()
        {
            if (thread != null)
            {
                thread.Start();
            }
        }

        public void join()
        {
            if (thread != null)
            {
                thread.Join();
            }
        }


        private void startProcess()
        {
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();


            logList.Add(process.StandardOutput.ReadToEnd());
            logList.Add(process.StandardError.ReadToEnd());

            listener.onProcessOver(processType);
        }

        public List<String> getLog()
        {
            return logList;
        }
    }
}
