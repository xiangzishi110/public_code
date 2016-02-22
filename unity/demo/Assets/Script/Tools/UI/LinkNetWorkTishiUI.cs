using System.Collections;

namespace Tools.UI
{
    /// <summary>
    /// 链接网络提示UI
    /// </summary>
    public class LinkNetWorkTishiUI
    {
        private static LinkNetWorkTishiUI _instance;
        public static LinkNetWorkTishiUI instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LinkNetWorkTishiUI();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 链接网络提示,true开启;false,关闭;
        /// </summary>
        /// <param name="isbool"></param>
        public void State(bool isbool, float num = 5f)
        {
            //LoadingUI.instance.ShowLoadingCicle(isbool, num);
            Logger.LogError("这里要添加提示的具体实现");
        }
    }
}
