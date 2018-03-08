using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptViz.ViewModel
{
    public class TabItemViewModel : VMBase
    {
        public string Header { get; set; }

        public object Content { get; set; }
    }
}
