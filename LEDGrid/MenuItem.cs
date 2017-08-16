using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDGrid
{
    class MenuItem : INotifyPropertyChanged
    {
        private string _name;
        private object _view;

        public MenuItem(string name, object view)
        {
            Name = name;
            View = view;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        public object View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                RaisePropertyChanged("View");
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
