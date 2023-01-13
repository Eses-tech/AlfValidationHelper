
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfValidationHelper
{
    public class Validation
    {
        public static string ConnectionString { get; set; }
        public int operationId;
        public int id;
        public List<ValidationItem> validationItems;
        public List<ValidationResult> validationResults;
        public List<KeyValuePair<string, object>> validationParams;
        public Validation()
        {
            validationItems = new List<ValidationItem>();
            
            validationParams = new List<KeyValuePair<string, object>>();

        }



        public void ExportToExcel(string path)
        {

            ValidationUtils.ExportToExcel(this, path);
        }
        public List<ValidationResult> Validate()
        {
            validationResults = new List<ValidationResult>();
            foreach (var item in validationItems)
            {
               ValidationResult result= item.Validate();
                if (result!=null)
                {
                    validationResults.Add(result);
                }
            }

            return validationResults;
        }
        public static string queryValidationId = "SELECT [ID]  ,[VALIDATIONTYPE] ,[VALIDATIONMESSAGE] ,[NONVALIDITEMNAME] ,[OPERATIONID] ,[VALIDATIONNAME] FROM [dbo].[TbPon000ValidationItems] where ID = ";
        public static string query = "SELECT [ID]  ,[VALIDATIONTYPE] ,[VALIDATIONMESSAGE] ,[NONVALIDITEMNAME] ,[OPERATIONID] ,[VALIDATIONNAME] FROM [dbo].[TbPon000ValidationItems] where OPERATIONID = ";
        public void FillValidationItems()
        {
            DataTable table = new DataTable();
         
            using (var da = new SqlDataAdapter(query+operationId, ConnectionString))
            {
                da.Fill(table);
            }

            foreach (var item in table.Rows)
            {
                ParseValidationItem((DataRow)item);
            }

        }


        public void FillValidationItemsForValidationSender()
        {

            validationItems.Clear();
            DataTable table = new DataTable();

            using (var da = new SqlDataAdapter(queryValidationId + id, ConnectionString))
            {
                da.Fill(table);
            }

            foreach (var item in table.Rows)
            {
                ParseValidationItemForValidationSender((DataRow)item);
            }

        }


        private void ParseValidationItem(DataRow item)
        {
            if (((Int32) item["VALIDATIONTYPE"])==0)
            {
                QueryValidation add = new QueryValidation();
                add.ValidationMessage = item["VALIDATIONMESSAGE"].ToString();
                add.ValidationQuery = GetValidationQuery(item["ID"].ToString());
                add.ValidationType = 0;
                add.ValidationName = item["VALIDATIONNAME"].ToString();
                add.NonValidItemName = item["NONVALIDITEMNAME"].ToString();
                add.Validation = this;
                validationItems.Add(add);
            }

            
        }



        private void ParseValidationItemForValidationSender(DataRow item)
        {
            if (((Int32)item["VALIDATIONTYPE"]) == 0)
            {
                QueryValidation add = new QueryValidation();
                add.ValidationMessage = item["VALIDATIONMESSAGE"].ToString();
                add.ValidationQuery = GetValidationQueryForValidationSender(item["ID"].ToString());
                add.ValidationType = 0;
                add.ValidationName = item["VALIDATIONNAME"].ToString();
                add.NonValidItemName = item["NONVALIDITEMNAME"].ToString();
                add.Validation = this;
                validationItems.Add(add);
            }


        }

        private string GetValidationQuery(string validationId)
        {

            DataTable table = new DataTable();
            String validationQuery = "SELECT [ID] ,[ITEMID],[VALIDATIONQUERY]  FROM[dbo].[TbPon000QueryValidation] where ITEMID = ";
            using (var da = new SqlDataAdapter(validationQuery + validationId, ConnectionString))
            {
                da.Fill(table);
            }

            foreach (var item in table.Rows)
            {
                return ((DataRow)item)["VALIDATIONQUERY"].ToString();
            }
            throw new Exception("Sorgu Bulunamadı");
        }



        private string GetValidationQueryForValidationSender(string validationId)
        {

            DataTable table = new DataTable();
            String validationQuery = "SELECT [ID] ,[ITEMID],[VALIDATIONSENDERQUERY]  FROM[dbo].[TbPon000QueryValidation] where ITEMID = ";
            using (var da = new SqlDataAdapter(validationQuery + validationId, ConnectionString))
            {
                da.Fill(table);
            }

            foreach (var item in table.Rows)
            {
                return ((DataRow)item)["VALIDATIONSENDERQUERY"].ToString();
            }
            throw new Exception("Sorgu Bulunamadı");
        }
    }
}
