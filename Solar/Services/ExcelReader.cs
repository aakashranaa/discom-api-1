using OfficeOpenXml;
using Solar.Models;
using System;
using System.Collections.Generic;
using System.IO;

public class ExcelReader
{
    List<Record> records;

    public List<Record> ReadExcel(string filePath)
    {
        if (records != null && records.Count > 0)
        {
            return records;
        }

        this.records = new List<Record>();

        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first worksheet

            if (worksheet == null)
            {
                return records;
            }

            int rowCount = worksheet.Dimension.Rows;

            // Assuming the column order is as follows: name, master_key, address, Consumer_district, Consumer_Pin_CODE, conn_load, Email_Id, Phone_Number, Division_NAME, sub_Division_NAME, circle_name, Division_CODE, SUB_Division_CODE, circle_code, sol_inst_cap
            for (int row = 2; row <= rowCount; row++) // Assuming first row is header
            {
                records.Add(new Record
                {
                    consumer_name = worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                    consumer_id = worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                    consumer_address = worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                    consumer_lg_district_code = worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                    consumer_pin_code = worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                    connection_load = worksheet.Cells[row, 6].Value?.ToString()?.Trim(),
                    consumer_email = worksheet.Cells[row, 7].Value?.ToString()?.Trim(),
                    consumer_mobile = worksheet.Cells[row, 8].Value?.ToString()?.Trim(),
                    division_name = worksheet.Cells[row, 9].Value?.ToString()?.Trim(),
                    sub_division_name = worksheet.Cells[row, 10].Value?.ToString()?.Trim(),
                    circle_name = worksheet.Cells[row, 11].Value?.ToString()?.Trim(),
                    division_code = worksheet.Cells[row, 12].Value?.ToString()?.Trim(),
                    sub_division_code = worksheet.Cells[row, 13].Value?.ToString()?.Trim(),
                    circle_code = worksheet.Cells[row, 14].Value?.ToString()?.Trim(),
                    existing_installed_capacity = worksheet.Cells[row, 15].Value?.ToString()?.Trim(),
                    connection_type = "0", // we dont have this column in excel
                    status_code = "200"
                    
                });
            }
        }

        return records;
    }
}


public class Record
{
    public string status_code { get; set; }
    public string? consumer_address { get; set; }
    public string? consumer_lg_district_code { get; set; }
    public string? consumer_pin_code { get; set; }

    public string? consumer_id { get; set; }
    public string? connection_load { get; set; }
    public string? circle_name { get; set; }
    public string? circle_code { get; set; }
    public string? division_name { get; set; }
    public string? division_code { get; set; }
    public string? sub_division_name { get; set; }
    public string? sub_division_code { get; set; }
    public string? connection_type { get; set; }
    public string? consumer_mobile { get; set; }
    public string? consumer_email { get; set; }
    public string? consumer_name { get; set; }
    public string? existing_installed_capacity { get; set; }
}
