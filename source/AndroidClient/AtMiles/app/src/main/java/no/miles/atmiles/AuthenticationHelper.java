package no.miles.atmiles;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import no.miles.atmiles.employee.Constants;

public class AuthenticationHelper {
    static final String ClientID = "6jsWdVCPDiKSdSKi2n7naqmy7eeO703H";
    static final String Tenant = "atmiles";
    static final String Callback = "oob://atmiles/android";
    //static final String Connection = "";

    public void checkLogin(Activity activity) {
        SharedPreferences settings = activity.getSharedPreferences(Constants.SETTINGS_NAME, 0);
        boolean shouldDoLogin = false;

        if(!settings.contains(Constants.SETTINGS_JSON_WEB_TOKEN)){
            shouldDoLogin = true;
        }

        if(!settings.contains(Constants.SETTINGS_JSON_WEB_TOKEN_EXPIRE)){
            shouldDoLogin = true;
        }
        else{
            long expire = settings.getLong(Constants.SETTINGS_JSON_WEB_TOKEN_EXPIRE,0L)*1000L;
            long now = java.lang.System.currentTimeMillis();
            if (expire < now){
                shouldDoLogin = true;
            }
        }

        if(shouldDoLogin){
            sendToLogin(activity);
        }
    }

    public void cleanLoginData(Activity activity) {
        SharedPreferences settings = activity.getSharedPreferences(Constants.SETTINGS_NAME,0);
        SharedPreferences.Editor editor = settings.edit();
        editor.clear();
        editor.commit();
    }

    public String getJsonWebToken(Activity activity) {
        checkLogin(activity);
        SharedPreferences settings = activity.getSharedPreferences(Constants.SETTINGS_NAME,0);
        return settings.getString(Constants.SETTINGS_JSON_WEB_TOKEN, "");
    }

    private void sendToLogin(Activity activity) {
        Intent authActivityIntent = new Intent(activity,
                no.miles.atmiles.AuthenticationActivity.class);
        AuthenticationActivitySetup setup;
        setup = new AuthenticationActivitySetup(Tenant, ClientID, Callback);
        authActivityIntent.putExtra(AuthenticationActivity.AUTHENTICATION_SETUP, setup);
        activity.startActivityForResult(authActivityIntent, AuthenticationActivity.AUTH_REQUEST_COMPLETE);
    }
}
