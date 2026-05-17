/*package com.ecolink.app;
import android.os.Bundle;
import android.view.*;
import androidx.fragment.app.Fragment;
public class RewardsPenaltiesFragment extends Fragment {
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_rewards_all, container, false);
    }
}*/
package com.ecolink.app;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

public class RewardsPenaltiesFragment extends Fragment {
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        // HATA BURADAYDI: fragment_rewards_all yazıyordu. Şimdi penalties oldu.
        return inflater.inflate(R.layout.fragment_rewards_penalties, container, false);
    }
}