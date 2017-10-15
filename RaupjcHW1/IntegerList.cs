using System;

namespace RaupjcHW1
{
	public class IntegerList : IIntegerList
	{
		private int?[] _privateArray;
		private int _currentArrayPosition;

		public IntegerList()
		{
			_privateArray = new int?[4];
		}

		public IntegerList(int initialSize)
		{
			if (initialSize < 0)
			{
				throw new ArgumentException("Initial size must ge greater than 0");
			}
			else
			{
				_privateArray = new int?[initialSize];
			}
		}

		public void Add(int item)
		{
			if (_privateArray.Length <= _currentArrayPosition)
			{
				int?[] newArray = new int?[_privateArray.Length * 2];
				_privateArray.CopyTo(newArray, 0);
				_privateArray = newArray;
			}
			_privateArray[_currentArrayPosition] = item;
			_currentArrayPosition +=1;
		}

		public bool Remove(int item)
		{
			for (int i = 0; i < _privateArray.Length; i++)
			{
				if (_privateArray[i] == item)
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
			for (int i = index; i < _privateArray.Length-1; i++)
			{
				_privateArray[i] = _privateArray[i + 1];
			}
			_currentArrayPosition--;
			return true;

		}

		public int GetElement(int index)
		{
			if (index <= _currentArrayPosition && _privateArray[index] != null)
			{
				return (int)_privateArray[index];
			}
			else
			{
				throw new IndexOutOfRangeException("Index is outside the range of the array");
			}

		}

		public int IndexOf(int item)
		{
			for (int i = 0; i < _privateArray.Length; i++)
			{
				if (_privateArray[i] == item)
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
			_privateArray = new int?[4];
			_currentArrayPosition = 0;
		}

		public bool Contains(int item)
		{
			for (int i = 0; i < _currentArrayPosition; i++)
			{
				if (_privateArray[i] == item)
				{
					return true;
				}
			}
			return false;
		}
	}
}
