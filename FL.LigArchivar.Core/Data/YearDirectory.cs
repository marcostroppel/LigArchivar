﻿using System.Collections.Immutable;
using System.IO;

namespace FL.LigArchivar.Core.Data
{
    public class YearDirectory : IFileSystemItem
    {
        private DirectoryInfo _yearDirectory;

        private YearDirectory(DirectoryInfo yearDirectory)
        {
            _yearDirectory = yearDirectory;
            Name = _yearDirectory.Name;
        }

        public string Name { get; }

        public bool Valid => true;

        public IImmutableList<IFileSystemItem> Children => ImmutableList<IFileSystemItem>.Empty;

        public static bool TryCreate(DirectoryInfo yearDirectory, out YearDirectory directory)
        {
            directory = null;
            var name = yearDirectory.Name;

            if (!int.TryParse(name, out var year))
                return false;

            // Des kaa beim beschta Willa it sei.
            var under1700 = year < 1700;
            var over3000 = year > 3000;
            if (under1700 || over3000)
                return false;

            directory = new YearDirectory(yearDirectory);
            return true;
        }
    }
}
