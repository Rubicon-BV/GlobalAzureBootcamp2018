using SimpleEchoBot.Models;
using System;

namespace SimpleEchoBot.Controllers
{
    [Serializable]
    public class DynamicsContextController
    {
        public DynamicsContextController()
        {

        }

        public bool WelcomeMessageSent = false;

        public bool CustomerIdentified
        {
            get {
                return CustomerId.HasValue;
            }
        }

        public Guid? CustomerId = null;

        public string FirstName = null;

        public SalesOrderDetail CustomerCh = null;

        public string ErrorCode = null;

        public bool ErrorCodeAsked = false;

        public DateTime? AppointmentScheduledOn = null;
    }
}