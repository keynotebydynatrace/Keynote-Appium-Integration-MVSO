package com.keynote.REST;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.InputStream;
import java.io.StringWriter;
import java.net.MalformedURLException;
import java.net.URL;


public class KeynoteRESTClient {

    private String email;
    private String pass;
    private URL accessServerUrl;
    private String sessionID;


    public KeynoteRESTClient(String email, String pass, String url) throws Exception{
        this.email = email;
        this.pass = pass;
        try {
            this.accessServerUrl = new URL(url);
        }catch(MalformedURLException e){
            // non recoverable error
            throw new Exception("Invalid Access URL given.");
        }
    }

    // disconnect on garbage collection
    protected void finalize() {
        logoutSession();
    }


    public Boolean createSession() throws Exception
    {
        // session id already exists
        if (sessionID != null) {
            throw new Exception("Session already exists. Please instantiate a new KeynoteRESTClient object");
        }

        try {
            InputStream[] stream = createSessionParams();
            JSONObject jsonObject;
            if(stream != null) {
                jsonObject = JSONUtils.getJsonObject(stream[0]);
                String status = (String)jsonObject.get("status");


                System.out.println("Status: " +  status);
                // wasn't able to create session
                if(!status.equals("SUCCESS")){
                    throw new Exception("Failed to create session");
                }

                sessionID = (String)jsonObject.get("sessionID");
                return true;
            }
        } catch(Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    protected InputStream[] createSessionParams() throws JSONException {
        JSONObject jsonObject = new JSONObject();
        jsonObject.put("email", email);
        jsonObject.put("password", pass);
        StringWriter writer = new StringWriter();
        jsonObject.write(writer);
        return RESTUtils.restRequest(accessServerUrl + "/portal/establish-api-session",
                "POST", "application/json", "application/json", writer.toString());
    }

    public String lockDevice(int mcd)
    {
        try {
            StringWriter writer = createLockDeviceParams();
            InputStream stream[] = RESTUtils.restRequest(accessServerUrl + "/device/lock-device/" + mcd,
                    "POST", "application/json", "application/json", writer.toString());
            if(stream != null) {
                JSONObject jsonObject = JSONUtils.getJsonObject(stream[0]);
                //Boolean status = (Boolean)jsonObject.get("success");
                String ensembleURL = (String)jsonObject.get("ensembleServerURL");
                return ensembleURL;
            }
        } catch(Exception e) {
            e.printStackTrace();
        }
        return "";
    }

    protected  StringWriter createLockDeviceParams() throws JSONException {
        JSONObject jsonObject = new JSONObject();
        jsonObject.put("sessionID", sessionID);
        StringWriter writer = new StringWriter();
        jsonObject.write(writer);
        return writer;
    }

    public String startAppium(int mcd)
    {
        try {
            InputStream stream[] =
                    RESTUtils.restRequest(accessServerUrl + "/device/" + sessionID + "/start-appium/" + mcd,
                            "GET", "application/json", "text/plain", null);
            if(stream != null) {
                return BufferUtils.getStringBuffer(stream[0]).toString();
            }
        } catch(Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    public String logoutSession()
    {
        try {
            StringWriter writer = createLockDeviceParams();
            InputStream stream[] =
                    RESTUtils.restRequest(accessServerUrl + "/portal/logout-api-session",
                            "POST", "application/json", "application/json", writer.toString());
            if(stream != null) {
                return BufferUtils.getStringBuffer(stream[0]).toString();
            }
        } catch(Exception e) {
            e.printStackTrace();
        }
        return null;


    }
}
