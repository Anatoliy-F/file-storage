namespace BuisnessLogicLayer.Enums
{
    /// <summary>
    /// Represents the result of a request to the service
    /// Success - operation processed successfully
    /// NotFound - the object to perform the operation on does not exist
    /// AccessDenied - the object with which the operation is to be performed does not belong to the user
    /// Error - an exception occurred while trying to process the request
    /// </summary>
    public enum ResponseResult
    {
        Success,
        NotFound,
        AccessDenied,
        Error
    }
}
