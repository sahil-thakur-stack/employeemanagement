namespace EmployeeManagement.Models
{
    public static class DateTimeConverter
    {
        // Converts Unix timestamp to UTC DateTime
        public static DateTime UnixTimestampToDateTime(float unixTimestamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp);
            // Convert to UTC DateTime
            return dateTimeOffset.UtcDateTime;
        }

        // Converts DateTime to Unix timestamp
        public static float DateTimeToUnixTimestamp(DateTime dateTime)
        {
            // Ensure dateTime is in UTC before converting to Unix timestamp
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero);
            return (float)dateTimeOffset.ToUnixTimeSeconds();
        }
    }





}
