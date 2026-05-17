package com.ecolink.app.network.models;

import com.google.gson.annotations.SerializedName;

public class AuthRequest {

    // C# tarafının beklediği isimler (Büyük/Küçük harfe tam uyumlu)
    @SerializedName("FirstName")
    public String firstName;

    @SerializedName("LastName")
    public String lastName;

    @SerializedName("Email")
    public String email;

    @SerializedName("Password")
    public String password;

    // 1. KAPI: Sadece GİRİŞ (Login) yaparken kullanılır
    public AuthRequest(String email, String password) {
        this.email = email;
        this.password = password;
    }

    // 2. KAPI: Yeni KAYIT (Register) yaparken kullanılır
    public AuthRequest(String fullName, String email, String password) {
        // Kullanıcının girdiği tek parça ismi (Örn: "Hayrünnisa Koran") boşluktan ikiye bölüyoruz
        String[] parts = fullName.trim().split(" ", 2);
        this.firstName = parts[0]; // İlk kısım Ad (FirstName)
        this.lastName = (parts.length > 1) ? parts[1] : "Öğrenci"; // İkinci kısım Soyad (Yoksa "Öğrenci" yazsın)

        this.email = email;
        this.password = password;
    }
}