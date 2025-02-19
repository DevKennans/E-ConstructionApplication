using Newtonsoft.Json;

namespace EConstructionApp.WebAPI.Extensions.Exceptions.Helpers
{
    public class ExceptionResponseModel : ErrorStatusCode
    {
        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
