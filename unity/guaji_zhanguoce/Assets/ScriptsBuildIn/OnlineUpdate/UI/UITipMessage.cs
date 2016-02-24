using System;
using UnityEngine;

namespace update{
    public class UITipMessage
    {
        private static System.Action<int> _callBack;
        /// <summary>
        /// 单个确定按钮提示框
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="trans"></param>
        public static void show(string msg,Transform trans,System.Action<int> action){
            if (trans == null) return;
            _callBack = action;
            trans.FindChild("TishiUI/Label").GetComponent<UILabel>().text = msg;
            trans.FindChild("TishiUI").gameObject.SetActive(true);
            trans.FindChild("TishiUI/Button_1").gameObject.SetActive(false);
            trans.FindChild("TishiUI/Button_0").localPosition = new Vector3(0, -92, 0);
            trans.FindChild("TishiUI/Button_0").GetComponent<SendEventCollider>().initData(EventListener.onClick, null, AgainBtOnCall);
            trans.FindChild("TishiUI/Button_1").GetComponent<SendEventCollider>().initData(EventListener.onClick, null, AgainBtOnCall);
        }
        
        /// <summary>
        /// 确定取消按钮提示框
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="trans"></param>
        public static void showChoose(string msg, Transform trans, System.Action<int> action)
        {
            if (trans == null) return;
            _callBack = action;
            trans.FindChild("TishiUI/Label").GetComponent<UILabel>().text = msg;
            trans.FindChild("TishiUI").gameObject.SetActive(true);
            trans.FindChild("TishiUI/Button_1").gameObject.SetActive(true);
            trans.FindChild("TishiUI/Button_0").localPosition = new Vector3(-127, -92, 0);
            trans.FindChild("TishiUI/Button_0").GetComponent<SendEventCollider>().initData(EventListener.onClick, null, AgainBtOnCall);
            trans.FindChild("TishiUI/Button_1").GetComponent<SendEventCollider>().initData(EventListener.onClick, null, AgainBtOnCall);
        }
        private static void AgainBtOnCall(EventListener arg1, GameObject arg2, object arg3, object arg4)
        {
            int index = int.Parse(arg2.name.Split('_')[1]);
            if (_callBack != null)
            {
                _callBack(index);
                _callBack = null;
            }
        }
    }
}
