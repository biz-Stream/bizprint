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

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.util.logging.Logger;

/**
 * URL 変換用のツール・クラスです。
 */
public abstract class PDFUrlTools {
    private static final Logger LOGGER = Logger.getLogger(PDFUrlTools.class.getName());

    /**
     *  変換テーブル
     */
    protected static boolean[] UNCHANGES = new boolean[] {
        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,     //
        false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,     //
        false, false, false, false, false, false, false, false, false, false, true,  false, false, true , true , false,     //  !"#$%&'()*+,-./
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, false, false,     // 0123456789:;<=>?
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,      // @ABCDEFGHIJKLMNO
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, true,      // PQRSTUVWXYZ[\]^_
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,      // @abcdefghijklmno
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, false,     // pqrstuvwxyz{|}~
    };

    /**
     *  インスタンスを生成します。
     */
    protected PDFUrlTools() {
        super();
    }

    /**
     *  文字列を URL 文字列に変換します。
     *
     *      @param      text        文字列
     *
     *      @return     URL 文字列
     * @throws IOException
     */
    public static String encode(String text) throws IOException {
        StringBuilder buf = new StringBuilder(text.length());
        boolean changed = false;
        ByteArrayOutputStream bbb = new ByteArrayOutputStream(10);
        OutputStreamWriter writer = new OutputStreamWriter(bbb, "UTF-8");

        for (int i = 0; i < text.length(); i++) {
            int ci = (int) text.charAt(i);

            if ((ci <= 0x7f) && UNCHANGES[ci & 0x7f]) {
                buf.append((char) ci);
            }
            else {
                if (ci == ' ') {
                    buf.append('+');
                    changed = true;
                }
                else {
                    bbb.reset();
                    writer = new OutputStreamWriter(bbb, "UTF-8");

                    writer.write(ci);

                    if ((ci >= 0xd800) && (ci <= 0xdbff)) {
                        if ((i + 1) < text.length()) {
                            ci = (int) text.charAt(i + 1);

                            if ((ci >= 0xdc00) && (ci <= 0xdfff)) {
                                writer.write(ci);
                                i++;
                            }
                        }
                    }

                    writer.flush();

                    byte[] ba = bbb.toByteArray();

                    for (int j = 0; j < ba.length; j++) {
                        buf.append('%');

                        char ch1 = Character.forDigit((ba[j] >> 4) & 0x0f, 16);
                        char ch2 = Character.forDigit( ba[j]       & 0x0f, 16);

                        if (Character.isLetter(ch1)) {
                            ch1 &= 0xdf;
                        }

                        if (Character.isLetter(ch2)) {
                            ch2 &= 0xdf;
                        }

                        buf.append(ch1);
                        buf.append(ch2);
                    }

                    changed = true;
                }
            }
        }

        return (changed ? buf.toString() : text);
    }

    /**
     *  URL 文字列を文字列に変換します。
     *
     *      @param      text        URL 文字列
     *
     *      @return     文字列
     * @throws UnsupportedEncodingException
     */
    public static String decode(String text) throws UnsupportedEncodingException {
        StringBuilder buf = new StringBuilder();
        boolean changed = false;
        int length = text.length();


        for (int i = 0; i < length; i++) {
            char ch = text.charAt(i);

            switch (ch) {
                case '%': {
                    byte[] bytes = new byte[(length - i) / 3];
                    int p = 0;

                    while (((i + 2) < length) && (ch == '%')) {
                        bytes[p++] = (byte) Integer.parseInt(text.substring(i + 1, i + 3), 16);
                        i += 3;

                        if (i < length) {
                            ch = text.charAt(i);
                        }
                    }

                    if ((i < length) && (ch == '%')) {
                        LOGGER.severe("Error in URL decoding.");
                        throw new UnsupportedEncodingException("Error in URL decoding.");
                    }

                    buf.append(new String(bytes, 0, p, "UTF-8"));

                    changed = true;
                    --i;
                    break;
                }

                case '+': {
                    buf.append(' ');
                    changed = true;
                    break;
                }

                default: {
                    buf.append(ch);
                    break;
                }
            }
        }

        return (changed ? buf.toString() : text);
    }
}
