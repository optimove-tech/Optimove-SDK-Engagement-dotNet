using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Optimove.Optigration.Sdk.Tests.Models
{
	public class TestObject
	{
		public int? Int { get; set; }

		public string String { get; set; }

		public bool Bool { get; set; }

		public float Float { get; set; }

		public double Double { get; set; }

		public TestObject2 Object { get; set; }

		public DateTime DateTime { get; set; }

		public int[] IntArray { get; set; }

		public List<int> IntList { get; set; }

		public List<TestObject2> ObjectList { get; set; }

		public TestObject2[] ObjectArray { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			var secondObject = obj as TestObject;
			if (secondObject == null)
			{
				return false;
			}
			return (Int == secondObject.Int)
				&& (String == secondObject.String)
				&& (Bool == secondObject.Bool)
				&& (Float == secondObject.Float)
				&& (Double == secondObject.Double)
				&& (DateTime.ToString() == secondObject.DateTime.ToString())
				&& (Object.Equals(secondObject.Object))
				&& (Enumerable.SequenceEqual(IntArray, secondObject.IntArray))
				&& (Enumerable.SequenceEqual(IntList, secondObject.IntList))
				&& (Enumerable.SequenceEqual(ObjectArray, secondObject.ObjectArray))
				&& (Enumerable.SequenceEqual(ObjectList, secondObject.ObjectList));
		}

		public override int GetHashCode()
		{
			return Int.GetHashCode() ^ String.GetHashCode();
		}
	}
}
