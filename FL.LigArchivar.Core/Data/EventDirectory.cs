﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using FL.LigArchivar.Core.Utilities;

namespace FL.LigArchivar.Core.Data
{
    public class EventDirectory : IFileSystemItem
    {
        private EventDirectory(IDirectoryInfo directory, IFileSystemItem parent, bool isValid, string clubChar, string year, string month, string day, string eventName)
        {
            Name = directory.Name;
            Directory = directory;
            Parent = parent;
            IsValid = isValid;
            ClubChar = clubChar;
            Year = year;
            Month = month;
            Day = day;
            EventName = eventName;

            FilePrefix = clubChar + "_" + year + "-" + month + "-" + day + "_";

            Children = ImmutableList<DataFiles>.Empty;
        }

        public void LoadChildren()
        {
            var files = Directory.GetFiles();
            var children = new List<DataFiles>();

            foreach (var file in files)
            {
                var instance = new DataFile(file);
                var existing = children.FirstOrDefault(item => item.Name == instance.Name);
                if (existing == null)
                {
                    var dataFiles = new DataFiles(instance, this);
                    children.Add(dataFiles);
                }
                else
                {
                    existing.AddFile(instance);
                }
            }

            Children = children.ToImmutableList();
        }

        public void Rename(int startNumber)
        {
            try
            {
                var localChildren = Children;
                var number = startNumber;

                Func<DataFiles, bool> predicate = item => !item.IsIgnored;

                foreach (var child in localChildren.Where(predicate))
                {
                    if (child.IsLonely)
                    {
                        child.Delete();
                    }
                    else
                    {
                        var newName = $"{FilePrefix}{number:000}";
                        child.RenameFiles(newName);
                        ++number;
                    }
                }
            }
            finally
            {
                LoadChildren();
            }
        }

        public void RenameToFileDateTime()
        {
            try
            {
                var localChildren = Children;

                Func<DataFiles, bool> predicate = item => !item.IsIgnored;

                foreach (var child in localChildren.Where(predicate))
                {
                    if (child.IsLonely)
                    {
                        child.Delete();
                    }
                    else
                    {
                        child.RenameFilesToFileDateTime();
                    }
                }
            }
            finally
            {
                LoadChildren();
            }
        }

        public void SortByName()
        {
            Children = Children.OrderBy(item => item.Name).ToImmutableList();
        }

        public void SortByDate()
        {
            Children = Children.OrderBy(item => item.Files[0].LastWriteTimeUtc).ToImmutableList();
        }

        public string Name { get; }

        public bool IsValid { get; }

        public IFileSystemItem Parent { get; }

        public IDirectoryInfo Directory { get; }

        public string ClubChar { get; }

        public string Year { get; }

        public string Month { get; }

        public string Day { get; }

        public string EventName { get; }

        public string FilePrefix { get; }

        public IImmutableList<DataFiles> Children { get; private set; }

        public static bool TryCreate(IDirectoryInfo eventDirectory, IFileSystemItem parent, out IFileSystemItem directory)
        {
            directory = null;

            var name = eventDirectory.Name;

            var expectedClubChar = parent.GetClubChar();
            var expectedYear = parent.GetYear();

            var regex = new Regex(Patterns.EventDirectory);

            var match = regex.Match(name);

            if (!match.Success)
                return false;

            if (match.Groups.Count != 6)
                return false;

            var actualClubChar = match.Groups[1].Value;
            var actualYear = match.Groups[2].Value;
            var actualMonth = match.Groups[3].Value;
            var actualDay = match.Groups[4].Value;
            var actualEventName = match.Groups[5].Value;

            var doesClubCharMatch = expectedClubChar == actualClubChar;
            var doesYearMatch = expectedYear == actualYear;
            var isValid = doesClubCharMatch && doesYearMatch;

            directory = new EventDirectory(eventDirectory, parent, isValid, actualClubChar, actualYear, actualMonth, actualDay, actualEventName);
            return true;
        }
    }
}
