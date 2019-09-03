using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadFileTest1.Models
{
    public class FileInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string Location { get; set; }
        public DateTime Timestamp { get; set; }
        public string Path { get; set; }

        public FileInformation() { }

    }
}