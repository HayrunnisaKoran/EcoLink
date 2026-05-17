package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class WasteBin {

    @SerializedName(value = "id", alternate = {"Id", "wasteBinId", "WasteBinId"})
    private Integer wasteBinId;

    @SerializedName(value = "location", alternate = {"Location", "locationName", "LocationName"})
    private String locationName;

    @SerializedName(value = "type", alternate = {"Type", "typeName", "TypeName"})
    private String type;

    // C#'tan gelen 'Latitude' ve 'Longitude' değerlerini tam yakalamak için
    @SerializedName(value = "latitude", alternate = {"Latitude", "lat"})
    private Double latitude;

    @SerializedName(value = "longitude", alternate = {"Longitude", "lng"})
    private Double longitude;

    @SerializedName(value = "fillLevel", alternate = {"FillLevel", "fillLevelPercent", "FillLevelPercent"})
    private Double fillLevelPercent;

    public int getWasteBinId() { return wasteBinId != null ? wasteBinId : 0; }
    public String getLocationName() { return locationName != null ? locationName : "Bilinmeyen Konum"; }
    public String getType() { return type != null ? type : "Bilinmiyor"; }
    public double getLatitude() { return latitude != null ? latitude : 0.0; }
    public double getLongitude() { return longitude != null ? longitude : 0.0; }
    public double getFillLevelPercent() { return fillLevelPercent != null ? fillLevelPercent : 0.0; }
}
