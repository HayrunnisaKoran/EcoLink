package com.ecolink.app;

import android.os.Bundle;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.AuthRequest;
import com.ecolink.app.network.models.AuthResponse;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class RegisterActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);

        TextView tvTitle = findViewById(R.id.tvRegisterTitle);
        if (tvTitle != null) tvTitle.setText(R.string.register_title);

        TextView tvBack = findViewById(R.id.tvBackToLogin);
        if (tvBack != null) tvBack.setOnClickListener(v -> finish());

        EditText etRegName = findViewById(R.id.etRegName);
        EditText etRegEmail = findViewById(R.id.etRegEmail);
        EditText etRegPass = findViewById(R.id.etRegPass);

        findViewById(R.id.btnRegisterSubmit).setOnClickListener(v -> {
            String name = etRegName.getText().toString().trim();
            String email = etRegEmail.getText().toString().trim();
            String pass = etRegPass.getText().toString().trim();

            if (name.isEmpty() || email.isEmpty() || pass.isEmpty()) {
                Toast.makeText(this, "Lütfen tüm alanları doldurun!", Toast.LENGTH_SHORT).show();
                return;
            }

            if (!email.endsWith("@gmail.com") && !email.endsWith("@ogr.cbu.edu.tr")) {
                Toast.makeText(this, "Sadece @gmail.com veya @ogr.cbu.edu.tr uzantılı e-postalar ile kayıt olabilirsiniz.", Toast.LENGTH_LONG).show();
                return;
            }

            AuthRequest request = new AuthRequest(name, email, pass);

            RetrofitClient.getApiService().registerUser(request).enqueue(new Callback<>() {
                @Override
                public void onResponse(@NonNull Call<AuthResponse> call, @NonNull Response<AuthResponse> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        Toast.makeText(RegisterActivity.this, "Kayıt Başarılı! Lütfen giriş yapın.", Toast.LENGTH_LONG).show();
                        finish();
                    } else {
                        Toast.makeText(RegisterActivity.this, "Kayıt reddedildi! E-posta kullanılıyor olabilir.", Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onFailure(@NonNull Call<AuthResponse> call, @NonNull Throwable t) {
                    Toast.makeText(RegisterActivity.this, getString(R.string.server_connection_error, t.getMessage()), Toast.LENGTH_LONG).show();
                }
            });
        });
    }
}