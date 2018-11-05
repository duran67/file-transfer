using System;
using Microsoft.Extensions.FileProviders;

namespace FileTransfer.Models
{
    public class FileInformation
    {
        public FileInformation(IFileInfo info)
        {
            Name = info.Name;
            Length = info.Length;
            LastModified = info.LastModified;
        }
        public string Name { get; }
        public long Length { get; }
        public DateTimeOffset LastModified { get; }
    }
}
