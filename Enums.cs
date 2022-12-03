namespace AoeBoardgame
{
    enum UiState
    {
        MainMenu,
        Sandbox,
        LobbyBrowser,
        MultiplayerGame,
        CreatingLobby
    }

    enum GameState
    {
        MyTurn,
        MovingObject,
        PlacingBuilding
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
        // Town center
        FeudalAge,
        CastleAge,
        ImperialAge,

        // Barracks
        FeudalSwordsmen,
        VeteranSwordsmen,
        EliteSwordsmen,

        VeteranArchers,
        EliteArchers,

        VeteranPikemen,
        ElitePikemen,

        // Stable
        EliteKnights,
        Bloodlines,         // Cavalry HP
        Husbandry,          // Cavalry Speed

        // Siege workshop
        HeavyCatapults,
        SiegeEngineers,     // Siege weapons +1 range

        // Blacksmith
        ScaleMailArmor,     // All military units +1 melee armor
        Fletching,          // Archers +1 attack
        Forging,            // Melee units +1 attack

        ChainMailArmor,     // Archers +1/+1 armor
        IronCasting,        // Melee units +1 attack
        BodkinArrow,        // Archers +1 attack

        PlateMailArmor,     // Melee units +1/+2 armor
        BlastFurnace,       // All military units +2 attack

        // University
        // Feudal age
        Loom,               // Villager HP
        Wheelbarrow,        // All gather rates
        DoubleBitAxe,       // Wood gather rate
        Housing,            // Gather group size +1

        // Castle age
        BowSaw,             // Wood gather rate
        HandCart,           // Villager speed
        IronPickaxes,       // Mining gather rates
        MurderHoles,        // Castle & tower minimum range
        Masonry,            // Castle, tower & TC HP

        // Imperial age
        CropRotation,       // 2 farmers per farm
        Conscription,       // Swordsman gold cost and train time reduction
        SupplyLines,        // Army size +1

        // Castle
        // England
        EliteLongbowmen,
        Agriculture,        // British farmers gather +3

        // France
        EliteThrowingAxemen,
        Chivalry,           // Knights +20 HP
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
