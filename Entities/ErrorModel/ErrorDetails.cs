using System.Text.Json;

namespace Entities.ErrorModel;
public class ErrorDetails {
    public Int32 StatusCode { get; set; }
    public String? Message { get; set; }

    public override String ToString() {
        return JsonSerializer.Serialize(this);
    }
}