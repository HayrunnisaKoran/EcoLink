using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System.Drawing;
using System.IO;

namespace web_backend.Services
{
    // Modelin Girdi (Input) Şeması
    // TensorFlow modeline veriyi bu sınıftaki özellikle vereceğiz.
    public class ModelInput
    {
        [ImageType(224, 224)] // Modelin genellikle beklediği piksel boyutu (Modelinize göre değişebilir, standart 224'tür)
        public Bitmap Image { get; set; }
    }

    // Modelin Çıktı (Output) Şeması
    // TensorFlow modelinden dönen sonuçları bu sınıfla alacağız.
    public class ModelOutput
    {
        [ColumnName("output_0")] //  modelin çıktı layer'ının adı 
        public float[] Score { get; set; }
    }

    public class PredictionService : IPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

        // Model ve Label yollarını belirtiyoruz
        private readonly string _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "model.tflite"); 
        private readonly string _labelsPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "labels.txt"); 

        public PredictionService()
        {
            _mlContext = new MLContext();
            LoadModel();
        }

        // Modeli sadece uygulama ayağa kalktığında bir kere yüklemek performansı artırır.
        private void LoadModel()
        {
            // 1. Girdi için boş bir şema oluşturuluyor
            var dataView = _mlContext.Data.LoadFromEnumerable(new List<ModelInput>());

            // 2. TensorFlow Lite modelinin sisteme yüklenmesi için boru hattı (pipeline) kuruluyor
            var pipeline = _mlContext.Model.LoadTensorFlowModel(_modelPath)
                .ScoreTensorFlowModel(
                    outputColumnNames: new[] { "output_0" },
                    inputColumnNames: new[] { "input_layer_1" }, // Modelin girdi layer'ı
                    addBatchDimensionInput: true);

            // 3. Model eğitilmiş olduğu için sadece yapıyı fitliyoruz (eşleştiriyoruz)
            _model = pipeline.Fit(dataView);

            // 4. Tahmin motoru oluşturuluyor
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);
        }

        public async Task<string> IdentifyWasteAsync(byte[] imageBytes)
        {
            // 1. Byte array olarak gelen fotoğrafı Bitmap formatına çeviriyoruz
            using var ms = new MemoryStream(imageBytes);
            using var bitmap = new Bitmap(ms);

            // 2. Model için giriş verisini oluşturuyoruz
            var input = new ModelInput { Image = bitmap };

            // 3. Modeli çalıştırıp tahmini alıyoruz (Burası sihrin gerçekleştiği yer)
            var result = _predictionEngine.Predict(input);

            // 4. En yüksek skorlu sonucun Index'ini buluyoruz
            float maxScore = result.Score.Max();
            int maxIndex = result.Score.ToList().IndexOf(maxScore);

            // 5. Bulunan Index'i labels.txt ile eşleştirip atık türünü döndürüyoruz
            return await GetLabelByIndexAsync(maxIndex);
        }

        // Labels.txt dosyasından ilgili satırı (kategoriyi) okuyan yardımcı metod
        private async Task<string> GetLabelByIndexAsync(int index)
        {
            var labels = await File.ReadAllLinesAsync(_labelsPath); 
            if (index >= 0 && index < labels.Length)
            {
                return labels[index]; // Örn: "plastik" veya "cam" dönecek
            }
            return "Bilinmeyen Atik";
        }
    }
}