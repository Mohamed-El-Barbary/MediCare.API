using Health.Shared.CommonResponses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IAttachmentService
    {
        Task<Result<string>> UploadImageAsync(IFormFile file, string folderName);
        Task<Result<bool>> DeleteImageAsync(string imageUrl);
        Task<Result<string>> ReplaceImageAsync(string oldImageUrl, IFormFile newFile, string folderName);
    }
}
