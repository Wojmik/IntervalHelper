using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace IntervalHelper
{
#if NET7_0_OR_GREATER
	/// <summary>
	/// Factory class for <see cref="DiscontinuousIntervalSplitter{T}"/> instances
	/// </summary>
	public static class DiscontinuousIntervalSplitter
	{
		/// <summary>
		/// Creates a new instance of <see cref="DiscontinuousIntervalSplitter{T}"/> for types supporting increment and decrement operators
		/// </summary>
		/// <typeparam name="T">Interval elements type</typeparam>
		/// <param name="comparer">Interval's element comparer</param>
		/// <returns>New instance of <see cref="DiscontinuousIntervalSplitter{T}"/></returns>
		public static DiscontinuousIntervalSplitter<T> Create<T>(IComparer<T>? comparer = null)
			where T : IIncrementOperators<T>, IDecrementOperators<T>
		{
			return new DiscontinuousIntervalSplitter<T>(Increment, Decrement, comparer);
		}

		private static T Increment<T>(T value)
			where T : IIncrementOperators<T>
		{
			return ++value;
		}
		private static T Decrement<T>(T value)
			where T : IDecrementOperators<T>
		{
			return --value;
		}
	}
#endif

	/// <summary>
	/// Collection of intervals that automatically splits added intervals into unique intervals.
	/// Intervals are threated as closed intervals [left, right].
	/// </summary>
	/// <typeparam name="T">Interval elements type</typeparam>
	public class DiscontinuousIntervalSplitter<T> : IReadOnlyList<Interval<T>>
	{
		private readonly Func<T, T> _increment;
		private readonly Func<T, T> _decrement;
		private readonly IComparer<T> _comparer;
		private readonly List<Interval<T>> _uniqueIntervals;

		/// <summary>
		/// Number of non-overlapping intervals in the collection
		/// </summary>
		public int Count { get => _uniqueIntervals.Count; }

		/// <summary>
		/// Gets the interval at the specified index in the collection
		/// </summary>
		/// <param name="index">The zero-based index of the interval to retrieve</param>
		/// <returns>The interval at the specified index</returns>
		public Interval<T> this[int index] { get => _uniqueIntervals[index]; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="incremet">Method to increment interval's element by one</param>
		/// <param name="decrement">Method to decrement interval's element by one</param>
		/// <param name="comparer">Interval's element comparer</param>
		/// <exception cref="ArgumentNullException"><paramref name="incremet"/> or <paramref name="decrement"/> is null</exception>
		public DiscontinuousIntervalSplitter(Func<T, T> incremet, Func<T, T> decrement, IComparer<T>? comparer = null)
		{
			_increment = incremet ?? throw new ArgumentNullException(nameof(incremet));
			_decrement = decrement ?? throw new ArgumentNullException(nameof(decrement));
			_comparer = comparer ?? Comparer<T>.Default;
			_uniqueIntervals = new List<Interval<T>>();
		}

		/// <summary>
		/// Adds a new interval to the collection, ensuring that intervals remain non-overlapping and sorted.
		/// </summary>
		/// <param name="interval">Closed interval [left, right] to add</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Interval{T}.Left"/> is greater than <see cref="Interval{T}.Right"/>.</exception>
		public void Add(Interval<T> interval)
		{
			Add(interval.Left, interval.Right);
		}

		/// <summary>
		/// Adds a new interval defined by the specified <paramref name="left"/> and <paramref name="right"/> bounds to the
		/// collection, ensuring that intervals remain non-overlapping and sorted.
		/// </summary>
		/// <param name="left">The inclusive lower bound of the interval to add. Must be less than or equal to <paramref name="right"/>.</param>
		/// <param name="right">The inclusive upper bound of the interval to add. Must be greater than or equal to <paramref name="left"/>.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="left"/> is greater than <paramref name="right"/>.</exception>
		public void Add(T left, T right)
		{
			if (_comparer.Compare(left, right) > 0)
				throw new ArgumentException("Left must be less than or equal to right");

			//Go to first involved interval
			int i = GetFirstInvolvedIndex(left);
			for (; i < _uniqueIntervals.Count; i++)
			{
				var current = _uniqueIntervals[i];

				var leftVsCurrentLeft = _comparer.Compare(left, current.Left);
				//Is something to insert before current
				if (leftVsCurrentLeft < 0)
				{
					//Adding:  5 -
					//Current:   10 -
					//If whole interval is before current, insert and return
					if (_comparer.Compare(right, current.Left) < 0)
					{
						//Adding:  5 - 9
						//Current:       10 -
						//Insert and leave
						_uniqueIntervals.Insert(i, new Interval<T>(left, right));
						return;
					}
					else//Inserting interval has to be cut away by current interval
					{
						//Adding:  5 - 13
						//Current:   10 -
						//Insert   5 - 9, continue with 10 - 13
						_uniqueIntervals.Insert(i, new Interval<T>(left, _decrement(current.Left)));
						left = current.Left;
						i++;
					}
				}
				else if (leftVsCurrentLeft > 0)
				{
					//Adding:     13 -
					//Current: 10 -
					//Insert   10 - 12, change current to 13 -
					_uniqueIntervals[i] = new Interval<T>(current.Left, _decrement(left));
					current = new Interval<T>(left, current.Right);
					i++;
					_uniqueIntervals.Insert(i, current);
				}

				//At this point, left has to be equal to current.Left

				var rightVsCurrentRight = _comparer.Compare(right, current.Right);
				if (rightVsCurrentRight < 0)
				{
					//Adding:  - 17
					//Current:   - 20
					//Change to - 17, insert 18 - 20 and leave
					_uniqueIntervals[i] = new Interval<T>(left, right);
					i++;
					_uniqueIntervals.Insert(i, new Interval<T>(_increment(right), current.Right));
					return;
				}
				else if (rightVsCurrentRight > 0)
				{
					//Adding :   - 24
					//Current: - 20
					//Continue with 21 -
					left = _increment(current.Right);
				}
				else
				{
					//Adding:  - 20
					//Current: - 20
					//Nothing left to add, leave
					return;
				}
			}

			//Something is still left to add, add it at the end
			_uniqueIntervals.Add(new Interval<T>(left, right));
		}

		private int GetFirstInvolvedIndex(T left)
		{
			var i = 0;
			for (; i < _uniqueIntervals.Count; i++)
				if (_comparer.Compare(_uniqueIntervals[i].Right, left) >= 0)
					break;
			return i;
		}

		/// <summary>
		/// Clears the collection
		/// </summary>
		public void Clear()
		{
			_uniqueIntervals.Clear();
		}

		/// <summary>
		/// Reduces the memory overhead of the collection by resizing its internal data structures to fit the current number of elements
		/// </summary>
		public void TrimExcess()
		{
			_uniqueIntervals.TrimExcess();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection of non-overlapping intervals
		/// </summary>
		/// <returns>An enumerator for the collection of non-overlapping intervals</returns>
		public List<Interval<T>>.Enumerator GetEnumerator()
		{
			return _uniqueIntervals.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection of non-overlapping intervals
		/// </summary>
		/// <returns>An enumerator for the collection of non-overlapping intervals</returns>
		IEnumerator<Interval<T>> IEnumerable<Interval<T>>.GetEnumerator()
		{
			return ((IEnumerable<Interval<T>>)_uniqueIntervals).GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection of non-overlapping intervals
		/// </summary>
		/// <returns>An enumerator for the collection of non-overlapping intervals</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Interval<T>>)this).GetEnumerator();
		}
	}
}