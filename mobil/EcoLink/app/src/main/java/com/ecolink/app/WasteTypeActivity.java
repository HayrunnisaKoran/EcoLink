package com.ecolink.app;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.WasteBin;
import com.google.android.material.bottomnavigation.BottomNavigationView;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class WasteTypeActivity extends AppCompatActivity {

    private int binId = -1;
    private int userId = -1;
    private double binLat = 0.0;
    private double binLng = 0.0;
    private TextView tvLocationName;
    private TextView tvLocationDesc;
    private boolean isDataLoaded = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_waste_type);

        tvLocationName = findViewById(R.id.tvLocationName);
        tvLocationDesc = findViewById(R.id.tvLocationDesc);

        binId = getIntent().getIntExtra("BIN_ID", -1);
        userId = getIntent().getIntExtra("USER_ID", -1);

        Log.d("EcoLink_Debug", "WasteTypeActivity Başlatıldı. Aranan BinId: " + binId);

        // Servisten kutu listesini çek ve ID'yi eşleştir
        fetchBinLocation(binId);

        findViewById(R.id.btnBack).setOnClickListener(v -> finish());
        
        // Buton tıklamaları
        findViewById(R.id.kutu_plastik_id_buraya).setOnClickListener(v -> goToCamera("Plastik"));
        findViewById(R.id.kutu_cam_id_buraya).setOnClickListener(v -> goToCamera("Cam"));
        findViewById(R.id.kutu_kagit_id_buraya).setOnClickListener(v -> goToCamera("Kağıt"));
        findViewById(R.id.kutu_metal_id_buraya).setOnClickListener(v -> goToCamera("Metal"));
        findViewById(R.id.kutu_organik_id_buraya).setOnClickListener(v -> goToCamera("Organik"));

        setupBottomNavigation();
    }

    private void fetchBinLocation(int searchedBinId) {
        RetrofitClient.getApiService().getAllBins().enqueue(new Callback<List<WasteBin>>() {
            @Override
            public void onResponse(@NonNull Call<List<WasteBin>> call, @NonNull Response<List<WasteBin>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<WasteBin> bins = response.body();
                    boolean found = false;
                    for (WasteBin bin : bins) {
                        if (bin.getWasteBinId() == searchedBinId) {
                            tvLocationName.setText(bin.getLocationName());
                            tvLocationDesc.setText(getString(R.string.bin_active_format, bin.getWasteBinId()));
                            
                            // SQL'den gelen gerçek koordinatları yakalıyoruz
                            binLat = bin.getLatitude();
                            binLng = bin.getLongitude();
                            isDataLoaded = true;
                            
                            Log.d("EcoLink_Debug", "Kutu Eşleşti: " + bin.getLocationName() + " | Lat: " + binLat + " | Lng: " + binLng);
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        Log.e("EcoLink_Debug", "Kutu bulunamadı! ID: " + searchedBinId);
                        tvLocationName.setText(R.string.bin_not_found);
                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<WasteBin>> call, @NonNull Throwable t) {
                Log.e("EcoLink_Debug", "Servis hatası: " + t.getMessage());
                Toast.makeText(WasteTypeActivity.this, "Kutular yüklenemedi", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void goToCamera(String wasteType) {
        if (!isDataLoaded || binLat == 0.0) {
            Toast.makeText(this, "Konum bilgisi bekleniyor...", Toast.LENGTH_SHORT).show();
            return;
        }

        Intent intent = new Intent(this, PhotoCaptureActivity.class);
        intent.putExtra("BIN_ID", binId);
        intent.putExtra("USER_ID", userId);
        intent.putExtra("SELECTED_WASTE_TYPE", wasteType);
        
        // KRİTİK: Bir sonraki sayfaya SQL'den gelen gerçek koordinatları gönderiyoruz
        intent.putExtra("BIN_LAT", binLat);
        intent.putExtra("BIN_LNG", binLng);
        
        startActivity(intent);
    }

    private void setupBottomNavigation() {
        BottomNavigationView bottomNavigation = findViewById(R.id.bottomNavigation);
        if (bottomNavigation == null) return;
        bottomNavigation.setSelectedItemId(R.id.nav_camera);
        bottomNavigation.setOnItemSelectedListener(item -> {
            int id = item.getItemId();
            if (id == R.id.nav_camera) return true;
            Intent intent = null;
            if (id == R.id.nav_home) intent = new Intent(this, MainActivity.class);
            else if (id == R.id.nav_stats) intent = new Intent(this, StatisticsActivity.class);
            else if (id == R.id.nav_rewards) intent = new Intent(this, RewardsActivity.class);
            else if (id == R.id.nav_profile) intent = new Intent(this, ProfileActivity.class);
            if (intent != null) {
                intent.putExtra("USER_ID", userId);
                startActivity(intent);
                return true;
            }
            return false;
        });
    }
}
