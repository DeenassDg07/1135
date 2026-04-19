using _1125.DB;
using _1125.Model;
using _1125.VMTools;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace _1125.ViewModel
{
    internal class EditingAddingWindowViewModel : BaseVM
    {
        private Product _product;
        private Category _selectedCategory;

        public ObservableCollection<Category> Categories { get; }

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                Signal(nameof(Product));
            }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    Product.CategoryId = _selectedCategory?.Id ?? 0;
                    Signal(nameof(SelectedCategory));
                    Signal(nameof(Product));
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand UploadImageCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand DeleteCommand { get; }


        public EditingAddingWindowViewModel(Product product)
        {
            Categories = new ObservableCollection<Category>(ProductDB.GetDb().SelectCategorys());

            Product = product != null ? new Product
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Availability = product.Availability,
                ImageData = product.ImageData?.ToArray(),
                CategoryId = product.CategoryId
            } : new Product();

            SelectedCategory = Categories.FirstOrDefault(c => c.Id == Product.CategoryId);

            SaveCommand = new CommandVM(() =>
            {
                if (ValidateProduct())
                {
                    try
                    {
                        if (Product.Id == 0)
                            ProductDB.GetDb().Insert(Product);
                        else
                            ProductDB.GetDb().Update(Product);

                        Close?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                    }
                }
            });

            UploadImageCommand = new CommandVM(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Images|*.jpg;*.png;*.bmp"
                };

                if (dialog.ShowDialog() == true)
                {
                    Product.ImageData = File.ReadAllBytes(dialog.FileName);
                    Signal(nameof(Product.ImageBitmap));
                }
            });

            DeleteCommand = new CommandVM(() =>
            {

                ProductDB.GetDb().Delete(Product);
                MessageBox.Show("Товар удален");
                Close?.Invoke();

            });

            CloseCommand = new CommandVM(() =>
            {
                Close?.Invoke();
            });
        }

        private bool ValidateProduct()
        {
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                MessageBox.Show("Название обязательно!");
                return false;
            }

            if (Product.Price <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля!");
                return false;
            }

            if (SelectedCategory == null)
            {
                MessageBox.Show("Выберите категорию!");
                return false;
            }

            return true;
        }

        public Action Close { get; set; }
    }
}
