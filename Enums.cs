namespace AoeBoardgame
{
    enum GameState
    {
        MyTurn,
        MovingObject,
        PlacingBuilding,
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
        Orange,
        Purple
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
