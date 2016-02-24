using System.Security.Cryptography;

namespace update
{
    /// <summary>
    /// 资源加密与解密
    /// </summary>
    public class EnDeTool
    {
        private static string des1 = "plasuq81nvkaeriogvm9apeyrfnvki02";

        public static string Des1
        {
            set { des1 = value; }
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EnStr(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Key = System.Text.Encoding.UTF8.GetBytes(des1);
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypto = rijndael.CreateEncryptor();
            byte[] resBts = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] bytes = crypto.TransformFinalBlock(resBts, 0, resBts.Length);
            return System.Convert.ToBase64String(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DeStr(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Key = System.Text.Encoding.UTF8.GetBytes(des1);
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypto = rijndael.CreateDecryptor();
            byte[] resBts = System.Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(crypto.TransformFinalBlock(resBts, 0, resBts.Length));
        }
    }
}

