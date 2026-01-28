namespace IdeKusgozManagement.Domain.Enums
{
    public enum NotificationType
    {
        /// <summary>
        /// Sistem bildirimi
        /// </summary>
        System = 0,

        /// <summary>
        /// İzin talebi bildirimi
        /// </summary>
        LeaveRequest = 1,

        /// <summary>
        /// İş kaydı bildirimi
        /// </summary>
        WorkRecord = 2,

        /// <summary>
        /// Mesaj bildirimi
        /// </summary>
        Message = 3,

        Advance = 4,
        CompanyPayment = 5,
    }
}