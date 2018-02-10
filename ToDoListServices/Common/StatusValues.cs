namespace ToDoListServices.Common
{
    using System;
    using System.Text.RegularExpressions;
    using ToDoListServices.Common.ErrorHandling;

    /// <summary>
    /// holds valid StatusValues values
    /// </summary>
    /// <remarks>idea -- turn this into a stored table so user can add custom status values</remarks>
    public class StatusValues
    {
        public const string New = "New";
        public const string Started = "Started";
        public const string Deferred = "Deferred";
        public const string Completed = "Completed";

        public static bool IsValid(string statusValue)
        {
            return Regex.IsMatch(
                $"{New}|{Started}|{Deferred}|{Completed}", 
                statusValue, 
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// returns status that matches 'valid value' format
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetValidStatus(string status)
        {
            if(!StatusValues.IsValid(status))
                throw new InvalidStatusException(status, "Invalid status value.");

            if (status.Equals(StatusValues.New, StringComparison.OrdinalIgnoreCase))
                return StatusValues.New;

            if (status.Equals(StatusValues.Started, StringComparison.OrdinalIgnoreCase))
                return StatusValues.Started;

            if (status.Equals(StatusValues.Deferred, StringComparison.OrdinalIgnoreCase))
                return StatusValues.Deferred;

            if (status.Equals(StatusValues.Completed, StringComparison.OrdinalIgnoreCase))
                return StatusValues.Completed;

            // should not reach this line
            return status;
        }
    }
}
