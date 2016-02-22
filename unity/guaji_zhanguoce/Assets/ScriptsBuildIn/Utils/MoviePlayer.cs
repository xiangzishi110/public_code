using UnityEngine;
using System.Collections;

/// <summary>
/// 视频播放器
/// </summary>
public class MoviePlayer : MonoBehaviour 
{
    /// <summary>
    /// 手机采用路径播放(相对于StreamingAssets目录)
    /// </summary>
    public string mobileMovPath = "";
  
    void Start ()
    {
          #if UNITY_ANDROID  &&  !UNITY_EDITOR
        //Handheld.PlayFullScreenMovie(mobileMovPath, Color.black, FullScreenMovieControlMode.Hidden);
        Handheld.PlayFullScreenMovie(mobileMovPath, Color.black, FullScreenMovieControlMode.Full);
          #elif UNITY_IPHONE  &&  !UNITY_EDITOR
        //Handheld.PlayFullScreenMovie(mobileMovPath, Color.black, FullScreenMovieControlMode.Hidden);
        Handheld.PlayFullScreenMovie(mobileMovPath, Color.black, FullScreenMovieControlMode.Full);
        #endif
    }
  
}

