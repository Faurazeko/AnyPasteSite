namespace AnypasteSite.Models
{

    public class SiteFile
    {
        public string Link { get; set; }

        public FileType Type { get; set; }

        public SiteFile(string link, FileType type)
        {
            Link = link;
            Type = type;
        }

        public enum FileType
        {
            Audio,
            Video,
            Image,
            Text,
            Other
        }
    }
}
