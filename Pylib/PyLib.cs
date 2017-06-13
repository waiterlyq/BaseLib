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
        public static DataTable getPyDt(DataTable dt,int ic)
        {
            DataTable dtpy = new DataTable();
            dtpy.Columns.Add("cn");
            dtpy.Columns.Add("cvz");
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
                    int iflag = 0;
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
    }
}
