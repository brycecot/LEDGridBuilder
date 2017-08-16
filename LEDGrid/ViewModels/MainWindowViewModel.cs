using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEDGrid.Views;

namespace LEDGrid.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {

        private MenuItem[] _modes = new MenuItem[2];
        private bool _menuToggleButton =false;

        public MainWindowViewModel()
        {
            Modes[0] = new MenuItem("Draw", new DrawGrid { DataContext = new DrawGridViewModel()});
            Modes[1] = new MenuItem("Animate", new Animation{ DataContext = new AnimationViewModel() });
        }

        public MenuItem[] Modes
        {
            get
            {
                return _modes;
            }
            set
            {
                _modes = value;
                RaisePropertyChanged("Modes");
            }
        }
        public string Meow
        {
            get;
            set;
        } = "Meow";

        public bool MenuToggleButton
        {
            get
            {
                return _menuToggleButton;
            }
            set
            {
                _menuToggleButton = value;
                RaisePropertyChanged("MenuToggleButton");
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
