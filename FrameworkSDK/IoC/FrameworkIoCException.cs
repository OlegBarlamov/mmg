using System;

// ReSharper disable once CheckNamespace
namespace FrameworkSDK.IoC
{
	public class FrameworkIoCException : FrameworkException
	{
		public FrameworkIoCException()
		{
		}

		public FrameworkIoCException(string message)
			: base(message)
		{
		}

		public FrameworkIoCException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
