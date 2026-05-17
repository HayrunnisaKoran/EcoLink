package com.ecolink.app.network.models;

public class Badge {
    private int badgeId;
    private String badgeName;
    private String description;
    private Integer requiredPoints;
    private Integer requiredRecordCount;
    private String iconName;
    private boolean isActive;

    public int getBadgeId() { return badgeId; }
    public String getBadgeName() { return badgeName; }
    public String getDescription() { return description; }
    public String getIconName() { return iconName; }
}
