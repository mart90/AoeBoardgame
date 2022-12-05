using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class ResearchLibrary
    {
        private readonly List<Research> _researchCollection;

        public Research GetByResearchEnum(ResearchEnum researchEnum)
        {
            return _researchCollection.SingleOrDefault(e => e.ResearchEnum == researchEnum);
        }

        public ResearchLibrary()
        {
            _researchCollection = new List<Research>
            {
                new Research
                {
                    ResearchEnum = ResearchEnum.FeudalAge,
                    UiName = "Feudal age",
                    UiDescription = "Unlocks new units, upgrades and buildings",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 300)
                    },
                    TurnsToComplete = 5,
                    Effect = player => 
                    {
                        player.Age = 2;

                        foreach (Scout scout in player.OwnedObjects.Where(e => e is Scout))
                        {
                            scout.AttackDamage += 3;
                        }

                        player.Civilization.UnlockFeudalAge(player);
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.CastleAge,
                    UiName = "Castle age",
                    UiDescription = "Unlocks new units, upgrades and buildings",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 300),
                        new ResourceCollection(Resource.Gold, 300)
                    },
                    TurnsToComplete = 6,
                    Effect = player =>
                    {
                        player.Age = 3;
                        player.Civilization.UnlockCastleAge(player);
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.ImperialAge,
                    UiName = "Imperial age",
                    UiDescription = "Unlocks the most powerful units and upgrades",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 300),
                        new ResourceCollection(Resource.Gold, 300),
                        new ResourceCollection(Resource.Iron, 300)
                    },
                    TurnsToComplete = 8,
                    Effect = player =>
                    {
                        player.Age = 4;
                        player.Civilization.UnlockImperialAge(player);
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.FeudalSwordsmen,
                    UiName = "Feudal\nswordsmen",
                    UiDescription = "Upgrades your swordsmen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 50)
                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        SwordsmanFactory factory = (SwordsmanFactory)player.GetFactoryByObjectType(typeof(Swordsman));

                        factory.UpgradeLevel = 1;
                        factory.HitPoints += 15;
                        factory.AttackDamage += 2;
                        factory.RangedArmor += 1;

                        foreach (Swordsman unit in player.OwnedObjects.Where(e => e is Swordsman))
                        {
                            unit.UpgradeLevel = 1;
                            unit.MaxHitPoints += 15;
                            unit.HitPoints += 15;
                            unit.AttackDamage += 2;
                            unit.RangedArmor += 1;
                        }

                        if (player.Age > 2)
                        {
                            player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                            {
                                ResearchEnum.VeteranSwordsmen
                            });
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.VeteranSwordsmen,
                    UiName = "Veteran\nswordsmen",
                    UiDescription = "Upgrades your swordsmen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        SwordsmanFactory factory = (SwordsmanFactory)player.GetFactoryByObjectType(typeof(Swordsman));

                        factory.UpgradeLevel = 2;
                        factory.HitPoints += 15;
                        factory.AttackDamage += 2;
                        factory.MeleeArmor += 1;
                        factory.RangedArmor += 1;

                        foreach (Swordsman unit in player.OwnedObjects.Where(e => e is Swordsman))
                        {
                            unit.UpgradeLevel = 2;
                            unit.MaxHitPoints += 15;
                            unit.HitPoints += 15;
                            unit.AttackDamage += 2;
                            unit.MeleeArmor += 1;
                            unit.RangedArmor += 1;
                        }

                        if (player.Age > 3)
                        {
                            player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                            {
                                ResearchEnum.EliteSwordsmen
                            });
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.EliteSwordsmen,
                    UiName = "Elite\nswordsmen",
                    UiDescription = "Upgrades your swordsmen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 100),
                        new ResourceCollection(Resource.Iron, 200)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        SwordsmanFactory factory = (SwordsmanFactory)player.GetFactoryByObjectType(typeof(Swordsman));

                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 15;
                        factory.AttackDamage += 2;
                        factory.MeleeArmor += 1;
                        factory.RangedArmor += 1;

                        foreach (Swordsman unit in player.OwnedObjects.Where(e => e is Swordsman))
                        {
                            unit.UpgradeLevel = 3;
                            unit.MaxHitPoints += 15;
                            unit.HitPoints += 15;
                            unit.AttackDamage += 2;
                            unit.MeleeArmor += 1;
                            unit.RangedArmor += 1;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.VeteranArchers,
                    UiName = "Veteran\narchers",
                    UiDescription = "Upgrades your archers",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 150),
                        new ResourceCollection(Resource.Iron, 50),

                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        ArcherFactory factory = (ArcherFactory)player.GetFactoryByObjectType(typeof(Archer));

                        factory.UpgradeLevel = 2;
                        factory.HitPoints += 20;
                        factory.AttackDamage += 3;

                        foreach (Archer unit in player.OwnedObjects.Where(e => e is Archer))
                        {
                            unit.UpgradeLevel = 2;
                            unit.MaxHitPoints += 20;
                            unit.HitPoints += 20;
                            unit.AttackDamage += 3;
                        }

                        if (player.Age > 3)
                        {
                            player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                            {
                                ResearchEnum.EliteArchers
                            });
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.EliteArchers,
                    UiName = "Elite\narchers",
                    UiDescription = "Upgrades your archers",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 250),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        ArcherFactory factory = (ArcherFactory)player.GetFactoryByObjectType(typeof(Archer));

                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 20;
                        factory.AttackDamage += 3;

                        foreach (Archer unit in player.OwnedObjects.Where(e => e is Archer))
                        {
                            unit.UpgradeLevel = 3;
                            unit.MaxHitPoints += 20;
                            unit.HitPoints += 20;
                            unit.AttackDamage += 3;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.VeteranPikemen,
                    UiName = "Veteran\npikemen",
                    UiDescription = "Upgrades your pikemen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 50),
                        new ResourceCollection(Resource.Iron, 100),

                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        PikemanFactory factory = (PikemanFactory)player.GetFactoryByObjectType(typeof(Pikeman));

                        factory.UpgradeLevel = 2;
                        factory.HitPoints += 20;
                        factory.AttackDamage += 2;

                        foreach (Pikeman unit in player.OwnedObjects.Where(e => e is Pikeman))
                        {
                            unit.UpgradeLevel = 2;
                            unit.MaxHitPoints += 20;
                            unit.HitPoints += 20;
                            unit.AttackDamage += 2;
                        }

                        if (player.Age > 3)
                        {
                            player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                            {
                                ResearchEnum.ElitePikemen
                            });
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.ElitePikemen,
                    UiName = "Elite\npikemen",
                    UiDescription = "Upgrades your pikemen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 100),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        PikemanFactory factory = (PikemanFactory)player.GetFactoryByObjectType(typeof(Pikeman));

                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 20;
                        factory.AttackDamage += 2;

                        foreach (Pikeman unit in player.OwnedObjects.Where(e => e is Pikeman))
                        {
                            unit.UpgradeLevel = 3;
                            unit.MaxHitPoints += 20;
                            unit.HitPoints += 20;
                            unit.AttackDamage += 2;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.EliteKnights,
                    UiName = "Elite\nknights",
                    UiDescription = "Upgrades your knights",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 150),
                        new ResourceCollection(Resource.Iron, 250)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        KnightFactory factory = (KnightFactory)player.GetFactoryByObjectType(typeof(Knight));

                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 30;
                        factory.AttackDamage += 4;
                        factory.MeleeArmor += 1;
                        factory.RangedArmor += 2;

                        foreach (Knight unit in player.OwnedObjects.Where(e => e is Knight))
                        {
                            unit.UpgradeLevel = 3;
                            unit.MaxHitPoints += 30;
                            unit.HitPoints += 30;
                            unit.AttackDamage += 4;
                            unit.MeleeArmor += 1;
                            unit.RangedArmor += 2;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Bloodlines,
                    UiName = "Bloodlines",
                    UiDescription = "Cavalry +10 HP",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 100),
                        new ResourceCollection(Resource.Gold, 50)
                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        KnightFactory knightFactory = (KnightFactory)player.GetFactoryByObjectType(typeof(Knight));
                        knightFactory.HitPoints += 10;

                        foreach (Knight knight in player.OwnedObjects.Where(e => e is Knight))
                        {
                            knight.MaxHitPoints += 10;
                            knight.HitPoints += 10;
                        }

                        ScoutFactory scoutFactory = (ScoutFactory)player.GetFactoryByObjectType(typeof(Scout));
                        scoutFactory.HitPoints += 10;

                        foreach (Scout scout in player.OwnedObjects.Where(e => e is Scout))
                        {
                            scout.MaxHitPoints += 10;
                            scout.HitPoints += 10;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Husbandry,
                    UiName = "Husbandry",
                    UiDescription = "Cavalry +1 speed",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 100),
                        new ResourceCollection(Resource.Gold, 200)
                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        KnightFactory knightFactory = (KnightFactory)player.GetFactoryByObjectType(typeof(Knight));
                        knightFactory.Speed += 1;

                        foreach (Knight knight in player.OwnedObjects.Where(e => e is Knight))
                        {
                            knight.Speed += 1;
                        }

                        ScoutFactory scoutFactory = (ScoutFactory)player.GetFactoryByObjectType(typeof(Scout));
                        scoutFactory.Speed += 1;

                        foreach (Scout scout in player.OwnedObjects.Where(e => e is Scout))
                        {
                            scout.Speed += 1;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.HeavyCatapults,
                    UiName = "Heavy\ncatapults",
                    UiDescription = "Upgrades your catapults",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 250),
                        new ResourceCollection(Resource.Iron, 400)
                    },
                    TurnsToComplete = 6,
                    Effect = player =>
                    {
                        CatapultFactory factory = (CatapultFactory)player.GetFactoryByObjectType(typeof(Catapult));
                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 30;
                        factory.AttackDamage += 20;
                        factory.RangedArmor += 3;

                        foreach (Catapult unit in player.OwnedObjects.Where(e => e is Catapult))
                        {
                            unit.UpgradeLevel = 3;
                            unit.HitPoints += 30;
                            unit.MaxHitPoints += 30;
                            unit.AttackDamage += 20;
                            unit.RangedArmor += 3;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.SiegeEngineers,
                    UiName = "Siege\nengineers",
                    UiDescription = "Siege weapons +1 range",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 100),
                        new ResourceCollection(Resource.Gold, 100),
                        new ResourceCollection(Resource.Iron, 250)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        CatapultFactory catapultFactory = (CatapultFactory)player.GetFactoryByObjectType(typeof(Catapult));
                        catapultFactory.Range += 1;

                        foreach (Catapult catapult in player.OwnedObjects.Where(e => e is Catapult))
                        {
                            catapult.Range += 1;
                        }

                        TrebuchetFactory trebuchetFactory = (TrebuchetFactory)player.GetFactoryByObjectType(typeof(Trebuchet));
                        trebuchetFactory.Range += 1;

                        foreach (Trebuchet treb in player.OwnedObjects.Where(e => e is Trebuchet))
                        {
                            treb.Range += 1;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.ScaleMailArmor,
                    UiName = "Scale mail\narmor",
                    UiDescription = "All military units +1 melee armor",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IMilitaryUnit))
                        {
                            factory.MeleeArmor++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IMilitaryUnit))
                        {
                            unit.MeleeArmor++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Fletching,
                    UiName = "Fletching",
                    UiDescription = "Archers +1 attack",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 50)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IArcher).Cast<IMilitaryUnit>())
                        {
                            factory.AttackDamage++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IArcher))
                        {
                            unit.AttackDamage++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Forging,
                    UiName = "Forging",
                    UiDescription = "Melee military units +1 attack",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IMilitaryUnit && !(e is IHasRange)))
                        {
                            factory.AttackDamage++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IMilitaryUnit && !(e is IHasRange)))
                        {
                            unit.AttackDamage++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.ChainMailArmor,
                    UiName = "Chain mail\narmor",
                    UiDescription = "Archers +1/+1 armor",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 100),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IArcher).Cast<IMilitaryUnit>())
                        {
                            factory.RangedArmor++;
                            factory.MeleeArmor++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IArcher).Cast<IMilitaryUnit>())
                        {
                            unit.MeleeArmor++;
                            unit.RangedArmor++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.IronCasting,
                    UiName = "Iron casting",
                    UiDescription = "Melee military units +1 attack",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IMilitaryUnit && !(e is IHasRange)))
                        {
                            factory.AttackDamage++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IMilitaryUnit && !(e is IHasRange)))
                        {
                            unit.AttackDamage++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.BodkinArrow,
                    UiName = "Bodkin arrow",
                    UiDescription = "Archers +1 attack",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IArcher).Cast<IMilitaryUnit>())
                        {
                            factory.AttackDamage++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IArcher))
                        {
                            unit.AttackDamage++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.PlateMailArmor,
                    UiName = "Plate mail\narmor",
                    UiDescription = "Melee military units +1/+2 armor",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 150),
                        new ResourceCollection(Resource.Iron, 250)
                    },
                    TurnsToComplete = 6,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IMilitaryUnit && !(e is IHasRange)))
                        {
                            factory.RangedArmor += 2;
                            factory.MeleeArmor++;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IMilitaryUnit && !(e is IHasRange)))
                        {
                            unit.RangedArmor += 2;
                            unit.MeleeArmor++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.BlastFurnace,
                    UiName = "Blast furnace",
                    UiDescription = "All military units +2 attack",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 150),
                        new ResourceCollection(Resource.Iron, 250)
                    },
                    TurnsToComplete = 6,
                    Effect = player =>
                    {
                        foreach (IMilitaryUnit factory in player.Factories.Where(e => e is IMilitaryUnit))
                        {
                            factory.AttackDamage += 2;
                        }

                        foreach (IMilitaryUnit unit in player.OwnedObjects.Where(e => e is IMilitaryUnit))
                        {
                            unit.AttackDamage += 2;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Loom,
                    UiName = "Loom",
                    UiDescription = "Villagers +20 HP",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 30)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        VillagerFactory factory = (VillagerFactory)player.GetFactoryByObjectType(typeof(Villager));
                        factory.HitPoints += 20;

                        foreach (Villager unit in player.OwnedObjects.Where(e => e is Villager))
                        {
                            unit.HitPoints += 20;
                            unit.MaxHitPoints += 20;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Wheelbarrow,
                    UiName = "Wheelbarrow",
                    UiDescription = "All gather rates +1 per turn per gatherer",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 100),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        foreach (ResourceGatherRate gatherRate in player.GatherRates)
                        {
                            gatherRate.GatherRate++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.DoubleBitAxe,
                    UiName = "Double-bit\naxe",
                    UiDescription = "Wood gather rate +1 per turn per gatherer",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 100),
                        new ResourceCollection(Resource.Iron, 50)
                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        player.GatherRates.Single(e => e.Resource == Resource.Wood).GatherRate++;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Housing,
                    UiName = "Housing",
                    UiDescription = "Mines and lumber camps can hold +1 gatherer",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 100),
                        new ResourceCollection(Resource.Stone, 50)
                    },
                    TurnsToComplete = 3,
                    Effect = player =>
                    {
                        MineFactory mineFactory = (MineFactory)player.GetFactoryByObjectType(typeof(Mine));
                        mineFactory.MaxUnits++;

                        LumberCampFactory lumberCampFactory = (LumberCampFactory)player.GetFactoryByObjectType(typeof(LumberCamp));
                        lumberCampFactory.MaxUnits++;

                        foreach (Mine mine in player.OwnedObjects.Where(e => e is Mine))
                        {
                            mine.MaxUnits++;
                        }

                        foreach (LumberCamp lumberCamp in player.OwnedObjects.Where(e => e is LumberCamp))
                        {
                            lumberCamp.MaxUnits++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.BowSaw,
                    UiName = "Bow saw",
                    UiDescription = "Wood gather rate +2 per turn per gatherer",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 100),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        player.GatherRates.Single(e => e.Resource == Resource.Wood).GatherRate += 2;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.HandCart,
                    UiName = "Hand cart",
                    UiDescription = "Villagers +1 speed",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 100),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        foreach (VillagerFactory factory in player.Factories.Where(e => e is VillagerFactory))
                        {
                            factory.Speed++;
                        }

                        foreach (Villager unit in player.OwnedObjects.Where(e => e is Villager))
                        {
                            unit.Speed++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.IronPickaxes,
                    UiName = "Iron pickaxes",
                    UiDescription = "All mines gather rate +1 per turn per gatherer",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Gold, 50),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        player.GatherRates.Single(e => e.Resource == Resource.Gold).GatherRate++;
                        player.GatherRates.Single(e => e.Resource == Resource.Stone).GatherRate++;
                        player.GatherRates.Single(e => e.Resource == Resource.Iron).GatherRate++;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.MurderHoles,
                    UiName = "Murder holes",
                    UiDescription = "Removes the minimum range from towers and castles",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 50),
                        new ResourceCollection(Resource.Iron, 100)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        TowerFactory towerFactory = (TowerFactory)player.GetFactoryByObjectType(typeof(Tower));
                        towerFactory.HasMinimumRange = false;

                        GuardTowerFactory guardTowerFactory = (GuardTowerFactory)player.GetFactoryByObjectType(typeof(GuardTower));
                        guardTowerFactory.HasMinimumRange = false;

                        CastleFactory castleFactory = (CastleFactory)player.GetFactoryByObjectType(typeof(Castle));
                        castleFactory.HasMinimumRange = false;

                        foreach (Tower tower in player.OwnedObjects.Where(e => e is Tower))
                        {
                            tower.HasMinimumRange = false;
                        }

                        foreach (GuardTower guardTower in player.OwnedObjects.Where(e => e is GuardTower))
                        {
                            guardTower.HasMinimumRange = false;
                        }

                        foreach (Castle castle in player.OwnedObjects.Where(e => e is Castle))
                        {
                            castle.HasMinimumRange = false;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Masonry,
                    UiName = "Masonry",
                    UiDescription = "Town centers, towers and castles +50% HP",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 150),
                        new ResourceCollection(Resource.Stone, 150)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        TownCenterFactory townCenterFactory = (TownCenterFactory)player.GetFactoryByObjectType(typeof(TownCenter));
                        townCenterFactory.HitPoints += townCenterFactory.HitPoints / 2;

                        TowerFactory towerFactory = (TowerFactory)player.GetFactoryByObjectType(typeof(Tower));
                        towerFactory.HitPoints += towerFactory.HitPoints / 2;

                        GuardTowerFactory guardTowerFactory = (GuardTowerFactory)player.GetFactoryByObjectType(typeof(GuardTower));
                        guardTowerFactory.HitPoints += guardTowerFactory.HitPoints / 2;

                        CastleFactory castleFactory = (CastleFactory)player.GetFactoryByObjectType(typeof(Castle));
                        castleFactory.HitPoints += castleFactory.HitPoints / 2;

                        foreach (TownCenter tc in player.OwnedObjects.Where(e => e is TownCenter))
                        {
                            tc.HitPoints += tc.MaxHitPoints / 2;
                            tc.MaxHitPoints += tc.MaxHitPoints / 2;
                        }

                        foreach (Tower tower in player.OwnedObjects.Where(e => e is Tower))
                        {
                            tower.HitPoints += tower.MaxHitPoints / 2;
                            tower.MaxHitPoints += tower.MaxHitPoints / 2;
                        }

                        foreach (GuardTower guardTower in player.OwnedObjects.Where(e => e is GuardTower))
                        {
                            guardTower.HitPoints += guardTower.MaxHitPoints / 2;
                            guardTower.MaxHitPoints += guardTower.MaxHitPoints / 2;
                        }

                        foreach (Castle castle in player.OwnedObjects.Where(e => e is Castle))
                        {
                            castle.HitPoints += castle.MaxHitPoints / 2;
                            castle.MaxHitPoints += castle.MaxHitPoints / 2;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.CropRotation,
                    UiName = "Crop rotation",
                    UiDescription = "Farms +1 gatherer limit",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 150),
                        new ResourceCollection(Resource.Gold, 150)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        FarmFactory factory = (FarmFactory)player.GetFactoryByObjectType(typeof(Farm));
                        factory.MaxUnits++;

                        foreach (Farm farm in player.OwnedObjects.Where(e => e is Farm))
                        {
                            farm.MaxUnits++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Conscription,
                    UiName = "Conscription",
                    UiDescription = "Swordsmen no longer cost gold and train one turn faster",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 250),
                        new ResourceCollection(Resource.Gold, 100)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        SwordsmanFactory factory = (SwordsmanFactory)player.GetFactoryByObjectType(typeof(Swordsman));
                        factory.Cost.Single(e => e.Resource == Resource.Gold).Amount = 0;
                        factory.TurnsToComplete--;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.SupplyLines,
                    UiName = "Supply lines",
                    UiDescription = "Armies can contain an extra unit",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 200),
                        new ResourceCollection(Resource.Gold, 200)
                    },
                    TurnsToComplete = 5,
                    Effect = player =>
                    {
                        ArmyFactory factory = (ArmyFactory)player.GetFactoryByObjectType(typeof(Army));
                        factory.MaxUnits++;

                        RangedArmyFactory rangedFactory = (RangedArmyFactory)player.GetFactoryByObjectType(typeof(RangedArmy));
                        rangedFactory.MaxUnits++;

                        foreach (Army army in player.OwnedObjects.Where(e => e is Army))
                        {
                            army.MaxUnits++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.EliteLongbowmen,
                    UiName = "Elite\nlongbowmen",
                    UiDescription = "Upgrades your longbowmen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 300),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        LongbowmanFactory factory = (LongbowmanFactory)player.GetFactoryByObjectType(typeof(Longbowman));

                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 20;
                        factory.AttackDamage += 3;

                        foreach (Longbowman unit in player.OwnedObjects.Where(e => e is Longbowman))
                        {
                            unit.UpgradeLevel = 3;
                            unit.MaxHitPoints += 20;
                            unit.HitPoints += 20;
                            unit.AttackDamage += 3;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Agriculture,
                    UiName = "Agriculture",
                    UiDescription = "Farmers gather +5 food per turn",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 150),
                        new ResourceCollection(Resource.Gold, 250)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        player.GatherRates.Single(e => e.Resource == Resource.Food).GatherRate += 5;
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.EliteThrowingAxemen,
                    UiName = "Elite\nthrowing axemen",
                    UiDescription = "Upgrades your throwing axemen",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Wood, 300),
                        new ResourceCollection(Resource.Iron, 150)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        ThrowingAxemanFactory factory = (ThrowingAxemanFactory)player.GetFactoryByObjectType(typeof(ThrowingAxeman));

                        factory.UpgradeLevel = 3;
                        factory.HitPoints += 20;
                        factory.AttackDamage += 3;
                        factory.ArmorPierce++;

                        foreach (ThrowingAxeman unit in player.OwnedObjects.Where(e => e is ThrowingAxeman))
                        {
                            unit.UpgradeLevel = 3;
                            unit.MaxHitPoints += 20;
                            unit.HitPoints += 20;
                            unit.AttackDamage += 3;
                            unit.ArmorPierce++;
                        }
                    }
                },

                new Research
                {
                    ResearchEnum = ResearchEnum.Chivalry,
                    UiName = "Chivalry",
                    UiDescription = "Knights +20 HP",
                    Cost = new List<ResourceCollection>
                    {
                        new ResourceCollection(Resource.Food, 200),
                        new ResourceCollection(Resource.Gold, 200)
                    },
                    TurnsToComplete = 4,
                    Effect = player =>
                    {
                        KnightFactory factory = (KnightFactory)player.GetFactoryByObjectType(typeof(Knight));
                        factory.HitPoints += 20;

                        foreach (Knight unit in player.OwnedObjects.Where(e => e is Knight))
                        {
                            unit.HitPoints += 20;
                            unit.MaxHitPoints += 20;
                        }
                    }
                },
            };
        }
    }
}
