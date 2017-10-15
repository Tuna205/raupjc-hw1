using System;
using System.Collections;
using System.Collections.Generic;

namespace RaupjcHW1_zad3
{
	public class GenericList <X> : IGenericList <X>
	{
		private X[] _privateArray;
		private int _currentArrayPosition;

		public GenericList()
		{
			_privateArray = new X[4];
		}

		public GenericList(int initialSize)
		{
			if (initialSize < 0)
			{
				throw new ArgumentException("Initial size must ge greater than 0");
			}
			else
			{
				_privateArray = new X[initialSize];
			}
		}


		public void Add(X item)
		{
			if (_privateArray.Length <= _currentArrayPosition) // pazi na rubni
			{
				X[] newArray = new X[_privateArray.Length * 2];
				_privateArray.CopyTo(newArray, 0);
				_privateArray = newArray;
			}
			_privateArray[_currentArrayPosition++] = item; // provjerit
		}

		public bool Remove(X item)
		{
			for (int i = 0; i < _privateArray.Length; i++)
			{
				if (Equals(_privateArray[i],item))//(_privateArray[i] != null && _privateArray[i].Equals(item))
				{
					RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public bool RemoveAt(int index)
		{
			if (index > _privateArray.Length)
			{
				throw new IndexOutOfRangeException("Index is outside the range of the array");
			}
			for (int i = index; i < _privateArray.Length - 1; i++)
			{
				_privateArray[i] = _privateArray[i + 1];
			}
			_currentArrayPosition--;
			return true;
		}

		public X GetElement(int index)
		{
			if (index <= _currentArrayPosition)
			{
				return _privateArray[index];
			}
			else
			{
				throw new IndexOutOfRangeException("Index is outside the range of the array");
			}
		}

		public int IndexOf(X item)
		{
			for (int i = 0; i < _privateArray.Length; i++)
			{
				if (Object.Equals(_privateArray[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		public int Count
		{
			get { return _currentArrayPosition; }
		}
		public void Clear()
		{
			_privateArray = new X[4];
			_currentArrayPosition = 0;
		}

		public bool Contains(X item)
		{
			for (int i = 0; i < _currentArrayPosition; i++)
			{
				if (Object.Equals(_privateArray[i], item))
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerator<X> GetEnumerator()
		{
			return new GenericListEnumerator<X>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}