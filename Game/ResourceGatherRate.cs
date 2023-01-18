namespace AoeBoardgame
{
    class ResourceGatherRate
    {
        public ResourceGatherRate(Resource resource, int gatherRate)
        {
            Resource = resource;
            GatherRate = gatherRate;
        }

        public Resource Resource { get; set; }
        public int GatherRate { get; set; }
    }
}
