﻿using System.Collections.Immutable;

using System.IO.Abstractions;
using System.Linq;
using FL.LigArchivar.Core.Utilities;

namespace FL.LigArchivar.Core.Data
{
    public class AssetDirectory : FileSystemItemBase
    {
        private const string _folderStructureFolderName = @"Ordnerstruktur";

        private static readonly IImmutableList<string> _allowedNames = new[]
        {
            @"Digitalfoto",
            @"Ton",
            @"Video",
        }.ToImmutableList();
        private bool _isPicturesDirectory;

        private AssetDirectory(IDirectoryInfo assetDirectory, IFileSystemItem parent)
            : base(assetDirectory, assetDirectory.Name, parent, true, TryCreateChild)
        {
            _isPicturesDirectory = assetDirectory.Name == "Digitalfoto";
        }

        public bool IsPicturesDirectory
        {
            get => _isPicturesDirectory;
        }

        public static bool TryCreate(IDirectoryInfo assetDirectory, IFileSystemItem parent, out IFileSystemItem directory)
        {
            directory = null;

            if (!DirectoryEx.Exists(assetDirectory.FullName))
                return false;

            var name = assetDirectory.Name;

            if (_allowedNames.All(item => item != name))
                return false;

            directory = new AssetDirectory(assetDirectory, parent);
            return true;
        }

        private static bool TryCreateChild(IDirectoryInfo directory, IFileSystemItem parent, out IFileSystemItem fileSystemItem)
        {
            var isYear = YearDirectory.TryCreate(directory, parent, out fileSystemItem);
            if (isYear)
                return true;

            if (directory.Name == _folderStructureFolderName)
            {
                fileSystemItem = new IgnoredFileSystemItem(directory, parent);
                return true;
            }

            return false;
        }
    }
}
