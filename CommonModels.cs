using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptchaBypassTest
{
    public class SchoolData
    {
        public SchoolData(string[] data)
        {
            Action<string>[] PropertyMappings =
            {
                x=>this.Number=Convert.ToInt32(x==""?"0":x),
                x=>this.Dni=x,
                x=>this.FullName=x,
                x=>this.School=x,
                x=>this.Title=x,
                x=>this.Speciality=x,
                x=>this.GradeDate=x,
                x=>this.RefNumber=x,
                x=>this.Actions=x,
            };
            for (int i = 0; i < data.Count(); i++)
            {
                PropertyMappings[i](data[i]);
            }
        }

        public SchoolData()
        {

        }

        public int Number { get; set; }
        public string Dni { get; set; }
        public string FullName { get; set; }
        public string School { get; set; }
        public string Title { get; set; }
        public string Speciality { get; set; }
        public string GradeDate { get; set; }
        public string RefNumber { get; set; }
        public string Actions { get; set; }
    }
}
