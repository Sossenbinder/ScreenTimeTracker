using System;

namespace ScreenTimeTracker.Common.DataTypes
{
	public abstract class EnumClass
	{
		public int Code { get; }

		public string? Name { get; }

		protected EnumClass(
			int code,
			string? name = null)
		{
			Code = code;
			Name = name;
		}

		public static bool operator ==(EnumClass left, EnumClass right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EnumClass left, EnumClass right)
		{
			return !(left == right);
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is EnumClass comparator))
			{
				return false;
			}

			return Equals(comparator);
		}

		protected bool Equals(EnumClass other)
		{
			var typeMatch = GetType() == other.GetType();

			if (!typeMatch)
			{
				return false;
			}

			return other.Code == Code;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Code, Name);
		}
	}
}