using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfValidationHelper
{
    public class ValidationResult
    {

        public ValidationResult()
        {
            ValidationMessages = new List<string>();
            NonValidItems = new List<string>();
        }
        public List<string> ValidationMessages;
        public List<string> NonValidItems;
        public List<Dictionary<string, string>> NonValidData { get; set; }

        public ValidationItem validationItem;
   
    }
}
