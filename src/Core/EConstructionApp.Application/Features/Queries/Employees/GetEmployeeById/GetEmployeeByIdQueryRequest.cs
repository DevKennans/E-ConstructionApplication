﻿using MediatR;

namespace EConstructionApp.Application.Features.Queries.Employees.GetEmployeeById
{
    public class GetEmployeeByIdQueryRequest : IRequest<GetEmployeeByIdQueryResponse>
    {
        public Guid EmployeeId { get; set; } /*
                                              * Type of this employee is AppUser, but in according service it will be checked for another kind of users like admin & moderator.
                                              * EmployeeId from AppUser must be bind with Employee data!
                                              */
    }
}
