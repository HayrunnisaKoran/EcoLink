package com.ecolink.app;

import android.content.Intent;
import android.os.Bundle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;
import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.tabs.TabLayout;

public class StatisticsActivity extends AppCompatActivity {

    private TabLayout tabLayout;
    private BottomNavigationView bottomNavigationView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_statistics);

        tabLayout = findViewById(R.id.tabLayout);
        bottomNavigationView = findViewById(R.id.bottomNavigation);

        // Uygulama açıldığında ilk sekme (Öğrenciler) seçili gelsin
        replaceFragment(new StudentsFragment());

        // Alt menüde "Sıralama" (nav_stats) ikonunu aktif/seçili olarak göster
        bottomNavigationView.setSelectedItemId(R.id.nav_stats);

        // Sekme Değiştirme Olayı
        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                switch (tab.getPosition()) {
                    case 0:
                        replaceFragment(new StudentsFragment());
                        break;
                    case 1:
                        replaceFragment(new FacultiesFragment());
                        break;
                    case 2:
                        replaceFragment(new DepartmentsFragment());
                        break;
                }
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {}

            @Override
            public void onTabReselected(TabLayout.Tab tab) {}
        });

        // Geri butonu
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // ======== ALT MENÜ YÖNLENDİRMELERİ ========
        bottomNavigationView.setOnItemSelectedListener(item -> {
            int itemId = item.getItemId();

            if (itemId == R.id.nav_home) {
                startActivity(new Intent(StatisticsActivity.this, MainActivity.class));
                overridePendingTransition(0, 0); // KOPUKLUK HİSSİNİ ENGELLEYEN KOD
                finish();
                return true;

            } else if (itemId == R.id.nav_camera) {
                startActivity(new Intent(StatisticsActivity.this, QrScanActivity.class));
                overridePendingTransition(0, 0); // KOPUKLUK HİSSİNİ ENGELLEYEN KOD
                finish();
                return true;

            } else if (itemId == R.id.nav_stats) {
                // Zaten Sıralama sayfasındayız, hiçbir şey yapma
                return true;

            } else if (itemId == R.id.nav_rewards) {
                // Not: Eğer ödüller sayfanın adı farklıysa 'RewardsActivity' kısmını değiştir
                startActivity(new Intent(StatisticsActivity.this, RewardsActivity.class));
                overridePendingTransition(0, 0); // KOPUKLUK HİSSİNİ ENGELLEYEN KOD
                finish();
                return true;

            } else if (itemId == R.id.nav_profile) {
                startActivity(new Intent(StatisticsActivity.this, ProfileActivity.class));
                overridePendingTransition(0, 0); // KOPUKLUK HİSSİNİ ENGELLEYEN KOD
                finish();
                return true;
            }

            return false;
        });
    }

    private void replaceFragment(Fragment fragment) {
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.replace(R.id.fragmentContainer, fragment);
        transaction.commit();
    }
}