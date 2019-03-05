namespace AoeBoardgame
{
    enum GameState
    {
        TurnStart,
        MainPhase,
        TurnEnd
    }

    enum Resource
    {
        Food,
        Wood,
        Gold,
        Iron,
        Stone
    }

    enum ResearchEnum
    {
        FeudalAge,
        CastleAge,
        ImperialAge,
    }

    enum TileType
    {
        Dirt,
        Forest,
        StoneMine,
        GoldMine,
        IronMine
    }

    enum TileColor
    {
        Default,
        Blue,
        Red,
        Green,
        Teal,
        Pink,
        Orange
    }

    enum Direction
    {
        Default,
        NorthWest,
        NorthEast,
        East,
        SouthEast,
        SouthWest,
        West
    }
}
