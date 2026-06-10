using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.ConsultationModule.Enums;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction.ConsultationModule;
using Health.Services.Specifications.ConsultationSpecification;
using Health.Services.Specifications.DoctorSpecification;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationDTOs;
using Health.Shared.DTOs.ConsultationSessionDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Health.Services.ServicesImplementation.ConsultationModule
{
    public class ConsultationSessionService : IConsultationSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ConsultationSessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ConsultationSessionDTO>> StartSessionAsync(int consultationId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);
            if (consultationResult.IsFailure)
                return consultationResult.Errors.First();

            var consultation = consultationResult.Value;
            var result = ValidateStartSession(consultation);
            if (result.IsFailure)
                return result.Errors.First();

            consultation.Status = ConsultationStatus.InProgress;
            consultation.StartedAt = DateTime.Now;

            _unitOfWork.GetRepository<Consultation, int>().Update(consultation);
            var isSaved = await _unitOfWork.SaveChanges() > 0;
            if (!isSaved)
                return Error.Failure("Consultation.Failure", "Error in starting consultation, try again");

            return await MapConsultationSessionDtoAsync(consultation);
        }

        public async Task<Result<ConsultationSessionDTO>> EndSessionAsync(int consultationId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);
            if (consultationResult.IsFailure)
                return consultationResult.Errors.First();

            var consultation = consultationResult.Value;

            //if (consultation.Status == ConsultationStatus.InProgress)
            //    return Error.Failure("Consultation.Failure", "Already in progress");

            consultation.Status = ConsultationStatus.Completed;
            consultation.EndedAt = DateTime.UtcNow;

            var activeConnections = consultation.Connections.Where(c => c.IsActive).ToList();
            foreach (var connection in activeConnections)
                connection.IsActive = false;

            _unitOfWork.GetRepository<Consultation, int>().Update(consultation);

            var isSaved = await _unitOfWork.SaveChanges() > 0;
            if (!isSaved)
                return Error.Failure("Consultation.Failure", "Error in starting consultation, try again");

            return await MapConsultationSessionDtoAsync(consultation);
        }

        public async Task<Result<ConsultationSessionDTO>> JoinSessionAsync(int consultationId, int userId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);
            if (consultationResult.IsFailure)
                return consultationResult.Errors.First();

            var consultation = consultationResult.Value;
            if (!await IsParticipant(userId, consultationId))
                return Error.Unauthorized("User.Unauthorized", "You are not a participant");

            return await MapConsultationSessionDtoAsync(consultation);
        }

        public async Task<Result> LeaveSessionAsync(int consultationId, int userId)
        {
            var spec = new ActiveConnectionByUserSpecification(consultationId, userId);
            var session = await _unitOfWork.GetRepository<SessionConnection, int>().GetByIdAsync(spec);

            if (session is null)
                return Result.Fail(Error.NotFound("Connection.NotFound", "Conections for this consultation not found"));

            if (!await IsParticipant(userId, consultationId))
                return Result.Fail(Error.Unauthorized("User.Unauthorized", "You are not a participant"));

            session.IsActive = false;
            _unitOfWork.GetRepository<SessionConnection, int>().Update(session);

            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Result.Fail(Error.Failure("Session.Failure", "try again"));

            return Result.Ok();
        }

        public async Task<Result> AddConnectionAsync(int consultationId, int userId, string connectionId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);
            if (consultationResult.IsFailure)
                return Result.Fail(Error.NotFound("Consultation.NotFound", $"Consultation with id:{consultationId} not found"));

            var consultation = consultationResult.Value;
            var connection = new SessionConnection()
            {
                ConsultationId = consultationId,
                ConnectionId = connectionId,
                UserId = userId,
                ConnectedAt = DateTime.UtcNow,
                IsActive = true
            };

            consultation.Connections.Add(connection);
            _unitOfWork.GetRepository<Consultation, int>().Update(consultation);

            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Result.Fail(Error.Failure("SessionConnection.Failure", "Connection not added ,try again"));

            return Result.Ok();
        }

        public async Task<Result> RemoveConnectionAsync(string connectionId)
        {
            var spec = new ConsultationByConnectionIdSpecification(connectionId);

            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);

            if (consultation is null)
                return Result.Fail(Error.NotFound("SessionConnection.NotFound", $"Connection with id:{connectionId} not found"));

            var connection = consultation.Connections.FirstOrDefault(c => c.ConnectionId == connectionId && c.IsActive);

            if (connection is null)
                return Result.Fail(Error.NotFound("SessionConnection.NotFound", $"Active connection with id:{connectionId} not found"));

            connection.IsActive = false;
            //connection.DisconnectedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Consultation, int>().Update(consultation);
            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Result.Fail(Error.Failure("SessionConnection.Failure", "Connection not removed, try again"));

            return Result.Ok();
        }

        public async Task<Result<int>> GetConsultationIdByConnectionAsync(string connectionId)
        {
            var spec = new ConsultationByConnectionIdSpecification(connectionId);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);

            if (consultation is null)
                return Result<int>.Fail(Error.NotFound("SessionConnection.NotFound", $"Connection with id:{connectionId} not found"));

            return consultation.Id;
        }

        public async Task<Result<ConsultationSessionDTO?>> GetSessionAsync(int consultationId)
        {
            var consultation = await GetConsultationAsync(consultationId);
            return consultation is null
                ? null
                : _mapper.Map<ConsultationSessionDTO>(consultation.Value);
        }

        public async Task<Result<bool>> IsUserConnectedAsync(int consultationId, int userId)
        {
            var spec = new ActiveConnectionByUserSpecification(consultationId, userId);
            var connection = await _unitOfWork.GetRepository<SessionConnection, int>().GetByIdAsync(spec);

            return connection is not null;
        }

        //public async Task<Result<bool>> CanJoinAsync(int consultationId, int userId)
        //{
        //    var consultationResult = await GetConsultationAsync(consultationId);

        //    if (consultationResult.IsFailure)
        //        return Result<bool>.Fail(consultationResult.Errors.First());

        //    var consultation = consultationResult.Value;

        //    if (!await IsParticipant(userId, consultationId))
        //        return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "User is not a participant"));

        //    if (consultation.Status == ConsultationStatus.Completed)
        //        return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Consultation is completed"));

        //    if (consultation.Status == ConsultationStatus.Cancelled)
        //        return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Consultation is cancelled"));

        //    if (consultation.Status == ConsultationStatus.InProgress)
        //        return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Consultation is not in progress"));

        //    var validationResult = ValidateStartSession(consultation);
        //    if (validationResult.IsFailure)
        //        return Result<bool>.Fail(validationResult.Errors.First());

        //    // 1:1 — room is full if both already connected
        //    var activeConnections = consultation.Connections.Count(c => c.IsActive);

        //    if (consultation.IsFull(activeConnections))
        //    {
        //        var alreadyConnected = consultation.Connections
        //            .Any(c => c.UserId == userId && c.IsActive);

        //        if (!alreadyConnected)
        //            return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Session is full"));
        //    }

        //    return Result<bool>.Ok(true);
        //}

        public async Task<Result<bool>> CanJoinAsync(int consultationId, int userId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);

            if (consultationResult.IsFailure)
                return Result<bool>.Fail(consultationResult.Errors.First());

            var consultation = consultationResult.Value;

            if (!await IsParticipant(userId, consultationId))
                return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "User is not a participant"));

            if (consultation.Status == ConsultationStatus.Completed)
                return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Consultation is completed"));

            if (consultation.Status == ConsultationStatus.Cancelled)
                return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Consultation is cancelled"));

            var activeConnections = consultation.Connections
                .Count(c => c.IsActive);

            if (activeConnections >= 2)
            {
                var alreadyConnected = consultation.Connections
                    .Any(c => c.UserId == userId && c.IsActive);

                if (!alreadyConnected)
                    return Result<bool>.Fail(Error.Failure("Consultation.JoinDenied", "Session is full"));
            }

            return Result<bool>.Ok(true);
        }

        public async Task<IEnumerable<SessionParticipantDTO>> GetParticipantsAsync(int consultationId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);

            if (consultationResult.IsFailure)
                return [];

            var consultation = consultationResult.Value;

            var activeConnections = consultation.Connections
                .Where(c => c.IsActive)
                .ToDictionary(c => c.UserId, c => c.ConnectedAt);

            return new List<SessionParticipantDTO>{
                new SessionParticipantDTO
                {
                    UserId = consultation.DoctorId,
                    Name = consultation.Doctor.DisplayName,
                    Role = "Doctor",
                    IsConnected = activeConnections.ContainsKey(consultation.DoctorId),
                    ConnectedAt = activeConnections.GetValueOrDefault(consultation.DoctorId)
                },
                new SessionParticipantDTO
                {
                    UserId = consultation.PatientId,
                    Name = consultation.Patient.DisplayName,
                    Role = "Patient",
                    IsConnected = activeConnections.ContainsKey(consultation.PatientId),
                    ConnectedAt = activeConnections.GetValueOrDefault(consultation.PatientId)
                }
            };
        }

        public async Task<int?> GetPeerUserIdAsync(int consultationId, int userId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);

            var consultation = consultationResult.Value;

            if (consultation is null) return null;

            if (!await IsParticipant(userId, consultationId))
                return null;

            return GetPeerId(consultation, userId);
        }

        public async Task<Result<int>> GetProfileIdAsync(string role, string userId)
        {
            return role switch
            {
                "Doctor" => await GetDoctorProfileIdAsync(userId),
                "Patient" => await GetPatientProfileIdAsync(userId),
                _ => Result<int>.Fail(Error.Validation("", "Invalid role"))
            };
        }

        #region Helper Method

        private Result ValidateStartSession(Consultation consultation)
        {
            if (consultation.Status == ConsultationStatus.InProgress)
                return Result.Fail(Error.Failure("Consultation.Failure", "Already in progress"));

            if (consultation.Status != ConsultationStatus.Scheduled)
                return Result.Fail(Error.Failure("Consultation.Failure", "Not scheduled"));

            if (consultation.Type != ConsultationType.Online)
                return Result.Fail(Error.Failure("Consultation.Failure", "Only online allowed"));

            //if (consultation.ScheduledAt > DateTime.UtcNow)
            //    return Result.Fail(Error.Failure("Consultation.Failure", "Too early"));

            return Result.Ok();
        }

        private async Task<Result<string>> GetDoctorNameAsync(int id)
        {
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(id);

            if (doctor is null)
                return Error.NotFound("Doctor.NotFound", $"Doctor with id:{id} not found");

            return Result<string>.Ok(doctor.DisplayName);
        }

        private async Task<Result<string>> GetPatientNameAsync(int id)
        {
            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(id);

            if (patient is null)
                return Error.NotFound("Patient.NotFound", $"Patient with id:{id} not found");

            return Result<string>.Ok(patient.DisplayName);
        }

        private async Task<Result<ConsultationSessionDTO>> MapConsultationSessionDtoAsync(Consultation consultation)
        {
            var consultationDto = _mapper.Map<ConsultationSessionDTO>(consultation);

            var doctorNameResult = await GetDoctorNameAsync(consultation.DoctorId);
            if (!doctorNameResult.IsSuccess)
                return doctorNameResult.Errors.First();

            var patientNameResult = await GetPatientNameAsync(consultation.PatientId);
            if (!patientNameResult.IsSuccess)
                return patientNameResult.Errors.First();

            consultationDto.DoctorName = doctorNameResult.Value;
            consultationDto.PatientName = patientNameResult.Value;

            return Result<ConsultationSessionDTO>.Ok(consultationDto);
        }

        private async Task<Result<Consultation>> GetConsultationAsync(int consultationId)
        {
            var spec = new ConsultationByIdSpecification(consultationId);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);
            if (consultation is null)
                return Error.NotFound("Consultation.NotFound", $"Consultation with id:{consultationId} not found");

            return consultation;
        }

        private async Task<bool> IsParticipant(int? userId, int consultationId)
        {
            var consultationResult = await GetConsultationAsync(consultationId);
            var consultation = consultationResult.Value;
            return userId == consultation.PatientId || userId == consultation.DoctorId;
        }

        private async Task<Result<int>> GetDoctorProfileIdAsync(string userId)
        {
            var spec = new DoctorByIdSpecification(userId);

            var doctor = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetByIdAsync(spec);

            if (doctor is null)
                return Result<int>.Fail(Error.NotFound("Doctor.NotFound", "Doctor profile not found"));

            return Result<int>.Ok(doctor.Id);
        }

        private async Task<Result<int>> GetPatientProfileIdAsync(string userId)
        {
            var spec = new PatientByIdWithoutIncludes(userId);

            var patient = await _unitOfWork
                .GetRepository<PatientProfile, int>()
                .GetByIdAsync(spec);

            if (patient is null)
                return Result<int>.Fail(Error.NotFound("Patient.NotFound", "Patient profile not found"));

            return Result<int>.Ok(patient.Id);
        }

        private static int GetPeerId(Consultation consultation, int userId)
        {
            return consultation.DoctorId == userId
                ? consultation.PatientId
                : consultation.DoctorId;
        }

        #endregion
    }
}
