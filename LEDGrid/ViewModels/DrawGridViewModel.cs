using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEDGrid.Models;
using LEDGrid;
using System.Windows.Input;
using System.ComponentModel;
using System.Drawing;
using ColorPickerWPF;
using MaterialDesignThemes.Wpf;
using Xceed.Wpf.Toolkit;
using Microsoft.Win32;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LEDGrid.ViewModels
{
    class DrawGridViewModel : INotifyPropertyChanged
    {
        private LED[][] _lEDMatrix;
        private int _redSlider = 255;
        private int _greenSlider = 255;
        private int _blueSlider = 255;
        private int _redSlider2 = 0;
        private int _greenSlider2 = 0;
        private int _blueSlider2 = 0;

        public DrawGridViewModel()
        {
            _lEDMatrix = new LED[10][];
            for(int i =0;i<10;i++)
                _lEDMatrix[i] = new LED[10];
            for(int i=0;i<10;i++)
                for(int j=0;j<10;j++)
                    _lEDMatrix[i][j] = new LED();
            RaisePropertyChanged("LEDMatrix");
        }

        public LED[][] LEDMatrix
        {
            get
            {
                return _lEDMatrix;
            }
        }
        public int RedSlider
        {
            get
            {
                return _redSlider;
            }
            set
            {
                _redSlider = value;
                RaisePropertyChanged("RedSlider");
                RaisePropertyChanged("LEDColor");
            }
        }
        public int GreenSlider
        {
            get
            {
                return _greenSlider;
            }
            set
            {
                _greenSlider = value;
                RaisePropertyChanged("GreenSlider");
                RaisePropertyChanged("LEDColor");
            }
        }
        public int BlueSlider
        {
            get
            {
                return _blueSlider;
            }
            set
            {
                _blueSlider = value;
                RaisePropertyChanged("BlueSlider");
                RaisePropertyChanged("LEDColor");
            }
        }
        public int RedSlider2
        {
            get
            {
                return _redSlider2;
            }
            set
            {
                _redSlider2 = value;
                RaisePropertyChanged("RedSlider2");
                RaisePropertyChanged("LEDColor2");
            }
        }
        public int GreenSlider2
        {
            get
            {
                return _greenSlider2;
            }
            set
            {
                _greenSlider2 = value;
                RaisePropertyChanged("GreenSlider2");
                RaisePropertyChanged("LEDColor2");
            }
        }
        public int BlueSlider2
        {
            get
            {
                return _blueSlider2;
            }
            set
            {
                _blueSlider2 = value;
                RaisePropertyChanged("BlueSlider2");
                RaisePropertyChanged("LEDColor2");
            }
        }
        public string LEDColor
        {
            get
            {
                return "#" + RedSlider.ToString("X2") + GreenSlider.ToString("X2") + BlueSlider.ToString("X2");
            }
        }
        public string LEDColor2
        {
            get
            {
                return "#" + RedSlider2.ToString("X2") + GreenSlider2.ToString("X2") + BlueSlider2.ToString("X2");
            }
        }
        #region Commands
        private RelayCommand _changeColor;
        public ICommand ChangeColor
        {
            get
            {
                if (_changeColor == null)
                    _changeColor = new RelayCommand(param => ChangeLED(param));
                return _changeColor;
            }
        }
        private RelayCommand _changeColor2;
        public ICommand ChangeColor2
        {
            get
            {
                if (_changeColor2 == null)
                    _changeColor2 = new RelayCommand(param => ChangeLED2(param));
                return _changeColor2;
            }
        }
        private RelayCommand _colorPallette;
        public ICommand ColorPallette
        {
            get
            {
                if (_colorPallette == null)
                {
                    _colorPallette = new RelayCommand(param => ShowColors(1));
                }
                return _colorPallette;
            }
        }
        private RelayCommand _colorPallette2;
        public ICommand ColorPallette2
        {
            get
            {
                if (_colorPallette2 == null)
                {
                    _colorPallette2 = new RelayCommand(param => ShowColors(2));
                }
                return _colorPallette2;
            }
        }
        private RelayCommand _openImage;
        public ICommand OpenImage
        {
            get
            {
                if (_openImage == null)
                {
                    _openImage = new RelayCommand(param => LoadImage());
                }
                return _openImage;
            }
        }
        private RelayCommand _saveCanvas;
        public ICommand SaveCanvas
        {
            get
            {
                if (_saveCanvas == null)
                {
                    _saveCanvas = new RelayCommand(param => SaveImage());
                }
                return _saveCanvas;
            }
        }
        #endregion 
        public void ChangeLED(object obj)
        {
            System.Windows.Controls.Canvas caller = (System.Windows.Controls.Canvas)obj;
            string temp = caller.Uid;
            int row = Convert.ToInt32(temp.Substring(0, temp.IndexOf(" ")));
            temp = temp.Substring(temp.IndexOf(" ") + 1);
            int col = Convert.ToInt32(temp);

            _lEDMatrix[row][col].Red = RedSlider;
            _lEDMatrix[row][col].Green = GreenSlider;
            _lEDMatrix[row][col].Blue = BlueSlider;
        }
        public void ChangeLED2(object obj)
        {
            System.Windows.Controls.Canvas caller = (System.Windows.Controls.Canvas)obj;
            string temp = caller.Uid;
            int row = Convert.ToInt32(temp.Substring(0, temp.IndexOf(" ")));
            temp = temp.Substring(temp.IndexOf(" ") + 1);
            int col = Convert.ToInt32(temp);

            _lEDMatrix[row][col].Red = RedSlider2;
            _lEDMatrix[row][col].Green = GreenSlider2;
            _lEDMatrix[row][col].Blue = BlueSlider2;

        }

       

        public void ShowColors(int num)
        {

            System.Windows.Media.Color colorin;
            if (num == 2)
                colorin = System.Windows.Media.Color.FromRgb((byte)RedSlider2, (byte)GreenSlider2, (byte)BlueSlider2);
            else
                colorin = System.Windows.Media.Color.FromRgb((byte)RedSlider, (byte)GreenSlider, (byte)BlueSlider);

            System.Windows.Media.Color color;          
            bool ok = ColorPickerWindow.ShowDialog(out color,colorin);
            if (num == 2)
            {
                RedSlider2 = color.R;
                GreenSlider2 = color.G;
                BlueSlider2 = color.B;
            }
            else
            {
                RedSlider = color.R;
                GreenSlider = color.G;
                BlueSlider = color.B;
            }
            
        }

        public void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images Files|*.png;*.bmp;*.gif;*.exif;*.jpg;*.tiff";
            if(openFileDialog.ShowDialog()==true)
            {
                Bitmap img = new Bitmap(openFileDialog.FileName);
                Bitmap resized = ImageResize.ResizeImage(img, 10, 10);
               // Bitmap resized = new Bitmap(img, new Size(10, 10));
                for(int i=0;i<resized.Width;i++)
                    for(int j=0;j<resized.Height;j++)
                    {
                        Color pixel = resized.GetPixel(j, i);
                        _lEDMatrix[i][j].Red = pixel.R;
                        _lEDMatrix[i][j].Blue = pixel.B;
                        _lEDMatrix[i][j].Green = pixel.G;
                    }
            }
        }
        public void SaveImage()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "Png Files (*.png)|*.png";
            if(saveFileDialog.ShowDialog()==true)
            {
                Bitmap toSave = new Bitmap(_lEDMatrix[0].Length, _lEDMatrix.Length);
                for(int i=0;i<_lEDMatrix.Length;i++)
                    for(int j=0;j<_lEDMatrix[0].Length;j++)
                    {
                        Color pixel = Color.FromArgb(_lEDMatrix[i][j].Red, _lEDMatrix[i][j].Green, _lEDMatrix[i][j].Blue);
                        toSave.SetPixel(j, i, pixel);

                    }
                toSave.Save(saveFileDialog.FileName);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Sets UpdateFlags for a variable that has been changed
        /// </summary>
        /// <param name="propertyName"> name of the varibale to be changed</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
