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

public class PDFBatchPrintStatusTest {

    @Test
    public void testConstructorSetsJobId() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        assertEquals("job-001", status.getJobId());
    }

    @Test
    public void testConstructorInitializesFieldsToNull() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        assertNull(status.getJobName());
        assertNull(status.getPrinterName());
        assertNull(status.getDateTime());
        assertNull(status.getStatusCode());
        assertNull(status.getStatus());
        assertNull(status.getErrorCode());
        assertNull(status.getErrorCause());
        assertNull(status.getErrorDetails());
    }

    @Test
    public void testSetAndGetJobName() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setJobName("TestJob");
        assertEquals("TestJob", status.getJobName());
    }

    @Test
    public void testSetAndGetPrinterName() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setPrinterName("Printer1");
        assertEquals("Printer1", status.getPrinterName());
    }

    @Test
    public void testSetAndGetDateTime() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setDateTime("2024-01-01T12:00:00");
        assertEquals("2024-01-01T12:00:00", status.getDateTime());
    }

    @Test
    public void testSetAndGetStatusCode() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setStatusCode("200");
        assertEquals("200", status.getStatusCode());
    }

    @Test
    public void testSetAndGetStatus() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setStatus("Printed");
        assertEquals("Printed", status.getStatus());
    }

    @Test
    public void testSetAndGetErrorCode() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setErrorCode("E001");
        assertEquals("E001", status.getErrorCode());
    }

    @Test
    public void testSetAndGetErrorCause() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setErrorCause("Paper jam");
        assertEquals("Paper jam", status.getErrorCause());
    }

    @Test
    public void testSetAndGetErrorDetails() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus("job-001");
        status.setErrorDetails("Tray 1 paper jam");
        assertEquals("Tray 1 paper jam", status.getErrorDetails());
    }

    @Test
    public void testStatusConstants() {
        assertEquals(0x00000001, PDFBatchPrintStatus.JOB_STATUS_PAUSED);
        assertEquals(0x00000002, PDFBatchPrintStatus.JOB_STATUS_ERROR);
        assertEquals(0x00000004, PDFBatchPrintStatus.JOB_STATUS_DELETING);
        assertEquals(0x00000008, PDFBatchPrintStatus.JOB_STATUS_SPOOLING);
        assertEquals(0x00000010, PDFBatchPrintStatus.JOB_STATUS_PRINTING);
        assertEquals(0x00000020, PDFBatchPrintStatus.JOB_STATUS_OFFLINE);
        assertEquals(0x00000040, PDFBatchPrintStatus.JOB_STATUS_PAPEROUT);
        assertEquals(0x00000080, PDFBatchPrintStatus.JOB_STATUS_PRINTED);
        assertEquals(0x00000100, PDFBatchPrintStatus.JOB_STATUS_DELETED);
        assertEquals(0x00000200, PDFBatchPrintStatus.JOB_STATUS_BLOCKED_DEVQ);
        assertEquals(0x00000400, PDFBatchPrintStatus.JOB_STATUS_USER_INTERVENTION);
        assertEquals(0x00000800, PDFBatchPrintStatus.JOB_STATUS_RESTART);
        assertEquals(0x00001000, PDFBatchPrintStatus.JOB_STATUS_COMPLETE);
    }

    @Test
    public void testConstructorWithNullJobId() {
        PDFBatchPrintStatus status = new PDFBatchPrintStatus(null);
        assertNull(status.getJobId());
    }
}
