using UnityEngine;
using System.Collections;
/// <summary>
/// AES 加密工具
/// </summary>
public class AESTool
{

    //============================================================
    //
    // 
    const int ASSET_HEADER_SIZE = 8;
    static byte[] AssetBundleHeader = new byte[ASSET_HEADER_SIZE] { 85, 110, 105, 116, 121, 87, 101, 98 };// == UnityWeb
    static public bool NeedDecrypt(byte[] bBuf)
    {
        if (bBuf.Length < ASSET_HEADER_SIZE)
            return true;
        //
        for (int iLoop = 0; iLoop < ASSET_HEADER_SIZE; ++iLoop)
        {
            if (AssetBundleHeader[iLoop] != bBuf[iLoop])
                return true;
        }

        return false;
    }

    //============================================================
    //
    // Decrypt bytes, and create assetbundle from memory
    static public AssetBundle DecryptCreateFromMemory(byte[] bytes)
    {
        // Decrypt data...
        if (bytes.Length >= Aes.AES_CODE_LEN)
        {
            Aes.Instance.Decode(bytes, Aes.AES_CODE_LEN, bytes);
        }

        return AssetBundle.CreateFromMemoryImmediate(bytes);
    }
}
