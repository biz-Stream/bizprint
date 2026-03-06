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

import static org.junit.Assert.*;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.lang.reflect.Field;

import org.junit.Before;
import org.junit.Test;

import net.lingala.zip4j.io.inputstream.ZipInputStream;
import net.lingala.zip4j.model.LocalFileHeader;

public class CreateEncryptSppTest {

    private CreateEncryptSpp spp;

    @Before
    public void setUp() {
        spp = new CreateEncryptSpp();
    }

    private String getPassword(String userPass) throws Exception {
        Field beforeField = CreateEncryptSpp.class.getDeclaredField("constPassBefore");
        beforeField.setAccessible(true);
        Field afterField = CreateEncryptSpp.class.getDeclaredField("constPassAfter");
        afterField.setAccessible(true);
        return (String) beforeField.get(spp) + userPass + (String) afterField.get(spp);
    }

    // --- input validation ---

    @Test(expected = BizPrintException.class)
    public void testCreateCompressedSppNullParamTxt() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write(new byte[]{1});
        spp.createCompressedSpp(null, "job1", pdf);
    }

    @Test(expected = BizPrintException.class)
    public void testCreateCompressedSppNullJobName() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write(new byte[]{1});
        spp.createCompressedSpp("params", null, pdf);
    }

    @Test(expected = BizPrintException.class)
    public void testCreateCompressedSppEmptyJobName() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write(new byte[]{1});
        spp.createCompressedSpp("params", "", pdf);
    }

    @Test(expected = BizPrintException.class)
    public void testCreateCompressedSppNullPdfDoc() throws Exception {
        spp.createCompressedSpp("params", "job1", null);
    }

    @Test(expected = BizPrintException.class)
    public void testCreateCompressedSppEmptyPdfDoc() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        spp.createCompressedSpp("params", "job1", pdf);
    }

    // --- successful creation ---

    @Test
    public void testCreateCompressedSppReturnsNonEmptyBytes() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write("dummy pdf content".getBytes("UTF-8"));
        byte[] result = spp.createCompressedSpp("printerName=test", "testjob", pdf);
        assertNotNull(result);
        assertTrue(result.length > 0);
    }

    @Test
    public void testCreateCompressedSppContainsTwoEntries() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write("dummy pdf content".getBytes("UTF-8"));

        String password = getPassword("");
        byte[] result = spp.createCompressedSpp("printerName=test", "testjob", pdf);

        try (ZipInputStream zis = new ZipInputStream(
                new ByteArrayInputStream(result), password.toCharArray())) {
            int entryCount = 0;
            LocalFileHeader header;
            while ((header = zis.getNextEntry()) != null) {
                entryCount++;
                byte[] buf = new byte[4096];
                while (zis.read(buf) != -1) {
                    // drain
                }
            }
            assertEquals(2, entryCount);
        }
    }

    @Test
    public void testCreateCompressedSppEntryNames() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write("dummy pdf content".getBytes("UTF-8"));

        String password = getPassword("");
        byte[] result = spp.createCompressedSpp("printerName=test", "myjob", pdf);

        try (ZipInputStream zis = new ZipInputStream(
                new ByteArrayInputStream(result), password.toCharArray())) {
            LocalFileHeader header1 = zis.getNextEntry();
            assertEquals("param.txt", header1.getFileName());
            byte[] buf = new byte[4096];
            while (zis.read(buf) != -1) {
                // drain
            }

            LocalFileHeader header2 = zis.getNextEntry();
            assertEquals("myjob.pdf", header2.getFileName());
            while (zis.read(buf) != -1) {
                // drain
            }
        }
    }

    @Test
    public void testCreateCompressedSppJobNameWithPdfExtension() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write("dummy pdf content".getBytes("UTF-8"));

        String password = getPassword("");
        byte[] result = spp.createCompressedSpp("printerName=test", "myjob.pdf", pdf);

        try (ZipInputStream zis = new ZipInputStream(
                new ByteArrayInputStream(result), password.toCharArray())) {
            zis.getNextEntry();
            byte[] buf = new byte[4096];
            while (zis.read(buf) != -1) {
                // drain
            }

            LocalFileHeader header2 = zis.getNextEntry();
            assertEquals("myjob.pdf", header2.getFileName());
            while (zis.read(buf) != -1) {
                // drain
            }
        }
    }

    @Test
    public void testCreateCompressedSppWithUserPassword() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write("dummy pdf content".getBytes("UTF-8"));
        spp.setPassword("userpass");

        String password = getPassword("userpass");
        byte[] result = spp.createCompressedSpp("printerName=test", "testjob", pdf);

        try (ZipInputStream zis = new ZipInputStream(
                new ByteArrayInputStream(result), password.toCharArray())) {
            LocalFileHeader header = zis.getNextEntry();
            assertNotNull(header);
        }
    }

    @Test
    public void testCreateCompressedSppParamContent() throws Exception {
        ByteArrayOutputStream pdf = new ByteArrayOutputStream();
        pdf.write("dummy pdf content".getBytes("UTF-8"));

        String paramText = "printerName=TestPrinter\nnumberOfCopy=3";
        String password = getPassword("");
        byte[] result = spp.createCompressedSpp(paramText, "testjob", pdf);

        try (ZipInputStream zis = new ZipInputStream(
                new ByteArrayInputStream(result), password.toCharArray())) {
            zis.getNextEntry();

            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            byte[] buf = new byte[4096];
            int len;
            while ((len = zis.read(buf)) != -1) {
                baos.write(buf, 0, len);
            }
            String content = baos.toString("UTF-8");
            assertEquals(paramText, content);
        }
    }

    // --- setPassword ---

    @Test
    public void testSetPassword() {
        spp.setPassword("test123");
        // no exception means success; password is used internally
    }
}
