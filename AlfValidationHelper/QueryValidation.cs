
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfValidationHelper
{
    class QueryValidation : ValidationItem
    {
        public string ValidationQuery { get; set; }

        public Validation Validation { get; set; }
        public override ValidationResult Validate()
        {
            ValidationResult validationResult = new ValidationResult();
            validationResult.validationItem = this;
            var table = new DataTable();
            using (var da = new SqlDataAdapter(ValidationQuery, Validation.ConnectionString))
            {
                foreach (var item in Validation.validationParams) 
                {
                    da.SelectCommand.Parameters.AddWithValue(item.Key,item.Value);
                }
                da.Fill(table);
            }
            if (table.Rows.Count>0)
            {
             var items=   ValidationUtils.GetDataTableDictionaryList(table);

                foreach (var item in items)
                {
                    validationResult.NonValidItems.Add(item[NonValidItemName]);
                    validationResult.ValidationMessages.Add(GeneratateMessage(item));
                }

                validationResult.NonValidData = items;
            }
            return validationResult;
        }


    }
}
