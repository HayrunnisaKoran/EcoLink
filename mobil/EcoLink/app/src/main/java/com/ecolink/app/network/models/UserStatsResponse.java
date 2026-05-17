package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class UserStatsResponse {

    @SerializedName("totalPoints")
    private int totalPoints;

    @SerializedName("trustScore")
    private int trustScore;

    public int getTotalPoints() {
        return totalPoints;
    }

    public void setTotalPoints(int totalPoints) {
        this.totalPoints = totalPoints;
    }

    public int getTrustScore() {
        return trustScore;
    }

    public void setTrustScore(int trustScore) {
        this.trustScore = trustScore;
    }
}
