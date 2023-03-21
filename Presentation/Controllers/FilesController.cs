using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Presentation.Controllers;
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase {
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file) {
        if(this.ModelState.IsValid is false)
            return this.BadRequest();

        // folder
        String folder = Path.Combine(Directory.GetCurrentDirectory(), "Media");
        if(Directory.Exists(folder) is false)
            Directory.CreateDirectory(folder);

        // path
        String path = Path.Combine(folder, file?.FileName);

        // stream
        using FileStream stream = new(path, FileMode.Create);
        await file.CopyToAsync(stream);


        // response body
        return this.Ok(new {
            file = file.FileName,
            path = path,
            size = file.Length
        });
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download(String fileName) {
        if(this.ModelState.IsValid is false)
            return this.BadRequest();

        // filePath
        String filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", fileName);

        // ContentType : (MIME)
        FileExtensionContentTypeProvider provider = new();
        if(provider.TryGetContentType(fileName, out String? contentType) is false) {
            contentType = "application/octet-stream";
        }

        // Read
        Byte[] bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return this.File(bytes, contentType, Path.GetFileName(filePath));
    }
}