namespace AoeBoardgame
{
    class TowerFactory : PlaceableObjectFactory
    {
        private int _hitPoints;
        private int _attackDamage;
        private int _range;
        private int _lineOfSight;
        private int _maxUnitsGarrisoned;

        public const int CastleAddedHitPoints = 100;
        public const int ImperialAddedHitPoints = 100;

        public const int CastleAddedAttackDamage = 20;
        public const int ImperialAddedAttackDamage = 20;

        public TowerFactory(TextureLibrary textureLibrary) :
            base(textureLibrary)
        {
            Type = typeof(Tower);
        }

        public override PlaceableObject Get(Player player) 
        {
            return new Tower(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                AttackDamage = _attackDamage,
                Range = _range,
                LineOfSight = _lineOfSight,
                MaxUnitsGarrisoned = _maxUnitsGarrisoned
            };
        }

        protected override void SetBaseStats()
        {
            _hitPoints = 200;
            _attackDamage = 20;
            _range = 3;
            _lineOfSight = 4;
            _maxUnitsGarrisoned = 1;

            Cost = new ResourceCollection(0, 50, 0, 0, 100);
        }

        public override void UpgradeToFeudalAge() { }

        public override void UpgradeToCastleAge()
        {
            _hitPoints += CastleAddedHitPoints;
            _attackDamage += CastleAddedAttackDamage;
        }

        public override void UpgradeToImperialAge()
        {
            _hitPoints += ImperialAddedHitPoints;
            _attackDamage += ImperialAddedAttackDamage;
        }
    }
}
