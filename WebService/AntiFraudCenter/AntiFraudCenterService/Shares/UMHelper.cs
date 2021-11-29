namespace AntiFraudCenterService.Shares
{
    using System;

    public static class UMHelper
    {
        public static bool IsUnderMaintaince(bool IsUnderMaintenanceTime, string UMFrom, string UMTo)
        {
            var currentTime = DateTime.UtcNow;
            DateTime maintainceFrom;
            DateTime maintainceTo;

            if (IsUnderMaintenanceTime
                && DateTime.TryParse(UMFrom, out maintainceFrom)
                && DateTime.TryParse(UMTo, out maintainceTo)
                && currentTime >= maintainceFrom
                && currentTime <= maintainceTo)
            {
                return true;
            }

            return false;
        }
    }
}