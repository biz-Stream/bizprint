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

import java.io.UnsupportedEncodingException;

import org.junit.Test;

public class PDFBatchPrintStreamTest {

    // --- createConnectUrl ---

    @Test
    public void testCreateConnectUrlBasic() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.createConnectUrl();
        assertEquals("http://localhost:3000/doprint", stream.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithHttpPrefix() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("http://example.com");
        stream.createConnectUrl();
        assertEquals("http://example.com:3000/doprint", stream.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithPort() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("http://example.com:8080");
        stream.createConnectUrl();
        assertEquals("http://example.com:8080/doprint", stream.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithTrailingSlash() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("http://example.com/");
        stream.createConnectUrl();
        assertEquals("http://example.com:3000/doprint", stream.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithDoprint() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("http://example.com:8080/doprint");
        stream.createConnectUrl();
        assertEquals("http://example.com:8080/doprint", stream.serverUrl);
    }

    @Test
    public void testCreateConnectUrlCustomPort() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("9090");
        stream.createConnectUrl();
        assertEquals("http://localhost:9090/doprint", stream.serverUrl);
    }

    // --- setBatchPrintPort ---

    @Test
    public void testSetBatchPrintPortValid() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("8080");
        assertEquals("8080", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortMinBoundary() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("1024");
        assertEquals("1024", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortMaxBoundary() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("65535");
        assertEquals("65535", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortBelowMin() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("1023");
        assertEquals("3000", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortAboveMax() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("65536");
        assertEquals("3000", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortNonNumeric() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("abc");
        assertEquals("3000", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortNull() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort(null);
        assertEquals("3000", stream.portno);
    }

    @Test
    public void testSetBatchPrintPortEmpty() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        stream.setBatchPrintPort("");
        assertEquals("3000", stream.portno);
    }

    // --- decode ---

    @Test
    public void testDecode() throws UnsupportedEncodingException {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        assertEquals("hello world", stream.decode("hello+world"));
    }

    @Test
    public void testDecodePercent() throws UnsupportedEncodingException {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        assertEquals("a=b", stream.decode("a%3Db"));
    }

    // --- getters (initial values) ---

    @Test
    public void testInitialGettersAreNull() {
        PDFBatchPrintStream stream = new PDFBatchPrintStream("localhost");
        assertNull(stream.getResult());
        assertNull(stream.getErrorCode());
        assertNull(stream.getErrorCause());
        assertNull(stream.getErrorDetails());
        assertNull(stream.getJobId());
    }
}
