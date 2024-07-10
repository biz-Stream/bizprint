﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizPrintCommon;

namespace SilentPdfPrinter
{
    /// <summary>
    /// 指定ファイル削除クラス
    /// </summary>
    public class FileDeleter
    {
        /// <summary>
        /// 指定されたファイルを削除する。読み取り専用属性の場合は解除する
        /// </summary>
        /// <param name="path">削除対象ファイルパス</param>
        /// <returns>true:成功、false：失敗</returns>
        public static bool DeleteFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                LogUtility.OutputLog("037", path);
                return true;
            }
            FileAttributes attr = System.IO.File.GetAttributes(path);
            //読み取り専用チェック
            if ((attr & System.IO.FileAttributes.ReadOnly) ==
                System.IO.FileAttributes.ReadOnly)
            {
                LogUtility.OutputLog("032");
                try
                {
                    //読み取り専用解除
                    System.IO.File.SetAttributes(path, attr & (~System.IO.FileAttributes.ReadOnly));
                }
                catch (Exception e)
                {
                    //解除失敗
                    LogUtility.OutputLog("033", e.Message);
                    return false;
                }
            }
            //削除実行
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception e)
            {
                //削除失敗
                LogUtility.OutputLog("034", e.Message);
                return false;
            }
            LogUtility.OutputLog("031", path);
            return true;
        }
    }
}