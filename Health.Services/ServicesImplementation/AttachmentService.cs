using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Health.Domain.Contracts;
using Health.Domain.Entities.IdentityModule;
using Health.Shared.CommonResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Error = Health.Shared.CommonResponses.Error;

namespace Health.Services.ServicesImplementation
{
    public class AttachmentService : IAttachmentService
    {

        private readonly Cloudinary _cloudinary;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(
            IOptions<CloudinarySettings> config,
            ILogger<AttachmentService> logger)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };

            _logger = logger;
        }

        public async Task<Result<string>> UploadImageAsync(IFormFile file, string folderName)
        {
            var validationResult = ValidateFile(file);

            if (validationResult.IsFailure)
                return Result<string>.Fail(validationResult.Errors.ToList());

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName,
                PublicId = Guid.NewGuid().ToString(),
                Transformation = new Transformation()
                                    .Quality("auto")
                                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                return Error.Failure(
                    "Image.UploadFailed",
                    result.Error.Message);
            }

            return result.SecureUrl.ToString();
        }

        public async Task<Result<bool>> DeleteImageAsync(string imageUrl)
        {
            var publicIdResult = ExtractPublicId(imageUrl);

            if (!publicIdResult.IsSuccess)
                return Result<bool>.Fail(publicIdResult.Errors.ToList());

            var deleteParams = new DeletionParams(publicIdResult.Value);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Result != "ok")
            {
                return
                    Error.Failure("Image.DeleteFailed", "Failed to delete image");
            }

            return Result<bool>.Ok(true);
        }

        public async Task<Result<string>> ReplaceImageAsync(
            string oldImageUrl,
            IFormFile newFile,
            string folderName)
        {
            var deleteResult = await DeleteImageAsync(oldImageUrl);

            if (deleteResult.IsFailure)
                return Result<string>.Fail(deleteResult.Errors.ToList());

            return await UploadImageAsync(newFile, folderName);
        }

        #region Helpers

        private Result ValidateFile(IFormFile file)
        {
            var errors = new List<Error>();

            if (file == null || file.Length == 0)
                errors.Add(Error.Validation("Image.Empty", "Image file is empty"));

            if (file != null)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

                if (!allowedTypes.Contains(file.ContentType))
                    errors.Add(Error.Validation("Image.InvalidType", "Invalid image type"));

                if (file.Length > 5 * 1024 * 1024)
                    errors.Add(Error.Validation("Image.SizeExceeded", "Image size exceeds 5MB"));
            }

            return errors.Count > 0
                ? Result.Fail(errors)
                : Result.Ok();
        }

        private Result<string> ExtractPublicId(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Error.Validation("Image.UrlEmpty", "Image URL is required");

            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                var uploadIndex = Array.IndexOf(segments, "upload");

                if (uploadIndex == -1 || uploadIndex + 2 >= segments.Length)
                {
                    return Error.Validation("Image.InvalidUrl", "Invalid Cloudinary URL format");
                }

                var publicIdWithExtension =
                    string.Join("/", segments.Skip(uploadIndex + 2));

                var publicId = Path.ChangeExtension(publicIdWithExtension, null);

                return Result<string>.Ok(publicId);
            }
            catch
            {
                return Error.Validation("Image.InvalidUrl", "Malformed image URL");
            }
        }

        #endregion

    }
}
