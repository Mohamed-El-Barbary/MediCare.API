using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationDTOs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Health.Services.Abstraction.ConsultationModule
{
    public interface IConsultationService
    {
        Task<Result<ConsultationDTO>> CreateAsync(CreateConsultationDTO request);
        Task<Result<ConsultationDTO>> GetByIdAsync(int id);
        Task<Result<PaginatedResult<ConsultationSummaryDTO>>> GetByDoctorIdAsync(string doctorId, ConsultationSpecParams specParams);
        Task<Result<PaginatedResult<ConsultationSummaryDTO>>> GetByPatientIdAsync(string patientId, ConsultationSpecParams specParams);

        // Medical Data
        Task<Result<ConsultationDTO>> UpdateMedicalDataAsync(int consultationId, UpdateMedicalDataDTO request);

        // Lifecycle
        Task<Result<ConsultationDTO>> StartConsultationAsync(int consultationId);
        Task<Result<ConsultationDTO>> EndConsultationAsync(int consultationId);
        Task<string> GetStatusAsync(int consultationId);

        // Prescription
        Task<Result<ConsultationDTO>> AddPrescriptionAsync(int consultationId, CreatePrescriptionDTO request);
        Task<Result<ConsultationDTO>> UpdatePrescriptionAsync(int consultationId, int prescriptionId, UpdatePrescriptionDTO request);
        Task<Result<bool>> DeletePrescriptionAsync(int consultationId, int prescriptionId);

        // Prescription Items
        Task<Result<ConsultationDTO>> AddPrescriptionItemAsync(int consultationId, int prescriptionId, CreatePrescriptionItemDTO request);
        Task<Result<ConsultationDTO>> UpdatePrescriptionItemAsync(int consultationId, int prescriptionId, int itemId, UpdatePrescriptionItemDTO request);
        Task<Result<bool>> DeletePrescriptionItemAsync(int consultationId, int prescriptionId, int itemId);
    }
}
