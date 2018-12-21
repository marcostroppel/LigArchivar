﻿using System.Collections.Immutable;
using System.Linq;
using FL.LigArchivar.Core.Data;

namespace FL.LigArchivar.ViewModels.Data
{
    public class EventTreeViewItem : TreeViewItemBase
    {
        private readonly EventDirectory _inner;

        public EventTreeViewItem(EventDirectory inner, ITreeViewItem parent)
            : base(inner.Name, parent, inner.IsValid)
        {
            _inner = inner;
        }

        public string FilePrefix => _inner.FilePrefix;

        public IImmutableList<FileListItem> Files
        {
            get => _files;
            set
            {
                if (_files != value)
                {
                    _files = value;
                    NotifyOfPropertyChange(nameof(Files));
                }
            }
        }

        public override IImmutableList<ITreeViewItem> Children { get; } = ImmutableList<ITreeViewItem>.Empty;

        private IImmutableList<FileListItem> _files = ImmutableList<FileListItem>.Empty;

        internal void LoadChildren()
        {
            _inner.LoadChildren();
            Files = _inner.Children
                .Where(item => !item.IsIgnored)
                .Select(item => new FileListItem(item))
                .ToImmutableList();
        }
    }
}
