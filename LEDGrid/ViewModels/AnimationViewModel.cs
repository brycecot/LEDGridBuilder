using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEDGrid.Models;
using System.Windows.Input;
using System.Threading;
using ColorPickerWPF;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Drawing.Imaging;
using ImageMagick;

namespace LEDGrid.ViewModels
{
    class AnimationViewModel : INotifyPropertyChanged
    {
        private List<LED[][]> _frames = new List<LED[][]>();
        private LED[][] _clipboard = new LED[10][];
        private int _frameIndex = 0;
        private double _framesPerSecond = 1;
        private bool _play = false;
        private BackgroundWorker animator;
        private int _red1 = 255;
        private int _red2 = 0;
        private int _green1 = 255;
        private int _green2 = 0;
        private int _blue1 = 255;
        private int _blue2 = 0;
        private bool _colorPicker;

        public AnimationViewModel()
        {
            Frames.Add(new LED[10][]);
            for (int i = 0; i < 10; i++)
                Frames[0][i] = new LED[10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    Frames[Frames.Count - 1][i][j] = new LED();
            RaisePropertyChanged("Frames");
            animator = new BackgroundWorker();
            animator.DoWork += new DoWorkEventHandler(Animate);

            for (int i = 0; i < 10; i++)
                _clipboard[i] = new LED[10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    _clipboard[i][j] = new LED();

        }

        #region Getters/Setters
        public List<LED[][]> Frames
        {
            get
            {
                return _frames;
            }
            set
            {
                _frames = value;
                RaisePropertyChanged("Frames");
               
            }
        }
        public int FrameIndex
        {
            get
            {
                return _frameIndex;
            }
            set
            {
                _frameIndex = value;
                RaisePropertyChanged("FrameIndex");
                RaisePropertyChanged("FrameIndexPlus1");
                if(FrameIndex>-1 && FrameIndex<Frames.Count)
                RaisePropertyChanged("CurrentFrame");
            }
        }
        public int FrameIndexPlus1
        {
            get
            {
                return FrameIndex + 1;
            }
            set
            {
                FrameIndex = value - 1;

            }
        }
        public double FramesPerSecond
        {
            get
            {
                return _framesPerSecond;
            }
            set
            {
                if ( value <= 0.0)
                    throw new ArgumentException("Speed not valid");
                else
                {
                    _framesPerSecond = value;
                    RaisePropertyChanged("FramesPerSecond");
                }
            }
        }
        public bool Play
        {
            get
            {
                return _play;
            }
            set
            {
                _play = value;
                if (value)
                    animator.RunWorkerAsync();
            }
        }
        public string Color1ToString
        {
            get
            {
                return "#" + _red1.ToString("X2") + _green1.ToString("X2") + _blue1.ToString("X2");
            }
        }
        public string Color2ToString
        {
            get
            {
                return "#" + _red2.ToString("X2") + _green2.ToString("X2") + _blue2.ToString("X2");
            }
        }
        public LED[][] CurrentFrame
        {
            get
            {
                return Frames[FrameIndex];
            }
                
        }
        public bool ColorPicker
        {
            get
            {
                return _colorPicker;
            }
            set
            {
                _colorPicker = value;
                RaisePropertyChanged("ColorPicker");
            }
        }
        #endregion

        #region Commands
        private RelayCommand _newFrame;
        public ICommand NewFrame
        {
            get
            {
                if (_newFrame == null)
                {
                    _newFrame = new RelayCommand(param => AddFrame(FrameIndex));
                }
                return _newFrame;
            }
        }
        private RelayCommand _deleteFrame;
        public ICommand DeleteFrame
        {
            get
            {
                if (_deleteFrame == null)
                {
                    _deleteFrame = new RelayCommand(param => RemoveFrame(FrameIndex));
                }
                return _deleteFrame;
            }
        }
        private RelayCommand _nextFrame;
        public ICommand NextFrame
        {
            get
            {
                if(_nextFrame == null)
                {
                    _nextFrame = new RelayCommand(param => Next());
                }
                return _nextFrame;
            }
        }
        private RelayCommand _prevFrame;
        public ICommand PrevFrame
        {
            get
            {
                if (_prevFrame == null)
                {
                    _prevFrame = new RelayCommand(param => Prev());
                }
                return _prevFrame;
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
        private RelayCommand _copy;
        public ICommand Copy
        {
            get
            {
                if (_copy == null)
                    _copy = new RelayCommand(param => CopyToClipboard());
                return _copy;
            }
        }
        private RelayCommand _paste;
        public ICommand Paste
        {
            get
            {
                if (_paste == null)
                    _paste = new RelayCommand(param => PasteClipboard());
                return _paste;
            }
        }
        private RelayCommand _openImages;
        public ICommand OpenImages
        {
            get
            {
                if (_openImages == null)
                {
                    _openImages = new RelayCommand(param => LoadImages());
                }
                return _openImages;
            }
        }
        private RelayCommand _saveCanvases;
        public ICommand SaveCanvases
        {
            get
            {
                if (_saveCanvases == null)
                {
                    _saveCanvases = new RelayCommand(param => SaveImages());
                }
                return _saveCanvases;
            }
        }
        private RelayCommand _rotateRight;
        public ICommand RotateRight
        {
            get
            {
                if (_rotateRight == null)
                {
                    _rotateRight = new RelayCommand(param => RotateFrame(1));
                }
                return _rotateRight;
            }
        }
        private RelayCommand _rotateLeft;
        public ICommand RotateLeft
        {
            get
            {
                if (_rotateLeft == null)
                {
                    _rotateLeft = new RelayCommand(param => RotateFrame(-1));
                }
                return _rotateLeft;
            }
        }
        private RelayCommand _chooseColor1;
        public ICommand ChooseColor1
        {
            get
            {
                if(_chooseColor1 == null)
                {
                    _chooseColor1 = new RelayCommand(param => SetColor1(param));
                }
                return _chooseColor1;
            }
        }
        private RelayCommand _chooseColor2;
        public ICommand ChooseColor2
        {
            get
            {
                if (_chooseColor2 == null)
                {
                    _chooseColor2 = new RelayCommand(param => SetColor2(param));
                }
                return _chooseColor2;
            }
        }
        #endregion

        public void AddFrame(int index)
        {
            Frames.Insert(index+1,new LED[10][]);
            for (int i = 0; i < 10; i++)
                Frames[index + 1][i] = new LED[10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    Frames[index+1][i][j] = new LED();
            RaisePropertyChanged("Frames");
        }
        public void RemoveFrame(int index)
        {
            if (Frames.Count > 1)
            {
                Frames.RemoveAt(index);
                RaisePropertyChanged("Frames");
                if (FrameIndex == Frames.Count)
                    FrameIndex--;
                else
                {
                    FrameIndex++;
                    FrameIndex--;
                }
            }
        }
        public void Next()
        {
            if (FrameIndex < Frames.Count - 1)
                FrameIndex++;
        }

        public void Prev()
        {
            if (FrameIndex > 0)
                FrameIndex--;
        }

        private void Animate(object sender, DoWorkEventArgs e)
        {
            while(Play)
            {
                if (FrameIndex < Frames.Count - 1)
                    FrameIndex++;
                else
                    FrameIndex = 0;
                Thread.Sleep((int)(1000 * (1 / FramesPerSecond)));

            }
        }
        public void ShowColors(int num)
        {
            System.Windows.Media.Color colorin;

            if (num == 2)
                colorin = System.Windows.Media.Color.FromRgb((byte)_red2, (byte)_green2, (byte)_blue2);
            else
                colorin = System.Windows.Media.Color.FromRgb((byte)_red1, (byte)_green1, (byte)_blue1);

            System.Windows.Media.Color color;
            bool ok = ColorPickerWindow.ShowDialog(out color,colorin);

            if (num == 2)
            {
                _red2 = color.R;
                _green2 = color.G;
                _blue2 = color.B;
                RaisePropertyChanged("Color2ToString");
            }
            else
            {
                _red1 = color.R;
                _green1 = color.G;
                _blue1 = color.B;
                RaisePropertyChanged("Color1ToString");
            }

        }
        public void LoadImages()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images Files|*.png;*.bmp;*.gif;*.exif;*.jpg;*.tiff";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames[0].EndsWith(".gif"))
                {
                    Image gif = Image.FromFile(openFileDialog.FileName);
                    PropertyItem item = gif.GetPropertyItem(0x5100);
                    int delay = (item.Value[0] + item.Value[1] * 256) * 10;
                    if (item.Value[4] != 0 || item.Value[5] != 0)
                        FramesPerSecond = (1000.0 / delay);
                    else
                        FramesPerSecond = (item.Value.Length/2)/(delay/100.0);
                    FrameDimension dim = new FrameDimension(gif.FrameDimensionsList[0]);
                    int frameCount = gif.GetFrameCount(dim);
                    
                    for (int i=0;i<frameCount;i++)
                    {
                        if(i!=0)
                            AddFrame(FrameIndex + i -1);
                        gif.SelectActiveFrame(dim,i);
                        Bitmap test = new Bitmap(gif);
                        test = ImageResize.ResizeImage(test, 10, 10);
                        for (int k = 0; k < test.Width; k++)
                            for (int j = 0; j < test.Height; j++)
                            {
                                Color pixel = test.GetPixel(j, k);
                                Frames[FrameIndex + i ][k][j].Red = pixel.R;
                                Frames[FrameIndex + i ][k][j].Blue = pixel.B;
                                Frames[FrameIndex + i ][k][j].Green = pixel.G;
                            }
                    }

                }
                else
                {
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        if(i!=0)
                            AddFrame(FrameIndex + i -1);
                        Bitmap img = new Bitmap(openFileDialog.FileNames[i]);
                        Bitmap resized = ImageResize.ResizeImage(img, 10, 10);

                        for (int k = 0; k < resized.Width; k++)
                            for (int j = 0; j < resized.Height; j++)
                            {
                                Color pixel = resized.GetPixel(j, k);
                                Frames[FrameIndex + i ][k][j].Red = pixel.R;
                                Frames[FrameIndex + i ][k][j].Blue = pixel.B;
                                Frames[FrameIndex + i ][k][j].Green = pixel.G;
                            }
                    }
                }
                
            }
        }
        public void SaveImages()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "Gif Files (*.gif)|*.gif";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (MagickImageCollection collection = new MagickImageCollection())
                {
                    Bitmap toSave = new Bitmap(Frames[0][0].Length, Frames[0].Length);
                    for (int i = 0; i < Frames.Count; i++)
                    {
                        for (int k = 0; k < Frames[0].Length; k++)
                        {
                            for (int j = 0; j < Frames[0][0].Length; j++)
                            {
                                Color pixel = Color.FromArgb(Frames[i][k][j].Red, Frames[i][k][j].Green, Frames[i][k][j].Blue);
                                toSave.SetPixel(j, k, pixel);

                            }
                        }
                        MagickImage mk = new MagickImage(toSave);
                        collection.Add(mk);
                        collection[i].AnimationDelay = (int)(100 * (1 / FramesPerSecond));
                    }

                    // Optionally reduce colors
                    QuantizeSettings settings = new QuantizeSettings();
                    settings.Colors = 256;
                    collection.Quantize(settings);

                    // Optionally optimize the images (images should have the same size).
                    collection.Optimize();

                    // Save gif
                    collection.Write(saveFileDialog.FileName);
                }
            }

        }
        public void ChangeLED(object obj)
        {
            System.Windows.Controls.Canvas caller = (System.Windows.Controls.Canvas)obj;
            string temp = caller.Uid;
            int row = Convert.ToInt32(temp.Substring(0, temp.IndexOf(" ")));
            temp = temp.Substring(temp.IndexOf(" ") + 1);
            int col = Convert.ToInt32(temp);

            CurrentFrame[row][col].Red = _red1;
            CurrentFrame[row][col].Green = _green1;
            CurrentFrame[row][col].Blue = _blue1;
        }
        public void ChangeLED2(object obj)
        {
            System.Windows.Controls.Canvas caller = (System.Windows.Controls.Canvas)obj;
            string temp = caller.Uid;
            int row = Convert.ToInt32(temp.Substring(0, temp.IndexOf(" ")));
            temp = temp.Substring(temp.IndexOf(" ") + 1);
            int col = Convert.ToInt32(temp);

            CurrentFrame[row][col].Red = _red2;
            CurrentFrame[row][col].Green = _green2;
            CurrentFrame[row][col].Blue = _blue2;

        }
        public void CopyToClipboard()
        {
            for (int i = 0; i < CurrentFrame.Length; i++)
                for (int j = 0; j < CurrentFrame[i].Length; j++)
                    CurrentFrame[i][j].CopyTo(_clipboard[i][j]);
        }
        public void PasteClipboard()
        {
            for (int i = 0; i < CurrentFrame.Length; i++)
                for (int j = 0; j < CurrentFrame[i].Length; j++)
                    _clipboard[i][j].CopyTo(CurrentFrame[i][j]);
        }
        public void RotateFrame(int direction)
        {
            if(direction==1)//Left
            {
                AddFrame(FrameIndex);
                for (int i = 0; i < Frames[0].Length; i++)
                    for (int j = 0; j < Frames[0].Length; j++)
                    {
                        Frames[FrameIndexPlus1][i][j] = Frames[FrameIndex][Frames[0].Length - j - 1][i];
                    }
                RemoveFrame(FrameIndex);
            }
            else//Right
            {
                AddFrame(FrameIndex);
                for (int i = 0; i < Frames[0].Length; i++)
                    for (int j = 0; j < Frames[0].Length; j++)
                    {
                        Frames[FrameIndexPlus1][i][j] = Frames[FrameIndex][j][Frames[0].Length - i - 1];
                    }
                RemoveFrame(FrameIndex);
            }
        }
        public void SetColor1(object obj)
        {
            System.Windows.Controls.Canvas caller = (System.Windows.Controls.Canvas)obj;
            _red1 = ((System.Windows.Media.SolidColorBrush)caller.Background).Color.R;
            _green1 = ((System.Windows.Media.SolidColorBrush)caller.Background).Color.G;
            _blue1 = ((System.Windows.Media.SolidColorBrush)caller.Background).Color.B;
            RaisePropertyChanged("Color1ToString");
        }
        public void SetColor2(object obj)
        {
            System.Windows.Controls.Canvas caller = (System.Windows.Controls.Canvas)obj;
            _red2 = ((System.Windows.Media.SolidColorBrush)caller.Background).Color.R;
            _green2 = ((System.Windows.Media.SolidColorBrush)caller.Background).Color.G;
            _blue2 = ((System.Windows.Media.SolidColorBrush)caller.Background).Color.B;
            RaisePropertyChanged("Color2ToString");
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
