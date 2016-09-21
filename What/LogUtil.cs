using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace What
{
    class LogUtil
    {
        /// <summary>  
        /// 追加显示文本  
        /// </summary>  
        /// <param name="color">文本颜色</param>  
        /// <param name="text">显示文本</param>  
        private static void LogAppend(RichTextBox tb, Color color, string text)
        {
            try
            {
                if (tb != null && text != null)
                {
                    tb.SelectionColor = color;
                    tb.AppendText("\n" + text);
                    tb.SelectionStart = tb.Text.Length;
                }
            }
            catch (Exception e) {
            }
           
               // tb.ScrollToCaret();
          
            

        }
        /// <summary>  
        /// 显示错误日志  
        /// </summary>  
        /// <param name="text"></param>  
        public static void LogError(RichTextBox tb, string text)
        {
            LogAppend(tb, Color.Red, text);
        }
        /// <summary>  
        /// 显示警告信息  
        /// </summary>  
        /// <param name="text"></param>  
        public static void LogWarning(RichTextBox tb, string text)
        {
            LogAppend(tb, Color.Violet, text);
        }
        /// <summary>  
        /// 显示信息  
        /// </summary>  
        /// <param name="text"></param>  
        public static void LogMessage(RichTextBox tb, string text)
        {
            LogAppend(tb, Color.Black, text);
        }
    }
}
