/*package com.ecolink.app.network.models;

import java.util.List;

public class UploadResponse {
    public String message;
    public int points;
    public String detectedType;
    public boolean success;
    public List<String> newBadges;

    public UploadResponse() {}

    public UploadResponse(String message, int points, String detectedType, boolean success) {
        this.message = message;
        this.points = points;
        this.detectedType = detectedType;
        this.success = success;
    }
}*/
package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class UploadResponse {
    @SerializedName("success")
    public boolean success;

    @SerializedName("message")
    public String message;

    @SerializedName("points")
    public int points;

    @SerializedName("detectedType")
    public String detectedType;

    @SerializedName("newBadges")
    public List<String> newBadges;
}