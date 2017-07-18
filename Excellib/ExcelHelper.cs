using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace Excellib
{
    public class ExcelHelper
    {
        public DataTable GetDtByExcel(string filePath)
        {
            DataTable dt = new DataTable();
            IWorkbook wb = null;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (filePath.IndexOf(".xlsx") > 0) // 2007版本  
            {
                wb = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook  
            }
            else if (filePath.IndexOf(".xls") > 0) // 2003版本  
            {
                wb = new HSSFWorkbook(fileStream);  //xls数据读入workbook  
            }
            else
            {
                return dt;
            }
            ISheet ish = wb.GetSheetAt(0);
            IRow header = ish.GetRow(ish.FirstRowNum);
            for(int i = 0;i<header.LastCellNum;i++)
            {
               // dt.Columns.Add(header.GetCell(i).StringCellValue, header.GetCell(i).CellType);
            }
            for(int i = ish.FirstRowNum+1; i <ish.LastRowNum;i++)
            {
                DataRow dr = dt.NewRow();
                for(int j=0;j<header.LastCellNum;j++)
                {
                    dr[j] = ;
                }
            } 
            return dt;
        }
    }
}
