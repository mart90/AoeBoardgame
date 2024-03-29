﻿using System.Collections.Generic;

namespace AoeBoardgame
{
    class PikemanFactory : PlaceableObjectFactory, ICanBeUpgraded, IMilitaryUnit, IInfantry
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }
        public int RangedArmor { get; set; }
        public int MeleeArmor { get; set; }
        public int ArmorPierce { get; set; }

        private int _speed;
        private int _lineOfSight;

        public int UpgradeLevel { get; set; }

        public PikemanFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Pikeman);
            TurnsToComplete = 2;
            UpgradeLevel = 1;
        }

        protected override void SetBaseValues()
        {
            UiName = "Pikeman";
            UiDescription = "Anti-cavalry unit";

            HitPoints = 15;
            _speed = 2;
            AttackDamage = 3;
            _lineOfSight = 3;
            MeleeArmor = 0;
            RangedArmor = 0;

            Cost = new List<ResourceCollection> 
            {
                new ResourceCollection(Resource.Food, 30),
                new ResourceCollection(Resource.Gold, 10),
                new ResourceCollection(Resource.Iron, 10)
            };
        }

        public override PlaceableObject Get(Player player)
        {
            return new Pikeman(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                Speed = _speed,
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
