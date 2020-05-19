using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogsAndStrategy.Models
{
    public class Shedule
    {
        public int Id { get; set; }

        public ComparsionList<Period> Periods { get; set; }
        public byte[] ByteProperty { get; set; }

        public static string ListToStr(ComparsionList<Period> periods)
        {
            return JsonSerializer.Serialize(periods);
        }

        public static ComparsionList<Period> StrToList(string serializePeriods)
        {
            return JsonSerializer.Deserialize<ComparsionList<Period>>(serializePeriods);
        }
    }
}
