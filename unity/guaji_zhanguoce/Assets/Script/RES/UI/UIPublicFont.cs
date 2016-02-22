using UnityEngine;
using System.Collections;

public static class UIPublicFont
{
    /// <summary>
    /// UI字体
    /// </summary>
    public static Font _UIFont;

	// Use this for initialization
    static public void Init()
    {
        //获取UI字体
        GameObject FontObj = GameObject.Find("GameControl/Font");
        if (null != FontObj)
        {
            GUIText tex = FontObj.GetComponent<GUIText>();
            if (null != tex)
            {
                _UIFont = tex.font;
            }
        }
	}
}
