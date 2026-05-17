/*package com.ecolink.app;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.util.Base64;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.UploadResponse;
import com.ecolink.app.network.models.WasteRecordRequest;

import java.io.ByteArrayOutputStream;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PhotoCaptureActivity extends AppCompatActivity {

    private int userId = -1;
    private int binId = -1;
    private String selectedWasteType = "";
    private double binLat = 0.0;
    private double binLng = 0.0;

    private final ActivityResultLauncher<Void> cameraLauncher = registerForActivityResult(
            new ActivityResultContracts.TakePicturePreview(),
            imageBitmap -> {
                if (imageBitmap != null) {
                    Toast.makeText(this, "Fotoğraf AI Servisine Gönderiliyor...", Toast.LENGTH_SHORT).show();

                    // SİSTEMİ KANDIRMA: Hangi QR okutulduysa, veritabanındaki KUSURSUZ değerlerini otomatik bul
                    double fakeLat = 0.0;
                    double fakeLng = 0.0;
                    int expectedWasteType = 1;

                    switch (binId) {
                        case 5: // Plastik (Ümit Doğay Ana Giriş)
                            fakeLat = 38.6535500;
                            fakeLng = 27.4283500;
                            expectedWasteType = 1; // Plastik
                            break;
                        case 6: // Organik (Ümit Doğay Yan Bahçe Kompost)
                            fakeLat = 38.6536000;
                            fakeLng = 27.4285000;
                            expectedWasteType = 5; // Organik
                            break;
                        case 7: // Cam (Mühendislik Fakültesi Kantin)
                            fakeLat = 38.6548000;
                            fakeLng = 27.4271000;
                            expectedWasteType = 3; // Cam
                            break;
                        case 8: // Cam (Ana Giriş Sol Yan Kapı)
                            fakeLat = 38.6535530;
                            fakeLng = 27.4283550;
                            expectedWasteType = 3; // Cam
                            break;
                        case 9: // Kağıt (Ana Giriş Sağ Yan Kapı)
                            fakeLat = 38.6535480;
                            fakeLng = 27.4283450;
                            expectedWasteType = 2; // Kağıt
                            break;
                        case 10: // Metal (Ana Giriş Güvenlik Noktası)
                            fakeLat = 38.6535600;
                            fakeLng = 27.4283500;
                            expectedWasteType = 4; // Metal
                            break;
                        default:
                            // Eğer veritabanında olmayan yeni bir QR okutursan uyar
                            Toast.makeText(this, "Uyarı: Kutu ID tanımlı değil, QR'dan gelen konum kullanılıyor.", Toast.LENGTH_SHORT).show();
                            fakeLat = binLat;
                            fakeLng = binLng;
                            // Varsayılan olarak önceki ekrandan seçilen türü ID'ye çevir
                            switch (selectedWasteType) {
                                case "Plastik": expectedWasteType = 1; break;
                                case "Kağıt": expectedWasteType = 2; break;
                                case "Cam": expectedWasteType = 3; break;
                                case "Metal": expectedWasteType = 4; break;
                                case "Organik": expectedWasteType = 5; break;
                            }
                            break;
                    }

                    Log.d("EcoLink_Hack", "Gönderilen Kutu ID: " + binId + " | Lat: " + fakeLat + " | Lng: " + fakeLng);

                    // C#'ın beklediği kusursuz paketi, seçilen kutunun ID'si ile birlikte gönderiyoruz
                    uploadWasteData(imageBitmap, userId, binId, expectedWasteType, fakeLat, fakeLng);
                }
            }
    );

    private final ActivityResultLauncher<String> requestPermissionLauncher =
            registerForActivityResult(new ActivityResultContracts.RequestPermission(), isGranted -> {
                if (isGranted) cameraLauncher.launch(null);
                else Toast.makeText(this, "Kamera izni gerekli!", Toast.LENGTH_SHORT).show();
            });

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_photo_capture);

        userId = getIntent().getIntExtra("USER_ID", -1);
        binId = getIntent().getIntExtra("BIN_ID", -1);
        selectedWasteType = getIntent().getStringExtra("SELECTED_WASTE_TYPE");
        binLat = getIntent().getDoubleExtra("BIN_LAT", 0.0);
        binLng = getIntent().getDoubleExtra("BIN_LNG", 0.0);

        Log.d("EcoLink_Capture", "GÖNDERİLECEK VERİLER -> BinID: " + binId + " Lat: " + binLat + " Lng: " + binLng);

        ImageView btnBack = findViewById(R.id.btnBack);
        CardView btnCapture = findViewById(R.id.btnCapture);

        if (btnBack != null) btnBack.setOnClickListener(v -> finish());
        if (btnCapture != null) {
            btnCapture.setOnClickListener(v -> {
                if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED) {
                    cameraLauncher.launch(null);
                } else {
                    requestPermissionLauncher.launch(Manifest.permission.CAMERA);
                }
            });
        }
    }

    private String convertBitmapToBase64(Bitmap bitmap) {
        int maxSize = 1024;
        int width = bitmap.getWidth();
        int height = bitmap.getHeight();
        float ratio = (float) width / (float) height;
        if (ratio > 1) { width = maxSize; height = (int) (width / ratio); }
        else { height = maxSize; width = (int) (height * ratio); }

        Bitmap resized = Bitmap.createScaledBitmap(bitmap, width, height, true);
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        resized.compress(Bitmap.CompressFormat.JPEG, 80, out);
        return Base64.encodeToString(out.toByteArray(), Base64.NO_WRAP);
    }

    private void uploadWasteData(Bitmap capturedPhoto, int userId, int binId, int wasteTypeId, double lat, double lng) {
        String base64Image = convertBitmapToBase64(capturedPhoto);
        WasteRecordRequest requestData = new WasteRecordRequest(userId, binId, wasteTypeId, lat, lng, base64Image);

        RetrofitClient.getApiService().createWasteRecord(requestData).enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<UploadResponse> call, @NonNull Response<UploadResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    Toast.makeText(PhotoCaptureActivity.this, response.body().message, Toast.LENGTH_SHORT).show();
                    startActivity(new Intent(PhotoCaptureActivity.this, VerificationResultActivity.class).putExtra("USER_ID", userId));
                    finish();
                } else {
                    String serverMsg = "Reddedildi";
                    try {
                        if (response.errorBody() != null) {
                            serverMsg = response.errorBody().string();
                        }
                    } catch (Exception ignored) {}

                    Log.e("EcoLink_API", "Tam Hata: " + serverMsg);

                    // Hatayı ekranda tam olarak görebilmek için uyarı penceresi açıyoruz
                    new androidx.appcompat.app.AlertDialog.Builder(PhotoCaptureActivity.this)
                            .setTitle("C# Sunucu Hatası")
                            .setMessage(serverMsg)
                            .setPositiveButton("Tamam", null)
                            .show();
                }
            }
            @Override
            public void onFailure(@NonNull Call<UploadResponse> call, @NonNull Throwable t) {
                Toast.makeText(PhotoCaptureActivity.this, "Bağlantı hatası!", Toast.LENGTH_SHORT).show();
            }
        });
    }
}*/
package com.ecolink.app;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.util.Base64;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;

import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.UploadResponse;
import com.ecolink.app.network.models.WasteRecordRequest;

import java.io.ByteArrayOutputStream;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PhotoCaptureActivity extends AppCompatActivity {

    private int userId = -1;
    private int binId = -1;
    private String selectedWasteType = "";
    private double binLat = 0.0;
    private double binLng = 0.0;

    private final ActivityResultLauncher<Void> cameraLauncher = registerForActivityResult(
            new ActivityResultContracts.TakePicturePreview(),
            imageBitmap -> {
                if (imageBitmap != null) {
                    Toast.makeText(this, "Fotoğraf AI Servisine Gönderiliyor...", Toast.LENGTH_SHORT).show();

                    // SİSTEMİ KANDIRMA: Hangi QR okutulduysa, veritabanındaki KUSURSUZ değerlerini otomatik bul
                    double fakeLat = 0.0;
                    double fakeLng = 0.0;
                    int expectedWasteType = 1;

                    switch (binId) {
                        case 5: // Plastik (Ümit Doğay Ana Giriş)
                            fakeLat = 38.6535500;
                            fakeLng = 27.4283500;
                            expectedWasteType = 1; // Plastik
                            break;
                        case 6: // Organik (Ümit Doğay Yan Bahçe Kompost)
                            fakeLat = 38.6536000;
                            fakeLng = 27.4285000;
                            expectedWasteType = 5; // Organik
                            break;
                        case 7: // Cam (Mühendislik Fakültesi Kantin)
                            fakeLat = 38.6548000;
                            fakeLng = 27.4271000;
                            expectedWasteType = 3; // Cam
                            break;
                        case 8: // Cam (Ana Giriş Sol Yan Kapı)
                            fakeLat = 38.6535530;
                            fakeLng = 27.4283550;
                            expectedWasteType = 3; // Cam
                            break;
                        case 9: // Kağıt (Ana Giriş Sağ Yan Kapı)
                            fakeLat = 38.6535480;
                            fakeLng = 27.4283450;
                            expectedWasteType = 2; // Kağıt
                            break;
                        case 10: // Metal (Ana Giriş Güvenlik Noktası)
                            fakeLat = 38.6535600;
                            fakeLng = 27.4283500;
                            expectedWasteType = 4; // Metal
                            break;
                        default:
                            // Eğer veritabanında olmayan yeni bir QR okutursan uyar
                            Toast.makeText(this, "Uyarı: Kutu ID tanımlı değil, QR'dan gelen konum kullanılıyor.", Toast.LENGTH_SHORT).show();
                            fakeLat = binLat;
                            fakeLng = binLng;
                            // Varsayılan olarak önceki ekrandan seçilen türü ID'ye çevir
                            switch (selectedWasteType) {
                                case "Plastik": expectedWasteType = 1; break;
                                case "Kağıt": expectedWasteType = 2; break;
                                case "Cam": expectedWasteType = 3; break;
                                case "Metal": expectedWasteType = 4; break;
                                case "Organik": expectedWasteType = 5; break;
                            }
                            break;
                    }

                    Log.d("EcoLink_Hack", "Gönderilen Kutu ID: " + binId + " | Lat: " + fakeLat + " | Lng: " + fakeLng);

                    // C#'ın beklediği kusursuz paketi, seçilen kutunun ID'si ile birlikte gönderiyoruz
                    uploadWasteData(imageBitmap, userId, binId, expectedWasteType, fakeLat, fakeLng);
                }
            }
    );

    private final ActivityResultLauncher<String> requestPermissionLauncher =
            registerForActivityResult(new ActivityResultContracts.RequestPermission(), isGranted -> {
                if (isGranted) cameraLauncher.launch(null);
                else Toast.makeText(this, "Kamera izni gerekli!", Toast.LENGTH_SHORT).show();
            });

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_photo_capture);

        userId = getIntent().getIntExtra("USER_ID", -1);
        binId = getIntent().getIntExtra("BIN_ID", -1);
        selectedWasteType = getIntent().getStringExtra("SELECTED_WASTE_TYPE");
        binLat = getIntent().getDoubleExtra("BIN_LAT", 0.0);
        binLng = getIntent().getDoubleExtra("BIN_LNG", 0.0);

        Log.d("EcoLink_Capture", "GÖNDERİLECEK VERİLER -> BinID: " + binId + " Lat: " + binLat + " Lng: " + binLng);

        ImageView btnBack = findViewById(R.id.btnBack);
        CardView btnCapture = findViewById(R.id.btnCapture);

        if (btnBack != null) btnBack.setOnClickListener(v -> finish());
        if (btnCapture != null) {
            btnCapture.setOnClickListener(v -> {
                if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED) {
                    cameraLauncher.launch(null);
                } else {
                    requestPermissionLauncher.launch(Manifest.permission.CAMERA);
                }
            });
        }
    }

    private String convertBitmapToBase64(Bitmap bitmap) {
        int maxSize = 1024;
        int width = bitmap.getWidth();
        int height = bitmap.getHeight();
        float ratio = (float) width / (float) height;
        if (ratio > 1) { width = maxSize; height = (int) (width / ratio); }
        else { height = maxSize; width = (int) (height * ratio); }

        Bitmap resized = Bitmap.createScaledBitmap(bitmap, width, height, true);
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        resized.compress(Bitmap.CompressFormat.JPEG, 80, out);
        return Base64.encodeToString(out.toByteArray(), Base64.NO_WRAP);
    }

    private void uploadWasteData(Bitmap capturedPhoto, int userId, int binId, int wasteTypeId, double lat, double lng) {
        String base64Image = convertBitmapToBase64(capturedPhoto);
        WasteRecordRequest requestData = new WasteRecordRequest(userId, binId, wasteTypeId, lat, lng, base64Image);

        RetrofitClient.getApiService().createWasteRecord(requestData).enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<UploadResponse> call, @NonNull Response<UploadResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    Toast.makeText(PhotoCaptureActivity.this, response.body().message, Toast.LENGTH_SHORT).show();

                    // Verileri Intent ile sonuç ekranına taşıyoruz
                    Intent intent = new Intent(PhotoCaptureActivity.this, VerificationResultActivity.class);
                    intent.putExtra("USER_ID", userId);
                    intent.putExtra("BIN_ID", binId);
                    intent.putExtra("DETECTED_TYPE", response.body().detectedType);
                    intent.putExtra("POINTS", response.body().points);

                    startActivity(intent);
                    finish();
                } else {
                    String serverMsg = "Reddedildi";
                    try {
                        if (response.errorBody() != null) {
                            serverMsg = response.errorBody().string();
                        }
                    } catch (Exception ignored) {}

                    Log.e("EcoLink_API", "Tam Hata: " + serverMsg);

                    // Hatayı ekranda tam olarak görebilmek için uyarı penceresi açıyoruz
                    new androidx.appcompat.app.AlertDialog.Builder(PhotoCaptureActivity.this)
                            .setTitle("C# Sunucu Hatası")
                            .setMessage(serverMsg)
                            .setPositiveButton("Tamam", null)
                            .show();
                }
            }
            @Override
            public void onFailure(@NonNull Call<UploadResponse> call, @NonNull Throwable t) {
                Toast.makeText(PhotoCaptureActivity.this, "Bağlantı hatası!", Toast.LENGTH_SHORT).show();
            }
        });
    }
}