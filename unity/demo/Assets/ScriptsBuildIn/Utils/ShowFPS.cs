//----------------------------------------------
// 显示帧频
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/2
//----------------------------------------------

using UnityEngine;
using System.Collections;


/// <summary>
/// 显示帧频
/// </summary>
public class ShowFPS : MonoBehaviour
{
    /// <summary>
    /// ui文本
    /// </summary>
    private GUIText _gui;
    /// <summary>
    /// 更新间隔
    /// </summary>
    private float _updateInterval = 1.0f;
    /// <summary>
    /// 上次更新时间
    /// </summary>
    private float _lastInterval;
    /// <summary>
    /// 上次更新经过的帧数
    /// </summary>
    private int _frames;

    void Start()
    {
        _lastInterval = Time.realtimeSinceStartup;
        _frames = 0;
        GameObject uiObj = new GameObject("FPS Display", typeof(GUIText));
        uiObj.layer = LayerMask.NameToLayer("UI");
        //uiObj.hideFlags = HideFlags.HideAndDontSave;
        uiObj.transform.position = Vector3.zero;
        _gui = uiObj.guiText;
        _gui.pixelOffset = new Vector2(5, Screen.height);
        _gui.fontSize = 20;
        GameObject.DontDestroyOnLoad(_gui.gameObject);
    }

    void OnDisable()
    {
        if(_gui)
        {
            DestroyImmediate(_gui.gameObject);
        }
    }

    void Update()
    {
        ++_frames;
        float nowTime = Time.realtimeSinceStartup;
        if(nowTime > _lastInterval + _updateInterval)
        {
            float fps = _frames / (nowTime - _lastInterval);
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
            _gui.text = ms.ToString("f2") + " ms\n" + fps.ToString("f2") + " FPS";
            _frames = 0;
            _lastInterval = nowTime;
        }
    }
}
