using System;
using System.Windows;

namespace Zen.Eve.Colapsar.Models
{
    public class Passage:MvcModel
    {
        private int _passedMass;
        private bool _usedMwd;
        private DateTime _passageTime;
        private int _number;
        private PassageDirection _direction;
        private bool _jumpExecuted;
        private int _massLeft;

        public int PassedMass
        {
            get { return _passedMass; }
            set
            {
                if (value == _passedMass) return;
                _passedMass = value;
                OnPropertyChanged();
                if (Wormhole != null) Wormhole.OnPropertyChanged("Mass");
            }
        }

        public bool UsedMwd
        {
            get { return _usedMwd; }
            set
            {
                if (value.Equals(_usedMwd)) return;
                _usedMwd = value;
                OnPropertyChanged();
                OnPropertyChanged("UsedMwdText");
            }
        }
        public string UsedMwdText
        {
            get { return _usedMwd?"с MVD":"БЕЗ MVD"; }
            
        }

        public string DirectionText
        {
            get { return _direction==PassageDirection.Out ? "туда" : "обратно"; }

        }
        public DateTime PassageTime
        {
            get { return _passageTime; }
            set
            {
                if (value.Equals(_passageTime)) return;
                _passageTime = value;
                OnPropertyChanged();
            }
        }

        public WormholeModel Wormhole { get; set; }

        public int Number
        {
            get { return _number; }
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }

        public PassageDirection Direction
        {
            get { return _direction; }
            set
            {
                if (value == _direction) return;
                _direction = value;
                OnPropertyChanged();
            }
        }

        public bool JumpExecuted
        {
            get { return _jumpExecuted; }
            set
            {
                if (value.Equals(_jumpExecuted)) return;
                _jumpExecuted = value;
                OnPropertyChanged();
                OnPropertyChanged("JumpIconVisibility");
            }
        }

        public Visibility JumpIconVisibility { get { return JumpExecuted ? Visibility.Visible : Visibility.Collapsed; } }

        public int MassLeft
        {
            get { return _massLeft; }
            set
            {
                if (value == _massLeft) return;
                _massLeft = value;
                OnPropertyChanged();
            }
        }
    }
}