namespace IntervalHelper
{
	/// <summary>
	/// Interval
	/// </summary>
	/// <typeparam name="T">Interval elements type</typeparam>
	public readonly struct Interval<T>
	{
		/// <summary>
		/// Left boundary
		/// </summary>
		public readonly T Left;

		/// <summary>
		/// Right boundary
		/// </summary>
		public readonly T Right;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="left">Left boundary</param>
		/// <param name="right">Right boundary</param>
		public Interval(T left, T right)
		{
			Left = left;
			Right = right;
		}

		/// <summary>
		/// Deconstructor
		/// </summary>
		/// <param name="left">Left boundary</param>
		/// <param name="right">Right boundary</param>
		public void Deconstruct(out T left, out T right)
		{
			left = Left;
			right = Right;
		}

		/// <summary>
		/// Returns string representation
		/// </summary>
		/// <returns>String representation of the interval</returns>
		public override string ToString()
		{
			return $"{Left} - {Right}";
		}
	}
}