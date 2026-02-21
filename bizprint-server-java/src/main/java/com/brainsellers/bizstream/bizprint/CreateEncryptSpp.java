/*
 * Copyright 2024 BrainSellers.com Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.brainsellers.bizstream.bizprint;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.charset.Charset;
import java.util.logging.Logger;

import net.lingala.zip4j.exception.ZipException;
import net.lingala.zip4j.io.outputstream.ZipOutputStream;
import net.lingala.zip4j.model.ZipParameters;
import net.lingala.zip4j.model.enums.AesKeyStrength;
import net.lingala.zip4j.model.enums.CompressionLevel;
import net.lingala.zip4j.model.enums.CompressionMethod;
import net.lingala.zip4j.model.enums.EncryptionMethod;

/**
 * 暗号化されたsppファイルを作成するためのクラスです。
 */
public class CreateEncryptSpp {
    private static final Logger LOGGER = Logger.getLogger(CreateEncryptSpp.class.getName());

    /**
     *  sppパスワード可変部分
     */
    private String userPass = "";

    /**
     *  sppパスワード固定部分前半
     */
    private String constPassBefore = "___RANDOM_STRINGS1___";

    /**
     *  sppパスワード固定部分後半
     */
    private String constPassAfter = "___RANDOM_STRINGS2___";

    /**
     *  送信パラメータファイル名
     */
    protected String bsParamName = "param.txt";

    /**
     *  送信パラメータファイル長
     */
    protected int bsParamLength;

    /**
     *  送信PDFファイル長
     */
    protected int bsPdfLength;

    /**
     *  送信Zip圧縮する
     */
    protected CompressionMethod compDef = CompressionMethod.DEFLATE;

    /**
     *  送信Zip圧縮率
     */
    protected CompressionLevel compLevel = CompressionLevel.NORMAL;

    /**
     *  送信Zip暗号化メソッド
     */
    protected EncryptionMethod cryptMethod = EncryptionMethod.AES;

    /**
     *  送信Zip暗号化コード長
     */
    protected AesKeyStrength cryptType = AesKeyStrength.KEY_STRENGTH_256;

    /**
     *  コンストラクタ
     */
    public CreateEncryptSpp() {
    }

    public void setPassword(String value) {
        userPass = value;
    }

    /**
     *  圧縮のためのパラメータをセットします。
     *
     *      @return 圧縮パラメータ
     */
    private ZipParameters buildZipParameters() {
        ZipParameters zParams = new ZipParameters();

        // 圧縮アルゴリズムと圧縮率指定
        zParams.setCompressionMethod(compDef);
        zParams.setCompressionLevel(compLevel);

        // 暗号化フラグをtrueに設定 (暗号化する)
        zParams.setEncryptFiles(true);

        // 暗号化方式と強度指定
        zParams.setEncryptionMethod(cryptMethod);
        zParams.setAesKeyStrength(cryptType);

        return zParams;
    }

    /**
     *  sppファイルの圧縮処理本体です。
     *
     *      @param      paramTxt    印刷指定パラメータ文字列
     *      @param      jobName     ジョブ名
     *      @param      pdfdoc      印刷対象PDFファイルのストリーム
     *
     *      @return     圧縮済みsppファイルのバイト配列
     */
    public byte[] createCompressedSpp(String paramTxt, String jobName, ByteArrayOutputStream pdfdoc) throws IOException, BizPrintException {
        if (paramTxt == null) {
            LOGGER.severe("print parameter specification character string is NULL.");
            throw new BizPrintException("print parameter specification character string is NULL.");
        }

        if (jobName == null || jobName.length() == 0) {
            LOGGER.severe("JobNames not specified.");
            throw new BizPrintException("JobNames not specified.");
        }

        if (pdfdoc == null || pdfdoc.size() == 0) {
            LOGGER.severe("PDF File is NULL or size=0.");
            throw new BizPrintException("PDF File is NULL or size=0.");
        }

        // パスワード指定
        // bs固定パスワードは前後分割になっている
        String passwd = constPassBefore + userPass + constPassAfter;

        // 出力先ストリーム
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();

        // 解凍側はWindowsのみなので送信側で固定して、受信側でUTF-8で受ける
        ZipOutputStream zoutputStream = new ZipOutputStream(byteArrayOutputStream, passwd.toCharArray(), Charset.forName("UTF8"));
        ZipParameters zipParams = buildZipParameters();

        try {
            // パラメータファイル書き込み
            byte[] params = paramTxt.toString().getBytes("UTF-8");
            zipParams.setFileNameInZip(bsParamName);
            zoutputStream.putNextEntry(zipParams);
            zoutputStream.write(params);
            zoutputStream.closeEntry();

            // PDF実体ファイル定義。zipファイル内に拡張子付きで保存する
            if (!jobName.endsWith(".pdf")) {
                jobName = jobName + ".pdf";
            }
            final String _jobName = jobName;

            // PDFファイル書き込み
            zipParams.setFileNameInZip(_jobName);
            zoutputStream.putNextEntry(zipParams);
            zoutputStream.write(pdfdoc.toByteArray());
            zoutputStream.closeEntry();

        } catch (ZipException e) {
            LOGGER.severe("sppfile compression and encryption processing failed.(" + e.getMessage() + ")");
            throw new BizPrintException("sppfile compression and encryption processing failed.", e);
        } finally {
            zoutputStream.close();
        }

        return byteArrayOutputStream.toByteArray();
    }

}
