package com.keynote.REST;


import java.io.IOException;
import java.io.InputStream;

/*
    Package Private
    IO BufferUtils
 */

final class BufferUtils {
    static StringBuffer getStringBuffer(InputStream inputStream) throws IOException {
        byte [] returnBytes = new byte[1024];
        StringBuffer buffer = new StringBuffer();
        int length = 0;

        do {
            length = inputStream.read(returnBytes, 0, returnBytes.length);
            if (length > 0) {
                String returnMsg = new String(returnBytes, 0, length, "UTF-8");
                buffer.append(returnMsg);

            }
        } while (length > 0);

        buffer.trimToSize();
        return buffer;
    }
}
