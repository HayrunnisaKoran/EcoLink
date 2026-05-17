package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class AuthResponse {
    @SerializedName("success")
    public boolean success;

    @SerializedName("message")
    public String message;

    // C# tarafı "userId" gönderiyor ("id" DEĞİL)
    @SerializedName("userId")
    public int userId;

    // C# tarafı "firstName" gönderiyor ("fullName" DEĞİL)
    @SerializedName("firstName")
    public String fullName;

    // C# tarafı ekstra olarak "role" gönderiyor (Örn: Student)
    @SerializedName("role")
    public String role;
}