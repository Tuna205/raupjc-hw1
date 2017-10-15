using System;
using System.Collections;
using System.Collections.Generic;

namespace Pong
{
	class GenericListEnumerator<T> : IEnumerator<T>
	{
		private GenericList<T> _privateArray;
		private int _currentArrayPosition = -1;

		public GenericListEnumerator(GenericList<T> list)
		{
			_privateArray = list;
		}

		public bool MoveNext()
		{
			_currentArrayPosition++;
			return (_currentArrayPosition <= _privateArray.Count);
		}

		public void Reset()
		{
			_currentArrayPosition = -1;
		}

		public T Current
		{
			get
			{
				try
				{
					return _privateArray.GetElement(_currentArrayPosition);
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public void Dispose(){}
	}
}
