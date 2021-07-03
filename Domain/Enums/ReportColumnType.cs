using Domain.Helper;

namespace Domain.Enums
{
    public enum ReportColumnType
    {
        [StringValue("Text")]
        Text = 1,

        [StringValue("Date")]
        Date ,

        [StringValue("Number")]
        Number,
    }
}
