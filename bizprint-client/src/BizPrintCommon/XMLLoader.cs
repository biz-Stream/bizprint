// Copyright 2024 BrainSellers.com Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BizPrintCommon
{
    /// <summary>
    /// 設定ファイルのXMLを展開するクラス
    /// </summary>
    public class XMLLoader
    {
        /// Properties セクション
        private Hashtable SectionTable = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public XMLLoader()
        {
            this.SectionTable = new Hashtable();

        }
        /// <summary>
        /// 指定したセクション-エントリーの読み込みを行う
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <param name="entryName">エントリー名</param>
        /// <returns>エントリ値</returns>
        public string ReadEntry(
            string sectionName,
            string entryName)
        {
            return ReadEntry(sectionName, entryName, null);
        }
        // エントリーの読み込みを行う
        /// <summary>
        /// 指定したセクション-エントリーの読み込みを行う
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <param name="entryName">エントリー名</param>
        /// <param name="entryDefaultValue">デフォルト値</param>
        /// <returns>エントリ値</returns>
        public string ReadEntry(
            string sectionName,
            string entryName,
            string entryDefaultValue)
        {
            Hashtable entry = ReadSection(sectionName);
            Object entryValue = entry[entryName];
            if (entryValue == null || entryValue.ToString().Length == 0)
            {
                entryValue = entryDefaultValue;
            }
            return Convert.ToString(entryValue);
        }
        /// <summary>
        /// 指定したセクション-エントリーの読み込みを行う
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <param name="entryName">エントリー名</param>
        /// <param name="entryDefaultValue">デフォルト値</param>
        /// <returns>エントリ値</returns>
        public int ReadEntryInt(
            string sectionName,
            string entryName,
            int entryDefaultValue)
        {
            int retVal = entryDefaultValue;
            try
            {
                retVal = Int32.Parse(ReadEntry(sectionName, entryName));
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("012", entryName, entryDefaultValue.ToString(), ex.Message);
            }
            //マイナス値を設定できるものは無いので、マイナスはエラーとしてデフォルト値にする
            if (retVal < 0) {
                LogUtility.OutputLog("012", entryName, entryDefaultValue.ToString());
                retVal = entryDefaultValue;
            }
            return retVal;
        }
        //マイナス値を設定できるもの用
        public int ReadEntryIntCanMinus(
            string sectionName,
            string entryName,
            int entryDefaultValue)
        {
            int retVal = entryDefaultValue;
            try
            {
                retVal = Int32.Parse(ReadEntry(sectionName, entryName));
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("012", entryName, entryDefaultValue.ToString(), ex.Message);
            }
            return retVal;
        }
        public bool ReadEntryBool(
            string sectionName,
            string entryName,
            bool entryDefaultValue)
        {
            bool retVal = entryDefaultValue;
            try
            {
                retVal = Boolean.Parse(ReadEntry(sectionName, entryName));
            }
            catch (Exception)
            {
                LogUtility.OutputLog("012", entryName, entryDefaultValue.ToString());
            }
            return retVal;
        }
        /// <summary>
        /// セクションの読込を行う
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <returns>セクションハッシュ</returns>
        protected Hashtable ReadSection(
            string sectionName)
        {
            return ReadSection(this.SectionTable, sectionName);
        }
        /// <summary>
        /// セクションの読込を行う
        /// </summary>
        /// <param name="section">セクションハッシュ</param>
        /// <param name="sectionName">セクション名</param>
        /// <returns>セクションハッシュ</returns>
        protected static Hashtable ReadSection(
            Hashtable section,
            string sectionName)
        {
            Hashtable entry = null;
            Object o = section[sectionName];
            if (o is Hashtable)
            {
                entry = (o as Hashtable);
            }
            else
            {
                // ない場合は新規作成する
                entry = new Hashtable();
            }
            return entry;
        }

        /// <summary>
        /// XMLファイルの読み込み処理
        /// </summary>
        /// <param name="instm">入力ファイル</param>
        public void LoadFromXMLFile(string filename)
        {

            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                LoadFromXMLStream(stream);
            }
        }

        /// <summary>
        /// プロパティファイルの読み込みを行う
        /// </summary>
        /// <param name="instm">XMLファイルストリーム</param>
        public void LoadFromXMLStream(
            FileStream instm)
        {
            this.Clear();  // クリアする
            XmlDocument document = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(instm);
            document.Load(reader);
            try
            {
                ReadXMLItems(reader, document);
            }
            finally
            {
                reader.Close();
            }

            //ログにXMLの全内容を出力(エンコードはStringWriterの仕様上utf-16になる)
            try
            {
                System.IO.StringWriter writer = new System.IO.StringWriter();
                document.Save(writer);
                string readdata = writer.ToString();
                writer.Close();
                LogUtility.OutputLog("460", "\r\n"+readdata);
            }
            catch
            {
            }

        }
        /// <summary>
        /// プロパティクリア
        /// </summary>
        public void Clear()
        {
            this.SectionTable.Clear();
        }
        /// <summary>
        /// プロパティファイル項目の読み込みを行う
        /// </summary>
        /// <param name="reader">XMLリーダ</param>
        /// <param name="document">XMLドキュメント</param>
        protected void ReadXMLItems(
            XmlTextReader reader,
            XmlDocument document)
        {
            XmlNode docElement = document.DocumentElement;
            XmlNodeList nodeSelectionList = docElement.SelectNodes("section");
            foreach (XmlNode nodeSection in nodeSelectionList)
            {
                // セクション名
                string sectionName = nodeSection.Attributes["key"].Value;

                XmlNodeList nodeEntryList = nodeSection.SelectNodes("entry");
                foreach (XmlNode nodeEntry in nodeEntryList)
                {
                    string entryName = nodeEntry.Attributes["key"].Value;
                    Object entryValue = ReadXMLItems(reader, nodeEntry);
                    if (entryValue is IList)
                    {
                        WriteEntryList(sectionName, entryName, (entryValue as IList));
                    }
                    else
                    {
                        WriteEntry(sectionName, entryName, Convert.ToString(entryValue));
                    }
                }
            }
        }
        /// <summary>
        /// 指定したセクション-エントリーの書き込みを行う
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <param name="entryName">エントリー名</param>
        /// <param name="entryValue">エントリー値</param>
        /// <returns>書き込み成功時true</returns>
        public bool WriteEntry(
            string sectionName,
            string entryName,
            string entryValue)
        {
            Hashtable entry = ReadSection(sectionName);
            PutElement(entry, entryName, entryValue);
            PutElement(this.SectionTable, sectionName, entry);
            return true;
        }
        /// <summary>
        /// 指定したセクション-エントリーリストの書き込みを行う
        /// </summary>
        /// <param name="sectionName">セクション名</param>
        /// <param name="entryName">エントリー名</param>
        /// <param name="entryValue">エントリー値</param>
        /// <returns>書き込み成功時true</returns>
        public bool WriteEntryList(
            string sectionName,
            string entryName,
            IList entryList)
        {
            Hashtable entry = ReadSection(sectionName);
            PutElement(entry, entryName, entryList);
            PutElement(this.SectionTable, sectionName, entry);
            return true;
        }
        /// <summary>
        /// ハッシュの内容の置き換えを行う
        /// </summary>
        /// <param name="hashTable">ハッシュテーブル</param>
        /// <param name="key">キー</param>
        /// <param name="newValue">値</param>
        /// <returns>変更前の値</returns>
        public static System.Object PutElement(
            System.Collections.IDictionary hashTable,
            System.Object key, System.Object newValue)
        {
            System.Object element = hashTable[key];
            hashTable[key] = newValue;
            return element;
        }
        /// <summary>
        /// プロパティファイル項目の読み込みを行う
        /// </summary>
        /// <param name="reader">XMLリーダ</param>
        /// <param name="nodeEntry">XMLノード</param>
        /// <return>エントリー値</return>
        protected Object ReadXMLItems(
            XmlTextReader reader,
            XmlNode nodeEntry)
        {
            Object entryValue = nodeEntry.InnerText;
            string typeValue = nodeEntry.Attributes["type"].Value;
            if ("IList".Equals(typeValue))
            {
                IList list = new ArrayList();
                XmlNode nodeList = nodeEntry.SelectSingleNode("list");
                XmlNodeList nodeItemList = nodeList.SelectNodes("item");
                foreach (XmlNode nodeItem in nodeItemList)
                {
                    list.Add(nodeItem.InnerText);
                }
                entryValue = list;
            }
            return entryValue;
        }


    }

}
