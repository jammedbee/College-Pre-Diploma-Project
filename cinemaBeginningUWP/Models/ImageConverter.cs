using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace cinemaBeginningUWP.Models
{
    public sealed class ImageConverter
    {
        //public async Task<BitmapImage> ImageFromBytesold(byte[] bytes)
        //{
        //    BitmapImage image = new BitmapImage();
        //    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
        //    {
        //        await stream.WriteAsync(bytes.AsBuffer());
        //        stream.Seek(0);
        //        await image.SetSourceAsync(stream);
        //    }
        //    return image;
        //}

        public async Task<byte[]> ConvertRandomAccessStreamToByteArray(IRandomAccessStream stream)
        {
            DataReader dataReader = new DataReader(stream.GetInputStreamAt(0));
            byte[] bytes = new byte[stream.Size];
            await dataReader.LoadAsync((uint)stream.Size);
            dataReader.ReadBytes(bytes);
            return bytes;
        }

        public async Task<byte[]> BitmapImageToByteArrayAsync(BitmapImage image)
        {
            RandomAccessStreamReference streamReference = RandomAccessStreamReference.CreateFromUri(image.UriSource);
            IRandomAccessStreamWithContentType streamWithContent = await streamReference.OpenReadAsync();
            byte[] buffer = new byte[streamWithContent.Size];
            await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);
            return buffer;
        }

        public async Task<BitmapImage> ByteArrayToBitmapImageAsync(byte[] bytes)
        {
            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }
            return image;
        }

        //public async Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
        //{
        //    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
        //    {
        //        using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
        //        {
        //            writer.WriteBytes(byteArray);
        //            await writer.StoreAsync();
        //        }
        //        BitmapImage image = new BitmapImage();
        //        await image.SetSourceAsync(stream);
        //        return image;
        //    }
        //}
    }
}
