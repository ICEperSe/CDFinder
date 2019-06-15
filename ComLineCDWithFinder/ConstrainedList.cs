using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineCDWithFinder
{
    class ConstrainedList<T> : IConstrainedCollection<T>, IList<T>
    {
        public ConstrainedList(Predicate<T> constraint)
        {
            Constraint = constraint;
        }

        public ConstrainedList(Predicate<T> constraint, bool isSizeLimited, int size) 
            : this(constraint)
        {
            IsSizeLimited = isSizeLimited;
            Size = size;
            List = new List<T>(size);
        }

        private List<T> List { get; } = new List<T>();

        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear() => List.Clear();

        public bool Contains(T item) => List.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => List.CopyTo(array,arrayIndex);

        public void Add(T item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            if(!Constraint(item))
                throw new ArgumentException(nameof(item));
            if(IsSizeLimited && Count >= Size) throw new ArgumentException("Too many items");
            List.Add(item);
        }

        public bool TryAdd(T item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            if (!Constraint(item)) return false;
            if (IsSizeLimited && Count >= Size) return false;
            List.Add(item);
            return true;

        }

        public bool Remove(T item) => List.Remove(item);

        public int Count => List.Count;

        public bool IsReadOnly { get; }

        public Predicate<T> Constraint { get; }

        public bool IsSizeLimited { get; }

        public int? Size { get; }
        public int IndexOf(T item) => List.IndexOf(item);

        public void Insert(int index, T item) => List.Insert(index, item);

        public void RemoveAt(int index) => List.RemoveAt(index);

        public T this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }
    }
}
