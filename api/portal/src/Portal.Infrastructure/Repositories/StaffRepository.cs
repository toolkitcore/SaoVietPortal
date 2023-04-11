﻿using Portal.Domain.Entities;
using Portal.Domain.Interfaces;
using Portal.Infrastructure.Repositories.Common;

namespace Portal.Infrastructure.Repositories;

public class StaffRepository : RepositoryBase<Staff>, IStaffRepository
{
    public StaffRepository(ApplicationDbContext context) : base(context) { }

    public void AddStaff(Staff staff) => Insert(staff);

    public void UpdateStaff(Staff staff) => Update(staff);

    public void DeleteStaff(string staffId) => Delete(x => x.staffId == staffId);

    public Staff? GetStaffById(string? staffId) => GetById(staffId);

    public IEnumerable<Staff> GetStaff() => GetAll();
}