using System.Linq;

namespace AoeBoardgame
{
    interface ICanFormGroup : ICanMove, IAttacker
    {
        bool IsSubSelected { get; set; }
    }

    static class ICanFormGroupMethods
    {
        public static ICanFormGroup SubSelectedUnit<T>(this T group) where T : IContainsUnits
        {
            return group.Units.SingleOrDefault(e => e.IsSubSelected);
        }
    }
}
