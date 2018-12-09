using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace cinemaBeginningUWP
{
    public class Worker
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int PostId { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string PassportNumber { get; set; }
        public string PaymentCardNumber { get; set; }
        public string TIN { get; set; }
        public BitmapImage Photo { get; set; }
        public WriteableBitmap WriteableBitmap { get; set; }

        public Worker(int id, string fullName, string phoneNumber, 
            DateTime birthDate, int age, string passportNumber, string paymentCardNumber,
            string TIN, int postId, BitmapImage photo)
        {
            Id = id;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            Age = age;
            PassportNumber = passportNumber;
            PaymentCardNumber = paymentCardNumber;
            this.TIN = TIN;
            PostId = postId;
            if (photo == null)
            {
                Photo = new BitmapImage(new Uri("ms-appx:///cinemaBeginningUWP/Assets/abstract-user-flat-1.png"));
            }
            else
            {
                Photo = photo;
            }
            WriteableBitmap = new WriteableBitmap(100, 100);
        }
    }
}