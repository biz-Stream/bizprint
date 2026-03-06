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
import java.io.UnsupportedEncodingException;

import org.junit.Test;

public class PDFUrlToolsTest {

    // --- encode tests ---

    @Test
    public void testEncodeAsciiAlphanumeric() throws IOException {
        assertEquals("abc123", PDFUrlTools.encode("abc123"));
    }

    @Test
    public void testEncodeSpace() throws IOException {
        assertEquals("hello+world", PDFUrlTools.encode("hello world"));
    }

    @Test
    public void testEncodeJapanese() throws IOException {
        String encoded = PDFUrlTools.encode("\u3042");
        assertEquals("%E3%81%82", encoded);
    }

    @Test
    public void testEncodeMultipleJapanese() throws IOException {
        String encoded = PDFUrlTools.encode("\u3042\u3044\u3046");
        assertEquals("%E3%81%82%E3%81%84%E3%81%86", encoded);
    }

    @Test
    public void testEncodeSpecialChars() throws IOException {
        String encoded = PDFUrlTools.encode("a=b&c");
        assertEquals("a%3Db%26c", encoded);
    }

    @Test
    public void testEncodeEmptyString() throws IOException {
        assertEquals("", PDFUrlTools.encode(""));
    }

    @Test
    public void testEncodeUnchangedChars() throws IOException {
        assertEquals("*-._@", PDFUrlTools.encode("*-._@"));
    }

    @Test
    public void testEncodePercent() throws IOException {
        assertEquals("%25", PDFUrlTools.encode("%"));
    }

    @Test
    public void testEncodeMixedContent() throws IOException {
        String encoded = PDFUrlTools.encode("key=\u5024");
        assertEquals("key%3D%E5%80%A4", encoded);
    }

    // --- decode tests ---

    @Test
    public void testDecodeAscii() throws UnsupportedEncodingException {
        assertEquals("abc123", PDFUrlTools.decode("abc123"));
    }

    @Test
    public void testDecodePlus() throws UnsupportedEncodingException {
        assertEquals("hello world", PDFUrlTools.decode("hello+world"));
    }

    @Test
    public void testDecodePercent() throws UnsupportedEncodingException {
        assertEquals("\u3042", PDFUrlTools.decode("%E3%81%82"));
    }

    @Test
    public void testDecodeMultiplePercent() throws UnsupportedEncodingException {
        assertEquals("\u3042\u3044\u3046",
                PDFUrlTools.decode("%E3%81%82%E3%81%84%E3%81%86"));
    }

    @Test
    public void testDecodeSpecialChars() throws UnsupportedEncodingException {
        assertEquals("a=b&c", PDFUrlTools.decode("a%3Db%26c"));
    }

    @Test
    public void testDecodeEmptyString() throws UnsupportedEncodingException {
        assertEquals("", PDFUrlTools.decode(""));
    }

    @Test
    public void testDecodeMixed() throws UnsupportedEncodingException {
        assertEquals("key=\u5024", PDFUrlTools.decode("key%3D%E5%80%A4"));
    }

    // --- round-trip tests ---

    @Test
    public void testRoundTripAscii() throws IOException {
        String original = "hello world";
        assertEquals(original, PDFUrlTools.decode(PDFUrlTools.encode(original)));
    }

    @Test
    public void testRoundTripJapanese() throws IOException {
        String original = "\u3042\u3044\u3046\u3048\u304A";
        assertEquals(original, PDFUrlTools.decode(PDFUrlTools.encode(original)));
    }

    @Test
    public void testRoundTripMixed() throws IOException {
        String original = "test \u30C6\u30B9\u30C8 123";
        assertEquals(original, PDFUrlTools.decode(PDFUrlTools.encode(original)));
    }
}
