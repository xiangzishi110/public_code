using System;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using System.IO;

/// <summary>
/// 通信的GZip辅助类
/// </summary>
public class NetGZip
{
    /// Gzip压缩
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] Gzip(byte[] bytes)
    {
        MemoryStream ms = new MemoryStream();
        GZipOutputStream gzip = new GZipOutputStream(ms);
        gzip.Write(bytes, 0, bytes.Length);
        gzip.Close();
        return ms.ToArray();
    }

    /// Gzip解压
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] UnGzip(byte[] bytes)
    {
        GZipInputStream gzi = new GZipInputStream(new MemoryStream(bytes));
        MemoryStream re = new MemoryStream();
        int count = 0;
        byte[] data = new byte[4096];
        while ((count = gzi.Read(data, 0, data.Length)) != 0)
        {
            re.Write(data, 0, count);
        }
        byte[] depress = re.ToArray();
        return re.ToArray();
    }
}

