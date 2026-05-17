package com.ecolink.app.network.models;

import java.util.Date;

public class User {
    private int userId;
    private String firstName;
    private String lastName;
    private String email;
    private String passwordHash; // Giriş ve Kayıt için
    private String role;
    private Integer facultyId;
    private Integer departmentId;
    private int trustScore;
    private int totalPoints;
    private boolean isActive;
    private Date createdAt;

    // Boş constructor (GSON için)
    public User() {}

    // Login için constructor
    public User(String email, String passwordHash) {
        this.email = email;
        this.passwordHash = passwordHash;
    }

    // Getter ve Setter'lar
    public int getUserId() { return userId; }
    public String getFirstName() { return firstName; }
    public String getLastName() { return lastName; }
    public String getEmail() { return email; }
    public int getTotalPoints() { return totalPoints; }
    public int getTrustScore() { return trustScore; }
    public String getRole() { return role; }
}
