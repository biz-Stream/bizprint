/**
 *  @(#)CreateEncryptSpp.java
 *
 *  (C) Copyright BrainSellers.com Corporation. 2017- All Rights Reserved.
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
 *	暗号化されたsppファイルを作成するためのクラスです。
 *
 *		@version	1.10 2021/08/27
 *		@since		5.0.0
 */
public class CreateEncryptSpp {
//	static final Logger logger = LoggerFactory.getLogger(CreateEncryptSpp.class);
	private static final Logger LOGGER = Logger.getLogger(CreateEncryptSpp.class.getName());

	/**
	 *	sppパスワード可変部分
	 */
	private String userPass = "";

	/**
	 *	sppパスワード固定部分前半
	 */
	private String constPassBefore = "___RANDOM_STRINGS1___";

	/**
	 *	sppパスワード固定部分後半
	 */
	private String constPassAfter = "___RANDOM_STRINGS2___";

	/**
	 *	送信パラメータファイル名
	 */
	protected String bsParamName = "param.txt";

	/**
	 *	送信パラメータファイル長
	 */
	protected int bsParamLength;

	/**
	 *	送信PDFファイル長
	 */
	protected int bsPdfLength;

	/**
	 *	送信Zip圧縮する
	 */
	protected CompressionMethod compDef = CompressionMethod.DEFLATE;

	/**
	 *	送信Zip圧縮率
	 */
	protected CompressionLevel compLevel = CompressionLevel.NORMAL;

	/**
	 *	送信Zip暗号化メソッド
	 */
	protected EncryptionMethod cryptMethod = EncryptionMethod.AES;

	/**
	 *	送信Zip暗号化コード長
	 */
	protected AesKeyStrength cryptType = AesKeyStrength.KEY_STRENGTH_256;

	/**
	 *	コンストラクタ
	 */
	public CreateEncryptSpp() {
	}

	public void setPassword(String value){
		userPass = value;
	}

	/**
	 *	圧縮のためのパラメータをセットします。
	 *
	 *		@return	圧縮パラメータ
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
	 *	sppファイルの圧縮処理本体です。
	 *
	 *		@param		paramTxt	印刷指定パラメータ文字列
	 *		@param		jobName		ジョブ名
	 *		@param		pdfdoc		印刷対象PDFファイルのストリーム
	 *
	 *		@return		圧縮済みsppファイルのバイト配列
	 */
//	public byte[] createCompressedSpp(String paramTxt, String jobName, ByteArrayOutputStream pdfdoc) throws IOException, PDFRuntimeException {
	public byte[] createCompressedSpp(String paramTxt, String jobName, ByteArrayOutputStream pdfdoc) throws IOException, BizPrintException {
		if (paramTxt == null) {
//			logger.error(PDFMessages.getMessage(1423, null));
			LOGGER.severe("print parameter specification character string is NULL.");
//			PDFRuntimeException.throwException(1423, PDFMessages.getMessage(1423), null);
			throw new BizPrintException("print parameter specification character string is NULL.");
//			return null; //例外をthrowしない場合はnullを返す
		}

		if (jobName == null || jobName.length() == 0) {
//			logger.error(PDFMessages.getMessage(1424, null));
			LOGGER.severe("JobNames not specified.");
//			PDFRuntimeException.throwException(1424, PDFMessages.getMessage(1424), null);
			throw new BizPrintException("JobNames not specified.");
//			return null; //例外をthrowしない場合はnullを返す
		}

		if (pdfdoc == null || pdfdoc.size() == 0) {
//			logger.error(PDFMessages.getMessage(1425, null));
			LOGGER.severe("PDF File is NULL or size=0.");
//			PDFRuntimeException.throwException(1425, PDFMessages.getMessage(1425), null);
			throw new BizPrintException("PDF File is NULL or size=0.");
//			return null; //例外をthrowしない場合はnullを返す
		}

		// パスワード指定
		// bs固定パスワードは前後分割になっている
		String passwd = constPassBefore + userPass + constPassAfter;

		// 出力先ストリーム
		ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();

		//解凍側はWindowsのみなので送信側で固定して、受信側でUTF-8で受ける
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
//			logger.error(PDFMessages.getMessage(1426, e.getMessage()));
			LOGGER.severe("sppfile compression and encryption processing failed.(" + e.getMessage() + ")");
//			PDFRuntimeException.throwException(1426, PDFMessages.getMessage(1426), null);
			throw new BizPrintException("sppfile compression and encryption processing failed.", e);
		} finally {
			zoutputStream.close();
		}

		return byteArrayOutputStream.toByteArray();
	}

}
