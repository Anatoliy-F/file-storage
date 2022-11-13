namespace DataAccessLayer.Exceptions
{

	[Serializable]
	public class CustomConcurrencyException : CustomException
	{
		public CustomConcurrencyException() { }
		public CustomConcurrencyException(string message) : base(message) { }
		public CustomConcurrencyException(string message, Exception inner) : base(message, inner) { }
		protected CustomConcurrencyException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
