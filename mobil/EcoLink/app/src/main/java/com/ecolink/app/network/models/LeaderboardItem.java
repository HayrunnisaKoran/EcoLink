package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class LeaderboardItem {
    @SerializedName("name")
    public String name;

    @SerializedName("points")
    public int points;

    @SerializedName("subText") // Fakülte adı veya Bölüm adı için
    public String subText;

    @SerializedName("rank")
    public int rank;
}