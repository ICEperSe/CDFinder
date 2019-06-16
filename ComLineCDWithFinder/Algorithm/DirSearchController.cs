using System;
using System.Collections.Generic;
using System.IO;

namespace ComLineCDWithFinder.Algorithm
{
    public class DirSearchController : ISearchController<DirectoryInfo>
    {
        private readonly List<DirectoryInfo> _list = new List<DirectoryInfo>();
        private readonly Predicate<DirectoryInfo> GetCondition;
        private readonly int _maxCount;
        private readonly bool hasMaxCount;

        public DirSearchController(Predicate<DirectoryInfo> getCondition)
        {
            GetCondition = getCondition;
        }

        public DirSearchController(Predicate<DirectoryInfo> getCondition, int maxCount) 
            : this(getCondition)
        {
            _maxCount = maxCount;
            hasMaxCount = true;
        }

        public bool IsEnd => !hasMaxCount || _list.Count >= _maxCount;

        public void GetItem(DirectoryInfo item)
        { 

            if(GetCondition(item))
                _list.Add(item);
        }

        public DirectoryInfo[] FoundedItems => _list.ToArray();
    }
}