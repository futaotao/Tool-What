using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace What
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            What what = new What();
            what.StartPosition = FormStartPosition.Manual;
            int screenW = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width; 
            //int screenH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int formW = what.Width;
            //int formH = what.Height;
            what.Top = 0;
            what.Left = screenW - formW;

            Application.Run(what);
        }
    }
}
