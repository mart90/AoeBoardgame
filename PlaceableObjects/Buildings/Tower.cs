using System;

namespace AoeBoardgame
{
    class Tower : PlayerObject,
        IAttacker,
        IHasRange,
        IGarrisonable
    {
        public int AttackDamage { get; set; }
        public int Range { get; set; }
        public int MaxUnitsGarrisoned { get; set; }

        public Tower(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }

        public void Attack(IAttackable defender)
        {
            throw new NotImplementedException();
        }

        public override void UpgradeToFeudalAge() { }

        public override void UpgradeToCastleAge()
        {
            AddHitPoints(TowerFactory.CastleAddedHitPoints);
            AttackDamage += TowerFactory.CastleAddedAttackDamage;
        }

        public override void UpgradeToImperialAge()
        {
            AddHitPoints(TowerFactory.ImperialAddedHitPoints);
            AttackDamage += TowerFactory.ImperialAddedAttackDamage;
        }
    }
}
