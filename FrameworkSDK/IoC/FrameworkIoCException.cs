using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.IoC
{
	public sealed class FrameworkIocException : FrameworkException
	{
		public FrameworkIocException()
		{
		}

		public FrameworkIocException(string message)
			: base(message)
		{
		}

		public FrameworkIocException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
