﻿using Portal.Domain.Entities;
using Portal.Domain.Interfaces.Common;

namespace Portal.Application.Services;

public class StudentProgressService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentProgressService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public void AddStudentProgress(StudentProgress studentProgress) => _unitOfWork.studentProgressRepository.AddStudentProgress(studentProgress);

    public void UpdateStudentProgress(StudentProgress studentProgress) => _unitOfWork.studentProgressRepository.UpdateStudentProgress(studentProgress);

    public void DeleteStudentProgress(Guid progressId) => _unitOfWork.studentProgressRepository.DeleteStudentProgress(progressId);

    public StudentProgress? GetStudentProgress(Guid? progressId) => _unitOfWork.studentProgressRepository.GetStudentProgress(progressId);

    public IEnumerable<StudentProgress> GetStudentProgresses() => _unitOfWork.studentProgressRepository.GetStudentProgresses();
}