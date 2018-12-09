using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace cinemaBeginningUWP
{
    public class User
    {
        public string FullName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }
        public int PostId { get; }
        public BitmapImage Photo { get; }

        public User(string fullName, int postId)
        {
            FullName = fullName;
            PostId = postId;
            LastName = fullName.Substring(0, fullName.IndexOf(' '));
            fullName = fullName.Remove(0, fullName.IndexOf(' ') + 1);
            if (fullName.IndexOf(' ') == -1)
            {
                FirstName = fullName;
            }
            else
            {
                FirstName = fullName.Substring(0, fullName.IndexOf(' '));
                fullName = fullName.Remove(0, fullName.IndexOf(' ') + 1);
                MiddleName = fullName;
            }
            
            Photo = new BitmapImage();
        }

        public User(string fullName, int postId, BitmapImage photo) : this(fullName, postId)
        {
            Photo = photo;
        }
    }
}
