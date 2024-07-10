/**
 *  @(#)CreateEncryptSpp.java
 *
 *  (C) Copyright BrainSellers.com Corporation. 2017- All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.charset.Charset;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.brainsellers.pdf.PDFRuntimeException;
import com.brainsellers.pdf.internal.PDFMessages;

import net.lingala.zip4j.exception.ZipException;
import net.lingala.zip4j.io.outputstream.ZipOutputStream;
import net.lingala.zip4j.model.ZipParameters;
import net.lingala.zip4j.model.enums.AesKeyStrength;
import net.lingala.zip4j.model.enums.CompressionLevel;
import net.lingala.zip4j.model.enums.CompressionMethod;
import net.lingala.zip4j.model.enums.EncryptionMethod;

/**
 *	�Í������ꂽspp�t�@�C�����쐬���邽�߂̃N���X�ł��B
 *
 *		@version	1.10 2021/08/27
 *		@since		5.0.0
 */
public class CreateEncryptSpp {
	static final Logger logger = LoggerFactory.getLogger(CreateEncryptSpp.class);

	/**
	 *	spp�p�X���[�h�ϕ���
	 */
	private String userPass = "";

	/**
	 *	spp�p�X���[�h�Œ蕔���O��
	 */
	private String constPassBefore = "xxxxxxxxxx";

	/**
	 *	spp�p�X���[�h�Œ蕔���㔼
	 */
	private String constPassAfter = "xxxxxxxxxx";

	/**
	 *	���M�p�����[�^�t�@�C����
	 */
	protected String bsParamName = "param.txt";

	/**
	 *	���M�p�����[�^�t�@�C����
	 */
	protected int bsParamLength;

	/**
	 *	���MPDF�t�@�C����
	 */
	protected int bsPdfLength;

	/**
	 *	���MZip���k����
	 */
	protected CompressionMethod compDef = CompressionMethod.DEFLATE;

	/**
	 *	���MZip���k��
	 */
	protected CompressionLevel compLevel = CompressionLevel.NORMAL;

	/**
	 *	���MZip�Í������\�b�h
	 */
	protected EncryptionMethod cryptMethod = EncryptionMethod.AES;

	/**
	 *	���MZip�Í����R�[�h��
	 */
	protected AesKeyStrength cryptType = AesKeyStrength.KEY_STRENGTH_256;

	/**
	 *	�R���X�g���N�^
	 */
	public CreateEncryptSpp() {
	}

	public void setPassword(String value){
		userPass = value;
	}

	/**
	 *	���k�̂��߂̃p�����[�^���Z�b�g���܂��B
	 *
	 *		@return	���k�p�����[�^
	 */
	private ZipParameters buildZipParameters() {
		ZipParameters zParams = new ZipParameters();

		// ���k�A���S���Y���ƈ��k���w��
		zParams.setCompressionMethod(compDef);
		zParams.setCompressionLevel(compLevel);

		// �Í����t���O��true�ɐݒ� (�Í�������)
		zParams.setEncryptFiles(true);

		// �Í��������Ƌ��x�w��
		zParams.setEncryptionMethod(cryptMethod);
		zParams.setAesKeyStrength(cryptType);

		return zParams;
	}

	/**
	 *	spp�t�@�C���̈��k�����{�̂ł��B
	 *
	 *		@param		paramTxt	����w��p�����[�^������
	 *		@param		jobName		�W���u��
	 *		@param		pdfdoc		����Ώ�PDF�t�@�C���̃X�g���[��
	 *
	 *		@return		���k�ς�spp�t�@�C���̃o�C�g�z��
	 */
	public byte[] createCompressedSpp(String paramTxt, String jobName, ByteArrayOutputStream pdfdoc) throws IOException, PDFRuntimeException {
		if (paramTxt == null) {
			logger.error(PDFMessages.getMessage(1423, null));
			PDFRuntimeException.throwException(1423, PDFMessages.getMessage(1423), null);
			return null; //��O��throw���Ȃ��ꍇ��null��Ԃ�
		}

		if (jobName == null || jobName.length() == 0) {
			logger.error(PDFMessages.getMessage(1424, null));
			PDFRuntimeException.throwException(1424, PDFMessages.getMessage(1424), null);
			return null; //��O��throw���Ȃ��ꍇ��null��Ԃ�
		}

		if (pdfdoc == null || pdfdoc.size() == 0) {
			logger.error(PDFMessages.getMessage(1425, null));
			PDFRuntimeException.throwException(1425, PDFMessages.getMessage(1425), null);
			return null; //��O��throw���Ȃ��ꍇ��null��Ԃ�
		}

		// �p�X���[�h�w��
		// bs�Œ�p�X���[�h�͑O�㕪���ɂȂ��Ă���
		String passwd = constPassBefore + userPass + constPassAfter;

		// �o�͐�X�g���[��
		ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();

		//�𓀑���Windows�݂̂Ȃ̂ő��M���ŌŒ肵�āA��M����UTF-8�Ŏ󂯂�
		ZipOutputStream zoutputStream = new ZipOutputStream(byteArrayOutputStream, passwd.toCharArray(), Charset.forName("UTF8"));
		ZipParameters zipParams = buildZipParameters();

		try {
			// �p�����[�^�t�@�C����������
			byte[] params = paramTxt.toString().getBytes("UTF-8");
			zipParams.setFileNameInZip(bsParamName);
			zoutputStream.putNextEntry(zipParams);
			zoutputStream.write(params);
			zoutputStream.closeEntry();

			// PDF���̃t�@�C����`�Bzip�t�@�C�����Ɋg���q�t���ŕۑ�����
			if (!jobName.endsWith(".pdf")) {
				jobName = jobName + ".pdf";
			}
			final String _jobName = jobName;

			// PDF�t�@�C����������
			zipParams.setFileNameInZip(_jobName);
			zoutputStream.putNextEntry(zipParams);
			zoutputStream.write(pdfdoc.toByteArray());
			zoutputStream.closeEntry();

		} catch (ZipException e) {
			logger.error(PDFMessages.getMessage(1426, e.getMessage()));
			PDFRuntimeException.throwException(1426, PDFMessages.getMessage(1426), null);
		} finally {
			zoutputStream.close();
		}

		return byteArrayOutputStream.toByteArray();
	}

}
