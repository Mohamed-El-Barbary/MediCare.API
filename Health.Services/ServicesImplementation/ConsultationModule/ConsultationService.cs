using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.ConsultationModule.Enums;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction.ConsultationModule;
using Health.Services.Specifications.ConsultationSpecification;
using Health.Services.Specifications.DoctorSpecification;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;

namespace Health.Services.ServicesImplementation.ConsultationModule
{
    public class ConsultationService : IConsultationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ConsultationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ConsultationStatisticsDTO>> GetStatisticsAsync(string userId, string role)
        {
            var entityIdResult = await GetEntityIdByRoleAsync(userId, role);

            if (!entityIdResult.IsSuccess)
                return entityIdResult.Errors.First();

            var entityId = entityIdResult.Value;

            var repo = _unitOfWork.GetRepository<Consultation, int>();

            Expression<Func<Consultation, bool>> baseFilter =
                role == "Doctor"
                    ? c => c.DoctorId == entityId
                    : c => c.PatientId == entityId;

            var consultations = await repo.GetAllAsync(new ConsultationStatisticsSpecification(baseFilter));

            var result = new ConsultationStatisticsDTO(
                consultations.Count(),
                consultations.Count(x => x.Status == ConsultationStatus.Scheduled),
                consultations.Count(x => x.Status == ConsultationStatus.InProgress),
                consultations.Count(x => x.Status == ConsultationStatus.Completed),
                consultations.Count(x => x.Status == ConsultationStatus.Cancelled),
                consultations.Count(x => x.Status == ConsultationStatus.Missed)
            );

            return Result<ConsultationStatisticsDTO>.Ok(result);
        }

        public async Task<Result<ConsultationDTO>> CreateAsync(CreateConsultationDTO request)
        {
            var existing = await GetExistingConsultationAsync(request.AppointmentId);

            if (existing is not null)
                return await MapConsultationDtoAsync(existing.Value!);

            var validationResult = await ValidateConsultationDataAsync(request);

            if (!validationResult.IsSuccess)
                return validationResult.Errors.First();

            var mappedConsultation = _mapper.Map<Consultation>(request);

            await _unitOfWork.GetRepository<Consultation, int>().AddAsync(mappedConsultation);

            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Error.Validation("Validation.Error", "Error where saving data , try again later!");


            return await MapConsultationDtoAsync(mappedConsultation);
        }

        public async Task<Result<ConsultationDTO>> GetByIdAsync(int id)
        {
            var spec = new ConsultationByIdSpecification(id);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);
            if (consultation is null)
                return Error.NotFound("Consultation.NotFound", $"Consultation with {id} not found");

            return await MapConsultationDtoAsync(consultation);
        }

        public async Task<Result<PaginatedResult<ConsultationSummaryDTO>>> GetByDoctorIdAsync(string doctorId, ConsultationSpecParams specParams)
        {
            var id = await GetDoctorIdAsync(doctorId);
            var spec = new ConsultationByDoctorIdSpecification(id.Value, specParams);
            var countSpec = new ConsultationByDoctorIdCountSpecification(id.Value);

            var consultations = await _unitOfWork.GetRepository<Consultation, int>().GetAllAsync(spec);
            var totalCount = await _unitOfWork.GetRepository<Consultation, int>().CountAsync(countSpec);
            var consultationDtos = _mapper.Map<IEnumerable<ConsultationSummaryDTO>>(consultations);

            var paginatedResult = new PaginatedResult<ConsultationSummaryDTO>(specParams.PageIndex, specParams.PageSize, totalCount, consultationDtos);

            return Result<PaginatedResult<ConsultationSummaryDTO>>.Ok(paginatedResult);
        }

        public async Task<Result<PaginatedResult<ConsultationSummaryDTO>>> GetByPatientIdAsync(string patientId, ConsultationSpecParams specParams)
        {
            var id = await GetPatientIdAsync(patientId);
            var spec = new ConsultationByPatientIdSpecification(id.Value, specParams);
            var countSpec = new ConsultationByPatientIdCountSpecification(id.Value);

            var consultations = await _unitOfWork.GetRepository<Consultation, int>().GetAllAsync(spec);
            var totalCount = await _unitOfWork.GetRepository<Consultation, int>().CountAsync(countSpec);
            var consultationDtos = _mapper.Map<IEnumerable<ConsultationSummaryDTO>>(consultations);

            var paginatedResult = new PaginatedResult<ConsultationSummaryDTO>(specParams.PageIndex, specParams.PageSize, totalCount, consultationDtos);

            return Result<PaginatedResult<ConsultationSummaryDTO>>.Ok(paginatedResult);
        }

        // End

        public async Task<Result<ConsultationDTO>> UpdateMedicalDataAsync(int consultationId, UpdateMedicalDataDTO request)
        {
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(consultationId);
            if (consultation is null)
                return Error.NotFound("Consultation.NotFound", $"Consultation with id:{consultationId} not found");

            if (!string.IsNullOrEmpty(request.Symptoms))
                consultation.Symptoms = request.Symptoms;

            if (!string.IsNullOrEmpty(request.Diagnosis))
                consultation.Diagnosis = request.Diagnosis;

            if (!string.IsNullOrEmpty(request.Notes))
                consultation.Notes = request.Notes;

            _unitOfWork.GetRepository<Consultation, int>().Update(consultation);

            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Error.Validation("Consultation.Validation", "Validation error has occurred, Try again");

            return await MapConsultationDtoAsync(consultation);
        }

        // End

        public Task<Result<ConsultationDTO>> EndConsultationAsync(int consultationId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStatusAsync(int consultationId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ConsultationDTO>> StartConsultationAsync(int consultationId)
        {
            throw new NotImplementedException();
        }

        // End

        public async Task<Result<ConsultationDTO>> AddPrescriptionAsync(int consultationId, CreatePrescriptionDTO request)
        {
            var spec = new ConsultationByIdSpecification(consultationId);

            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);
            if (consultation is null)
                return Error.NotFound("Consultation.NotFound", $"Consultation with id:{consultationId} not found");

            var mappedPrescription = _mapper.Map<Prescription>(request);
            mappedPrescription.ConsultationId = consultationId;

            consultation.Prescriptions.Add(mappedPrescription);
            _unitOfWork.GetRepository<Consultation, int>().Update(consultation);

            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Error.Validation("ConsultationPrescription.Validation", "Validation error has occurred, Try again");

            return await MapConsultationDtoAsync(consultation);
        }

        public async Task<Result<ConsultationDTO>> UpdatePrescriptionAsync(int consultationId, int prescriptionId, UpdatePrescriptionDTO request)
        {
            var prescription = await _unitOfWork.GetRepository<Prescription, int>().GetByIdAsync(prescriptionId);

            if (prescription is null)
                return Error.NotFound("Prescription.NotFound", $"Prescription with id:{prescriptionId} not found");

            if (prescription.ConsultationId != consultationId)
                return Error.Validation("Prescription.InvalidRelation", "This prescription does not belong to this consultation");

            _mapper.Map(request, prescription);

            _unitOfWork.GetRepository<Prescription, int>().Update(prescription);
            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Error.Validation("Prescription.Validation", "Validation error has occurred, Try again");

            var spec = new ConsultationByIdSpecification(prescription.ConsultationId);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);

            return await MapConsultationDtoAsync(consultation!);
        }

        public async Task<Result<bool>> DeletePrescriptionAsync(int consultationId, int prescriptionId)
        {
            var prescription = await _unitOfWork.GetRepository<Prescription, int>().GetByIdAsync(prescriptionId);
            if (prescription is null)
                return Error.NotFound("Prescription.NotFound", $"Prescription with id:{prescriptionId} not found");

            if (prescription.ConsultationId != consultationId)
                return Error.Validation("Prescription.InvalidRelation", "This prescription does not belong to this consultation");

            _unitOfWork.GetRepository<Prescription, int>().Delete(prescription);

            var result = await _unitOfWork.SaveChanges() > 0;

            return result;
        }

        // End
        public async Task<Result<ConsultationDTO>> AddPrescriptionItemAsync(int consultationId, int prescriptionId, CreatePrescriptionItemDTO request)
        {
            var prescription = await _unitOfWork.GetRepository<Prescription, int>().GetByIdAsync(prescriptionId);
            if (prescription is null)
                return Error.NotFound("Consultation.NotFound", $"Consultation with id:{consultationId} not found");

            if (prescription.ConsultationId != consultationId)
                return Error.Validation("Prescription.InvalidRelation", "This prescription does not belong to this consultation");

            var prescriptionItems = _mapper.Map<PrescriptionItem>(request);
            prescription.Items.Add(prescriptionItems);

            _unitOfWork.GetRepository<Prescription, int>().Update(prescription);
            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Error.Validation("ConsultationPrescription.Validation", "Validation error has occurred, Try again");

            var spec = new ConsultationByIdSpecification(prescription.ConsultationId);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);

            return await MapConsultationDtoAsync(consultation!);
        }

        public async Task<Result<ConsultationDTO>> UpdatePrescriptionItemAsync(int consultationId, int prescriptionId, int itemId, UpdatePrescriptionItemDTO request)
        {
            var prescriptionSpec = new PrescriptionByIdSpecification(prescriptionId);
            var prescription = await _unitOfWork.GetRepository<Prescription, int>().GetByIdAsync(prescriptionSpec);

            if (prescription is null)
                return Error.NotFound("Prescription.NotFound", "");

            if (prescription.ConsultationId != consultationId)
                return Error.Validation("Prescription.InvalidRelation", "");

            var item = prescription.Items.FirstOrDefault(i => i.Id == itemId);

            if (item is null)
                return Error.NotFound("PrescriptionItem.NotFound", "");

            _mapper.Map(request, item);

            var result = await _unitOfWork.SaveChanges() > 0;

            if (!result)
                return Error.Validation("PrescriptionItem.UpdateFailed", "");

            var spec = new ConsultationByIdSpecification(consultationId);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);

            return await MapConsultationDtoAsync(consultation!);
        }

        public async Task<Result<bool>> DeletePrescriptionItemAsync(int consultationId, int prescriptionId, int itemId)
        {
            // 1. Get item directly
            var item = await _unitOfWork.GetRepository<PrescriptionItem, int>().GetByIdAsync(itemId);

            if (item is null)
                return Error.NotFound("PrescriptionItem.NotFound", $"PrescriptionItem with id:{itemId} not found");

            // 2. Get prescription for ownership validation
            var prescription = await _unitOfWork.GetRepository<Prescription, int>().GetByIdAsync(prescriptionId);

            if (prescription is null)
                return Error.NotFound("Prescription.NotFound", $"Prescription with id:{prescriptionId} not found");

            // 3. Validate relation: prescription belongs to consultation
            if (prescription.ConsultationId != consultationId)
                return Error.Validation("Prescription.InvalidRelation", "This prescription does not belong to this consultation");

            // 4. Validate item belongs to prescription
            if (item.PrescriptionId != prescriptionId)
                return Error.Validation("PrescriptionItem.InvalidRelation", "This item does not belong to this prescription");

            // 5. Delete
            _unitOfWork.GetRepository<PrescriptionItem, int>().Delete(item);

            var result = await _unitOfWork.SaveChanges() > 0;
            if (!result)
                return Error.Validation("PrescriptionItem.DeleteFailed", "Failed to delete prescription item");

            return true;
        }


        #region Helper Method

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

        private async Task<Result<Appointment>> ValidateConsultationDataAsync(CreateConsultationDTO request)
        {
            var appointment = await _unitOfWork
                .GetRepository<Appointment, int>()
                .GetByIdAsync(request.AppointmentId);

            if (appointment is null)
                return Error.NotFound("Appointment.NotFound", $"Appointment with id:{request.AppointmentId} not found");

            if (appointment.DoctorProfileId != request.DoctorId)
                return Error.Validation("Appointment.InvalidDoctor", "Appointment does not belong to this doctor");


            if (appointment.PatientProfileId != request.PatientId)
                return Error.Validation("Appointment.InvalidPatient", "Appointment does not belong to this patient");

            return Result<Appointment>.Ok(appointment);
        }

        private async Task<Result<Consultation?>> GetExistingConsultationAsync(int appointmentId)
        {
            var spec = new ConsultationByAppointmentIdSpecification(appointmentId);
            var consultation = await _unitOfWork.GetRepository<Consultation, int>().GetByIdAsync(spec);

            return consultation;
        }

        private async Task<Result<ConsultationDTO>> MapConsultationDtoAsync(Consultation consultation)
        {
            var consultationDto = _mapper.Map<ConsultationDTO>(consultation);

            var doctorNameResult = await GetDoctorNameAsync(consultation.DoctorId);
            if (!doctorNameResult.IsSuccess)
                return doctorNameResult.Errors.First();

            var patientNameResult = await GetPatientNameAsync(consultation.PatientId);
            if (!patientNameResult.IsSuccess)
                return patientNameResult.Errors.First();

            consultationDto.DoctorName = doctorNameResult.Value;
            consultationDto.PatientName = patientNameResult.Value;

            return Result<ConsultationDTO>.Ok(consultationDto);
        }

        private async Task<int?> GetPatientIdAsync(string PatientUserId)
        {
            var spec = new PatientByIdWithoutIncludes(PatientUserId);
            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);
            if (patient is null)
                return null;
            else
                return patient.Id;
        }
        private async Task<int?> GetDoctorIdAsync(string DoctorUserId)
        {
            var spec = new DoctorByIdSpecification(DoctorUserId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);
            if (doctor is null)
                return null;
            else
                return doctor.Id;
        }

        private async Task<Result<int>> GetEntityIdByRoleAsync(string userId, string role)
        {
            if (role == "Doctor")
            {
                var doctorId = await GetDoctorIdAsync(userId);

                if (doctorId is null)
                    return Error.NotFound("Doctor.NotFound", "Doctor not found");

                return doctorId.Value;
            }

            if (role == "Patient")
            {
                var patientId = await GetPatientIdAsync(userId);

                if (patientId is null)
                    return Error.NotFound("Patient.NotFound", "Patient not found");

                return patientId.Value;
            }

            return Error.Unauthorized("Role.Invalid", "Invalid role");
        }

        #endregion
    }
}
