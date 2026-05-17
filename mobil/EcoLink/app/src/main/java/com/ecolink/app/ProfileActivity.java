package com.ecolink.app;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.ecolink.app.network.ApiService;
import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.ProfileResponse;
import com.google.android.material.bottomnavigation.BottomNavigationView;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ProfileActivity extends AppCompatActivity {

    private TextView tvUserName, tvUserLevel, tvCo2Saved, tvTreesSaved;
    private LinearLayout btnHistory, btnEditProfile, btnNotifications, btnLogout;
    private int currentUserId = -1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        // 1. Arayüz Elemanlarını Bağla
        initViews();

        // 2. Geri Tuşunu Aktifleştir
        ImageView btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        // 3. Kullanıcı ID'sini Güvenli Hafızadan Al
        SharedPreferences prefs = getSharedPreferences("EcoLinkPrefs", Context.MODE_PRIVATE);
        currentUserId = prefs.getInt("USER_ID", -1);

        // 4. Alt Menü ve Profil İçi Menü Dinleyicilerini Kur
        setupBottomNavigation();
        setupMenuListeners(prefs);

        // 5. Backend'den Verileri Çek
        if (currentUserId != -1) {
            fetchProfileData(currentUserId);
        } else {
            Toast.makeText(this, "Oturum hatası, lütfen tekrar giriş yapın.", Toast.LENGTH_LONG).show();
            startActivity(new Intent(this, LoginActivity.class));
            finish();
        }
    }

    private void initViews() {
        // Üst Kısım ve İstatistikler
        tvUserName = findViewById(R.id.tvUserName);
        tvUserLevel = findViewById(R.id.tvUserLevel);
        tvCo2Saved = findViewById(R.id.tvCo2Saved);
        tvTreesSaved = findViewById(R.id.tvTreesSaved);

        // Hesap Yönetimi Alt Menüleri
        btnHistory = findViewById(R.id.btnHistory);
        btnEditProfile = findViewById(R.id.btnEditProfile);
        btnNotifications = findViewById(R.id.btnNotifications);
        btnLogout = findViewById(R.id.btnLogout);
    }

    // ProfileActivity.java içindeki metodu şu şekilde güncelle:

    private void setupMenuListeners(SharedPreferences prefs) {

        // 1. ATIK GEÇMİŞİ: Artık aktif!
        btnHistory.setOnClickListener(v -> {
            Intent intent = new Intent(ProfileActivity.this, HistoryActivity.class);
            intent.putExtra("USER_ID", currentUserId); // ID'yi mutlaka taşıyoruz
            startActivity(intent);
        });

        // 2. PROFİLİ DÜZENLE: Aktif hale getiriyoruz
        btnEditProfile.setOnClickListener(v -> {
            // EditProfileActivity'yi oluşturduğunda bu satırı açabilirsin
            // Intent intent = new Intent(ProfileActivity.this, EditProfileActivity.class);
            // intent.putExtra("USER_ID", currentUserId);
            // startActivity(intent);
            Toast.makeText(this, "Düzenleme ekranına yönlendiriliyorsunuz...", Toast.LENGTH_SHORT).show();
        });

        // 3. BİLDİRİM AYARLARI
        btnNotifications.setOnClickListener(v -> {
            Toast.makeText(this, "Bildirim ayarları çok yakında aktif olacak.", Toast.LENGTH_SHORT).show();
        });

        // 4. ÇIKIŞ YAP: Dinamik ve aktif temizlik
        btnLogout.setOnClickListener(v -> {
            prefs.edit().clear().apply(); // Tüm verileri sil
            Intent intent = new Intent(ProfileActivity.this, LoginActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
            startActivity(intent);
            finish();
        });
    }

    private void fetchProfileData(int userId) {
        ApiService apiService = RetrofitClient.getApiService();
        apiService.getUserProfileData(userId).enqueue(new Callback<ProfileResponse>() {
            @Override
            public void onResponse(Call<ProfileResponse> call, Response<ProfileResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ProfileResponse profile = response.body();

                    // Backend'den gelen verileri UI thread'inde ekrana basıyoruz
                    runOnUiThread(() -> {
                        tvUserName.setText(profile.getFullName());

                        String status = profile.getCurrentBadge() + " • Seviye " + profile.getLevel();
                        tvUserLevel.setText(status);

                        tvCo2Saved.setText(profile.getTotalRecords() + " Kayıt");
                        tvTreesSaved.setText(profile.getCompostContribution() + " Kompost");
                    });
                } else {
                    Log.e("Profile_API", "Veri çekilemedi. Hata Kodu: " + response.code());
                }
            }

            @Override
            public void onFailure(Call<ProfileResponse> call, Throwable t) {
                Log.e("Profile_API", "Sunucuya bağlanılamadı: " + t.getMessage());
                Toast.makeText(ProfileActivity.this, "Veriler güncellenemedi, bağlantınızı kontrol edin.", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void setupBottomNavigation() {
        BottomNavigationView bottomNavigation = findViewById(R.id.bottomNavigation);
        if (bottomNavigation != null) {
            bottomNavigation.setSelectedItemId(R.id.nav_profile);

            bottomNavigation.setOnItemSelectedListener(item -> {
                int id = item.getItemId();
                if (id == R.id.nav_profile) return true; // Zaten bu sayfadayız

                Intent intent = null;
                if (id == R.id.nav_home) {
                    intent = new Intent(this, MainActivity.class);
                } else if (id == R.id.nav_camera) {
                    intent = new Intent(this, QrScanActivity.class);
                } else if (id == R.id.nav_stats) {
                    intent = new Intent(this, StatisticsActivity.class);
                } else if (id == R.id.nav_rewards) {
                    intent = new Intent(this, RewardsActivity.class);
                }

                if (intent != null) {
                    // Kullanıcı ID'sini diğer sayfalara taşımayı unutmuyoruz
                    intent.putExtra("USER_ID", currentUserId);
                    startActivity(intent);
                    overridePendingTransition(0, 0);
                    return true;
                }
                return false;
            });
        }
    }
}