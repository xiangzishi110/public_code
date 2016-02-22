using UnityEngine;

  public  class RESLoadProgressUI : MonoBehaviour
    {
        private GameObject uiGo;
        private UIProgressBar _scrollBar;
        private int _hashCode = -1;
      void Awake() 
      {
          uiGo = transform.FindChild("UI").gameObject;
          _scrollBar = transform.FindChild("UI/Progress Bar").GetComponent<UIProgressBar>();
          RES.resProgressUIDele += Progress;

      }
    
        /// <summary>
        /// 进度百分比
        /// </summary>
        /// <param name="value"></param>
      public void Progress(int hashCode, float progress)
        {
           // Logger.Log(">>>>>> Progress " + _hashCode + " ; " + progress + " ;  " + hashCode);
            if (progress == 1f)
            {
                _hashCode = -1;
                Show(false);
                return;
            }

            if (_hashCode == -1)
            {
                _hashCode = hashCode;

                Show(true);

                _scrollBar.value = progress;
            }
            else if (_hashCode == hashCode)
            {
                _scrollBar.value = progress;
            }


        }

      /// <summary>
      /// 是否显示进度条。true,显示；
      /// </summary>
      /// <param name="isbool"></param>
      void Show(bool isbool)
      {
          if (uiGo.activeSelf != isbool)
          {
              uiGo.SetActive(isbool);
          }
      }
    }

