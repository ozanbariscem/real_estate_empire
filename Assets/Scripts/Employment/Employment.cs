using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Employment
{
    [MoonSharpUserData]
    public class Employment
    {
        private static int id_count;
        
        public int id;
        public string company_tag;
        public int employee_id;

        [JsonConstructor]
        public Employment(int id, string company_tag, int employee_id)
        {
            this.id = id;
            this.company_tag = company_tag;
            this.employee_id = employee_id;

            id_count++;
        }

        public Employment(string tag, int id)
        {
            this.id = id_count++;
            company_tag = tag;
            employee_id = id;
        }

        public override string ToString()
        {
            string s;
            s = $"{id} | Company: {company_tag} | Employee: {employee_id}";
            return s;
        }
    }
}
