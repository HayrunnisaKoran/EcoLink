/*package com.ecolink.app;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ImageView;

import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.button.MaterialButton;

public class VerificationResultActivity extends AppCompatActivity {

    private int userId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_verification_result);

        userId = getIntent().getIntExtra("USER_ID", -1);

        // 1. Geri Butonu
        ImageView btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        // 2. Tamamla Butonu
        MaterialButton btnDone = findViewById(R.id.btnDone);
        if (btnDone != null) {
            btnDone.setOnClickListener(v -> {
                // Ana sayfaya dön ve tüm geçmişi temizle
                Intent intent = new Intent(VerificationResultActivity.this, MainActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                intent.putExtra("USER_ID", userId);
                startActivity(intent);
            });
        }
    }
}*/
package com.ecolink.app;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.button.MaterialButton;

public class VerificationResultActivity extends AppCompatActivity {

    private int userId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_verification_result);

        // Intent'ten gelen dinamik verileri al
        userId = getIntent().getIntExtra("USER_ID", -1);
        int binId = getIntent().getIntExtra("BIN_ID", -1);
        int points = getIntent().getIntExtra("POINTS", 0);
        String detectedType = getIntent().getStringExtra("DETECTED_TYPE");

        // UI Elemanlarını bağla
        TextView tvWasteType = findViewById(R.id.tvWasteType);
        TextView tvBinId = findViewById(R.id.tvBinId);
        TextView tvPoints = findViewById(R.id.tvPoints);

        // API'den dönen verileri ekrana yazdır
        if (detectedType != null) {
            tvWasteType.setText(detectedType);
        }

        if (binId != -1) {
            tvBinId.setText("EcoBin #" + binId);
        }

        tvPoints.setText("+" + points);

        // 1. Geri Butonu
        ImageView btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        // 2. Tamamla Butonu
        MaterialButton btnDone = findViewById(R.id.btnDone);
        if (btnDone != null) {
            btnDone.setOnClickListener(v -> {
                // Ana sayfaya dön ve tüm geçmişi temizle
                Intent intent = new Intent(VerificationResultActivity.this, MainActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                intent.putExtra("USER_ID", userId);
                startActivity(intent);
            });
        }
    }
}