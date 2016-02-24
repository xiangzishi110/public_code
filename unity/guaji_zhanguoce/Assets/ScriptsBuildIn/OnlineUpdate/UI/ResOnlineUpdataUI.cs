using UnityEngine;
using System;
namespace update{
    public enum UIShowStatus
    {
        /// <summary>
        /// 开始状态
        /// </summary>
        START,
        /// <summary>
        /// 检查更新状态
        /// </summary>
        CHECK,
        /// <summary>
        /// 开始更新
        /// </summary>
        START_UPDATE,
        /// <summary>
        /// 更新中
        /// </summary>
        UPDATING,
    }


    /// <summary>
    /// 在线资源更新界面
    /// </summary>
    public class ResOnlineUpdataUI : MonoBehaviour
    {
        private float                      _size;
        private string                   _sizeStr;
        private System.Action       _startUpCall;
        private Transform              _uiTrans;
        private float                      maxUpTishi   = float.MaxValue;
        private UILabel             _bottomLabel;
        private UISlider              _slider;
        /// <summary>
        /// 是否更新完成
        /// </summary>
        private bool                      _updateComplete = false;
        /// <summary>
        /// 是否显示进度数
        /// </summary>
        private bool                        _showProgressNum = false;
        /// <summary>
        /// 是否更新完成
        /// </summary>
        public bool updateComplete
        {
            set{_updateComplete = value;}
        }
        /// <summary>
        /// 是否显示进度数
        /// </summary>
        public bool showProgressNum
        {
            set{_showProgressNum = value; }
        }

        void Start()
        {
            if (_uiTrans == null)
            {
                return;
            }
            _bottomLabel       =    _uiTrans.FindChild("Bottom/Label").GetComponent<UILabel>();
            _slider                   =    _uiTrans.FindChild("Bottom/Progress Bar").GetComponent<UISlider>();
            setStatus(UIShowStatus.CHECK);
        }
        /// <summary>
        /// 显示UI状态
        /// </summary>
        /// <param name="type"></param>
        public void setStatus(UIShowStatus type,string txt="")
        {
            if (null == _bottomLabel) return;
            switch (type)
            {
                case UIShowStatus.START:
                    _bottomLabel.text = "正在启动游戏";
                    break;
                case UIShowStatus.CHECK:
                    _bottomLabel.text = "正在检查版本,请稍后";
                    break;
                case UIShowStatus.START_UPDATE:
                    _bottomLabel.text = "请点击确定按钮，更新资源!";
                    break;
                case UIShowStatus.UPDATING:
                    _bottomLabel.text = txt;
                    break;
            }
        }

        /// <summary>
        /// 加载显示UI
        /// </summary>
        public void initLoadUI()
        {
            _uiTrans = getRootUI();
        }
        public void SetResNeedUpdateTotalSize(long size, System.Action startUpCall)
        {
            this._size                                     =    ResUpdateUtils.longToFloat(ResUpdateUtils.ByteToTrillionLong(size));
            this. _showCurrentLoadSize     =   0;
            this._willLoadedByte                  =   0;
            this._sizeStr                                 =           ResUpdateUtils.ByteToTrillion(size);
            this._startUpCall                            =    startUpCall;
            Transform uiTrans                        =   getRootUI();

            Logger.Log("更新资源数据:" + this._size + " ; " + _sizeStr);

            
            if (uiTrans == null)
            {
                if (this._startUpCall != null)
                {
                    this._startUpCall();
                }
                return;
            }
            else
            {
                
                if (this._size > maxUpTishi)
                {
                    string msg = "亲，是否更新" + this._sizeStr + "资源？取消则退出游戏!";
                    UITipMessage.showChoose(msg, uiTrans, BtOnCall);
                }
                else
                {
                    if (this._startUpCall != null)
                    {
                        this._startUpCall();
                    }
                }
            }
          
        }

        private void BtOnCall(int index)
        {
            if (index == 0)
            {
                //确定
                if (this._startUpCall != null)
                {
                    this._startUpCall();
                }
                    
            }else{
                //取消
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                {
                    setStatus(UIShowStatus.START_UPDATE);
                }else {
                    Application.Quit();
                }
            }
        }
        

        /// <summary>
        /// 更新完成资源总大小
        /// </summary>
        private long               _willLoadedByte = 0;
        /// <summary>
        /// 用于显示的当前更新大小
        /// </summary>
        private long                    _showCurrentLoadSize = 0;
        /// <summary>
        /// 更新速度
        /// </summary>
        private int                        _v = 1;
        /// <summary>
        /// 已经跳动的帧数，用于表现
        /// </summary>
        private int                       _showFrame = 0;
        /// <summary>
        /// 设置progress进度
        /// </summary>
        private void SetProgress(string szTxt, float fProgress, float fMax)
        {
            setStatus(UIShowStatus.UPDATING, szTxt);
            if( null != _slider )
            {
                _slider.value = 0f != fMax ? fProgress / fMax : 1f;
            }
        }
        /// <summary>
        /// 设置即将要下载完的长度
        /// </summary>
        /// <param name="willLoadedBayte"></param>
        public void setWillLoadedSize(long willLoadedBayte)
        {
            _willLoadedByte = willLoadedBayte;
            _v = (int)(_willLoadedByte - _showCurrentLoadSize) / 240;
        }
        public void Update()
        {
            if (_updateComplete == true)
            {
                return;
            }

            if (_showProgressNum == false && _willLoadedByte!=0)
            {
                _showCurrentLoadSize = _willLoadedByte;

                if( _size > 0f )
                {
                    string szTxt = "已下载 " + ResUpdateUtils.ByteToTrillion(_showCurrentLoadSize) + " ,总共 " + _sizeStr;
                    float fCurrSize = ResUpdateUtils.longToFloat(ResUpdateUtils.ByteToTrillionLong(_showCurrentLoadSize));
                    SetProgress(szTxt, fCurrSize, _size);
                }
                return;
            }

            //做更新时候进度条的更新效果，数字隔一段时间跳动一次
            _showFrame++;
            if ((_showFrame > 60 && _showFrame<120) || _showFrame>180 )
            {
                if (_showFrame > 240)
                {
                    _showFrame = 0;
                }
            }
            else
            {
                if (_showCurrentLoadSize + _v < _willLoadedByte)
                {
                    _showCurrentLoadSize += _v;
                }
                else
                {
                    _showCurrentLoadSize = _willLoadedByte;
                }

                if (_size > 0f)
                {
                    string szTxt = "已下载 " + ResUpdateUtils.ByteToTrillion(_showCurrentLoadSize) + " ,总共 " + _sizeStr;
                    float fCurrSize = ResUpdateUtils.longToFloat(ResUpdateUtils.ByteToTrillionLong(_showCurrentLoadSize));
                    SetProgress(szTxt, fCurrSize, _size);
                }                
            }
        }


        #region 资源热更新失败

        private System.Action             _UpFailCall;
        /// <summary>
        ///UpFailCall： 重新更新回调,默认是重新更新
        /// </summary>
        /// <param name="UpFailCall"></param>
        public void OnlineUpdateFail(System.Action UpFailCall, string text = "亲，更新资源错误？确定重新更新，取消则退出游戏!")
        {
            this._UpFailCall = UpFailCall;
            Transform uiTrans = getRootUI();
            
            if (uiTrans == null)
            {
                if (UpFailCall != null)
                {
                    UpFailCall();
                }
                return;
            }
            else
            {
                UITipMessage.showChoose(text, uiTrans, AgainBtOnCall);
            }
        }
        /// <summary>
        ///UpFailCall： 重新更新回调,默认是重新更新
        /// </summary>
        /// <param name="UpFailCall"></param>
        public void OnlineUpdateNotSuppered()
        {
            string text = "游戏版本过低，请下载最新版本体验游戏!";
            this._UpFailCall = exitGame;
            Transform uiTrans = getRootUI();
            if (uiTrans == null)
            {
                if (_UpFailCall != null)
                {
                    _UpFailCall();
                }
                return;
            }else{
                UITipMessage.show(text, uiTrans, AgainBtOnCall);
            }
        }
        private void exitGame()
        {
            Application.Quit();
        }

        private void AgainBtOnCall(int index)
        {
            if (index == 0)
            {
                //确定
                if (this._UpFailCall != null)
                {
                    this._UpFailCall();
                }
            }
            else
            {
                //取消
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                {
                    setStatus(UIShowStatus.START_UPDATE);
                }else{
                    Application.Quit();
                }
            }
        }
        /// <summary>
        /// 获取提示框ui
        /// </summary>
        /// <returns></returns>
        private Transform getRootUI()
        {
            if (_uiTrans != null) return _uiTrans;
            Transform uiTrans = (Instantiate(Resources.Load("ResOnlineUpUI/ResOnlineUpUI")) as GameObject).transform;
            uiTrans.parent = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("UI")).transform;
            uiTrans.localPosition = Vector3.zero;
            uiTrans.localScale = Vector3.one;
            return uiTrans;
        }
        #endregion 资源热更新失败
        //==================================================//
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            if (_uiTrans != null)
            {
                NGUITools.Destroy(_uiTrans.gameObject);
            }
            UnityEngine.Object.DestroyObject(gameObject);
            _instance = null;
        }
        private static ResOnlineUpdataUI       _instance;
        /// <summary>
        /// 热更新ui实例
        /// </summary>
        public static ResOnlineUpdataUI instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("ResOnlineUpdataUI").AddComponent<ResOnlineUpdataUI>();
                }
                return _instance;
            }
        }
    }
}

