﻿using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Villager : PlayerObject,
        ICanMove,
        IAttacker,
        ICanMakeBuildings
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public List<Type> BuildingTypesAllowedToMake { get; set; }
        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public Villager(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public void Attack(IAttackable defender)
        {
            throw new NotImplementedException();
        }

        public void MakeBuilding<T>(Tile destinationTile) where T : PlayerObject
        {

        }

        public override void UpgradeToFeudalAge()
        {
            AddHitPoints(VillagerFactory.FeudalAddedHitPoints);
            AttackDamage += VillagerFactory.FeudalAddedAttackDamage;
            BuildingTypesAllowedToMake.AddRange(VillagerFactory.FeudalAddedBuildings);
        }

        public override void UpgradeToCastleAge()
        {
            AddHitPoints(VillagerFactory.CastleAddedHitPoints);
            AttackDamage += VillagerFactory.CastleAddedAttackDamage;
            BuildingTypesAllowedToMake.AddRange(VillagerFactory.CastleAddedBuildings);
        }

        public override void UpgradeToImperialAge()
        {
            AddHitPoints(VillagerFactory.ImperialAddedHitPoints);
            AttackDamage += VillagerFactory.ImperialAddedAttackDamage;
        }
    }
}
