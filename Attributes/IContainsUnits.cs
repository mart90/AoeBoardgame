using System.Collections.Generic;

namespace AoeBoardgame
{
    interface IContainsUnits
    {
        List<ICanFormGroup> Units { get; }
        int MaxUnits { get; set; }
    }
}
