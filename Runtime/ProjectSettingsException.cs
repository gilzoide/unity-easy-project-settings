using System;

namespace Gilzoide.EasyProjectSettings
{
    public class ProjectSettingsException : Exception
    {
        public ProjectSettingsException(string message) : base(message)
        {
        }

        public ProjectSettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
