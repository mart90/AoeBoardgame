﻿using System.Collections.Generic;

namespace AoeBoardgame
{
    class KnightFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, ICavalry
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int MeleeArmor { get; set; }
        public int RangedArmor { get; set; }
        public int Speed { get; set; }
        public int ArmorPierce { get; set; }

        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public KnightFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Knight);
            TurnsToComplete = 3;
            UpgradeLevel = 2;
        }

        protected override void SetBaseValues()
        {
            UiName = "Knight";
            UiDescription = "Armored cavalry";

            HitPoints = 40;
            Speed = 4;
            AttackDamage = 8;
            _lineOfSight = 4;
            MeleeArmor = 2;
            RangedArmor = 3;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 50),
                new ResourceCollection(Resource.Gold, 30),
                new ResourceCollection(Resource.Iron, 70)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Knight(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = Speed,
                LineOfSight = _lineOfSight,
                AttackDamage = AttackDamage,
                ArmorPierce = ArmorPierce,
                GoldConsumption = 1,
                UpgradeLevel = UpgradeLevel,
                RangedArmor = RangedArmor,
                MeleeArmor = MeleeArmor
            };
        }
    }
}
