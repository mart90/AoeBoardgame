namespace AoeBoardgame.Multiplayer
{
    class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public double LastElo { get; set; }

        public User ToUser()
        {
            return new User
            {
                Id = Id,
                Username = Username,
                LastElo = LastElo
            };
        }
    }
}
