namespace Optimove.SDK.Engager.Tests.Models
{
	public class TestObject2
	{
		public int? Property1 { get; set; }

		public string Property2 { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			var secondObj = obj as TestObject2;
			if (secondObj == null)
			{
				return false;
			}

			return (this.Property1 == secondObj.Property1)
				&& (this.Property2 == secondObj.Property2);
		}

		public override int GetHashCode()
		{
			return Property1.GetHashCode() ^ Property2.GetHashCode();
		}
	}
}
