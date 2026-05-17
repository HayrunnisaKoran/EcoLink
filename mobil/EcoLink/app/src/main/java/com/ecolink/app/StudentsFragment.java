package com.ecolink.app;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import com.ecolink.app.network.RetrofitClient;
import com.ecolink.app.network.models.LeaderboardItem;
import java.util.List;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class StudentsFragment extends Fragment {

    private TextView tvFirstTitle, tvFirstPoints, tvSecondTitle, tvSecondPoints, tvThirdTitle, tvThirdPoints;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        // Öğrenci tasarımı
        View view = inflater.inflate(R.layout.fragment_students, container, false);

        tvFirstTitle = view.findViewById(R.id.tvFirstTitle);
        tvFirstPoints = view.findViewById(R.id.tvFirstPoints);
        tvSecondTitle = view.findViewById(R.id.tvSecondTitle);
        tvSecondPoints = view.findViewById(R.id.tvSecondPoints);
        tvThirdTitle = view.findViewById(R.id.tvThirdTitle);
        tvThirdPoints = view.findViewById(R.id.tvThirdPoints);

        fetchLeaderboard();
        return view;
    }

    private void fetchLeaderboard() {
        RetrofitClient.getApiService().getStudentLeaderboard().enqueue(new Callback<List<LeaderboardItem>>() {
            @Override
            public void onResponse(@NonNull Call<List<LeaderboardItem>> call, @NonNull Response<List<LeaderboardItem>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<LeaderboardItem> list = response.body();
                    if (list.size() >= 3) {
                        if (tvFirstTitle != null) tvFirstTitle.setText(list.get(0).name);
                        if (tvFirstPoints != null) tvFirstPoints.setText(list.get(0).points + " Puan");

                        if (tvSecondTitle != null) tvSecondTitle.setText(list.get(1).name);
                        if (tvSecondPoints != null) tvSecondPoints.setText(list.get(1).points + " Puan");

                        if (tvThirdTitle != null) tvThirdTitle.setText(list.get(2).name);
                        if (tvThirdPoints != null) tvThirdPoints.setText(list.get(2).points + " Puan");
                    }
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<LeaderboardItem>> call, @NonNull Throwable t) {
                if (isAdded() && getContext() != null) {
                    Toast.makeText(getContext(), "Öğrenci verisi alınamadı", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }
}