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
import java.util.ArrayList;

import org.junit.Test;

public class PDFJaxpStatusParserTest {

    private ByteArrayInputStream toStream(String xml) {
        return new ByteArrayInputStream(xml.getBytes());
    }

    @Test
    public void testParseSuccessResponse() throws Exception {
        String xml = "<Response><Result>SUCCESS</Result></Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));
        assertEquals("SUCCESS", parser.getResult());
        assertNull(parser.getErrorCode());
    }

    @Test
    public void testParseErrorResponse() throws Exception {
        String xml = "<Response>"
                + "<Result>ERROR</Result>"
                + "<ErrorCode>E001</ErrorCode>"
                + "<ErrorCause>Paper jam</ErrorCause>"
                + "<ErrorDetails>Tray 1 paper jam</ErrorDetails>"
                + "</Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));
        assertEquals("ERROR", parser.getResult());
        assertEquals("E001", parser.getErrorCode());
        assertEquals("Paper jam", parser.getErrorCause());
        assertEquals("Tray 1 paper jam", parser.getErrorDetails());
    }

    @Test
    public void testParseSinglePrintStatus() throws Exception {
        String xml = "<Response>"
                + "<Result>SUCCESS</Result>"
                + "<PrintStatus JobId=\"job-001\">"
                + "<JobName>TestJob</JobName>"
                + "<PrinterName>Printer1</PrinterName>"
                + "<DateTime>2024-01-01T12:00:00</DateTime>"
                + "<StatusCode>128</StatusCode>"
                + "<Status>Printed</Status>"
                + "</PrintStatus>"
                + "</Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));

        assertEquals("SUCCESS", parser.getResult());
        ArrayList statuses = parser.getPrintStatus();
        assertEquals(1, statuses.size());

        PDFBatchPrintStatus status = (PDFBatchPrintStatus) statuses.get(0);
        assertEquals("job-001", status.getJobId());
        assertEquals("TestJob", status.getJobName());
        assertEquals("Printer1", status.getPrinterName());
        assertEquals("2024-01-01T12:00:00", status.getDateTime());
        assertEquals("128", status.getStatusCode());
        assertEquals("Printed", status.getStatus());
    }

    @Test
    public void testParseMultiplePrintStatuses() throws Exception {
        String xml = "<Response>"
                + "<Result>SUCCESS</Result>"
                + "<PrintStatus JobId=\"job-001\">"
                + "<JobName>Job1</JobName>"
                + "</PrintStatus>"
                + "<PrintStatus JobId=\"job-002\">"
                + "<JobName>Job2</JobName>"
                + "</PrintStatus>"
                + "</Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));

        ArrayList statuses = parser.getPrintStatus();
        assertEquals(2, statuses.size());

        PDFBatchPrintStatus status1 = (PDFBatchPrintStatus) statuses.get(0);
        assertEquals("job-001", status1.getJobId());
        assertEquals("Job1", status1.getJobName());

        PDFBatchPrintStatus status2 = (PDFBatchPrintStatus) statuses.get(1);
        assertEquals("job-002", status2.getJobId());
        assertEquals("Job2", status2.getJobName());
    }

    @Test
    public void testParsePrintStatusWithError() throws Exception {
        String xml = "<Response>"
                + "<Result>SUCCESS</Result>"
                + "<PrintStatus JobId=\"job-001\">"
                + "<ErrorCode>E002</ErrorCode>"
                + "<ErrorCause>Connection lost</ErrorCause>"
                + "<ErrorDetails>Network error</ErrorDetails>"
                + "</PrintStatus>"
                + "</Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));

        PDFBatchPrintStatus status = (PDFBatchPrintStatus) parser.getPrintStatus().get(0);
        assertEquals("E002", status.getErrorCode());
        assertEquals("Connection lost", status.getErrorCause());
        assertEquals("Network error", status.getErrorDetails());
    }

    @Test
    public void testParsePrintStatusWithoutJobIdIsIgnored() throws Exception {
        String xml = "<Response>"
                + "<Result>SUCCESS</Result>"
                + "<PrintStatus>"
                + "<JobName>NoIdJob</JobName>"
                + "</PrintStatus>"
                + "</Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));

        assertTrue(parser.getPrintStatus().isEmpty());
    }

    @Test
    public void testParseEmptyResponse() throws Exception {
        String xml = "<Response></Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));
        assertNull(parser.getResult());
        assertTrue(parser.getPrintStatus().isEmpty());
    }

    @Test(expected = org.xml.sax.SAXException.class)
    public void testParseDoctypeDeclarationIsRejected() throws Exception {
        String xml = "<?xml version=\"1.0\"?>"
                + "<!DOCTYPE foo ["
                + "<!ENTITY xxe SYSTEM \"file:///etc/passwd\">"
                + "]>"
                + "<Response><Result>&xxe;</Result></Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));
    }

    @Test
    public void testParseResultOnly() throws Exception {
        String xml = "<Response><Result>OK</Result></Response>";
        PDFJaxpStatusParser parser = new PDFJaxpStatusParser();
        parser.parse(toStream(xml));
        assertEquals("OK", parser.getResult());
        assertNull(parser.getErrorCode());
        assertNull(parser.getErrorCause());
        assertNull(parser.getErrorDetails());
        assertTrue(parser.getPrintStatus().isEmpty());
    }
}
