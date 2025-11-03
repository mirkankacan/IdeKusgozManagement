namespace IdeKusgozManagement.Domain.Enums
{
    public enum FileType
    {
        Expense = 0,
        LeaveRequest = 1,
        Other = 2,
        TrafficTicket = 3
    }

    public static class FileTypeExtensions
    {
        public static string ToFolderName(this FileType fileType)
        {
            return fileType.ToString() + "s";
        }

        public static string ToStringValue(this FileType fileType)
        {
            return fileType.ToString();
        }
    }
}