using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using com.brainsellers.bizstream.print;

[assembly: CLSCompliant(true)]

namespace com.brainsellers.bizstream.directprint
{
    /**
     * <summary>
     * �I�����C���E�^�C�v�̃_�C���N�g����̃X�g���[���𐶐����邽�߂̃N���X�ł��B
     * </summary>
     * <history>
     *  [rikiya]    2006/10/24    Created
     *  [swata]     2017/08/01    Changed
     * </history>
     */
    public class PDFDirectPrintStream : PDFCommonPrintStream
    {
        /// <summary>HTTP �����I�u�W�F�N�g</summary>
        protected HttpResponse response;

        /// <summary>������� URL</summary>
        protected String responseUrl;

        /// <summary>�I������</summary>
        [Obsolete("v5.0���p�~����܂����B")]
        protected String disposal;

        /// <summary>�t�@�C���ۑ�</summary>
        protected String saveFileName;

        /// <summary>����֎~�t���O</summary>
        [Obsolete("v5.0���p�~����܂����B")]
        protected Boolean printDeny;

        /// <summary>�u���E�U target ��</summary>
        protected String target;

        /// <summary>����ʒu�����t���O</summary>
        [Obsolete("v5.0���p�~����܂����B")]
        protected Boolean printAdjust;

        /// <summary>����_�C�A���O�\���t���O</summary>
        protected Boolean printDialog;

        /// <summary>�Í����t���O</summary>
        [Obsolete("v5.0���p�~����܂����B")]
        protected Boolean encryption;

        /// <summary>spp�t�@�C�����̈�Ӊ�</summary>
        protected Boolean sppnameUnified = true;

        /// <summary>������� URL</summary>
        protected String userPassword;

        /**
         * <summary>
         * <c>PDFDirectPrintStream</c>�N���X�̐V�K�C���X�^���X�𐶐������������܂��B
         * </summary>
         * <param name="response">HttpResponse�I�u�W�F�N�g</param>
         * <remarks>
         * <c>PDFDirectPrintStream</c>�N���X�̐V�K�C���X�^���X�𐶐����A
         * �w�肳�ꂽ HTTP �����I�u�W�F�N�g�ŏ��������܂��B
         * </remarks>
         * <example>
         * <code>
         *    Dim direct As PDFDirectPrintStream
         *    direct = New PDFDirectPrintStream(Response)
         * </code>
         * </example>
         */
        public PDFDirectPrintStream(HttpResponse response)
        {
            this.response = response;
            this.response.Clear();

            responseUrl = null;
            disposal = null;
            saveFileName = null;
            printDeny = false;
            target = null;
            printAdjust = false;
            printDialog = false;
            encryption = false;
            userPassword = "";
        }

        /**
         * <summary>�Í�������悤�ɃZ�b�g���܂��B</summary>
         */
        [Obsolete("v5.0���p�~����܂����B")]
        public void encryptPrintFile()
        {
            encryption = true;
        }

        /**
         * <summary>�Í������邩�ǂ�����Ԃ��܂��B</summary>
         * <returns>true�̏ꍇ�A�Í������܂�</returns>
         */
        [Obsolete("v5.0���p�~����܂����B")]
        public Boolean isEncrypt()
        {
            return encryption;
        }

        /**
         * <summary>����ʒu�����t���O���Z�b�g���܂��B</summary>
         * <param name="value">����ʒu�����t���O</param>
         */
        [Obsolete("v5.0���p�~����܂����B")]
        public void setPrintAdjust(Boolean value)
        {
        }

        /**
         * <summary>����֎~�t���O���Z�b�g���܂��B</summary>
         * <param name="value">����֎~�t���O</param>
         */
        [Obsolete("v5.0���p�~����܂����B")]
        public void setPrintDeny(Boolean value)
        {
            printDeny = value;
        }

        /**
         * <summary>����_�C�A���O�\���t���O���Z�b�g���܂��B</summary>
         * <param name="value">����_�C�A���O�\���t���O</param>
         * <remarks>
         * <para>���̃��\�b�h��true(����_�C�A���O��\������)���w�肵���ꍇ�A�ȉ��̃��\�b�h�ł̎w��͖�������܂��B</para>
         * <para>PDFCommonPrintStream#setNumberOfCopy(�������)</para>
         * <para>PDFCommonPrintStream#setFromPage(�J�n�y�[�W�ԍ�)</para>
         * <para>PDFCommonPrintStream#setToPage(�I���y�[�W�ԍ�)</para>
         * <para>PDFCommonPrintStream#setDoFit(�y�[�W�T�C�Y�ɍ��킹�Ĉ���t���O)</para>
         * </remarks>
         */
        public void setPrintDialog(Boolean value)
        {
            printDialog = value;
        }

        /**
         * <summary>�I�������� REGIDENT ���[�h�ɃZ�b�g���܂��B</summary>
         */
        [Obsolete("v5.0.0���p�~����܂����B")]
        public void setRegident()
        {
        }

        /**
         * <summary>������� URL ���Z�b�g���܂��B</summary>
         * <param name="value">������� URL</param>
         */
        public void setResponseUrl(String value)
        {
            responseUrl = value;
        }

        /**
         * <summary>�t�@�C���ۑ����Z�b�g���܂��B</summary>
         * <param name="value">�t�@�C���ۑ�</param>
         */
        public void setSaveFileName(String value)
        {
            saveFileName = value;
        }

        /**
         * <summary>�u���E�U target �����Z�b�g���܂��B</summary>
         * <param name="value">�u���E�U target ��</param>
         */
        public void setTarget(String value)
        {
            target = value;
        }

        /**
         * <summary>���[�U�p�X���[�h���Z�b�g���܂��B(����)</summary>
         * <param name="value">�����p�X���[�h</param>
         */
        public void setPassword(String value)
        {
            if (value == null)
            {
                throw new ArgumentException("setPassword");
            }
            userPassword = value;
        }

        /**
         * <summary>���[�U�p�X���[�h���Z�b�g���܂��B(base64�G���R�[�h�ς�)</summary>
         * <param name="value">base64�G���R�[�h�ς݃p�X���[�h</param>
         */
        public void setPasswordWithEncoded(String value)
        {
            if (value == null)
            {
                //��OThrow
                throw new ArgumentException("setPasswordWithEncoded");
            }

            try
            {
                byte[] decbytes = Convert.FromBase64String(value);
                Encoding enc = Encoding.GetEncoding("UTF-8");
                userPassword = enc.GetString(decbytes);

            }
            catch (Exception ex)
            {
                //��OThrow
                throw new ArgumentException("setPasswordWithEncoded Cannot Decode. " + ex.Message);
            }
        }

        /**
         * <summary>spp�t�@�C�����̈�Ӊ��t���O���Z�b�g���܂��B</summary>
         * <param name="value">true�̏ꍇ�Aspp�t�@�C��������ӂɂ��܂�</param>
         */
        public void setSppNameUnified(Boolean value)
        {
            sppnameUnified = value;
        }

        /**
         * <summary>�I�������� TERMINATE ���[�h�ɃZ�b�g���܂��B</summary>
         */
        [Obsolete("v5.0���p�~����܂����B")]
        public void setTerminate()
        {
            disposal = "TERMINATE";
        }

        /**
         * <summary>�o�b�t�@���N���[�Y���܂��B</summary>
         */
        public void Close()
        {
            StringBuilder tmp = new StringBuilder(parameters());
            String sep = (tmp.Length == 0) ? "" : "\n";

            if (responseUrl != null)
            {
                tmp.Append(sep + "responseURL=" + responseUrl);
                sep = "\n";
            }

            if (saveFileName != null)
            {
                tmp.Append(sep + "saveFileName=" + saveFileName);
            }

            if (target != null)
            {
                tmp.Append(sep + "target=" + target);
            }
            tmp.Append(sep + "printDialog=" + printDialog.ToString().ToLower() + sep);


            DirectPrintClientModule.CreateEncryptSpp createEnc = new DirectPrintClientModule.CreateEncryptSpp();
            createEnc.setPassword(userPassword);
            byte[] sendspp = createEnc.createCompressedSpp(tmp.ToString(), jobName, pdfdoc.ToArray());

            response.ContentType = "application/x-spp";
            response.AddHeader("Content-Length", sendspp.Length.ToString());

            if (sppnameUnified)
            {
                //���ݎ�������t�@�C�����쐬
                DateTime DT = new DateTime();
                DT = DateTime.Now;
                Random cRandom = new System.Random();
                int rand = cRandom.Next(1000);
                string fname_now = DT.ToString("_yyMMdd_HHmmss_") + rand.ToString("D4");
                response.AddHeader("Content-Disposition", "inline; filename=" + fname_now+".spp");
            }

            Stream output = response.OutputStream;

            output.Write(sendspp, 0, (int)sendspp.Length);
            output.Close();

            pdfdoc.Close();
        }
    }
}
