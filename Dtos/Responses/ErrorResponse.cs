namespace AuthenticationServer.API.Dtos.Responses
{
    public class ErrorResponse
    {
        public IEnumerable<string> ErrorMessages { get; set; }

        public ErrorResponse(IEnumerable<string> errorMessages)
        {
            ErrorMessages = errorMessages;
        }
        public ErrorResponse(string errorMessage) : this(new List<string>() { errorMessage })
        { }

    }
}
