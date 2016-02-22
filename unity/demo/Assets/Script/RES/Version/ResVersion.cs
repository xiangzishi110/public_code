
using System.Collections.Generic;
/// <summary>
/// RES 版本控制
/// </summary>
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class ResVersion
{
    /// <summary>
    /// 获取文件对应的md5值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
        public static string getCheckMD5(string filePath)
       {
           if (File.Exists(filePath) == false)
               return string.Empty;
           try
           {
               MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
               FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
               byte[] hashbytes = MD5.ComputeHash(fs);
               fs.Close();
               StringBuilder sBuilder = new StringBuilder();
               for (int i = 0, max = hashbytes.Length; i < max; i++)
               {
                   sBuilder.Append(hashbytes[i].ToString("x2"));
               }
               return sBuilder.ToString();
           }
           catch (System.Exception ex)
           {
               return string.Empty;
               Logger.LogError("getCheckMD5 Error : "+ex.Message +" ; "+filePath);
           }
       }

        /// <summary>
        /// 对比两文件夹下所有文件的md5
        /// </summary>
        /// <param name="fromDirectory">新版本资源的目录</param>
        /// <param name="checkDirectory">前版本资源的目录</param>
        /// <param name="newDirectory">需要更新资源的保存目录</param>
        public static void CheckDirectory(string fromDirectory,string checkDirectory,string newDirectory)
        {
            Dictionary<string, string> fromFileMd5 = GetFilesMd5(fromDirectory);
            Dictionary<string, string> checkFileMd5 = GetFilesMd5(checkDirectory);
            Dictionary<string, string> upFilePath = new Dictionary<string, string>();

            if (checkFileMd5.Count == 0)
            {
                //第一个版本
                upFilePath = fromFileMd5;
            }
            else
            {
                //比较两版本差异
                foreach (var item in fromFileMd5)
                {
                    if (checkFileMd5.ContainsKey(item.Key) == false || checkFileMd5[item.Key].Equals(item.Value) == false)
                    {
                        upFilePath.Add(item.Key, item.Value);
                    }
                }
            }

            FileCopyTo(fromDirectory, newDirectory, upFilePath);

            upFilePath = null;
            checkFileMd5 = null;
            fromFileMd5 = null;
        }
        
       

        /// <summary>
        /// 获取整个文件夹下所有文件的md5值
        /// key，文件的完整相对路径带后缀名；value，md5值
        /// </summary>
        /// <param name="directory">文件夹目录</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFilesMd5(string directory)
        {
            Dictionary<string, string> fileMd5 = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(directory)==false)
            {
                foreach (var item in new DirectoryInfo(directory).GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (RESTool.RightFile(item.Name))
                    {
                        //Debug.Log("文件MD5：" + RESTool.FolderDelimiter(item.FullName) + "; " + RESTool.FolderDelimiter(item.FullName).Replace(directory, "") + " ; " + getCheckMD5(item.FullName));

                        fileMd5.Add(RESTool.FolderDelimiter(item.FullName).Replace(directory, ""), getCheckMD5(item.FullName));
                    }
                }
            }
           
            return fileMd5;
        }
        /// <summary>
        /// 复制粘贴到新文件夹
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filesMd5">key，文件的完整相对路径带后缀名；value，md5值</param>
        public static void FileCopyTo(string fromDirectory,string toDirectory, Dictionary<string,string>  filesMd5)
        {
            
            FileInfo fileInfo = null;
         
            foreach (var item in filesMd5.Keys)
            {
                Logger.Log("原文件:"+fromDirectory+item);
                 Logger.Log("复制粘贴到:"+toDirectory+item);
                RESTool.CreateDirectory(RESTool.GetBuildABDirectory(toDirectory + item));
                fileInfo = new FileInfo(fromDirectory + item);
                fileInfo.CopyTo(toDirectory + item, true);
            }

            fileInfo = null;
         
        }


    }

