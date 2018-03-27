using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class Force : BaseType
    {
        [Display(Order = 1)] public float Amount { get; set; }
        [Display(Order = 2)] public object Flag { get; set; }
    }
}
