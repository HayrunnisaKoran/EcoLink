package com.ecolink.app.network;

import com.ecolink.app.network.models.AuthRequest;
import com.ecolink.app.network.models.AuthResponse;
import com.ecolink.app.network.models.DashboardResponse;
import com.ecolink.app.network.models.LeaderboardItem;
import com.ecolink.app.network.models.ProfileResponse;
import com.ecolink.app.network.models.UploadResponse;
import com.ecolink.app.network.models.User;
import com.ecolink.app.network.models.WasteBin;
import com.ecolink.app.network.models.WasteRecordRequest;

import java.util.List;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.PUT;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ApiService {

    @GET("WasteBin/all")
    Call<List<WasteBin>> getAllBins();

    @GET("UserApi/profile/{userId}")
    Call<User> getUserProfile(@Path("userId") int userId);

    @GET("UserApi/profile/{userId}")
    Call<DashboardResponse> getDashboardData(@Path("userId") int userId);

    @POST("Waste/upload")
    Call<UploadResponse> createWasteRecord(@Body WasteRecordRequest record);

    @PUT("UserApi/updatePoints/{userId}")
    Call<Void> updatePoints(@Path("userId") int userId, @Query("points") int points);

    @GET("UserApi/profile/{userId}")
    Call<ProfileResponse> getUserProfileData(@Path("userId") int userId);

    @POST("Auth/login")
    Call<AuthResponse> loginUser(@Body AuthRequest request);

    @POST("Auth/register")
    Call<AuthResponse> registerUser(@Body AuthRequest request);

    @GET("Leaderboard/students")
    Call<List<LeaderboardItem>> getStudentLeaderboard();

    @GET("Leaderboard/faculties")
    Call<List<LeaderboardItem>> getFacultyLeaderboard();

    @GET("Leaderboard/departments")
    Call<List<LeaderboardItem>> getDepartmentLeaderboard();

    // Kullanıcı bilgilerini güncelleme (PUT talebi)
    @PUT("UserApi/update/{userId}")
    Call<Void> updateUserInfo(@Path("userId") int userId, @Body User user);
}
