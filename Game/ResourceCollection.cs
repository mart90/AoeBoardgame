namespace AoeBoardgame
{
    class ResourceCollection
    {
        public ResourceCollection(Resource resource, int amount)
        {
            Resource = resource;
            Amount = amount;
        }

        public Resource Resource { get; set; }
        public int Amount { get; set; }
    }
}
