using System.ComponentModel;
using System.Runtime.CompilerServices;
using Zen.Eve.Colapsar.Annotations;

namespace Zen.Eve.Colapsar.Models
{
    public abstract class MvcModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}