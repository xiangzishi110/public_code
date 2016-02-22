using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tools.UI;
public class UITools : MonoBehaviour {


    public static  void setUISprite(Transform trans,string spriteName)
    {
        if (trans != null && string.IsNullOrEmpty(spriteName) == false && trans.GetComponent<UISprite>()!=null) 
        {
          trans.GetComponent<UISprite>().spriteName = spriteName;
        }
        if (trans != null && string.IsNullOrEmpty(spriteName) == false && trans.parent.GetComponent<UIButton>() != null)
        {
            trans.parent.GetComponent<UIButton>().normalSprite = spriteName;
        }
    }
  
    public static  void setUISprite(UISprite uiSprite, string spriteName)
    {
        if (uiSprite != null && string.IsNullOrEmpty(spriteName) == false)
        {
            uiSprite.spriteName = spriteName;
        }
    }
    /// <summary>
    /// 设置UISprite,并适配原始尺寸
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="spriteName"></param>
    public static  void setUISpriteASize(Transform trans, string spriteName)
    {
        if (trans != null && string.IsNullOrEmpty(spriteName) == false)
        {
            UISprite sp = trans.GetComponent<UISprite>();
            if (sp != null)
            {
                sp.spriteName = spriteName;
                sp.MakePixelPerfect();
                sp = null;
            }

        }
    }

    /// <summary>
    /// 当元宝,经验大于9W9时,以万为单位显示  
    /// </summary>
    /// <param name="uiLabel"></param>
    /// <param name="text"></param>
    public static  string SimplifyNumber(int num, int maxValue = 99999, int point = 1)
    {
        double pointDou = System.Math.Round(((float)num / 10000f), point);
        string _str = num > maxValue ? pointDou + "W" : num.ToString();
        return _str;

    }
    

    /// <summary>
    /// 设置UILable的字符串。 text,内容;
    /// </summary>
    /// <param name="uiLabel"></param>
    /// <param name="text"></param>
    static public void setUILable(UILabel uiLabel, string text)
    {
        if (uiLabel != null )
        {
            uiLabel.text = text;
        }
    }
    /// <summary>
    /// 设置UILable的字符串。 text,内容;
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="text"></param>
    static public void setUILable(Transform trans, string text)
    {
        if (trans != null && trans.GetComponent<UILabel>() !=null)
        {
            trans.GetComponent<UILabel>().text = text;
        }
    }
    /// <summary>
    /// 设置UILable的字符串。 text,内容; colorStr,颜色值
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="text"></param>
    /// <param name="colorStr">颜色值</param>
    static public void setUILable3(Transform trans, string text,string colorStr)
    {
        if (trans != null && trans.GetComponent<UILabel>() != null)
        {
            trans.GetComponent<UILabel>().text = GetColorStr(text,colorStr);
        }
    }
    /// <summary>
    /// 设置UILable的字符串。 text,内容; lv,等级;
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="text"></param>
    /// <param name="lv">等级</param>
    static public void setUILable4(Transform trans, string text, int lv)
    {
        if (trans != null && trans.GetComponent<UILabel>() != null)
        {
            trans.GetComponent<UILabel>().text = GetColorStr(text, lvColorStr(lv));
        }
    }

    /// <summary>
    /// 通过等级获取相应的品质(暂时只有体质系统用到)
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static string GetColorByLvl( int iLvl )
    {
        string szColor = White;
        if( iLvl <= 10 )
        {
            szColor = Green;
        }
        else if( iLvl <= 20 )
        {
            szColor = Blue;
        }
        else if( iLvl <= 30 )
        {
            szColor = Purple;
        }
        else
        {
            szColor = Orange;
        }

        return szColor;
    }

    /// <summary>
    /// 通过装备觉醒等级获取名称显示颜色
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static string GetEquipAwakeColorStr(int iAwakeLvl)
    {
        string szColor = White;
        if (iAwakeLvl == 0)
        {
            szColor = White;
        }
        else if (iAwakeLvl <= 4)
        {
            szColor = Green;
        }
        else if (iAwakeLvl <= 8)
        {
            szColor = Blue;
        }
        else if (iAwakeLvl <= 12)
        {
            szColor = Purple;
        }
        else
        {
            szColor = Orange;
        }

        return szColor;
    }



    /// <summary>
    /// 返回品质对应的颜色值
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static string lvColorStr(int lv)
    {
        switch (lv)
        {
            case (int)Quality.one:
                return White;

            case (int)Quality.two:
                return Green;

            case (int)Quality.three:
                return Blue;

            case (int)Quality.four:
                return Purple;

            case (int)Quality.five:
                return Orange;
            default:
                return White;
             
        }

    }
    /// <summary>
    ///返回对应颜色值的字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="colorStr"></param>
    /// <returns></returns>
    public static string GetColorStr(string value, string colorStr)
    {
        return "[" + colorStr + "]" + value + "[-]";
    }
    /// <summary>
    /// 设置UITexture的mainTexture。texturePath 相对路径，没后缀名;
    /// full==true,适配原始尺寸
    /// texturePath="",释放
    /// </summary>
    static public void setUITexture(Transform trans, string texturePath,bool full=false)
    {
        try
        {
        if (trans != null && trans.GetComponent<UITexture>() != null)
        {
                if (string.IsNullOrEmpty(texturePath))
                {
                    if (trans.GetComponent<UITexture>().mainTexture != null)
                    {
                        trans.GetComponent<UITexture>().mainTexture = null;
                        RES.AddRESRecommend(trans.gameObject,new RESBase("",RESFormat.PNG));
                    }
                }
                else 
                {
                    if (string.IsNullOrEmpty(RESTool.GetFileName(texturePath)))
                    {
                        Logger.LogError("UITools SetUITexture Error :flineName null;"+texturePath);
                        return;
                    }
                    
                    if(UpUITexture(trans,texturePath))
                    {
                        RES.LoadByURL(RES.GetLoadBase(texturePath, RESFormat.PNG), delegate()
                        {
                            if (trans == null)
                            {
                                return;
                            }
                            System.Object ob = RES.GetRES(texturePath, RESFormat.PNG);
                            if (ob == null)
                            {
                                Logger.LogError("加载图片错误:" + texturePath);

                                if (trans != null)
                                {
                                    setUITexture(trans, "");
                                }
                            }
                            else
                            {
                                 if (UpUITexture(trans, texturePath))
                                  {
                                      SetUITexture2(trans, ob as Texture2D, full);
                                      RES.AddRESRecommend(trans.gameObject, new RESBase(texturePath, RESFormat.PNG));
                                  }
                            }
                        }
                        );
                    }
                    else
                    {
                        System.Object ob = RES.GetRES(texturePath, RESFormat.PNG);
                        if (ob == null)
                        {
                            Logger.LogError("加载图片错误:" + texturePath);
                            setUITexture(trans, "");
                        }
                        else
                        {
                            SetUITexture2(trans, ob as Texture2D, full);
                            RES.AddRESRecommend(trans.gameObject, new RESBase(texturePath, RESFormat.PNG));
                        }
                    }
                }  
        }

       }
       catch (System.Exception ex)
       {

            Logger.LogError("UITools SetUITexture Exception:" + ex.Message + ";" + texturePath);
       }
    }
    /// <summary>
    /// 设置UITexture的mainTexture。texture Texture2D;
    /// full==true,适配原始尺寸
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="texture"></param>
    static public void SetUITexture2(Transform trans, Texture texture,bool full=false)
    {
        if (trans != null && trans.GetComponent<UITexture>() != null)
        {
            trans.GetComponent<UITexture>().mainTexture = texture;
            if (full)
            {
                trans.GetComponent<UITexture>().width = texture.width;
                trans.GetComponent<UITexture>().height = texture.height;
            }
        }
    }
    /// <summary>
    /// 是否需要更新UITexture中的Texture
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    static public bool UpUITexture(Transform trans,string filePath)
    {
        try
        {
            if (trans == null)
            {
                //Logger.LogError("UITools UpUITexture Exception:trans null;" + filePath);
                return false;
            }
            RESRecommend resCom = trans.GetComponent<RESRecommend>();
            if (resCom != null && resCom.ResKey != null && resCom.ResKey.key.Equals(filePath))
            {
                //Logger.LogWarning("UITools UpUITexture filePath existed "+filePath);
                return false;
            }
            return true;
        }
        catch (System.Exception ex)
        {

            Logger.LogError("UITools UpUITexture Exception:" + filePath+";"+ex.Message);
            return false;
        }

        
    }


    /// <summary>
    /// 销毁 Object
    /// </summary>
    /// <param name="ob"></param>
    static public void destroy(Object ob)
    {
        if(ob!=null)
         NGUITools.Destroy(ob);
    }

   


   #region 颜色值  白 1品质，绿2品质，蓝3品质，紫4品质，橙5品质。白品最低，橙品最高
   /// <summary>
   /// 纯黑
   /// </summary>
   static public string Black = "000000";
   /// <summary>
   /// 纯白
   /// </summary>
   static public string White = "ffffff";
   /// <summary>
   /// 纯绿
   /// </summary>
   static public string Green = "8eff20";
   /// <summary>
   /// 纯蓝
   /// </summary>
   static public string Blue = "38d3ff";
   /// <summary>
   /// 紫色
   /// </summary>
   static public string Purple = "b782ff";
   /// <summary>
   /// 橙色
   /// </summary>
   static public string Orange = "ffa70f";
    /// <summary>
    /// 纯红
    /// </summary>
   static public string Red = "FF0000";
    /// <summary>
    /// 纯红1 飘字用到过
    /// </summary>
   static public string Red1 = "fb1515";
    /// <summary>
    /// 装备评分文字颜色,黄色
    /// </summary>
   static public string EquipScoreColor = "fffd51";
    /// <summary>
    /// 元宝文字颜色，淡黄色
    /// </summary>
   static public string YuanbaoColor = "fff447";

   /// <summary>
   /// 装备属性文字颜色,淡蓝色
   /// </summary>
   static public string EquipPropertyColor = "8bbcdd";

    #endregion 颜色值

    /// <summary>
    /// 更新或重新自身以及所有子级的UIPanel Depth。panelDepth，自身UIPanel Depth。子级UIPanel Depth相加panelDepth
    /// </summary>
    /// <param name="go"></param>
    /// <param name="panelDepth"></param>
    public static void UpdateUIPanleDepth(GameObject go,int panelDepth) 
    {
        UIPanel uiRootPanel = go.GetComponent<UIPanel>();

        if (uiRootPanel != null)
        {
            int orDepth = uiRootPanel.depth;
            uiRootPanel.depth = panelDepth;
            UIPanel[] panels = go.GetComponentsInChildren<UIPanel>(true);
            if (panels != null && panels.Length > 0)
            {
                foreach (UIPanel panel in panels)
                {
                    if (panel != uiRootPanel)
                    {
                        panel.depth = panelDepth + (panel.depth - orDepth);
                    }
                }
            }
            uiRootPanel = null;
            panels = null;
        }
        else
        {
            UIPanel[] panels = go.GetComponentsInChildren<UIPanel>(true);
            if (panels != null && panels.Length > 0)
            {
                foreach (UIPanel panel in panels)
                {
                    panel.depth = panelDepth + (panel.depth % UIManager.intance._layerPanelDepth);
                }
            }
            uiRootPanel = null;
            panels = null;
        }
    }
    /// <summary>
    /// 更新所有子级的UIPanel Depth。parentGo，基准Panel GameObject,子级UIPanel Depth相加panelDepth
    /// </summary>
    /// <param name="go"></param>
    /// <param name="panelDepth"></param>
    public static void UpdateUIPanleChildDepth(GameObject go, GameObject parentGo)
    {
        if (parentGo.GetComponent<UIPanel>() == null)
            return;
        int panelDepth = parentGo.GetComponent<UIPanel>().depth;

        if (go.GetComponent<UIPanel>() != null)
        {
            if (go.GetComponent<UIPanel>().depth < panelDepth)
            {
                 go.GetComponent<UIPanel>().depth = panelDepth + go.GetComponent<UIPanel>().depth;
            }
        }
      
        UIPanel[] panels = go.GetComponentsInChildren<UIPanel>();
        //Debug.LogError("更新子级"+ panelDepth+"; "+panels.Length);
        if (panels != null && panels.Length >0)
        {
            foreach (UIPanel panel in panels)
            {
                //Debug.LogError("更新子级" + panelDepth + "; " + panel.depth);
                if (panel.depth < panelDepth)
                {
                    panel.depth = panelDepth + panel.depth;
                   // Debug.LogError("更新子级的depth：" + panelDepth + "; ");
                }

            }
        }
        panels = null;
    }

    public static Transform AddChild(Transform parnet, Transform target, string name = "")
    {
        if (target != null)
        {
            target.parent = parnet;
            target.localScale = Vector3.one;
            target.localPosition = Vector3.zero;
            if (string.IsNullOrEmpty(name) == false)
                target.name = name;
            return target;
        }
        return target;
    }
    public static Transform AddChild(Transform parnet, Transform target, Vector3 posotion, Vector3 scale, Quaternion  quater,string name = "")
    {
        if (target != null)
        {
            target.parent = parnet;
            target.localScale = scale;
            target.localPosition = posotion;
            target.localRotation = quater;
            if (string.IsNullOrEmpty(name) == false)
                target.name = name;
            return target;
        }
        return target;
    }
    public static Transform InstAddChild(GameObject prefab, Transform parnet, string name = "")
    {
        if (prefab != null)
        {
            Transform clone = (Instantiate(prefab) as GameObject).transform;
            clone.parent = parnet;
            clone.localScale = Vector3.one;
            clone.localPosition = Vector3.zero;
            if (string.IsNullOrEmpty(name) == false)
                clone.name = name;

            return clone;
        }
        return null;
    }

   
    



    



   



	  public static void SetActive(GameObject go, bool isbool) 
	  {
		  if (go!=null && go.activeSelf != isbool)
			  go.SetActive(isbool);
	  }
	  public static void SetActiveTrans(Transform go, bool isbool)
	  {
		  if (go != null && go.gameObject.activeSelf != isbool)
			  go.gameObject.SetActive(isbool);
	  }




  




  /// <summary>
  /// 销毁所有子物体
  /// </summary>
  /// <param name="parent"></param>
  public static void DestroyChild(Transform parent)
  {
      if (parent != null && parent.childCount>0)
      {
          List<GameObject> childGo = new List<GameObject>();
          for (int i = 0, length = parent.childCount; i < length; i++)
          {
              childGo.Add(parent.GetChild(i).gameObject);
          }
          foreach (var item in childGo)
          {
              NGUITools.Destroy(item);
          }
          childGo = null;
      }
  }

    /// <summary>
    /// Grid自动定位到对应item
    /// </summary>
    /// <param name="myGrid"></param>
    /// <param name="targetValue"></param>
  public static void GridGoTo(GameObject myGrid, int targetValue = 0)
  {
      if (targetValue > 0) targetValue -= 1; //保持前面有一个垫底
      //##自动定位start
      Vector3 vecPos = myGrid.transform.localPosition;//对象 transform 的localPosition初始坐标.
      Vector2 vecOff = myGrid.transform.GetComponent<UIPanel>().clipOffset;//对象 UIPanel 的clipOffset初始坐标.
      UIScrollView.Movement _type = myGrid.transform.GetComponent<UIScrollView>().movement;
      if (_type == UIScrollView.Movement.Vertical)
      {
          float heightNum = myGrid.transform.GetComponent<UIGrid>().cellHeight;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float centerNum = myGrid.transform.GetComponent<UIPanel>().GetViewSize().y;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float _value = (targetValue * heightNum) ;
          if (_value <= 0) _value = 0;
          vecOff.y -= _value;
          vecPos.y += _value;
      }
      else if (_type == UIScrollView.Movement.Horizontal)
      {
          float heightNum = myGrid.transform.GetComponent<UIGrid>().cellWidth;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float centerNum = myGrid.transform.GetComponent<UIPanel>().GetViewSize().x;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float _value = (targetValue * heightNum);
          if (_value <= 0) _value = 0;
          vecOff.x += _value;
          vecPos.x -= _value;

         
      }
      Logger.Log(targetValue + ":变量");
      myGrid.GetComponent<UIPanel>().clipOffset = vecOff;
      myGrid.transform.localPosition = vecPos;
  }
  /// <summary>
  /// Grid自动移动到对应item
  /// </summary>
  /// <param name="myGrid">容器Grid</param>
  /// <param name="vecPos">对象 transform 的localPosition初始坐标.</param>
  /// <param name="vecOff">对象 UIPanel 的clipOffset初始坐标.</param>
  /// <param name="targetValue">编号</param>
  public static void GridGoTo(GameObject myGrid, Vector3 vecPos,  Vector2 vecOff, int targetValue = 0)
  {
      if (targetValue > 0) targetValue -= 1; //保持前面有一个垫底
      //##自动定位start
      UIScrollView.Movement _type = myGrid.transform.GetComponent<UIScrollView>().movement;
      if (_type == UIScrollView.Movement.Vertical)
      {
          float heightNum = myGrid.transform.GetComponent<UIGrid>().cellHeight;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float centerNum = myGrid.transform.GetComponent<UIPanel>().GetViewSize().y;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float _value = (targetValue * heightNum);
          if (_value <= 0) _value = 0;
          vecOff.y -= _value;
          vecPos.y += _value;
      }
      else if (_type == UIScrollView.Movement.Horizontal)
      {
          float heightNum = myGrid.transform.GetComponent<UIGrid>().cellWidth;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float centerNum = myGrid.transform.GetComponent<UIPanel>().GetViewSize().x;//偏移值 分上下移动cellHeight 左右移动cellWidth;
          float _value = (targetValue * heightNum);
          if (_value <= 0) _value = 0;
          vecOff.x += _value;
          vecPos.x -= _value;


      }
      Logger.Log(targetValue + ":变量");
      SpringPanel _SpringPanel = myGrid.GetComponent<SpringPanel>();
      if (_SpringPanel == null) return;
      SpringPanel.Begin(myGrid, vecPos, 8f);
  }
 


   

  /// <summary>
  /// 设置特效的渲染层
  /// </summary>
  /// <param name="currentTransform"></param>
  public static void  SetRenderQueue(Transform currentTransform,int queueNum)
  {
      if (currentTransform.renderer != null && currentTransform.renderer.sharedMaterial != null)
      {
          currentTransform.renderer.sharedMaterial.renderQueue = queueNum;
      }
      if (currentTransform.childCount != 0)
      {
          foreach (Transform child in currentTransform)
          {
              SetRenderQueue(child,queueNum);
          }
      }
  }
  /// <summary>
  /// 设置3D模型的渲染层
  /// </summary>
  /// <param name="currentTransform"></param>
  public static void SetModelRenderQueue(Transform currentTransform, int queueNum)
  {
      if (currentTransform == null)
      {
          return;
      }
      if (currentTransform.GetComponent<Renderer>() != null && currentTransform.GetComponent<Renderer>().sharedMaterial != null)
      {
          currentTransform.GetComponent<Renderer>().material.renderQueue = queueNum;
      }
      if (currentTransform.childCount != 0)
      {
          foreach (Transform child in currentTransform)
          {
              SetModelRenderQueue(child, queueNum);
          }
      }
  }
  /// <summary>
  /// 获取UIPanle RenderQueue
  /// </summary>
  public static int GetPanelRenderQueue(Transform trans)
  {
      if( null == trans )
      {
          return 0;
      }

      UIPanel panel = trans.GetComponent<UIPanel>();
      if (null == panel)
      {
          return 0;
      }

      return panel.startingRenderQueue;
  }
  /// <summary>
  /// 获取UIPanle 最大 RenderQueue
  /// </summary>
  public static int GetPanelMaxRenderQueue(UIPanel trans)
  {
      if (null == trans || trans.drawCalls.Count == 0)
      {
          return 0;
      }
      int rdq = 0;
      for (int i = 0,length=trans.drawCalls.Count; i < length; i++)
      {
          if (rdq < trans.drawCalls[i].renderQueue)
              rdq = trans.drawCalls[i].renderQueue;
      }
      return rdq;
  }
  /// <summary>
  /// 获取UIPanel Depth
  /// </summary>
  public static int GetPanelDepth(Transform trans)
  {
      if (null == trans)
      {
          return 0;
      }

      UIPanel panel = trans.GetComponent<UIPanel>();
      if (null == panel)
      {
          return 0;
      }

      return panel.depth;
  }


      
     
   
    /// <summary>
    /// 正则表达式
    /// </summary>
    /// <param name="value"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    public static bool Regex(string value,string regex)
    {
        System.Text.RegularExpressions.Regex reges = new System.Text.RegularExpressions.Regex(regex);
        if (reges.IsMatch(value))
            return true;
        else
            return false;
    }

    /// <summary>
    /// ScrollView居中显示trans
    ///条件：ScrollView、UICenterOnChild
    /// </summary>
    /// <param name="trans"></param>
    public static void CenterScrollView(Transform trans)
    {
        if (trans == null)
            return;
        UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(trans.gameObject);
        UIPanel panel = NGUITools.FindInParents<UIPanel>(trans.gameObject);

        if (center != null)
        {
            if (center.enabled)
                center.CenterOn(trans);
        }
        else if (panel != null && panel.clipping != UIDrawCall.Clipping.None)
        {
            UIScrollView sv = panel.GetComponent<UIScrollView>();
            Vector3 offset = -panel.cachedTransform.InverseTransformPoint(trans.position);
            if (!sv.canMoveHorizontally) offset.x = panel.cachedTransform.localPosition.x;
            if (!sv.canMoveVertically) offset.y = panel.cachedTransform.localPosition.y;
            SpringPanel.Begin(panel.cachedGameObject, offset, 6f);
        }
    }

   
    public static void setResIcon(Transform item,string name,string num,string icon)
    {
        if (item == null) return;
        if (item.FindChild("Label name") != null)
        {
            item.FindChild("Label name").GetComponent<UILabel>().text = name;
        }
        if (item.FindChild("Label num") != null)
        {
            item.FindChild("Label num").GetComponent<UILabel>().text = "x"+num;
        }
        if(item.FindChild("Icon")!=null){
            item.FindChild("Icon").gameObject.SetActive(true);
            item.FindChild("Icon").GetComponent<UISprite>().spriteName = icon;
        }
        
    }

    

    
    /// <summary>
    /// 获取子物体,不存在实例化
    /// </summary>
    public static Transform GetTrans(Transform parnet,GameObject  prefab, string name)
    {
        Transform trans = parnet.FindChild(name);
        if (trans == null)
        {
            trans = UITools.InstAddChild(prefab, parnet, name);
        }
        return trans;
    }

  

       
}
