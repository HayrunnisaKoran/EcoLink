package com.ecolink.app;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ImageView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.tabs.TabLayout;

public class RewardsActivity extends AppCompatActivity {

    private int userId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_rewards);

        userId = getIntent().getIntExtra("USER_ID", -1);

        // 1. Geri Tuşunu Aktifleştir
        ImageView btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        // 2. Alt Menü (Bottom Navigation) Ayarları
        setupBottomNavigation();

        // 3. Tab (Sekme) ve Fragment Ayarları
        setupTabs();
    }

    private void setupBottomNavigation() {
        BottomNavigationView bottomNavigation = findViewById(R.id.bottomNavigation);
        if (bottomNavigation != null) {
            // Hangi sayfada olduğumuzu belirtiyoruz (Bu sayede ikon koyu yeşil yanacak)
            bottomNavigation.setSelectedItemId(R.id.nav_rewards);

            bottomNavigation.setOnItemSelectedListener(item -> {
                int id = item.getItemId();
                if (id == R.id.nav_rewards) return true; // Zaten bu sayfadayız

                Intent intent = null;
                if (id == R.id.nav_home) {
                    intent = new Intent(this, MainActivity.class);
                } else if (id == R.id.nav_camera) {
                    intent = new Intent(this, QrScanActivity.class);
                } else if (id == R.id.nav_stats) {
                    intent = new Intent(this, StatisticsActivity.class);
                } else if (id == R.id.nav_profile) {
                    intent = new Intent(this, ProfileActivity.class);
                }

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

    private void setupTabs() {
        TabLayout tabLayout = findViewById(R.id.rewardsTabLayout);
        if (tabLayout == null) return;

        // İlk açılışta "Tümü" fragment'ını yükle
        loadFragment(new RewardsAllFragment());

        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                switch (tab.getPosition()) {
                    case 0:
                        loadFragment(new RewardsAllFragment());
                        break;
                    case 1:
                        loadFragment(new RewardsOnlyFragment());
                        break;
                    case 2:
                        loadFragment(new RewardsPenaltiesFragment());
                        break;
                }
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {}

            @Override
            public void onTabReselected(TabLayout.Tab tab) {}
        });
    }

    private void loadFragment(Fragment fragment) {
        getSupportFragmentManager()
                .beginTransaction()
                .replace(R.id.rewardsContainer, fragment)
                .commit();
    }
}