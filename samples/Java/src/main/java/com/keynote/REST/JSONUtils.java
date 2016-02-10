package com.keynote.REST;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;

/*
    Package Private
    Static JSON Utils
 */
final class JSONUtils {
    static JSONObject getJsonObject(InputStream inputStream) throws IOException, JSONException {
        JSONObject jsonObject;
        StringBuffer buffer = BufferUtils.getStringBuffer(inputStream);
        jsonObject = new JSONObject(buffer.toString());
        return jsonObject;
    }

}
