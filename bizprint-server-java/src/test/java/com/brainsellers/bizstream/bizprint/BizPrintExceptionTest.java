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

public class BizPrintExceptionTest {

    @Test
    public void testMessageConstructor() {
        BizPrintException ex = new BizPrintException("test error");
        assertEquals("test error", ex.getMessage());
        assertNull(ex.getCause());
    }

    @Test
    public void testMessageAndCauseConstructor() {
        RuntimeException cause = new RuntimeException("root cause");
        BizPrintException ex = new BizPrintException("wrapper", cause);
        assertEquals("wrapper", ex.getMessage());
        assertSame(cause, ex.getCause());
    }

    @Test
    public void testIsException() {
        BizPrintException ex = new BizPrintException("test");
        assertTrue(ex instanceof Exception);
    }

    @Test
    public void testNullMessage() {
        BizPrintException ex = new BizPrintException(null);
        assertNull(ex.getMessage());
    }

    @Test
    public void testNullCause() {
        BizPrintException ex = new BizPrintException("msg", null);
        assertEquals("msg", ex.getMessage());
        assertNull(ex.getCause());
    }
}
