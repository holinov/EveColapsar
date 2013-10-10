using System;
using System.Windows.Input;

namespace Zen.Eve.Colapsar.Models
{
    public class WormholeListModel : MvcModel
    {
        private WormholeCollection _wormholes=new WormholeCollection()
            /*{
                new WormholeModel(){Class = "testWh-1"},
                new WormholeModel(){Class = "testWh-2"}
            }*/;
        private int _currentWormhole;

        public WormholeCollection Wormholes
        {
            get { return _wormholes; }
            set
            {
                if (Equals(value, _wormholes)) return;
                _wormholes = value;
                OnPropertyChanged();
            }
        }

        public int CurrentWormhole
        {
            get { return _currentWormhole; }
            set
            {
                if (value == _currentWormhole) return;
                _currentWormhole = value;
                OnPropertyChanged();
            }
        }

        #region Commands
        public AddWormholeCommand AddWormhole {get{return new AddWormholeCommand(this);}}
        public class AddWormholeCommand:ICommand
        {
            private readonly WormholeListModel _model;

            public AddWormholeCommand(WormholeListModel model)
            {
                _model = model;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                _model.Wormholes.Add(new WormholeModel());
                _model.CurrentWormhole = _model.Wormholes.Count - 1;
            }

            public event EventHandler CanExecuteChanged;
        }
        #endregion
    }
}