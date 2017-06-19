using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NPinyin;

namespace Pylib
{
    public class NPy
    {
        /// <summary>
        /// 获取dt中每个字符串格式字段的拼音展现形式
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public static DataTable getDtPy(DataTable dt, int ic)
        {
            DataTable dtpy = new DataTable();
            ///列名
            dtpy.Columns.Add("cn");
            ///列值
            dtpy.Columns.Add("cvz");
            ///列拼音
            dtpy.Columns.Add("cvpy");
            DataView dv = dt.DefaultView;
            ///将原dt中的中文转换为拼音并保存到对照表中
            for (int i = 0; i < ic; i++)
            {
                if (dt.Columns[i].DataType.Name == "String")
                {
                    ///获取列名
                    string strcn = dt.Columns[i].ColumnName;
                    ///每一列去重复取出内容列表
                    DataTable dtdis = dv.ToTable(true, strcn);
                    int idis = dtdis.Rows.Count;
                    int iflag = 1;
                    for (int j = 0; j < idis; j++)
                    {
                        string strcvz = dtdis.Rows[j][0].ToString();
                        string strcvpy = NPinyin.Pinyin.GetInitials(strcvz);
                        if (dtpy.Select("cn = '" + strcn + "' AND cvz <> '" + strcvz + "' AND cvpy='" + strcvpy + "'").Length > 0)
                        {
                            strcvpy += iflag.ToString();
                            iflag++;
                        }
                        dtpy.Rows.Add(strcn, strcvz, strcvpy);
                    }
                }
            }
            return dtpy;
        }

        /// <summary>
        /// 获取表头的中英对照
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public static DataTable getDtCNPy(DataTable dt, int ic)
        {
            DataTable dtCNpy = new DataTable();
            dtCNpy.Columns.Add("cnC");
            dtCNpy.Columns.Add("cnE");
            int iflag = 0;
            for (int i = 0; i < ic; i++)
            {
                string strcnC = dt.Columns[i].ColumnName;
                string strcnE = Pinyin.GetInitials(strcnC);
                if(dtCNpy.Select("cnE = '"+strcnE+"'").Length > 0)
                {
                    strcnE += iflag.ToString();
                    iflag++;
                }
                dtCNpy.Rows.Add(strcnC, strcnE);
            }
            return null;
        }

    }
}
