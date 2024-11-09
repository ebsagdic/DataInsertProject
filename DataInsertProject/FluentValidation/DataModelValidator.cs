using DataInsertProject.Models;
using FluentValidation;

namespace DataInsertProject.FluentValidation
{
    public class DataModelValidator : AbstractValidator<DataModel>
    {
        public DataModelValidator() 
        {
            RuleFor(x=>x.Data).NotNull().WithMessage("{PropertyName} is required}");
        }

    }

}
