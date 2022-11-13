using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Exceptions
{

	[Serializable]
	public class CustomRetryLimitExceededException : CustomException
	{
		public CustomRetryLimitExceededException() { }
		public CustomRetryLimitExceededException(string message) : base(message) { }
		public CustomRetryLimitExceededException(string message, Exception inner) : base(message, inner) { }
		protected CustomRetryLimitExceededException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
