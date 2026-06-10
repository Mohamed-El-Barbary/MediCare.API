using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationSessionDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.ConsultationModule
{
    public interface IConsultationSessionService
    {
        Task<Result<ConsultationSessionDTO>> StartSessionAsync(int consultationId);
        Task<Result<ConsultationSessionDTO>> EndSessionAsync(int consultationId);
        Task<Result<ConsultationSessionDTO>> JoinSessionAsync(int consultationId, int userId);
        Task<Result> LeaveSessionAsync(int consultationId, int userId);
        Task<Result> AddConnectionAsync(int consultationId, int userId, string connectionId);
        Task<Result> RemoveConnectionAsync(string connectionId);
        Task<Result<int>> GetConsultationIdByConnectionAsync(string connectionId);
        Task<Result<ConsultationSessionDTO?>> GetSessionAsync(int consultationId);
        Task<IEnumerable<SessionParticipantDTO>> GetParticipantsAsync(int consultationId);
        Task<Result<bool>> IsUserConnectedAsync(int consultationId, int userId);
        Task<Result<bool>> CanJoinAsync(int consultationId, int userId);
        Task<int?> GetPeerUserIdAsync(int consultationId, int userId);
        Task<Result<int>> GetProfileIdAsync(string role, string userId);
    }
}

