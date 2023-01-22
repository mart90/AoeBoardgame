namespace AoeBoardgame
{
    enum UiState
    {
        MainMenu,
        Sandbox,
        ChallengeBrowser,
        ChallengeAttempt,
        LoginScreen,
        LobbyBrowser,
        MultiplayerGame,
        CreatingLobby
    }

    enum GameState
    {
        Default,
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
        FeudalAge = 0,
        CastleAge = 1,
        ImperialAge = 2,

        // Barracks
        FeudalSwordsmen = 3,
        VeteranSwordsmen = 4,
        EliteSwordsmen = 5,

        VeteranArchers = 6,
        EliteArchers = 7,

        VeteranPikemen = 8,
        ElitePikemen = 9,

        // Stable
        EliteKnights = 10,
        Bloodlines = 11,        // Cavalry HP
        Husbandry = 12,         // Cavalry Speed

        // Siege workshop
        HeavyCatapults = 13,
        SiegeEngineers = 14,    // Siege weapons +1 range

        // Blacksmith
        ScaleMailArmor = 15,    // All military units +1 melee armor
        Fletching = 16,         // Archers +1 attack
        Forging = 17,           // Melee units +1 attack

        ChainMailArmor = 18,    // Archers +1/+1 armor
        IronCasting = 19,       // Melee units +1 attack
        BodkinArrow = 20,       // Archers +1 attack

        PlateMailArmor = 21,    // Melee units +1/+2 armor
        BlastFurnace = 22,      // All military units +2 attack

        // University
        // Feudal age
        Loom = 23,              // Villager HP
        Wheelbarrow = 24,       // All gather rates
        DoubleBitAxe = 25,      // Wood gather rate
        Housing = 26,           // Gather group size +1

        // Castle age
        BowSaw = 27,            // Wood gather rate
        HandCart = 28,          // Villager speed
        IronPickaxes = 29,      // Mining gather rates
        MurderHoles = 30,       // Castle & tower minimum range
        Masonry = 31,           // Castle, tower & TC HP

        // Imperial age
        CropRotation = 32,      // 2 farmers per farm
        Conscription = 33,      // Swordsman gold cost and train time reduction
        SupplyLines = 34,       // Army size +1

        // Castle
        // England
        EliteLongbowmen = 35,
        Agriculture = 36,       // British farmers gather +3

        // France
        EliteThrowingAxemen = 37,
        Chivalry = 38,          // Knights +20 HP
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
        BlueUsed,
        Red,
        RedUsed,
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

    enum ChallengeType
    {
        EarlyRush = 0,
        WonderRush = 1,
        Boom = 2
    }
    
    enum UiType
    {
        // Buttons
        LoginButton,
        RegisterButton,
        EndTurnButton,

        // Interface
        SidePanelBackground
    }
}
