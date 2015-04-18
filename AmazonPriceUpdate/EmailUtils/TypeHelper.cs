using System;

namespace EmailUtils
{
	public static class TypeHelper
	{
		public static bool IsSimpleOrNullableType(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}

			return IsSimpleType(type);
		}

		public static bool IsSimpleType(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(Decimal) || type == typeof(DateTime) || type == typeof(Guid);
		}
	}
}