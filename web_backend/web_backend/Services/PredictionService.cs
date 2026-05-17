using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace web_backend.Services
{
    public class ModelOutput
    {
        [ColumnName("dense")]
        public float[] Score { get; set; }
    }

    // MLImage yapısını kullanan yeni Input modelimiz
    public class ModelInput
    {
        [ImageType(224, 224)]
        public MLImage Image { get; set; }
    }

    public class PredictionService : IPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<ModelInput, ModelOutput> _predictionEngine;
        
        // Etiketleri bellekte tutmak için liste
        private List<string> _labels;

        //  Tahmin motorunu kilitlemek için bir anahtar objesi
        private static readonly object _predictionLock = new object();

        private readonly string _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "EcoLink_Model.onnx");
        private readonly string _labelsPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "labels.txt");

        public PredictionService()
        {
            _mlContext = new MLContext();
            _labels = new List<string>();
            LoadModel();
        }

        private void LoadModel()
        {
            if (!File.Exists(_modelPath))
                throw new FileNotFoundException("ONNX modeli bulunamadı!");

            if (File.Exists(_labelsPath))
            {
                _labels = File.ReadAllLines(_labelsPath).ToList();
            }

            var dataView = _mlContext.Data.LoadFromEnumerable(new List<ModelInput>());

            var pipeline = _mlContext.Transforms.ResizeImages(
                    outputColumnName: "ResizedImage",
                    imageWidth: 224,
                    imageHeight: 224,
                    inputColumnName: nameof(ModelInput.Image))
                .Append(_mlContext.Transforms.ExtractPixels(
                    outputColumnName: "input",
                    inputColumnName: "ResizedImage",
                    // 1. ADIM: Sadece RGB (3 kanal) istediğimizi teyit ediyoruz.
                    colorsToExtract: Microsoft.ML.Transforms.Image.ImagePixelExtractingEstimator.ColorBits.Rgb,
                    // 2. ADIM: Senin derleyicinin kabul ettiği büyük harf ARGB'yi yazıyoruz.
                    // Rgb seçili olduğu için Alpha'yı otomatik atacak, korkma.
                    orderOfExtraction: Microsoft.ML.Transforms.Image.ImagePixelExtractingEstimator.ColorsOrder.ARGB,
                    // 3. ADIM (HAYATİ): Netron'da 3, 224, 224 gördüğümüz için burası KESİNLİKLE FALSE olmalı.
                    interleavePixelColors: false,
                    // 4. ADIM: ONNX modelleri genellikle 0-1 arası float bekler.
                    scaleImage: 1f / 255f))
                .Append(_mlContext.Transforms.ApplyOnnxModel(
                    modelFile: _modelPath,
                    outputColumnNames: new[] { "dense" },
                    inputColumnNames: new[] { "input" }));

            _model = pipeline.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);
        }

        public async Task<string> IdentifyWasteAsync(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return "Geçersiz Fotoğraf";

            // Tahmin işlemini CPU'yu yormaması için Task.Run içine alıyoruz
            return await Task.Run(() =>
            {
                try
                {
                    // Bellekteki byte dizisini MLImage nesnesine çeviriyoruz
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var mlImage = MLImage.CreateFromStream(ms);
                        var input = new ModelInput { Image = mlImage };

                        ModelOutput result;

                        lock (_predictionLock)
                        {
                            result = _predictionEngine.Predict(input);
                        }

                        if (result?.Score == null) return "Tahmin Başarısız";

                        // DEBUG: Tüm skorları terminale yazdır ki modelin kafasını görelim
                        Console.WriteLine("--- MODEL SKORLARI ---");
                        for (int i = 0; i < _labels.Count; i++)
                        {
                            Console.WriteLine($"{_labels[i]}: %{result.Score[i] * 100:F2}");
                        }

                        float maxScore = result.Score.Max();
                        int maxIndex = Array.IndexOf(result.Score, maxScore);

                        // 2. İYİLEŞTİRME: Dosyadan değil, bellekteki listeden hızlıca getir
                        if (maxIndex >= 0 && maxIndex < _labels.Count)
                        {
                            return _labels[maxIndex];
                        }

                        return "Bilinmeyen Sınıf";
                    }
                }
                catch (Exception ex)
                {
                    // Hatayı yutmak yerine detayını konsola yazdıralım
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("AI HATASI DETAYI:");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("--------------------------");
                    return "Analiz Hatası";
                }
            });
            }

    }
}