package com.keynote.REST;

import java.io.InputStream;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Arrays;

final class RESTUtils {
   static InputStream[] restRequest(String url, String method, String contentType, String acceptType, String body) {
        InputStream response = null;
        InputStream responseError = null;
        HttpURLConnection conn = null;

        try {
            conn = (HttpURLConnection) new URL(url).openConnection();

            conn.setRequestMethod(method);

            conn.setRequestProperty("Content-Type", contentType);
            conn.setRequestProperty("Accept", acceptType);
            conn.setRequestProperty("Content-transfer-encoding", "UTF-8");
            conn.setRequestProperty("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            conn.setDoInput(true);

            if (body != null) {
                conn.setDoOutput(true);

                final OutputStream output = conn.getOutputStream();
                final OutputStreamWriter osWriter = new OutputStreamWriter(output, "UTF-8");

                osWriter.write(body);
                osWriter.flush();
                osWriter.close();
                output.close();
            }

            conn.connect();
            conn.disconnect();

            responseError = conn.getErrorStream();
            response = conn.getInputStream();
        } catch(Exception e) {
            e.printStackTrace();
        }

        return Arrays.asList(response, responseError).toArray(new InputStream[0]);
    }
}
