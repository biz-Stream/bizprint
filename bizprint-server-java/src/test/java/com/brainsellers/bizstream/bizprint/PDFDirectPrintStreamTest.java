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

import org.junit.Test;

public class PDFDirectPrintStreamTest {

    @Test
    public void testSetResponseUrl() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setResponseUrl("http://example.com/callback");
        assertEquals("http://example.com/callback", stream.responseUrl);
    }

    @Test
    public void testSetSaveFileName() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setSaveFileName("output.pdf");
        assertEquals("output.pdf", stream.saveFileName);
    }

    @Test
    public void testSetTarget() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setTarget("_blank");
        assertEquals("_blank", stream.target);
    }

    @Test
    public void testSetPrintDialogTrue() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setPrintDialog(true);
        assertEquals(Boolean.TRUE, stream.printDialog);
    }

    @Test
    public void testSetPrintDialogFalse() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setPrintDialog(false);
        assertEquals(Boolean.FALSE, stream.printDialog);
    }

    @Test
    public void testSetSppNameUnifiedTrue() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setSppNameUnified(true);
        assertEquals(Boolean.TRUE, stream.sppnameUnified);
    }

    @Test
    public void testSetSppNameUnifiedFalse() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        stream.setSppNameUnified(false);
        assertEquals(Boolean.FALSE, stream.sppnameUnified);
    }

    @Test
    public void testInitialValues() {
        PDFDirectPrintStream stream = new PDFDirectPrintStream(null);
        assertNull(stream.responseUrl);
        assertNull(stream.saveFileName);
        assertNull(stream.target);
        assertNull(stream.printDialog);
        assertTrue(stream.sppnameUnified);
    }
}
