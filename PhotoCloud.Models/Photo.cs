namespace PhotoCloud.Models
{
    public class Photo
    {
        public Photo()
        {
        }

        public Photo(string path)
        {
            this.Path = path;
        }

        public int PhotoId { get; set; }
        public string Path { get; set; }
    }
}
