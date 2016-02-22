using UnityEngine;
using System.Collections;

/// <summary>
/// 相机脚本：相机围看向目标
/// </summary>
public class CameraLookAt : MonoBehaviour 
{
    /// <summary>
    /// 目标位置
    /// </summary>
    public Vector3 lookPos;
    /// <summary>
    /// 创建一个旋转，沿着forward（z轴）并且头部沿着upwards（y轴）的约束注视
    /// </summary>
    public Vector3 upwards = new Vector3(0,1,0);
    /// <summary>
    /// 是否平滑
    /// </summary>
    public bool smooth = false;
    /// <summary>
    /// 平滑参数
    /// </summary>
    public float damping = 6.0f;

    public void LateUpdate()
    {
        if (smooth)
        {
            var rotation = Quaternion.LookRotation(lookPos - transform.position, upwards);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        else
        {
            transform.LookAt(lookPos, upwards);
        }
        //debug
        Debug.DrawLine(this.transform.position, lookPos);
    }
}
