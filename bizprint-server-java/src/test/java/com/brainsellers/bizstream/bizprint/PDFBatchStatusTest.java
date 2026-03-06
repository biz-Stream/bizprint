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
import java.util.Collection;

import org.junit.Test;

public class PDFBatchStatusTest {

    // --- createConnectUrl ---

    @Test
    public void testCreateConnectUrlBasic() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.portno = "3000";
        status.createConnectUrl();
        assertEquals("http://localhost:3000/getstatus", status.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithHttpPrefix() {
        PDFBatchStatus status = new PDFBatchStatus("http://example.com");
        status.portno = "3000";
        status.createConnectUrl();
        assertEquals("http://example.com:3000/getstatus", status.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithPort() {
        PDFBatchStatus status = new PDFBatchStatus("http://example.com:8080");
        status.portno = "3000";
        status.createConnectUrl();
        assertEquals("http://example.com:8080/getstatus", status.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithTrailingSlash() {
        PDFBatchStatus status = new PDFBatchStatus("http://example.com/");
        status.portno = "3000";
        status.createConnectUrl();
        assertEquals("http://example.com:3000/getstatus", status.serverUrl);
    }

    @Test
    public void testCreateConnectUrlWithGetstatus() {
        PDFBatchStatus status = new PDFBatchStatus("http://example.com:8080/getstatus");
        status.portno = "3000";
        status.createConnectUrl();
        assertEquals("http://example.com:8080/getstatus", status.serverUrl);
    }

    @Test
    public void testCreateConnectUrlCustomPort() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("9090");
        status.createConnectUrl();
        assertEquals("http://localhost:9090/getstatus", status.serverUrl);
    }

    // --- setBatchStatusPort ---

    @Test
    public void testSetBatchStatusPortValid() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("8080");
        assertEquals("8080", status.portno);
    }

    @Test
    public void testSetBatchStatusPortMinBoundary() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("1024");
        assertEquals("1024", status.portno);
    }

    @Test
    public void testSetBatchStatusPortMaxBoundary() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("65535");
        assertEquals("65535", status.portno);
    }

    @Test
    public void testSetBatchStatusPortBelowMin() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("1023");
        assertEquals("3000", status.portno);
    }

    @Test
    public void testSetBatchStatusPortAboveMax() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("65536");
        assertEquals("3000", status.portno);
    }

    @Test
    public void testSetBatchStatusPortNonNumeric() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.setBatchStatusPort("abc");
        assertEquals("3000", status.portno);
    }

    @Test
    public void testSetBatchStatusPortNull() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.portno = "5000";
        status.setBatchStatusPort(null);
        assertEquals("5000", status.portno);
    }

    @Test
    public void testSetBatchStatusPortEmpty() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        status.portno = "5000";
        status.setBatchStatusPort("");
        assertEquals("5000", status.portno);
    }

    // --- encode / decode ---

    @Test
    public void testEncode() throws IOException {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        assertEquals("hello+world", status.encode("hello world"));
    }

    @Test
    public void testDecode() throws IOException {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        assertEquals("hello world", status.decode("hello+world"));
    }

    // --- getters (initial values) ---

    @Test
    public void testInitialGettersAreNull() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        assertNull(status.getResult());
        assertNull(status.getErrorCode());
        assertNull(status.getErrorCause());
        assertNull(status.getErrorDetails());
    }

    @Test
    public void testGetPrintStatusInitiallyEmpty() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        Collection collection = status.getPrintStatus();
        assertNotNull(collection);
        assertTrue(collection.isEmpty());
    }

    @Test
    public void testGetPrintStatusByJobIdReturnsNull() {
        PDFBatchStatus status = new PDFBatchStatus("localhost");
        assertNull(status.getPrintStatus("nonexistent"));
    }
}
