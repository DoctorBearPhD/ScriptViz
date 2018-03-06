using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLib
{
    public class BaseFile
    {
        [Newtonsoft.Json.JsonProperty(Order = 3)]
        public string RawUassetHeaderDontTouch { get; set; }
    }
}
