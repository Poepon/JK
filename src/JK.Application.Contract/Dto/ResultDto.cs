namespace JK.Dto
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
    public class ResultDto<T>
    {
        public bool IsSuccess { get; set; }

        public T Data { get; set; }

        public string ErrorMessage { get; set; }
    }
}
