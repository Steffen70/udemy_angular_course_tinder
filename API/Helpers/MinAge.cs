using System;
using System.ComponentModel.DataAnnotations;

namespace API.Helpers
{
    public class MinAge : ValidationAttribute
    {
        private int _Limit;
        public MinAge(int Limit)
        {
            this._Limit = Limit;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is null)
                return null;
                
            DateTime bday = DateTime.Parse(value.ToString());
            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (bday > today.AddYears(-age))
                age--;
            if (age < _Limit)
                return new ValidationResult("Sorry you are not old enough");

            return null;
        }
    }
}