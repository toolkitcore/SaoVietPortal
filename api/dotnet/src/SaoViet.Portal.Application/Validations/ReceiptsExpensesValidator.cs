﻿//using System.Globalization;
//using FluentValidation;

//namespace Portal.Application.Validations;

//public class ReceiptsExpensesValidator : AbstractValidator<ReceiptsExpensesResponse>
//{
//    public ReceiptsExpensesValidator()
//    {
//        RuleFor(x => x.Type)
//            .NotEmpty().WithMessage("Type is required");
//        RuleFor(x => x.Date)
//            .Must(dateString => DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
//            .WithMessage("Date is not valid");
//        RuleFor(x => x.Amount)
//            .GreaterThan(0).WithMessage("Amount must be greater than 0");
//    }
//}