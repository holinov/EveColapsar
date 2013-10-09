using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zen.Eve.Colapsar.Models
{
    public class PassagesCollection:ObservableCollection<Passage>
    {
        public PassagesCollection()
        {
            
        }
        public PassagesCollection(List<Passage> passages)
            : base(passages)
        {
            
        }
    }
}