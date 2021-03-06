﻿using System.IO.Abstractions;

namespace FL.LigArchivar.Core.Data
{
    public class IgnoredFileSystemItem : FileSystemItemBase
    {
        private const string IgnoredPrefix = "<ignored>";

        public IgnoredFileSystemItem(IDirectoryInfo directory, IFileSystemItem parent)
            : base(directory, IgnoredPrefix + directory.Name, parent, true)
        {
        }
    }
}
