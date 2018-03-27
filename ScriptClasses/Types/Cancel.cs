using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class Cancel : BaseType
    {
        [Display(Order = 0)] public int CancelList { get; set; }
        [Display(Order = 1)] public int Type { get; set; }
    }
}
