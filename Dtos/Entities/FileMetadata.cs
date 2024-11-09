using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }

        public byte[] Data { get; set; }
    }
}
