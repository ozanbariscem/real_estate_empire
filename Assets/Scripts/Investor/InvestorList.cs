using System.Collections.Generic;

namespace Investor
{
    public class InvestorList
    {
        private static List<Investor> list;

        public InvestorList(List<Investor> list)
        {
            InvestorList.list = list;
        }

        public static Investor SafeGetInvestor(int id)
        {
            if (id < list.Count)
            {
                if (list[id].id == id)
                    return list[id];
                else
                    Console.Console.Run("log_error id mismatch at InvestorList, make sure index of members represents their id!");
            }
            return null;
        }
    }
}
