using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace com.brainsellers.bizstream.print
{
    /**
     * <summary>
     * �_�C���N�g����̋��ʃN���X�ł��B
     * </summary>
     * <history>
     *  [rikiya]    2006/10/24    Created
     *  [swata]     2017/08/01    Changed
     * </history>
     */
    public abstract class PDFCommonPrintStream
    {
        /// <summary>��i�g���C</summary>
        public static readonly String TRAY_UPPER = "UPPER";

        /// <summary>���i�g���C</summary>
        public static readonly String TRAY_MIDDLE = "MIDDLE";

        /// <summary>���i�g���C</summary>
        public static readonly String TRAY_LOWER = "LOWER";

        /// <summary>�荷���g���C</summary>
        public static readonly String TRAY_MANUAL = "MANUAL";

        /// <summary>�����I��</summary>
        public static readonly String TRAY_AUTO = "AUTO";

        /// <summary>�f�o�b�O�E�t���O</summary>
        protected static Boolean DEBUG_FLAG = false;

        /// <summary>�f�o�b�O�ۑ���</summary>
        protected static String DEBUG_SAVE = null;

        /**
         *	�f�o�b�O�ŏI����
         */
        protected static long DEBUG_LAST = 0;

        /// <summary>PDF �o�b�t�@</summary>
        protected MemoryStream pdfdoc;

        /// <summary>�v�����^��</summary>
        protected String printerName;

        /// <summary>�������</summary>
        protected Int32 numberOfCopy;

        /// <summary>�o�̓g���C</summary>
        protected String selectedTray;

        /// <summary>������ʎq</summary>
        protected String jobName;

        /// <summary>�y�[�W�T�C�Y�ɍ��킹�Ĉ��</summary>
        protected bool doFit;

        /// <summary>����͈͎w��J�n�y�[�W</summary>
        protected int fromPage;

        /// <summary>����͈͎w��I���y�[�W</summary>
        protected int toPage;

        /// <summary>
        /// �C���X�^���X�𐶐����A�w�肳�ꂽ HTTP �����I�u�W�F�N�g�ŏ��������܂��B
        /// </summary>
        public PDFCommonPrintStream()
        {
            pdfdoc = new MemoryStream();

            printerName = null;
            numberOfCopy = 1;
            selectedTray = null;
            jobName = null;
            doFit = true;
            fromPage = -1;
            toPage = -1;
        }
        
        /// <summary>
        /// ������� URL ������ɕϊ����܂��B
        /// </summary>
        /// <param name="text">������</param>
        /// <returns>URL ������</returns>
        protected String encode(String text)
        {
            return HttpUtility.UrlEncode(text, Encoding.UTF8);
        }

        /// <summary>
        /// �p�����[�^�������Ԃ��܂��B
        /// </summary>
        /// <returns>�p�����[�^������</returns>
        protected String parameters()
        {
            StringBuilder buf = new StringBuilder();
            String sep = "\n";

            if (printerName != null)
            {
                buf.Append(sep + "printerName=" + printerName);
                sep = "\n";
            }

            buf.Append(sep + "numberOfCopy=" + numberOfCopy.ToString());
            sep = "\n";

            if (selectedTray != null)
            {
                buf.Append(sep + "selectedTray=" + selectedTray);
                sep = "\n";
            }

            if (jobName != null)
            {
                buf.Append(sep + "jobName=" + jobName);
                sep = "\n";
            }
            else
            {
                jobName = "JobName_Default";
                buf.Append(sep + "jobName=" + jobName);
            }

            if (doFit == false)
            {
                buf.Append(sep + "doFit=false");
            }
            else {
                buf.Append(sep + "doFit=true");
            }

            if (fromPage > 0)
            {
                buf.Append(sep + "fromPage=" + fromPage.ToString());
            }

            if ((toPage > 0) && (toPage >= fromPage))
            {
                buf.Append(sep + "toPage=" + toPage.ToString());
            }


            return buf.ToString();
        }

        /// <summary>
        /// �o�b�t�@�t���b�V�����܂��B
        /// </summary>
        public void Flush()
        {
            pdfdoc.Flush();
        }

        /// <summary>
        /// PDF �f�[�^���o�b�t�@�ɏ����o���܂��B 
        /// </summary>
        /// <param name="pdf">PDF �f�[�^</param>
        public void WriteByte(byte pdf)
        {
            pdfdoc.WriteByte(pdf);
        }

        /// <summary>
        /// PDF �f�[�^���w��ʒu����w��o�C�g�������o�b�t�@�ɏ����o���܂��B 
        /// </summary>
        /// <param name="pdf">PDF �f�[�^</param>
        /// <param name="offset">�J�n�ʒu</param>
        /// <param name="length">�o�C�g��</param>
        public void Write(byte[] pdf, int offset, int length)
        {
            pdfdoc.Write(pdf, offset, length);
        }


        /// <summary>
        /// PDF �f�[�^���o�b�t�@�ɏ����o���܂�
        /// </summary>
        /// <param name="pdf">PDF �f�[�^</param>
        public void Write(byte[] pdf)
        {
            pdfdoc.Write(pdf, 0, pdf.Length);
        }

        /// <summary>�������܂ꂽ�o�C�g����Ԃ��܂��B</summary>
        /// <returns>�������܂ꂽ�o�C�g��</returns>
        public long size()
        {
            return pdfdoc.Length;
        }

        /// <summary>�v�����^�����Z�b�g���܂��B</summary>
        /// <param name="value">�v�����^��</param>

        public void setPrinterName(String value)
        {
            printerName = value;
        }

        /// <summary>����������Z�b�g���܂��B</summary>
        /// <param name="value">�������</param>
        public void setNumberOfCopy(int value)
        {
            if (value < 1)
            {
                //throw new ArgumentException();
                value = 1;
            }

            if (value > 999)
            {
                value = 999;
            }

            numberOfCopy = value;
        }

        /// <summary>�o�̓g���C���Z�b�g���܂��B</summary>
        /// <param name="value">�o�̓g���C</param>
        public void setSelectedTray(String value)
        {
            selectedTray = value;
        }

        /// <summary>������ʎq���Z�b�g���܂��B</summary>
        /// <param name="value">������ʎq</param>

        public void setJobName(String value)
        {
            jobName = value;
        }

        /// <summary>����ʒu�����t���O���Z�b�g���܂��B</summary>
        /// <param name="value">����ʒu�����t���O</param>
        public void setDoFit(bool value)
        {
            doFit = value;
        }

        /// <summary>����͈͂̊J�n�y�[�W���Z�b�g���܂��B</summary>
        /// <param name="value">����J�n�y�[�W</param>
        public void setFromPage(int value)
        {
            fromPage = value;
        }

        /// <summary>����͈͂̏I���y�[�W���Z�b�g���܂��B</summary>
        /// <param name="value">����I���y�[�W</param>
        public void setToPage(int value)
        {
            toPage = value;
        }

    }
}
