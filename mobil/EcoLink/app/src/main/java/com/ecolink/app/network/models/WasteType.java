package com.ecolink.app.network.models;

public class WasteType {
    private int wasteTypeId;
    private String typeName;
    private String description;
    private int basePoint;
    private String colorCode;
    private String iconName;

    public int getWasteTypeId() { return wasteTypeId; }
    public String getTypeName() { return typeName; }
    public int getBasePoint() { return basePoint; }
    public String getColorCode() { return colorCode; }
}
