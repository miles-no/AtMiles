package no.miles.atmiles;

import java.io.Serializable;

import org.json.JSONObject;

public class AuthenticationActivityResult implements Serializable {
    public String accessToken;
    public String JsonWebToken;
    public JSONObject User;
}