namespace AoeBoardgame
{
    class ResourceCollection
    {
        public int Food { get; set; }
        public int Wood { get; set; }
        public int Gold { get; set; }
        public int Iron { get; set; }
        public int Stone { get; set; }

        public ResourceCollection(int food, int wood = 0, int gold = 0, int iron = 0, int stone = 0)
        {
            Food = food;
            Wood = wood;
            Gold = gold;
            Iron = iron;
            Stone = stone;
        }

        public ResourceCollection()
        {
        }
    }
}
