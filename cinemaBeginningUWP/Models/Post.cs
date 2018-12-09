namespace cinemaBeginningUWP
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Post(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
