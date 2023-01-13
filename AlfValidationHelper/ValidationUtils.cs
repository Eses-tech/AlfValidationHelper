using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace AlfValidationHelper
{
    public class ValidationUtils
    {


        public static string StringFormat(string format, Dictionary<string, string> values)
        {
            var matches = Regex.Matches(format, @"\{(.+?)\}");
            List<string> words = (from Match matche in matches select matche.Groups[1].Value).ToList();

            return words.Aggregate(
                format,
                (current, key) =>
                {
                    int colonIndex = key.IndexOf(':');
                    return current.Replace(
                    "{" + key + "}",
                    colonIndex > 0
                        ? string.Format("{0:" + key.Substring(colonIndex + 1) + "}", values[key.Substring(0, colonIndex)])
                        : values[key].ToString());
                });
        }


        public static List<Dictionary<string, string>> GetDataTableDictionaryList(DataTable dt)
        {
            return dt.Rows.Cast<DataRow>().ToList().Select(
                row => dt.Columns.Cast<DataColumn>().ToDictionary(
                    column => column.ColumnName,
                    column => row[column].ToString()
                )).ToList();

            //return null;
        }

        public static void ExportToExcel(Validation validation,string path)
        {

          
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name ="Validasyon Özeti"
                };

                Columns columns = new Columns();

                //columns.Append(new Column() { Min = 1, Max = 3, Width = 20, CustomWidth = true });
                columns.Append(new Column() { Min = 1, Max = 1, Width = 40, CustomWidth = true });
                columns.Append(new Column() { Min = 2, Max = 2, Width = 70, CustomWidth = true });
                columns.Append(new Column() { Min = 3, Max = 3, Width = 20, CustomWidth = true });
                columns.Append(new Column() { Min = 3, Max = 3, Width = 20, CustomWidth = true });
                worksheetPart.Worksheet.InsertAt(columns,0);
                Row row = new Row();
                Cell headerNumber = createCell("Validasyon Numarası");
                row.Append(headerNumber);
                Cell header1 = createCell("Validasyon Adı");
                row.Append(header1);
                Cell header2 = createCell("Validasyon Mesajı");
                row.Append(header2);
                Cell header3 = createCell("Validasyon Verisi");
                row.Append(header3);
              

                sheetData.Append(row);

                fillMessages(sheetData, validation);

                sheets.Append(sheet);
               fillSheets(spreadsheetDocument , workbookpart, sheets, validation);


                    workbookpart.Workbook.Save();

                // Close the document.
                spreadsheetDocument.Close();
                return;

            }

        }

        private static void fillSheets(SpreadsheetDocument spreadsheetDocument, WorkbookPart workbookpart, Sheets sheets, Validation validation)
        {
            uint i = 2;
            foreach (var item in validation.validationResults)
            {
                if (item.NonValidData!=null&&item.NonValidData.Count>0)
                {
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    SheetData sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);


                    Sheet sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        Name = (validation.validationResults.IndexOf(item) + 1).ToString(),
                        SheetId =new UInt32Value(i)  ,
                    };
                    createHeaderColumns(item.NonValidData[0], sheetData);
                    foreach (var nonValidData in item.NonValidData)
                    {
                        Row row = new Row();
                        foreach (var columnName in item.NonValidData[0].Keys)
                        {

                            row.Append(createCell(nonValidData[columnName]));
                        }

                        sheetData.Append(row);
                    }
                    sheets.Append(sheet);
                    i++;
                }
               
            }
        }

        private static void createHeaderColumns(Dictionary<string, string> dictionary, SheetData sheet)
        {
            Row row = new Row();
            foreach (var item in dictionary.Keys)
            {
                row.Append(createCell(item));
            }
            sheet.Append(row);
        }

        private static void fillMessages(SheetData sheetData, Validation validation)
        {
            foreach (var item in validation.validationResults)
            {
                foreach (var validationMessage in item.ValidationMessages)
                {
                    Row row = new Row();
                    row.Append(createCell((validation.validationResults.IndexOf(item) + 1).ToString()));
                    row.Append(createCell(item.validationItem.ValidationName));
                    row.Append(createCell(validationMessage));
                    row.Append(createCell(item.NonValidItems[item.ValidationMessages.IndexOf(validationMessage)]));
                    sheetData.Append(row);
                }               

            }
        }

        private static Cell createCell(string cellValue,CellValues type)
        {
            Cell cell = new Cell() { CellValue = new CellValue(cellValue), DataType = type };

            return cell;

        }
        private static Cell createCell(string cellValue)
        {
            Cell cell = new Cell() { CellValue = new CellValue(cellValue), DataType = CellValues.String };

            return cell;

        }
    }
}
