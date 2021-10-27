using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Script.Serialization;
using System.Net.Mail;
using Interfaces.Service;
using Services.EPR;
using Core.Entities.Email;
namespace AspNetGroupBasedPermissions.Test
{
    [TestClass]
    public class MSRWorkflowTest
    {
        IMSRWorkflowService service;

        public MSRWorkflowTest()
        {
            service = new MSRWorkflowService();
        }

        [TestMethod]
        public void TestEmail()
        {
            service.SendEmailToRequester("1610-0012-IT");
        }
    }
}
