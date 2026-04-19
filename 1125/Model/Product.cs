using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace _1125.Model
{
    public class Product : INotifyPropertyChanged
    {
        private byte[] imageData;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Availability { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }

        // Новый параметр для хранения id категории
        public int CategoryId { get; set; }

        public byte[] ImageData
        {
            get => imageData;
            set
            {
                imageData = value;
                OnPropertyChanged(nameof(ImageData));
                OnPropertyChanged(nameof(ImageBitmap));
            }
        }

        public BitmapImage ImageBitmap
        {
            get
            {
                if (ImageData == null || ImageData.Length == 0) return null;
                using var ms = new System.IO.MemoryStream(ImageData);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

 
    }
}
