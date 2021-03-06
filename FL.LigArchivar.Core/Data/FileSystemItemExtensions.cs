﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace FL.LigArchivar.Core.Data
{
    public static class FileSystemItemExtensions
    {
        public static string GetYear(this IFileSystemItem self)
        {
            if (self == null)
                return null;

            var selfAsYear = self as YearDirectory;
            if (selfAsYear != null)
                return selfAsYear.Name;

            var year = GetYear(self.Parent);
            return year;
        }

        public static string GetClubChar(this IFileSystemItem self)
        {
            if (self == null)
                return null;

            var selfAsClub = self as ClubDirectory;
            if (selfAsClub != null)
                return selfAsClub.Name[0].ToString(CultureInfo.InvariantCulture);

            var clubChar = GetClubChar(self.Parent);
            return clubChar;
        }

        public static IFileSystemItem GetChild(this IFileSystemItemWithChildren self, string path)
        {
            var splitted = path.Split('\\');
            var items = splitted
                .Where(item => !string.IsNullOrWhiteSpace(item));

            var child = self.GetChild(items);
            return child;
        }

        public static IFileSystemItem GetChild(this IFileSystemItemWithChildren self, IEnumerable<string> path)
        {
            var name = path.FirstOrDefault();
            if (name == null)
                return self as IFileSystemItem;

            var child = self.Children
                .FirstOrDefault(item => item.Name == name);

            var nextPaths = path.Skip(1).ToImmutableList();
            if (nextPaths.IsEmpty)
                return child as IFileSystemItem;

            var childWithChildren = child as IFileSystemItemWithChildren;
            if (childWithChildren != null)
                child = childWithChildren.GetChild(nextPaths);

            return child;
        }

        public static bool IsInPictures(this IFileSystemItem self)
        {
            var asAsset = self as AssetDirectory;
            if (asAsset != null)
                return asAsset.IsPicturesDirectory;

            if (self.Parent == null)
                return false;

            return self.Parent.IsInPictures();
        }
    }
}
