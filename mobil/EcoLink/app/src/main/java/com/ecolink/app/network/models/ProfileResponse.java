package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class ProfileResponse {
    @SerializedName(value = "fullName", alternate = {"FullName"})
    private String fullName;

    @SerializedName(value = "facultyName", alternate = {"FacultyName"})
    private String facultyName;

    @SerializedName(value = "points", alternate = {"Points"})
    private int points;

    @SerializedName(value = "level", alternate = {"Level"})
    private int level;

    @SerializedName(value = "progress", alternate = {"Progress"})
    private int progress;

    @SerializedName(value = "totalRecords", alternate = {"TotalRecords"})
    private int totalRecords;

    @SerializedName(value = "compostContribution", alternate = {"CompostContribution"})
    private int compostContribution;

    @SerializedName(value = "currentBadge", alternate = {"CurrentBadge"})
    private String currentBadge;

    public String getFullName() { return fullName != null ? fullName : ""; }
    public String getFacultyName() { return facultyName != null ? facultyName : ""; }
    public int getPoints() { return points; }
    public int getLevel() { return level; }
    public int getProgress() { return progress; }
    public int getTotalRecords() { return totalRecords; }
    public int getCompostContribution() { return compostContribution; }
    public String getCurrentBadge() { return currentBadge != null ? currentBadge : ""; }
}
