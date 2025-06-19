namespace comApp.db
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Content { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        // ✅ Parameterloser Konstruktor (notwendig für object initializer)
        public ApiResponse() { }

        // ✅ Einfacher Konstruktor mit Erfolg & Inhalt
        public ApiResponse(bool isSuccess, string content)
        {
            IsSuccess = isSuccess;
            Content = content;
        }

        // ✅ Konstruktor mit Erfolg, Fehlernachricht, StatusCode, etc.
        public ApiResponse(bool isSuccess, string content, int statusCode, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            Content = content;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

    }
}
