namespace AoeBoardgame
{
    interface IEconomicBuilding : IContainsUnits
    {
        Resource Resource { get; set; }
    }
}
