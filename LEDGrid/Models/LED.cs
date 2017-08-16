using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDGrid.Models
{
    class LED : INotifyPropertyChanged
    {
        private int _red;
        private int _blue;
        private int _green;
        public LED()
        {
            _red = 255;
            _blue = 0;
            _green = 0;
        }
        public LED(int r, int b, int g)
        {
            _red = r;
            _blue = b;
            _green = g;
        }

        public int Red
        {
            get
            {
                return _red;
            }
            set
            {
                _red = value;
                RaisePropertyChanged("Red");
                RaisePropertyChanged("ToColorString");
            }
        }
        public int Blue
        {
            get
            {
                return _blue;
            }
            set
            {
                _blue = value;
                RaisePropertyChanged("Blue");
                RaisePropertyChanged("ToColorString");
            }

        }
        public int Green
        {
            get
            {
                return _green;
            }
            set
            {
                _green = value;
                RaisePropertyChanged("Green");
                RaisePropertyChanged("ToColorString");
            }
        }

        public string ToColorString
        {
            get
            {
            return "#" + Red.ToString("X2") + Green.ToString("X2") + Blue.ToString("X2"); 
            }
        }

        public LED CopyTo(LED toCopyTo)
        {
            toCopyTo.Red = this.Red;
            toCopyTo.Green = this.Green;
            toCopyTo.Blue = this.Blue;
            return toCopyTo;
        }
        public LED Copy()
        {
            return new LED(this.Red, this.Blue, this.Green);
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
