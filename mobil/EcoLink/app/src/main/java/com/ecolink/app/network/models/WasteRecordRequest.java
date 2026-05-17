package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class WasteRecordRequest {

    @SerializedName(value = "userId", alternate = {"UserId"})
    private int userId;

    @SerializedName(value = "wasteBinId", alternate = {"WasteBinId"})
    private int wasteBinId;

    @SerializedName(value = "wasteTypeId", alternate = {"WasteTypeId"})
    public int wasteTypeId;

    @SerializedName(value = "latitude", alternate = {"Latitude"})
    public double latitude;

    @SerializedName(value = "longitude", alternate = {"Longitude"})
    public double longitude;

    @SerializedName(value = "photoBase64", alternate = {"PhotoBase64", "ImageBase64"})
    private String photoBase64;

    public WasteRecordRequest(int userId, int wasteBinId, int wasteTypeId, double latitude, double longitude, String photoBase64) {
        this.userId = userId;
        this.wasteBinId = wasteBinId;
        this.wasteTypeId = wasteTypeId;
        this.latitude = latitude;
        this.longitude = longitude;
        this.photoBase64 = photoBase64;
    }
}
