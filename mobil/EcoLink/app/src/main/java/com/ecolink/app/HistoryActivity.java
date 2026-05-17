package com.ecolink.app;

import android.os.Bundle;
import android.widget.ImageView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.ecolink.app.network.RetrofitClient;
import java.util.ArrayList;
import java.util.List;

public class HistoryActivity extends AppCompatActivity {

    private RecyclerView rvHistory;
    private int userId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_history);

        userId = getIntent().getIntExtra("USER_ID", -1);

        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        rvHistory = findViewById(R.id.rvHistory);
        rvHistory.setLayoutManager(new LinearLayoutManager(this));

        // Burada backend'den verileri çekecek metodu çağırıyoruz
        fetchWasteHistory();
    }

    private void fetchWasteHistory() {
        // Backend'deki stats endpoint'inden verileri çekip adapter'a bağlayacağız
        // Şimdilik boş bir liste mantığını kuruyoruz
        Toast.makeText(this, "Veriler backend'den yükleniyor...", Toast.LENGTH_SHORT).show();
    }
}