using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
    using NPOI.SS.UserModel;

namespace mashovFinal.Utilities
{
    public class StringHelper
    {
        private static Random ran = new Random();

        public static string GeneratePassword()
        {

            string pass = "";
            pass = Membership.GeneratePassword(8, 0) + ran.Next(5, 10);
            pass = Regex.Replace(pass, @"[^a-zA-Z0-9]", m => ran.Next(0, 10).ToString());//להגדיר מתחלתחילה בלי תווים מיוחדים 

            return pass;
        }

    }

    public static class NpoiExtensions
    {
        public static string GetFormattedCellValue(this ICell cell, IFormulaEvaluator eval = null, string format = "MM/dd/yyyy")
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue;

                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            DateTime date = cell.DateCellValue;
                            ICellStyle style = cell.CellStyle;
                            // Excel uses lowercase m for month whereas .Net uses uppercase
                            //string format = style.GetDataFormatString().Replace('m', 'M');
                            return date.ToString(format);
                        }
                        else
                        {
                            return cell.NumericCellValue.ToString();
                        }

                    case CellType.Boolean:
                        return cell.BooleanCellValue ? "TRUE" : "FALSE";

                    case CellType.Formula:
                        if (eval != null)
                            return GetFormattedCellValue(eval.EvaluateInCell(cell));
                        else
                            return cell.CellFormula;

                    case CellType.Error:
                        return FormulaError.ForInt(cell.ErrorCellValue).String;
                        
                }
            }
            // null or blank cell, or unknown cell type
            return string.Empty;
        }
    }
}