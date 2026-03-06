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

import org.junit.Test;

public class PDFCommonStatusParserTest {

    @Test
    public void testGetInstanceReturnsPDFJaxpStatusParser() {
        PDFCommonStatusParser parser = PDFCommonStatusParser.getInstance();
        assertTrue(parser instanceof PDFJaxpStatusParser);
    }

    @Test
    public void testInitialValuesAreNull() {
        PDFCommonStatusParser parser = PDFCommonStatusParser.getInstance();
        assertNull(parser.getResult());
        assertNull(parser.getErrorCode());
        assertNull(parser.getErrorCause());
        assertNull(parser.getErrorDetails());
        assertNotNull(parser.getPrintStatus());
        assertTrue(parser.getPrintStatus().isEmpty());
    }

    @Test
    public void testParseViaBaseClass() throws Exception {
        String xml = "<Response><Result>SUCCESS</Result></Response>";
        PDFCommonStatusParser parser = PDFCommonStatusParser.getInstance();
        parser.parse(new ByteArrayInputStream(xml.getBytes()));
        assertEquals("SUCCESS", parser.getResult());
    }
}
