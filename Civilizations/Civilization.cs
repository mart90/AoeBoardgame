using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    abstract class Civilization
    {
        protected readonly TextureLibrary TextureLibrary;
        protected readonly ResearchLibrary ResearchLibrary;

        protected Civilization(TextureLibrary textureLibrary, ResearchLibrary researchLibrary)
        {
            TextureLibrary = textureLibrary;
            ResearchLibrary = researchLibrary;
        }

        public virtual IEnumerable<PlaceableObjectFactory> GetFactories(Player player)
        {
            var factories = new List<PlaceableObjectFactory>();

            factories.AddRange(new List<PlaceableObjectFactory>
            {
                // Buildings
                GetTownCenterFactory(),
                new LumberCampFactory(TextureLibrary),
                new MineFactory(TextureLibrary),
                GetFarmFactory(),
                GetUniversityFactory(),
                GetBarracksFactory(),
                GetStableFactory(),
                GetBlacksmithFactory(),
                GetSiegeWorkshopFactory(),
                new TowerFactory(TextureLibrary),
                new GuardTowerFactory(TextureLibrary),
                GetCastleFactory(),
                new WonderFactory(TextureLibrary),

                // Units
                new ArmyFactory(TextureLibrary),
                new RangedArmyFactory(TextureLibrary),
                new GathererGroupFactory(TextureLibrary),
                GetVillagerFactory(),
                GetArcherFactory(),
                GetSwordsmanFactory(),
                GetPikemanFactory(),
                GetScoutFactory(),
                GetKnightFactory(),
                GetCatapultFactory(),
                GetTrebuchetFactory()
            });

            return factories;
        }

        public abstract CastleFactory GetCastleFactory();

        public virtual IEnumerable<ResourceGatherRate> GetBaseGatherRates()
        {
            return new List<ResourceGatherRate>
            {
                new ResourceGatherRate(Resource.Food, 7),
                new ResourceGatherRate(Resource.Wood, 5),
                new ResourceGatherRate(Resource.Gold, 5),
                new ResourceGatherRate(Resource.Iron, 5),
                new ResourceGatherRate(Resource.Stone, 5)
            };
        }

        public virtual IEnumerable<ResourceCollection> GetStartingResources()
        {
            return new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Food, 9999),
                new ResourceCollection(Resource.Wood, 9999),
                new ResourceCollection(Resource.Gold, 9999),
                new ResourceCollection(Resource.Iron, 9999),
                new ResourceCollection(Resource.Stone, 9999)
                //new ResourceCollection(Resource.Food, 200),
                //new ResourceCollection(Resource.Wood, 100),
                //new ResourceCollection(Resource.Gold, 30),
                //new ResourceCollection(Resource.Iron, 0),
                //new ResourceCollection(Resource.Stone, 0)
            };
        }

        public virtual void UnlockFeudalAge(Player player)
        {
            player.AddAllowedBuildings<Villager>(new List<Type>
            {
                typeof(Blacksmith),
                typeof(University),
                typeof(Stable),
                typeof(Tower)
            });

            player.AddAllowedResearch<TownCenter>(new List<ResearchEnum>
            {
                ResearchEnum.CastleAge
            });

            player.AddAllowedUnits<Barracks>(new List<Type> 
            { 
                typeof(Pikeman), 
                typeof(Archer) 
            });
            player.AddAllowedResearch<Barracks>(new List<ResearchEnum> 
            { 
                ResearchEnum.FeudalSwordsmen 
            });
        }

        public virtual void UnlockCastleAge(Player player)
        {
            player.AddAllowedBuildings<Villager>(new List<Type>
            {
                typeof(TownCenter),
                typeof(SiegeWorkshop),
                typeof(Castle),
                typeof(GuardTower)
            });

            player.AddAllowedResearch<TownCenter>(new List<ResearchEnum>
            {
                ResearchEnum.ImperialAge
            });

            player.AddAllowedResearch<University>(new List<ResearchEnum>
            {
                ResearchEnum.HandCart,
                ResearchEnum.Housing,
                ResearchEnum.BowSaw,
                ResearchEnum.IronPickaxes,
                ResearchEnum.MurderHoles,
                ResearchEnum.Masonry
            });

            player.AddAllowedResearch<Blacksmith>(new List<ResearchEnum>
            {
                ResearchEnum.ChainMailArmor,
                ResearchEnum.IronCasting,
                ResearchEnum.BodkinArrow
            });

            player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
            {
                ResearchEnum.VeteranArchers,
                ResearchEnum.VeteranPikemen
            });

            SwordsmanFactory factory = (SwordsmanFactory)player.GetFactoryByObjectType(typeof(Swordsman));

            if (factory.UpgradeLevel == 1)
            {
                player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                {
                    ResearchEnum.VeteranSwordsmen
                });
            }

            player.AddAllowedUnits<Stable>(new List<Type>
            {
                typeof(Knight)
            });

            player.AddAllowedResearch<Stable>(new List<ResearchEnum>
            {
                ResearchEnum.Husbandry
            });
        }

        public virtual void UnlockImperialAge(Player player)
        {
            player.AddAllowedBuildings<Villager>(new List<Type>
            {
                typeof(Wonder)
            });

            player.AddAllowedResearch<University>(new List<ResearchEnum>
            {
                ResearchEnum.CropRotation,
                ResearchEnum.Conscription,
                ResearchEnum.SupplyLines
            });

            player.AddAllowedResearch<Blacksmith>(new List<ResearchEnum>
            {
                ResearchEnum.PlateMailArmor,
                ResearchEnum.BlastFurnace
            });

            player.AddAllowedUnits<SiegeWorkshop>(new List<Type>
            {
                typeof(Trebuchet)
            });
            player.AddAllowedResearch<SiegeWorkshop>(new List<ResearchEnum>
            {
                ResearchEnum.HeavyCatapults,
                ResearchEnum.SiegeEngineers
            });

            player.AddAllowedResearch<Stable>(new List<ResearchEnum>
            {
                ResearchEnum.EliteKnights
            });

            SwordsmanFactory swordsmanFactory = (SwordsmanFactory)player.GetFactoryByObjectType(typeof(Swordsman));
            ArcherFactory archerFactory = (ArcherFactory)player.GetFactoryByObjectType(typeof(Archer));
            PikemanFactory pikemanFactory = (PikemanFactory)player.GetFactoryByObjectType(typeof(Pikeman));

            if (swordsmanFactory.UpgradeLevel == 2)
            {
                player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                {
                    ResearchEnum.EliteSwordsmen
                });
            }
            if (archerFactory.UpgradeLevel == 2)
            {
                player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                {
                    ResearchEnum.EliteArchers
                });
            }
            if (pikemanFactory.UpgradeLevel == 2)
            {
                player.AddAllowedResearch<Barracks>(new List<ResearchEnum>
                {
                    ResearchEnum.ElitePikemen
                });
            }
        }

        public virtual VillagerFactory GetVillagerFactory()
        {
            return new VillagerFactory(TextureLibrary);
        }

        public virtual TownCenterFactory GetTownCenterFactory()
        {
            return new TownCenterFactory(TextureLibrary)
            {
                ResearchAllowedToMake = new List<ResearchEnum>
                {
                    ResearchEnum.FeudalAge
                }
            };
        }

        public virtual BarracksFactory GetBarracksFactory()
        {
            return new BarracksFactory(TextureLibrary);
        }

        public virtual UniversityFactory GetUniversityFactory()
        {
            return new UniversityFactory(TextureLibrary)
            {
                ResearchAllowedToMake = new List<ResearchEnum>
                {
                    ResearchEnum.Loom,
                    ResearchEnum.Wheelbarrow,
                    ResearchEnum.DoubleBitAxe
                }
            };
        }

        public virtual StableFactory GetStableFactory()
        {
            return new StableFactory(TextureLibrary)
            {
                ResearchAllowedToMake = new List<ResearchEnum>
                {
                    ResearchEnum.Bloodlines
                }
            };
        }

        public virtual SiegeWorkshopFactory GetSiegeWorkshopFactory()
        {
            return new SiegeWorkshopFactory(TextureLibrary);
        }

        public virtual BlacksmithFactory GetBlacksmithFactory()
        {
            return new BlacksmithFactory(TextureLibrary)
            {
                ResearchAllowedToMake = new List<ResearchEnum>
                {
                    ResearchEnum.ScaleMailArmor,
                    ResearchEnum.Fletching,
                    ResearchEnum.Forging
                }
            };
        }

        public virtual FarmFactory GetFarmFactory()
        {
            return new FarmFactory(TextureLibrary);
        }

        public virtual ArcherFactory GetArcherFactory()
        {
            return new ArcherFactory(TextureLibrary);
        }

        public virtual SwordsmanFactory GetSwordsmanFactory()
        {
            return new SwordsmanFactory(TextureLibrary);
        }

        public virtual PikemanFactory GetPikemanFactory()
        {
            return new PikemanFactory(TextureLibrary);
        }

        public virtual ScoutFactory GetScoutFactory()
        {
            return new ScoutFactory(TextureLibrary);
        }

        public virtual KnightFactory GetKnightFactory()
        {
            return new KnightFactory(TextureLibrary);
        }

        public virtual CatapultFactory GetCatapultFactory()
        {
            return new CatapultFactory(TextureLibrary);
        }

        public virtual TrebuchetFactory GetTrebuchetFactory()
        {
            return new TrebuchetFactory(TextureLibrary);
        }
    }
}
