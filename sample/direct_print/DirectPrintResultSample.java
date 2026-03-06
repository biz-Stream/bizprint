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

import java.io.IOException;
import java.io.PrintWriter;
import java.net.URLDecoder;

import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

/**
 * ダイレクト印刷結果画面サンプルプログラム
 *
 * ダイレクト印刷の印刷結果を受け取り、ブラウザに表示するサーブレットのサンプルです。
 * DirectPrintSample の setResponseUrl で指定されたURLとしてデプロイしてください。
 *
 * ダイレクト印刷クライアントは印刷完了後、以下のパラメータをGETリクエストで送信します:
 *   RESULT        - 印刷結果（SUCCESS / FAIL）
 *   ERROR_CODE    - エラーコード
 *   ERROR_CAUSE   - エラーの原因
 *   ERROR_DETAILS - エラーの詳細（URLエンコード済み）
 */
public class DirectPrintResultSample extends HttpServlet {

    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse res)
            throws IOException, ServletException {
        doGet(req, res);
    }

    @Override
    protected void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        response.setContentType("text/html; charset=UTF-8");
        PrintWriter pw = response.getWriter();
        pw.println("<html>");
        pw.println("<head>");
        pw.println("<title>DirectPrint Result</title>");
        pw.println("</head>");
        pw.println("<body>");
        pw.println("<br>");
        pw.println("RESULT = " + escapeHtml(request.getParameter("RESULT")));
        pw.println("<br>");
        pw.println("ERROR_CODE = " + escapeHtml(request.getParameter("ERROR_CODE")));
        pw.println("<br>");
        pw.println("ERROR_CAUSE = " + escapeHtml(request.getParameter("ERROR_CAUSE")));
        pw.println("<br>");
        String s = request.getParameter("ERROR_DETAILS");
        if (s != null) {
            pw.println("ERROR_DETAILS = " + escapeHtml(URLDecoder.decode(s, "UTF-8")));
        } else {
            pw.println("ERROR_DETAILS = null");
        }
        pw.println("<br>");
        pw.println("</body>");
        pw.println("</html>");
        pw.close();
    }

    private static String escapeHtml(String s) {
        if (s == null) {
            return "null";
        }
        return s.replace("&", "&amp;").replace("<", "&lt;")
                .replace(">", "&gt;").replace("\"", "&quot;");
    }
}
