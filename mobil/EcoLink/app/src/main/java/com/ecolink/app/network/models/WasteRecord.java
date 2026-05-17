package com.ecolink.app.network.models;

public class WasteRecord {
    private int userId;
    private int wasteBinId;
    private double latitude;
    private double longitude;
    private String imageBase64;
    private int wasteTypeId;

    public WasteRecord(int userId, int wasteBinId, double latitude, double longitude, String imageBase64, int wasteTypeId) {
        this.userId = userId;
        this.wasteBinId = wasteBinId;
        this.latitude = latitude;
        this.longitude = longitude;
        this.imageBase64 = imageBase64;
        this.wasteTypeId = wasteTypeId;
    }

    // Getter and Setters
    public int getUserId() { return userId; }
    public void setUserId(int userId) { this.userId = userId; }
    public int getWasteBinId() { return wasteBinId; }
    public void setWasteBinId(int wasteBinId) { this.wasteBinId = wasteBinId; }
    public double getLatitude() { return latitude; }
    public void setLatitude(double latitude) { this.latitude = latitude; }
    public double getLongitude() { return longitude; }
    public void setLongitude(double longitude) { this.longitude = longitude; }
    public String getImageBase64() { return imageBase64; }
    public void setImageBase64(String imageBase64) { this.imageBase64 = imageBase64; }
    public int getWasteTypeId() { return wasteTypeId; }
    public void setWasteTypeId(int wasteTypeId) { this.wasteTypeId = wasteTypeId; }
}
