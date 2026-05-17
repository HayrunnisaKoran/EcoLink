package com.ecolink.app.network.models;

public class UserResponse {
    public int userId;
    public String firstName;
    public String lastName;
    public String email;
    public int totalPoints;
    public double trustScore;
    public String facultyName;   // Faculties tablosundan Join ile gelecek
    public String departmentName; // Departments tablosundan Join ile gelecek
}