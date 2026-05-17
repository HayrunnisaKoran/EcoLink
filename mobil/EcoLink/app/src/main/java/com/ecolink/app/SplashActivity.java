package com.ecolink.app;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;

import androidx.appcompat.app.AppCompatActivity;

/**
 * Uygulamanın ilk açılış ekranıdır (Splash Screen).
 * Senin görselini 2 saniye boyunca tam ekran gösterir.
 */
@SuppressLint("CustomSplashScreen")
public class SplashActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // activity_splash.xml tasarım dosyasını bağlıyoruz (Senin görselin burada duruyor)
        setContentView(R.layout.activity_splash);

        // 2000 milisaniye (2 saniye) bekledikten sonra çalışacak kod bloğu
        new Handler(Looper.getMainLooper()).postDelayed(() -> {

            // 1. Telefonun hafızasına bakıyoruz (IS_LOGGED_IN değişkenini kontrol et)
            SharedPreferences prefs = getSharedPreferences("EcoLinkPrefs", MODE_PRIVATE);
            boolean isLoggedIn = prefs.getBoolean("IS_LOGGED_IN", false);

            Intent intent;
            if (isLoggedIn) {
                // Eğer daha önce giriş yaptıysa, hafızadaki user_id'yi al ve direkt Ana Sayfaya geç
                int userId = prefs.getInt("USER_ID", -1);
                intent = new Intent(SplashActivity.this, MainActivity.class);
                intent.putExtra("USER_ID", userId);
            } else {
                // Eğer giriş yapmamışsa veya çıkış yaptıysa Login sayfasına yönlendir
                intent = new Intent(SplashActivity.this, LoginActivity.class);
            }

            startActivity(intent);
            finish(); // Splash ekranını kapat ki geri tuşuna basınca tekrar açılmasın

        }, 2000); // 2000 ms = 2 saniye
    }
}