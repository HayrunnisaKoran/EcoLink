package com.ecolink.app;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.AuthRequest;
import com.ecolink.app.network.models.AuthResponse;
import com.google.android.material.button.MaterialButton;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {

    private EditText etEmail, etPassword;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        // Arayüz elemanlarını bağlayalım
        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        MaterialButton btnLogin = findViewById(R.id.btnLogin);
        MaterialButton btnGoogleLogin = findViewById(R.id.btnGoogleLogin);
        TextView tvRegister = findViewById(R.id.tvRegister);

        // API'YE GERÇEK BAĞLANTIYI YAPAN TEK TIKLAMA KODU
        btnLogin.setOnClickListener(v -> {
            String email = etEmail.getText().toString().trim();
            String password = etPassword.getText().toString().trim();

            if (email.isEmpty() || password.isEmpty()) {
                Toast.makeText(this, "Lütfen tüm alanları doldurun", Toast.LENGTH_SHORT).show();
                return;
            }

            // GÜVENLİK DUVARI
            if (!email.endsWith("@gmail.com") && !email.endsWith("@ogr.cbu.edu.tr")) {
                Toast.makeText(this, "Sadece @gmail.com veya @ogr.cbu.edu.tr uzantılı e-postalar ile giriş yapabilirsiniz.", Toast.LENGTH_LONG).show();
                return;
            }

            // API'ye gerçek isteği atıyoruz
            AuthRequest loginRequest = new AuthRequest(email, password);
            RetrofitClient.getApiService().loginUser(loginRequest).enqueue(new Callback<>() {
                @Override
                public void onResponse(@NonNull Call<AuthResponse> call, @NonNull Response<AuthResponse> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        AuthResponse auth = response.body();

                        // Veriyi kalıcı hafızaya yazıyoruz (Single Source of Truth)
                        SharedPreferences prefs = getSharedPreferences("EcoLinkPrefs", MODE_PRIVATE);
                        prefs.edit()
                                .putInt("USER_ID", auth.userId)
                                .putString("USER_NAME", auth.fullName)
                                .putBoolean("IS_LOGGED_IN", true)
                                .apply();

                        Intent intent = new Intent(LoginActivity.this, MainActivity.class);
                        startActivity(intent);
                        finish();
                    } else {
                        Toast.makeText(LoginActivity.this, "E-posta veya şifre hatalı!", Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onFailure(@NonNull Call<AuthResponse> call, @NonNull Throwable t) {
                    Toast.makeText(LoginActivity.this, getString(R.string.server_connection_error, t.getMessage()), Toast.LENGTH_LONG).show();
                }
            });
        });

        // Google Hesabımla Giriş Yap
        btnGoogleLogin.setOnClickListener(v -> startActivity(new Intent(LoginActivity.this, RegisterActivity.class)));

        // Kayıt Ol Yazısına Tıklama
        tvRegister.setOnClickListener(v -> startActivity(new Intent(LoginActivity.this, RegisterActivity.class)));
    }
}