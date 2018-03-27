using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class Type1 : BaseType
    {
        [Display(Order = 1)] public int Flag1 { get; set; }
        [Display(Order = 2)] public int Flag2 { get; set; }
    }
}
