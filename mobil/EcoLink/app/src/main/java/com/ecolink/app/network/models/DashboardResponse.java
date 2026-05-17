package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class DashboardResponse {
    @SerializedName(value = "fullName", alternate = {"FullName"})
    public String fullName;

    @SerializedName(value = "points", alternate = {"Points", "totalPoints", "TotalPoints"})
    public int totalPoints;

    @SerializedName(value = "totalRecords", alternate = {"TotalRecords", "totalWasteCount", "TotalWasteCount"})
    public int totalWasteCount;

    @SerializedName(value = "totalWeight", alternate = {"TotalWeight", "totalWasteWeight", "TotalWasteWeight"})
    public double totalWasteWeight;

    @SerializedName(value = "level", alternate = {"Level"})
    public String level;
}
