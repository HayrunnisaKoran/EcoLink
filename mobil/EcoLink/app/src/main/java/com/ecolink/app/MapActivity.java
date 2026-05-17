package com.ecolink.app;

import android.os.Bundle;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.preference.PreferenceManager;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.WasteBin;

import org.osmdroid.config.Configuration;
import org.osmdroid.tileprovider.tilesource.TileSourceFactory;
import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.MapView;
import org.osmdroid.views.overlay.Marker;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MapActivity extends AppCompatActivity {
    private MapView map = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Configuration.getInstance().load(this, PreferenceManager.getDefaultSharedPreferences(this));
        setContentView(R.layout.activity_map);

        ImageView btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        map = findViewById(R.id.map);
        if (map != null) {
            map.setTileSource(TileSourceFactory.MAPNIK);
            map.setMultiTouchControls(true);

            // DÜZELTME: Haritayı Ümit Doğay Arınç Kültür Merkezi koordinatlarına odaklıyoruz
            // SQL tablosunda ID 5 ve 6 bu konumdadır: 38.653550, 27.428350
            map.getController().setZoom(17.0);
            map.getController().setCenter(new GeoPoint(38.653550, 27.428350));
        }

        // ARKADAŞININ SERVİSİNDEN KUTULARI ÇEK
        fetchWasteBinsFromService();
    }

    private void fetchWasteBinsFromService() {
        RetrofitClient.getApiService().getAllBins().enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<List<WasteBin>> call, @NonNull Response<List<WasteBin>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<WasteBin> bins = response.body();

                    Log.d("EcoLink_Map", "C#'tan gelen kutu sayısı: " + bins.size());

                    if (map != null) {
                        map.getOverlays().clear();
                        for (WasteBin bin : bins) {
                            // Koordinatlar boş değilse haritaya ekle
                            if (bin.getLatitude() != 0.0 && bin.getLongitude() != 0.0) {
                                Marker marker = new Marker(map);
                                marker.setPosition(new GeoPoint(bin.getLatitude(), bin.getLongitude()));
                                marker.setTitle(bin.getLocationName());

                                String atikTuru = bin.getType() != null ? bin.getType() : "Bilinmiyor";
                                marker.setSnippet("Tür: " + atikTuru + " | Doluluk: %" + bin.getFillLevelPercent());

                                marker.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);
                                map.getOverlays().add(marker);
                            }
                        }
                        map.invalidate();
                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<WasteBin>> call, @NonNull Throwable t) {
                Toast.makeText(MapActivity.this, "Kutular yüklenemedi!", Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public void onResume() {
        super.onResume();
        if (map != null) map.onResume();
    }

    @Override
    public void onPause() {
        super.onPause();
        if (map != null) map.onPause();
    }
}
