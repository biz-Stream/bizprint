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

import java.io.IOException;
import java.util.Base64;

import org.junit.Before;
import org.junit.Test;

public class PDFCommonPrintStreamTest {

    private TestPrintStream stream;

    private static class TestPrintStream extends PDFCommonPrintStream {
        @Override
        public void close() throws IOException {
        }
    }

    @Before
    public void setUp() {
        stream = new TestPrintStream();
    }

    @Test
    public void testWriteInt() throws IOException {
        stream.write(65);
        assertEquals(1, stream.size());
    }

    @Test
    public void testWriteByteArray() throws IOException {
        byte[] data = {1, 2, 3, 4, 5};
        stream.write(data);
        assertEquals(5, stream.size());
    }

    @Test
    public void testWriteByteArrayWithOffset() throws IOException {
        byte[] data = {1, 2, 3, 4, 5};
        stream.write(data, 1, 3);
        assertEquals(3, stream.size());
    }

    @Test
    public void testSizeInitiallyZero() {
        assertEquals(0, stream.size());
    }

    @Test
    public void testSetPrinterName() throws IOException {
        stream.setPrinterName("TestPrinter");
        String params = stream.parameters();
        assertTrue(params.contains("printerName=TestPrinter"));
    }

    @Test
    public void testPrinterNameNotInParametersWhenNull() throws IOException {
        String params = stream.parameters();
        assertFalse(params.contains("printerName="));
    }

    @Test
    public void testSetNumberOfCopyNormal() throws IOException {
        stream.setNumberOfCopy(5);
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=5"));
    }

    @Test
    public void testSetNumberOfCopyBelowMin() throws IOException {
        stream.setNumberOfCopy(0);
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=1"));
    }

    @Test
    public void testSetNumberOfCopyNegative() throws IOException {
        stream.setNumberOfCopy(-10);
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=1"));
    }

    @Test
    public void testSetNumberOfCopyAboveMax() throws IOException {
        stream.setNumberOfCopy(1000);
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=999"));
    }

    @Test
    public void testSetNumberOfCopyBoundaryMin() throws IOException {
        stream.setNumberOfCopy(1);
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=1"));
    }

    @Test
    public void testSetNumberOfCopyBoundaryMax() throws IOException {
        stream.setNumberOfCopy(999);
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=999"));
    }

    @Test
    public void testNumberOfCopyDefaultWhenNotSet() throws IOException {
        String params = stream.parameters();
        assertTrue(params.contains("numberOfCopy=1"));
    }

    @Test
    public void testSetSelectedTray() throws IOException {
        stream.setSelectedTray(PDFCommonPrintStream.TRAY_UPPER);
        String params = stream.parameters();
        assertTrue(params.contains("selectedTray=UPPER"));
    }

    @Test
    public void testSelectedTrayNotInParametersWhenNull() throws IOException {
        String params = stream.parameters();
        assertFalse(params.contains("selectedTray="));
    }

    @Test
    public void testSetJobName() throws IOException {
        stream.setJobName("MyJob");
        String params = stream.parameters();
        assertTrue(params.contains("jobName=MyJob"));
    }

    @Test
    public void testSetJobNameNull() throws IOException {
        stream.setJobName(null);
        String params = stream.parameters();
        assertTrue(params.contains("jobName=JobName_Default"));
    }

    @Test
    public void testSetJobNameEmpty() throws IOException {
        stream.setJobName("");
        String params = stream.parameters();
        assertTrue(params.contains("jobName=JobName_Default"));
    }

    @Test
    public void testJobNameDefaultWhenNotSet() throws IOException {
        String params = stream.parameters();
        assertTrue(params.contains("jobName=JobName_Default"));
    }

    @Test
    public void testSetDoFitTrue() throws IOException {
        stream.setDoFit(true);
        String params = stream.parameters();
        assertTrue(params.contains("doFit=true"));
    }

    @Test
    public void testSetDoFitFalse() throws IOException {
        stream.setDoFit(false);
        String params = stream.parameters();
        assertTrue(params.contains("doFit=false"));
    }

    @Test
    public void testDoFitDefaultIsFalse() throws IOException {
        String params = stream.parameters();
        assertTrue(params.contains("doFit=false"));
    }

    @Test
    public void testSetFromPage() throws IOException {
        stream.setFromPage(3);
        String params = stream.parameters();
        assertTrue(params.contains("fromPage=3"));
    }

    @Test
    public void testFromPageNotInParametersWhenZero() throws IOException {
        stream.setFromPage(0);
        String params = stream.parameters();
        assertFalse(params.contains("fromPage="));
    }

    @Test
    public void testSetToPage() throws IOException {
        stream.setFromPage(1);
        stream.setToPage(5);
        String params = stream.parameters();
        assertTrue(params.contains("toPage=5"));
    }

    @Test
    public void testToPageNotShownWhenLessThanFromPage() throws IOException {
        stream.setFromPage(5);
        stream.setToPage(3);
        String params = stream.parameters();
        assertFalse(params.contains("toPage="));
    }

    @Test
    public void testToPageNotShownWhenZero() throws IOException {
        stream.setToPage(0);
        String params = stream.parameters();
        assertFalse(params.contains("toPage="));
    }

    @Test
    public void testSetPassword() {
        stream.setPassword("secret");
        assertEquals("secret", stream.userPassword);
    }

    @Test
    public void testSetPasswordNull() {
        stream.setPassword(null);
        assertEquals("", stream.userPassword);
    }

    @Test
    public void testSetPasswordWithEncoded() {
        String encoded = Base64.getEncoder().encodeToString("mypassword".getBytes());
        stream.setPasswordWithEncoded(encoded);
        assertEquals("mypassword", stream.userPassword);
    }

    @Test
    public void testSetPasswordWithEncodedNull() {
        stream.setPasswordWithEncoded(null);
        assertEquals("", stream.userPassword);
    }

    @Test
    public void testSetPasswordWithEncodedEmpty() {
        stream.setPasswordWithEncoded("");
        assertEquals("", stream.userPassword);
    }

    @Test
    public void testEncode() throws IOException {
        String result = stream.encode("hello world");
        assertEquals("hello+world", result);
    }

    @Test
    public void testTrayConstants() {
        assertEquals("UPPER", PDFCommonPrintStream.TRAY_UPPER);
        assertEquals("MIDDLE", PDFCommonPrintStream.TRAY_MIDDLE);
        assertEquals("LOWER", PDFCommonPrintStream.TRAY_LOWER);
        assertEquals("MANUAL", PDFCommonPrintStream.TRAY_MANUAL);
        assertEquals("AUTO", PDFCommonPrintStream.TRAY_AUTO);
    }

    @Test
    public void testFlush() throws IOException {
        stream.write(new byte[]{1, 2, 3});
        stream.flush();
        assertEquals(3, stream.size());
    }
}
