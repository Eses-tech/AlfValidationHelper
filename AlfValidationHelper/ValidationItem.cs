using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfValidationHelper
{
    public abstract class ValidationItem
    {
        //0: sql query 1: eba integration query 2:function 
        public int ValidationType { get; set; }

        public string ValidationName { get; set; }

        public string ValidationMessage { get; set; }

        public string NonValidItemName { get; set; }



        public abstract  ValidationResult Validate();

        public string GeneratateMessage(Dictionary<string,string> nonValidData)
        {
            return ValidationUtils.StringFormat(ValidationMessage, nonValidData);


        }
     


    }
}
