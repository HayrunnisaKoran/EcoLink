package com.ecolink.app;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.card.MaterialCardView;
import com.journeyapps.barcodescanner.ScanContract;
import com.journeyapps.barcodescanner.ScanOptions;

public class QrScanActivity extends AppCompatActivity {

    private MaterialCardView cvBuildingInfo;
    private View llStatusRow;
    private View llQrFrame;
    private TextView tvQrTitle;
    private TextView tvQrSub;
    private int currentUserId = -1;

    private final ActivityResultLauncher<ScanOptions> barcodeLauncher = registerForActivityResult(new ScanContract(), result -> {
        if (result.getContents() == null) {
            Toast.makeText(this, "QR Okuma İptal Edildi", Toast.LENGTH_SHORT).show();
        } else {
            // 1. Ham veriyi al
            String rawContent = result.getContents();

            // 2. KRİTİK ÇÖZÜM: Rakam olmayan tüm hayalet karakterleri, harfleri ve boşlukları sil
            String cleanBinId = rawContent.replaceAll("[^0-9]", "");

            Log.d("EcoLink_QR", "Ham QR: '" + rawContent + "' -> Temizlenen: '" + cleanBinId + "'");

            // Eğer sonuç boş kalırsa (QR'da hiç sayı yoksa) kamerayı kapatmadan uyar
            if (cleanBinId.isEmpty()) {
                Toast.makeText(this, "Geçersiz QR: Sayı bulunamadı!", Toast.LENGTH_LONG).show();
                return;
            }

            if (llQrFrame != null) llQrFrame.setVisibility(View.GONE);
            if (cvBuildingInfo != null) cvBuildingInfo.setVisibility(View.VISIBLE);
            if (llStatusRow != null) llStatusRow.setVisibility(View.VISIBLE);

            if (tvQrTitle != null) tvQrTitle.setText("Konum Onaylandı!");
            if (tvQrSub != null) tvQrSub.setText("EcoBin #" + cleanBinId + " tespit edildi.");

            new Handler(Looper.getMainLooper()).postDelayed(() -> {
                Intent intent = new Intent(QrScanActivity.this, WasteTypeActivity.class);
                try {
                    // 3. Artık tertemiz olan sayıyı gönül rahatlığıyla çeviriyoruz
                    int binId = Integer.parseInt(cleanBinId);
                    intent.putExtra("BIN_ID", binId);
                    intent.putExtra("USER_ID", currentUserId);
                    startActivity(intent);
                    finish();
                } catch (NumberFormatException e) {
                    Log.e("EcoLink_QR", "Hatalı QR Formatı: " + cleanBinId);
                    Toast.makeText(this, "Geçersiz QR Kod!", Toast.LENGTH_LONG).show();
                    finish();
                }
            }, 1500);
        }
    });

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_qr_scan);

        currentUserId = getIntent().getIntExtra("USER_ID", -1);
        cvBuildingInfo = findViewById(R.id.cvBuildingInfo);
        llStatusRow = findViewById(R.id.llStatusRow);
        llQrFrame = findViewById(R.id.llQrFrame);
        tvQrTitle = findViewById(R.id.tvQrTitle);
        tvQrSub = findViewById(R.id.tvQrSub);

        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        if (llQrFrame != null) {
            llQrFrame.setOnClickListener(v -> openCamera());
        }

        setupBottomMenu();
    }

    private void openCamera() {
        ScanOptions options = new ScanOptions();
        options.setPrompt("Lütfen kutudaki QR kodu okutun");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        options.setCaptureActivity(com.journeyapps.barcodescanner.CaptureActivity.class);
        barcodeLauncher.launch(options);
    }

    private void setupBottomMenu() {
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
                intent.putExtra("USER_ID", currentUserId);
                startActivity(intent);
                overridePendingTransition(0, 0);
                return true;
            }
            return false;
        });
    }
}