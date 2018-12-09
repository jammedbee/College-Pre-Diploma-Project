using System;
using Windows.UI.Xaml.Media.Imaging;

namespace cinemaBeginningUWP
{
    public class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public BitmapImage Poster { get; set; }

        public Film(int id, string name, int duration, string description, DateTime releaseDate, BitmapImage poster)
        {
            Id = id;
            Name = name;
            Duration = duration;Description = description;
            ReleaseDate = releaseDate;
            Poster = poster;
        }
    }
}
