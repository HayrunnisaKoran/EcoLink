package com.ecolink.app;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;
import android.widget.CheckBox;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.DashboardResponse;
import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.button.MaterialButton;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity {

    private TextView tvGreeting, tvPoints, tvWeight, tvCount, tvLevelName;
    private MaterialButton btnOpenMap;
    private CheckBox cbTask1, cbTask2;
    private int userId = -1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // 1. Arayüz elemanlarını bağla
        initViews();

        // 2. SharedPreferences'tan ID ile birlikte ismi de okuyalım
        SharedPreferences prefs = getSharedPreferences("EcoLinkPrefs", Context.MODE_PRIVATE);
        userId = prefs.getInt("USER_ID", -1);
        String savedName = prefs.getString("USER_NAME", "Kullanıcı"); // Login'de kaydedilen isim

        // Sunucudan veri gelene kadar yerel hafızadaki ismi göster
        if (tvGreeting != null) {
            tvGreeting.setText(getString(R.string.welcome_user, savedName));
        }

        // Eğer hafızada yoksa, Login'den gelen Intent'e bak
        if (userId == -1) {
            userId = getIntent().getIntExtra("USER_ID", -1);
        }

        // Güvenlik Kontrolü: Hala ID yoksa Login'e geri gönder
        if (userId == -1) {
            Toast.makeText(this, R.string.session_error, Toast.LENGTH_LONG).show();
            startActivity(new Intent(this, LoginActivity.class));
            finish();
            return;
        }

        // 3. Navigasyon ve Dinamik Veri Çekme
        setupBottomNavigation();
        setupListeners();

        // Sunucudan (Backend) güncel verileri çek
        fetchDashboardData(userId);
    }

    private void initViews() {
        tvGreeting = findViewById(R.id.tvGreeting);
        tvPoints = findViewById(R.id.tvPoints);
        tvWeight = findViewById(R.id.tvWeight);
        tvCount = findViewById(R.id.tvCount);
        tvLevelName = findViewById(R.id.tvLevelName);
        btnOpenMap = findViewById(R.id.btnOpenMap);
        cbTask1 = findViewById(R.id.cbTask1);
        cbTask2 = findViewById(R.id.cbTask2);
    }

    private void setupListeners() {
        // Harita Butonu Geçişi (ID taşıyarak)
        if (btnOpenMap != null) {
            btnOpenMap.setOnClickListener(v -> {
                Intent intent = new Intent(MainActivity.this, MapActivity.class);
                intent.putExtra("USER_ID", userId);
                startActivity(intent);
            });
        }

        // Görev Takibi
        if (cbTask1 != null) {
            cbTask1.setOnCheckedChangeListener((buttonView, isChecked) -> {
                int points = isChecked ? 50 : -50;
                updateServerPoints(points, isChecked ? "Görev tamamlandı!" : "Geri alındı.");
            });
        }

        if (cbTask2 != null) {
            cbTask2.setOnCheckedChangeListener((buttonView, isChecked) -> {
                int points = isChecked ? 80 : -80;
                updateServerPoints(points, isChecked ? "Karbon desteği sağlandı!" : "Geri alındı.");
            });
        }
    }

    private void fetchDashboardData(int userId) {
        RetrofitClient.getApiService().getDashboardData(userId).enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<DashboardResponse> call, @NonNull Response<DashboardResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    updateUI(response.body());
                } else {
                    Log.e("EcoLink_API", "Dashboard verisi alınamadı. Kod: " + response.code());
                }
            }

            @Override
            public void onFailure(@NonNull Call<DashboardResponse> call, @NonNull Throwable t) {
                Log.e("EcoLink_API", "Bağlantı hatası: " + t.getMessage());
            }
        });
    }

    private void updateUI(DashboardResponse data) {
        runOnUiThread(() -> {
            try {
                if (tvGreeting != null) tvGreeting.setText(getString(R.string.welcome_user, data.fullName));

                if (tvPoints != null) tvPoints.setText(String.valueOf(data.totalPoints));

                if (tvWeight != null) tvWeight.setText(getString(R.string.label_kg, String.valueOf(data.totalWasteWeight)));
                if (tvCount != null) tvCount.setText(getString(R.string.profile_records_count, data.totalWasteCount));
                if (tvLevelName != null) tvLevelName.setText(getString(R.string.level_prefix) + ": " + data.level);
            } catch (Exception e) {
                Log.e("MainActivity", "UI update failed", e);
            }
        });
    }

    private void updateServerPoints(int pointsToAdd, String message) {
        RetrofitClient.getApiService().updatePoints(userId, pointsToAdd).enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<Void> call, @NonNull Response<Void> response) {
                if (response.isSuccessful()) {
                    // Puan başarıyla güncellendiğinde arayüzü de tazele
                    fetchDashboardData(userId);
                    Toast.makeText(MainActivity.this, message, Toast.LENGTH_SHORT).show();
                }
            }
            @Override
            public void onFailure(@NonNull Call<Void> call, @NonNull Throwable t) {
                Toast.makeText(MainActivity.this, "Puan senkronize edilemedi.", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void setupBottomNavigation() {
        BottomNavigationView bottomNavigation = findViewById(R.id.bottomNavigation);
        if (bottomNavigation == null) return;
        
        bottomNavigation.setSelectedItemId(R.id.nav_home);
        bottomNavigation.setOnItemSelectedListener(item -> {
            int id = item.getItemId();
            if (id == R.id.nav_home) return true;

            Intent intent = null;
            if (id == R.id.nav_camera) intent = new Intent(this, QrScanActivity.class);
            else if (id == R.id.nav_stats) intent = new Intent(this, StatisticsActivity.class);
            else if (id == R.id.nav_rewards) intent = new Intent(this, RewardsActivity.class);
            else if (id == R.id.nav_profile) intent = new Intent(this, ProfileActivity.class);

            if (intent != null) {
                intent.putExtra("USER_ID", userId);
                startActivity(intent);
                overridePendingTransition(0, 0);
                return true;
            }
            return false;
        });
    }
}