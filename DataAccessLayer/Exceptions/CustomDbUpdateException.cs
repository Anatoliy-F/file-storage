namespace DataAccessLayer.Exceptions
{

	[Serializable]
	public class CustomDbUpdateException : CustomException
	{
		public CustomDbUpdateException() { }
		public CustomDbUpdateException(string message) : base(message) { }
		public CustomDbUpdateException(string message, Exception inner) : base(message, inner) { }
		protected CustomDbUpdateException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
