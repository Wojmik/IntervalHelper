using IntervalHelper;
using T = int;

namespace UnitTests
{
	[TestClass]
	public sealed class DiscontinuousIntervalSplitterUnitTest
	{
		[TestMethod]
		public void EmptyTest()
		{
			var splitter = CreateSplitter();
			AreEqual(new Interval<T>[] { },
				splitter
				);
		}
		
		[TestMethod]
		public void WrongIntervalTest()
		{
			var splitter = CreateSplitter();
			Assert.ThrowsException<ArgumentException>(() => splitter.Add(6, 5));
		}
		
		[TestMethod]
		public void SingleTest()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(10, 20));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 20),
				},
				splitter
				);

			//Repeat the same
			splitter.Add(new Interval<T>(10, 20));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 20),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void SingleNarrowTest()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(10, 10));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 10),
				},
				splitter
				);

			//Repeat the same
			splitter.Add(new Interval<T>(10, 10));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 10),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void NotOverlaping1Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(10, 20));
			splitter.Add(new Interval<T>(30, 40));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 20),
					new Interval<T>(30, 40),
				},
				splitter
				);
		}
		
		
		[TestMethod]
		public void NotOverlaping2Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(30, 40));
			splitter.Add(new Interval<T>(10, 20));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 20),
					new Interval<T>(30, 40),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void Overlaping1Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(3, 4));
			splitter.Add(new Interval<T>(10, 20));
			splitter.Add(new Interval<T>(6, 9));
			AreEqual(new Interval<T>[] {
					new Interval<T>(3, 4),
					new Interval<T>(6, 9),
					new Interval<T>(10, 20),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void Overlaping2Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(3, 4));
			splitter.Add(new Interval<T>(10, 20));
			splitter.Add(new Interval<T>(6, 13));
			AreEqual(new Interval<T>[] {
					new Interval<T>(3, 4),
					new Interval<T>(6, 9),
					new Interval<T>(10, 13),
					new Interval<T>(14, 20),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void Overlaping3Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(3, 4));
			splitter.Add(new Interval<T>(10, 20));
			splitter.Add(new Interval<T>(6, 13));
			AreEqual(new Interval<T>[] {
					new Interval<T>(3, 4),
					new Interval<T>(6, 9),
					new Interval<T>(10, 13),
					new Interval<T>(14, 20),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void Overlaping4Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(3, 4));
			splitter.Add(new Interval<T>(10, 20));
			splitter.Add(new Interval<T>(13, 16));
			AreEqual(new Interval<T>[] {
					new Interval<T>(3, 4),
					new Interval<T>(10, 12),
					new Interval<T>(13, 16),
					new Interval<T>(17, 20),
				},
				splitter
				);
		}
		
		[TestMethod]
		public void Overlaping5Test()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(3, 4));
			splitter.Add(new Interval<T>(10, 20));
			splitter.Add(new Interval<T>(6, 24));
			AreEqual(new Interval<T>[] {
					new Interval<T>(3, 4),
					new Interval<T>(6, 9),
					new Interval<T>(10, 20),
					new Interval<T>(21, 24),
				},
				splitter
				);
		}

		[TestMethod]
		public void Test1()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(10, 20));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 20),
				},
				splitter
				);

			splitter.Add(new Interval<T>(4, 4));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(10, 20),
				},
				splitter
				);

			splitter.Add(new Interval<T>(30, 40));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(10, 20),
					new Interval<T>(30, 40),
				},
				splitter
				);

			splitter.Add(new Interval<T>(8, 42));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(8, 9),
					new Interval<T>(10, 20),
					new Interval<T>(21, 29),
					new Interval<T>(30, 40),
					new Interval<T>(41, 42),
				},
				splitter
				);
		}

		[TestMethod]
		public void Test2()
		{
			var splitter = CreateSplitter();

			splitter.Add(new Interval<T>(10, 20));
			AreEqual(new Interval<T>[] {
					new Interval<T>(10, 20),
				},
				splitter
				);

			splitter.Add(new Interval<T>(4, 4));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(10, 20),
				},
				splitter
				);

			splitter.Add(new Interval<T>(30, 40));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(10, 20),
					new Interval<T>(30, 40),
				},
				splitter
				);

			splitter.Add(new Interval<T>(50, 60));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(10, 20),
					new Interval<T>(30, 40),
					new Interval<T>(50, 60),
				},
				splitter
				);

			splitter.Add(new Interval<T>(8, 42));
			AreEqual(new Interval<T>[] {
					new Interval<T>(4, 4),
					new Interval<T>(8, 9),
					new Interval<T>(10, 20),
					new Interval<T>(21, 29),
					new Interval<T>(30, 40),
					new Interval<T>(41, 42),
					new Interval<T>(50, 60),
				},
				splitter
				);
		}

		private DiscontinuousIntervalSplitter<T> CreateSplitter()
		{
#if NET7_0_OR_GREATER
			return DiscontinuousIntervalSplitter.Create<T>();
#else
			return new DiscontinuousIntervalSplitter<T>(x => x + 1, x => x - 1);
#endif
		}

		private void AreEqual(IReadOnlyList<Interval<T>> expected, DiscontinuousIntervalSplitter<T> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);
			var i = 0;
			foreach (var interval in actual)
			{
				Assert.AreEqual(expected[i].Left, interval.Left);
				Assert.AreEqual(expected[i].Right, interval.Right);
				i++;
			}
		}
	}
}