//using AMS.Models;
//using FluentValidation;

//namespace AMS.Validators
//{
//    public class UserValidator : AbstractValidator<User>
//    {
//        public UserValidator()
//        {
//            RuleFor(x => x.Username)
//                .NotEmpty().WithMessage("Username is required.")
//                .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

//            RuleFor(x => x.PasswordHash)
//                .NotEmpty().WithMessage("Password is required.")
//                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

//            RuleFor(x => x.Role)
//                .NotEmpty().WithMessage("Role is required.")
//                .MaximumLength(50).WithMessage("Role cannot exceed 50 characters.");

//            RuleFor(x => x.EmployeeId)
//                .GreaterThan(0).When(x => x.EmployeeId.HasValue)
//                .WithMessage("Employee ID must be greater than 0 if provided.");
//        }
//    }
//}
